# 0007. Store booking dates as two `DateOnly` columns, not a `daterange`

Date: 2026-06-21
Status: Accepted

## Context

Availability search (#37) needs to find listings free for a requested check-in/check-out range — i.e. exclude listings with an overlapping confirmed booking. The original design modelled a booking's stay as a PostgreSQL `daterange` (`during`), mapped from a `DateRange` value object, with a GiST exclusion constraint (`EXCLUDE USING gist (listing_id WITH =, during WITH &&)`) intended to both enforce "no two overlapping confirmed bookings" and serve as the availability-lookup index.

In practice, querying that `daterange` from EF Core fought the framework at every turn:

- The `DateRange` **value converter is opaque to query translation** — EF cannot see inside a value-converted property in a `Where`, so `b.During.Overlaps(...)` does not translate.
- Making the overlap translate required either a `HasDbFunction` mapping to the `&&` operator — which depends on an **internal Npgsql API** (`PgBinaryExpression`, suppressed `EF1001`) plus explicit `HasParameter(...).HasStoreType("daterange")` — or expressing the overlap in **interpolated raw SQL**, which loses compile-time name safety and, when typed filters were composed over it, hit a complex-type (`Money`) column-naming bug.

Both routes are disproportionate machinery for the current scope. Critically, the GiST exclusion constraint enforces a **write-side invariant** (preventing concurrent double-booking) — and there is **no booking write path yet**; bookings (#5) are not implemented. We were paying for a guarantee we cannot yet exercise.

## Decision

Model a booking's stay as **two plain `DateOnly` columns**, `check_in` and `check_out`, and drop the `daterange` column, the `DateRange` value object, the `DateRangeConverter`, and the GiST exclusion constraint.

Availability search becomes pure, fully-typed LINQ using half-open semantics:

```csharp
!l.Bookings.Any(b => b.Status == BookingStatus.Confirmed
                     && b.CheckIn < checkOut
                     && checkIn < b.CheckOut)
```

backed by a composite **b-tree** index `(listing_id, check_in, check_out)`. No range type, no GiST, no raw SQL, no internal EF APIs. `check_out` is exclusive by domain convention (the guest leaves that day), so back-to-back stays do not overlap.

This is a deliberate application of "finish the business slice before optimizing, and introduce complexity only when it hurts." The range/GiST work was prototyped and rejected for now; this ADR records that.

## Consequences

- **Simpler, framework-aligned read path.** The search query is ordinary LINQ that any reader can follow; no `EF1001` suppression, no value-converter gymnastics, no raw SQL to keep in sync with column names.
- **No database-level protection against overlapping bookings until #5.** This is the real trade-off, and it is acceptable *only* because no code writes bookings yet. The half-open comparison enforces correctness in the read query, but nothing stops two concurrent confirmed bookings from being inserted once a write path exists. **#5 must restore that invariant before booking creation ships** — otherwise this becomes a correctness hole, not just a deferred optimization.
- **The GiST/range work is relocated, not discarded.** When #5 adds it, `check_in`/`check_out` remain the source of truth and a **generated** `daterange` column derives the range purely to back the exclusion constraint:
  ```sql
  ALTER TABLE bookings ADD COLUMN during daterange
    GENERATED ALWAYS AS (daterange(check_in, check_out, '[)')) STORED;
  ALTER TABLE bookings ADD CONSTRAINT bookings_no_overlap_confirmed
    EXCLUDE USING gist (listing_id WITH =, during WITH &&) WHERE (status = 2);
  ```
  This keeps the search on the typed columns and the invariant on the constraint, without re-introducing a value-converted `daterange` into the query path.
- **b-tree suffices at current scale.** The composite index serves the `listing_id =` equality plus the `check_in` range; the `check_out` bound is a residual filter. A GiST index would index intervals more directly, but that only matters at a scale we are nowhere near. Revisit if availability search becomes hot.
- The exclusion-constraint integration test (`BookingExclusionConstraintTests`) is **skipped** with a pointer to #5 rather than deleted, so the intent and the GiST learning survive.

## References

- PostgreSQL range types & exclusion constraints — docs §8.17 "Range Types" and §"Constraints" → Exclusion Constraints (deferred to #5).
- EF Core value conversions, "Limitations" — value-converted properties cannot be used in most server-side query operations (learn.microsoft.com/ef/core/modeling/value-conversions).
- Generated columns — PostgreSQL docs §5.4 "Generated Columns" (the #5 path for deriving the range from the two date columns).
