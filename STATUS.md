# StayNGo — working status

One-screen "where are we." Read at session start; update at session end. Link tickets/PRs/ADRs — don't restate them.

_Updated: 2026-06-21 · branch: `main`_

## Where we are
Phase 1. **Identity (EPIC #2)** and the **Listings host lifecycle (EPIC #3, #32)** are done. **EPIC #4 Search** has its first two slices merged: public guest browse (#35) and **availability date-range search (#37)**.

The merged #32 work (owner-scoped, by-hand):
- Endpoints: create draft, edit-draft, edit-published (price/desc/photos only), publish, archive, list-mine (paginated) under `/host/listings`.
- Domain: rich `Listing` with a guarded state machine (`Draft → Published → Archived`); publish gate names missing fields; `private set` throughout.
- Value objects: `Money` (owned type), `IanaTimeZone` (value converter) — kept out of the wire (DTOs use primitives).
- Infra: `GlobalExceptionHandler` (`DomainException`→409, `RecordNotFoundException`→404), OpenAPI Bearer scheme for Scalar, fail-fast pending-migration check at startup.
- Tests: **26 unit** + **8 integration** (Testcontainers Postgres). All green.

## In progress
- _Nothing in flight._

## Done (latest first)
- 2026-06-21 — **#38 merged: availability date-range search (#37 closed)** — `GET /listings?checkIn&checkOut` excludes listings with an overlapping confirmed booking; **pivoted away from `daterange`+GiST** to two `DateOnly` columns (`check_in`/`check_out`) + composite b-tree + pure-LINQ half-open overlap. `DateRange` VO/converter removed; migration `ReplaceBookingDuringWithCheckInCheckOut`. **ADR-0007**. No-double-booking DB invariant **deferred to #5** (mandatory there); exclusion test skipped with that pointer. +3 integration tests (19 total)
- 2026-06-20 — **#36 merged: public guest browse (#35 closed, EPIC #4 first slice)** — public `/listings` group (anonymous; host CRUD moved to `/host/listings`), `GET /listings` published-only + paginated + Location/MinCapacity/MinPrice/MaxPrice/Currency filters, `GET /listings/{id}` detail (404 on non-published), `PageResult<T>` envelope, `MoneyConvertor.FromMajorUnits` boundary conversion; +9 integration tests (17 total)
- 2026-06-20 — **#33 + #34 merged: Listings host lifecycle (#32 closed)** — full owner CRUD/state-machine, `IanaTimeZone` VO, exception handler, tests; `RenameListingTimeZoneIdToTimeZone` migration
- ~2026-06-16 — Identity EPIC #2 closed; ADR-0006 (JIT provisioning) merged (#31)
- 2026-06-14 — #28/#27/#25 merged: frontend `/identity/me`, backend CORS + Clerk authority, strict tsconfig
- ~2026-06-12 — #23/#21 merged: Docker Node 24 + container-build on PRs; shadcn baseline; ADR-0004/0005

## Next
1. **Frontend wire-up** — point `SearchPage` (now with date pickers) + `ListingDetailPage` at the live `/listings` endpoints (still on mock).
2. **Booking (EPIC #5)** — and per **ADR-0007** this is where the **no-double-booking invariant must be restored**: add a generated `daterange` column + GiST `EXCLUDE` constraint (the GiST learning milestone, relocated here). `check_in`/`check_out` stay the source of truth. Un-skip `BookingExclusionConstraintTests`.
3. Known follow-ups carried from #33/#35 (ticket if picking up): request validation (`RespectNullableAnnotations` / FluentValidation — `required` ≠ non-null), idempotency on state-changing endpoints (deferred per #32), and the single-currency `Currency` filter kept in #35 pending a multi-currency decision.

## Cross-session gotchas (not obvious from code)
- **Integration tests:** the test factory must apply migrations via a **standalone `DbContext` before touching `Services`** — accessing `Services` starts the host, which runs the fail-fast pending-migration check. (See `IntegrationTestFactory.InitializeAsync`.)
- **Serilog + multiple test hosts:** `CreateBootstrapLogger()` freezes the global logger; a second `WebApplicationFactory` host re-freezes → "logger already frozen". Fixed via `preserveStaticLogger: true` in `Program.cs`. Don't revert that.
- **EF value-object mapping lives in `DbContext.ConfigureConventions`:** single-column VOs (`IanaTimeZone`) use `Properties<T>().HaveConversion<>()`; multi-column VOs (`Money`) use `ComplexProperties<T>()`. Add new VOs there, not per-entity. **Caveat (ADR-0007):** a value-converted property is *opaque to query translation* — you can't filter by anything inside it in LINQ. `DateRange` was removed for exactly this reason (booking dates are now two plain `DateOnly` columns).
- `dotnet user-secrets` are **per-Windows-account**; `dotnet ef` needs `ASPNETCORE_ENVIRONMENT=Development` to load the secret connection string.
- Tickets carry **by-hand** vs **by-claude** labels: by-hand = the user implements, Claude only advises/reviews. #32 is by-hand.
- Agent shell runs as Windows user `vm`; the user's IDE runs as another account → different user-secrets stores.
