# 001. GiST exclusion constraint — no overlapping confirmed bookings

> **Pointer note.** Task 12's planned learning output (plan §12.7) is written, in own words, in the
> Obsidian Concept note [[GiST exclusion constraint]] (`docs/Concepts/GiST exclusion constraint.md`).
> This file exists so the path a future reader expects from the Phase 0 plan resolves — see the
> [Phase 0 retro](../retros/phase-0-retro.md) for why the learning landed in the Concept note instead.

## As built (Task 12)

- Constraint `bookings_no_overlap_confirmed` on `bookings`:
  `EXCLUDE USING gist (listing_id WITH =, during WITH &&) WHERE (status = 2)`.
- **`WHERE (status = 2)`, not `status = 'Confirmed'`** — `BookingStatus` is stored as `int` (a
  deviation from the plan; see the retro). The Concept note shows the idealized `'Confirmed'` form;
  the migration uses the integer, so every enum value must keep its explicit `= N` annotation.
- `btree_gist` extension is required to put the scalar `listing_id` into a GiST index beside the
  `during` range. Added via Npgsql annotation (`HasPostgresExtension`), not raw `CREATE EXTENSION`.
- Migration `InitialScheme`; verified by `BookingExclusionConstraintTests`, which inserts two
  overlapping confirmed bookings and asserts SQLSTATE `23P01`.

See [[GiST exclusion constraint]] for the conceptual write-up and references.
