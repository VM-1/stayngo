# StayNGo — working status

One-screen "where are we." Read at session start; update at session end. Link tickets/PRs/ADRs — don't restate them.

_Updated: 2026-06-20 · branch: `main`_

## Where we are
Phase 1. **Identity (EPIC #2)** and the **Listings host lifecycle (EPIC #3, #32)** are done. The **public guest browse (EPIC #4 first slice, #35)** is now **merged to `main`** too.

The merged #32 work (owner-scoped, by-hand):
- Endpoints: create draft, edit-draft, edit-published (price/desc/photos only), publish, archive, list-mine (paginated) under `/host/listings`.
- Domain: rich `Listing` with a guarded state machine (`Draft → Published → Archived`); publish gate names missing fields; `private set` throughout.
- Value objects: `Money` (owned type), `IanaTimeZone` (value converter) — kept out of the wire (DTOs use primitives).
- Infra: `GlobalExceptionHandler` (`DomainException`→409, `RecordNotFoundException`→404), OpenAPI Bearer scheme for Scalar, fail-fast pending-migration check at startup.
- Tests: **26 unit** + **8 integration** (Testcontainers Postgres). All green.

## In progress
- _Nothing in flight._

## Done (latest first)
- 2026-06-20 — **#36 merged: public guest browse (#35 closed, EPIC #4 first slice)** — public `/listings` group (anonymous; host CRUD moved to `/host/listings`), `GET /listings` published-only + paginated + Location/MinCapacity/MinPrice/MaxPrice/Currency filters, `GET /listings/{id}` detail (404 on non-published), `PageResult<T>` envelope, `MoneyConvertor.FromMajorUnits` boundary conversion; +9 integration tests (17 total)
- 2026-06-20 — **#33 + #34 merged: Listings host lifecycle (#32 closed)** — full owner CRUD/state-machine, `IanaTimeZone` VO, exception handler, tests; `RenameListingTimeZoneIdToTimeZone` migration
- ~2026-06-16 — Identity EPIC #2 closed; ADR-0006 (JIT provisioning) merged (#31)
- 2026-06-14 — #28/#27/#25 merged: frontend `/identity/me`, backend CORS + Clerk authority, strict tsconfig
- ~2026-06-12 — #23/#21 merged: Docker Node 24 + container-build on PRs; shadcn baseline; ADR-0004/0005

## Next
1. **Next #4 slice — date-range / availability search** (deferred from #35): filter browse by available date range, backed by the GiST exclusion-constraint index (spec §5.4; stub in `BookingExclusionConstraintTests`). This is the named learning milestone (GiST/`tstzrange`).
2. Frontend wire-up — point `SearchPage` + `ListingDetailPage` at the now-live `/listings` endpoints (they're still on mock).
3. Or jump to Booking (EPIC #5).
4. Known follow-ups carried from #33/#35 (ticket if picking up): request validation (`RespectNullableAnnotations` / FluentValidation — `required` ≠ non-null), idempotency on state-changing endpoints (deferred per #32), and the single-currency `Currency` filter kept in #35 pending a multi-currency decision.

## Cross-session gotchas (not obvious from code)
- **Integration tests:** the test factory must apply migrations via a **standalone `DbContext` before touching `Services`** — accessing `Services` starts the host, which runs the fail-fast pending-migration check. (See `IntegrationTestFactory.InitializeAsync`.)
- **Serilog + multiple test hosts:** `CreateBootstrapLogger()` freezes the global logger; a second `WebApplicationFactory` host re-freezes → "logger already frozen". Fixed via `preserveStaticLogger: true` in `Program.cs`. Don't revert that.
- **EF value-object mapping lives in `DbContext.ConfigureConventions`:** single-column VOs (`DateRange`, `IanaTimeZone`) use `Properties<T>().HaveConversion<>()`; multi-column VOs (`Money`) use `ComplexProperties<T>()`. Add new VOs there, not per-entity.
- `dotnet user-secrets` are **per-Windows-account**; `dotnet ef` needs `ASPNETCORE_ENVIRONMENT=Development` to load the secret connection string.
- Tickets carry **by-hand** vs **by-claude** labels: by-hand = the user implements, Claude only advises/reviews. #32 is by-hand.
- Agent shell runs as Windows user `vm`; the user's IDE runs as another account → different user-secrets stores.
