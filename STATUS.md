# StayNGo — working status

One-screen "where are we." Read at session start; update at session end. Link tickets/PRs/ADRs — don't restate them.

_Updated: 2026-06-21 · branch: `main`_

## Where we are
Phase 1. **Identity (#2)**, **Listings (#3)**, **Search (#4)** done. **EPIC #5 Booking** is mostly built: request + host confirm/reject + guest "trips" / host "reservations" reads merged (#40), idempotency hardened to a required header (#41). The no-double-booking GiST exclusion constraint is **restored** here (ADR-0007). Remaining booking slice: **guest cancellation**.

The merged #32 work (owner-scoped, by-hand):
- Endpoints: create draft, edit-draft, edit-published (price/desc/photos only), publish, archive, list-mine (paginated) under `/host/listings`.
- Domain: rich `Listing` with a guarded state machine (`Draft → Published → Archived`); publish gate names missing fields; `private set` throughout.
- Value objects: `Money` (owned type), `IanaTimeZone` (value converter) — kept out of the wire (DTOs use primitives).
- Infra: `GlobalExceptionHandler` (`DomainException`→409, `RecordNotFoundException`→404), OpenAPI Bearer scheme for Scalar, fail-fast pending-migration check at startup.
- Tests: **26 unit** + **8 integration** (Testcontainers Postgres). All green.

## In progress
- _Nothing in flight._

## Done (latest first)
- 2026-06-26 — **#41 merged: idempotency key → required `Idempotency-Key` header** (#40 review follow-up) — empty/missing key → 400 before insert; replay keys off the header value
- 2026-06-25 — **#40 merged: Booking request + host confirm/reject (#39 closed, EPIC #5 slices 1+2)** — `POST /bookings` (Pending, price fixed = nights×nightly), `POST /reservations/{id}/confirm|reject` (owner-scoped guarded state machine), `GET /me/trips` + `GET /reservations` (pending-first, paginated). **GiST exclusion constraint restored** (generated `during` daterange, `EXCLUDE … WHERE status=Confirmed`) per ADR-0007 → `23P01`→`DomainException`→409. Per-user idempotency unique index + replay. Frontend Trips/Reservations wired. +tests (30 integration)
- 2026-06-21 — **#38 merged: availability date-range search (#37 closed)** — `GET /listings?checkIn&checkOut` excludes listings with an overlapping confirmed booking; **pivoted away from `daterange`+GiST** to two `DateOnly` columns (`check_in`/`check_out`) + composite b-tree + pure-LINQ half-open overlap. `DateRange` VO/converter removed; migration `ReplaceBookingDuringWithCheckInCheckOut`. **ADR-0007**. No-double-booking DB invariant **deferred to #5** (mandatory there); exclusion test skipped with that pointer. +3 integration tests (19 total)
- 2026-06-20 — **#36 merged: public guest browse (#35 closed, EPIC #4 first slice)** — public `/listings` group (anonymous; host CRUD moved to `/host/listings`), `GET /listings` published-only + paginated + Location/MinCapacity/MinPrice/MaxPrice/Currency filters, `GET /listings/{id}` detail (404 on non-published), `PageResult<T>` envelope, `MoneyConvertor.FromMajorUnits` boundary conversion; +9 integration tests (17 total)
- 2026-06-20 — **#33 + #34 merged: Listings host lifecycle (#32 closed)** — full owner CRUD/state-machine, `IanaTimeZone` VO, exception handler, tests; `RenameListingTimeZoneIdToTimeZone` migration
- ~2026-06-16 — Identity EPIC #2 closed; ADR-0006 (JIT provisioning) merged (#31)
- 2026-06-14 — #28/#27/#25 merged: frontend `/identity/me`, backend CORS + Clerk authority, strict tsconfig
- ~2026-06-12 — #23/#21 merged: Docker Node 24 + container-build on PRs; shadcn baseline; ADR-0004/0005

## Next
1. **Finish Booking (#5): guest cancellation** — the last write slice. Guest cancels their own booking before check-in (24h rule in the listing's timezone, per spec §5.3). `Booking.Cancel()` already exists; needs endpoint + service + the 24h/timezone rule + tests. Then #5 can close.
2. **db-notes owed from #39** (by-hand learning deliverable): `docs/db-notes/` on the GiST exclusion constraint and on idempotent requests (~300 words each, own words).
3. Then **frontend** (EPIC #6 Frontend Shell) — wire remaining mock pages (SearchPage date filters, ListingDetailPage) to live endpoints; then Observability (#7), Deploy (#8).
4. Carried follow-ups (ticket if picking up): request validation (FluentValidation — `required` ≠ non-null), single-currency `Currency` filter (#35) pending a multi-currency decision.

## Cross-session gotchas (not obvious from code)
- **Integration tests:** the test factory must apply migrations via a **standalone `DbContext` before touching `Services`** — accessing `Services` starts the host, which runs the fail-fast pending-migration check. (See `IntegrationTestFactory.InitializeAsync`.)
- **Serilog + multiple test hosts:** `CreateBootstrapLogger()` freezes the global logger; a second `WebApplicationFactory` host re-freezes → "logger already frozen". Fixed via `preserveStaticLogger: true` in `Program.cs`. Don't revert that.
- **EF value-object mapping lives in `DbContext.ConfigureConventions`:** single-column VOs (`IanaTimeZone`) use `Properties<T>().HaveConversion<>()`; multi-column VOs (`Money`) use `ComplexProperties<T>()`. Add new VOs there, not per-entity. **Caveat (ADR-0007):** a value-converted property is *opaque to query translation* — you can't filter by anything inside it in LINQ. `DateRange` was removed for exactly this reason (booking dates are now two plain `DateOnly` columns).
- `dotnet user-secrets` are **per-Windows-account**; `dotnet ef` needs `ASPNETCORE_ENVIRONMENT=Development` to load the secret connection string.
- Tickets carry **by-hand** vs **by-claude** labels: by-hand = the user implements, Claude only advises/reviews. #32 is by-hand.
- Agent shell runs as Windows user `vm`; the user's IDE runs as another account → different user-secrets stores.
