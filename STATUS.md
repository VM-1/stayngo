# StayNGo — working status

One-screen "where are we." Read at session start; update at session end. Link tickets/PRs/ADRs — don't restate them.

_Updated: 2026-06-26 · branch: `main`_

## Where we are
Phase 1. **Backend is feature-complete** — Identity (#2), Listings (#3), Search (#4), and **Booking (#5, now closed)** all shipped. Booking covers request → host confirm/reject → guest cancel (24h cut-off in listing tz) + guest "trips" / host "reservations" reads; no-double-booking GiST constraint (ADR-0007), required `Idempotency-Key` header.

**Remaining for the Phase-1 base:** wire the frontend (EPIC #6) to the live API, then deploy (#8) + observability basics (#7). Done-bar (StayNGo.md §"Done ="): a stranger registers → publishes a listing → books someone else's, on the live URL.

The merged #32 work (owner-scoped, by-hand):
- Endpoints: create draft, edit-draft, edit-published (price/desc/photos only), publish, archive, list-mine (paginated) under `/host/listings`.
- Domain: rich `Listing` with a guarded state machine (`Draft → Published → Archived`); publish gate names missing fields; `private set` throughout.
- Value objects: `Money` (owned type), `IanaTimeZone` (value converter) — kept out of the wire (DTOs use primitives).
- Infra: `GlobalExceptionHandler` (`DomainException`→409, `RecordNotFoundException`→404), OpenAPI Bearer scheme for Scalar, fail-fast pending-migration check at startup.
- Tests: **26 unit** + **8 integration** (Testcontainers Postgres). All green.

## In progress
- _Nothing in flight._

## Done (latest first)
- 2026-06-26 — **#43 merged: guest cancellation (#42 closed, EPIC #5 closed)** — `PUT /me/trips/{id}/cancel`; `Booking.Cancel(now, tz)` allowed Pending/Confirmed, rejected within 24h of check-in anchored to 15:00 in the listing's IANA timezone (`now` injected → unit-testable); cancelling a confirmed stay frees the dates. +6 entity unit + 2 integration tests
- 2026-06-26 — **#41 merged: idempotency key → required `Idempotency-Key` header** (#40 review follow-up) — empty/missing key → 400 before insert; replay keys off the header value
- 2026-06-25 — **#40 merged: Booking request + host confirm/reject (#39 closed, EPIC #5 slices 1+2)** — `POST /bookings` (Pending, price fixed = nights×nightly), `POST /reservations/{id}/confirm|reject` (owner-scoped guarded state machine), `GET /me/trips` + `GET /reservations` (pending-first, paginated). **GiST exclusion constraint restored** (generated `during` daterange, `EXCLUDE … WHERE status=Confirmed`) per ADR-0007 → `23P01`→`DomainException`→409. Per-user idempotency unique index + replay. Frontend Trips/Reservations wired. +tests (30 integration)
- 2026-06-21 — **#38 merged: availability date-range search (#37 closed)** — `GET /listings?checkIn&checkOut` excludes listings with an overlapping confirmed booking; **pivoted away from `daterange`+GiST** to two `DateOnly` columns (`check_in`/`check_out`) + composite b-tree + pure-LINQ half-open overlap. `DateRange` VO/converter removed; migration `ReplaceBookingDuringWithCheckInCheckOut`. **ADR-0007**. No-double-booking DB invariant **deferred to #5** (mandatory there); exclusion test skipped with that pointer. +3 integration tests (19 total)
- 2026-06-20 — **#36 merged: public guest browse (#35 closed, EPIC #4 first slice)** — public `/listings` group (anonymous; host CRUD moved to `/host/listings`), `GET /listings` published-only + paginated + Location/MinCapacity/MinPrice/MaxPrice/Currency filters, `GET /listings/{id}` detail (404 on non-published), `PageResult<T>` envelope, `MoneyConvertor.FromMajorUnits` boundary conversion; +9 integration tests (17 total)
- 2026-06-20 — **#33 + #34 merged: Listings host lifecycle (#32 closed)** — full owner CRUD/state-machine, `IanaTimeZone` VO, exception handler, tests; `RenameListingTimeZoneIdToTimeZone` migration
- ~2026-06-16 — Identity EPIC #2 closed; ADR-0006 (JIT provisioning) merged (#31)
- 2026-06-14 — #28/#27/#25 merged: frontend `/identity/me`, backend CORS + Clerk authority, strict tsconfig
- ~2026-06-12 — #23/#21 merged: Docker Node 24 + container-build on PRs; shadcn baseline; ADR-0004/0005

## Next
1. **Frontend wiring (EPIC #6)** — backend is done, so this is the main remaining base work. Pages still on `lib/mock.ts`: `SearchPage` (browse + filters + dates), `ListingDetailPage`, `TripsPage`, `HostReservationsPage`, `HostListingsPage`; plus write flows `ReservePage` (POST /bookings + Idempotency-Key header) and `CreateListingPage`. Scaffolding exists (Clerk auth, `lib/api.ts` fetch wrapper, react-query, shadcn); `AccountPage` is the wired exemplar. Per-page: endpoint hooks + swap mock + loading/error/empty states.
2. **Deploy (#8)** + **Observability basics (#7)** — Fly.io app + managed Postgres + secrets + migrations-on-boot + serve FE + prod Clerk keys. Then run the done-bar smoke on the live URL.
3. **db-notes owed (by-hand learning)**: `docs/db-notes/` on the GiST exclusion constraint, idempotent requests (#39), and a short note on anchoring a deadline to a place's local time (#42).
4. Carried follow-ups (ticket if picking up): request validation (FluentValidation — `required` ≠ non-null); single-currency `Currency` filter (#35) pending a multi-currency decision.

## Cross-session gotchas (not obvious from code)
- **Integration tests:** the test factory must apply migrations via a **standalone `DbContext` before touching `Services`** — accessing `Services` starts the host, which runs the fail-fast pending-migration check. (See `IntegrationTestFactory.InitializeAsync`.)
- **Serilog + multiple test hosts:** `CreateBootstrapLogger()` freezes the global logger; a second `WebApplicationFactory` host re-freezes → "logger already frozen". Fixed via `preserveStaticLogger: true` in `Program.cs`. Don't revert that.
- **EF value-object mapping lives in `DbContext.ConfigureConventions`:** single-column VOs (`IanaTimeZone`) use `Properties<T>().HaveConversion<>()`; multi-column VOs (`Money`) use `ComplexProperties<T>()`. Add new VOs there, not per-entity. **Caveat (ADR-0007):** a value-converted property is *opaque to query translation* — you can't filter by anything inside it in LINQ. `DateRange` was removed for exactly this reason (booking dates are now two plain `DateOnly` columns).
- `dotnet user-secrets` are **per-Windows-account**; `dotnet ef` needs `ASPNETCORE_ENVIRONMENT=Development` to load the secret connection string.
- Tickets carry **by-hand** vs **by-claude** labels: by-hand = the user implements, Claude only advises/reviews. #32 is by-hand.
- Agent shell runs as Windows user `vm`; the user's IDE runs as another account → different user-secrets stores.
