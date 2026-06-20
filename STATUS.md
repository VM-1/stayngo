# StayNGo — working status

One-screen "where are we." Read at session start; update at session end. Link tickets/PRs/ADRs — don't restate them.

_Updated: 2026-06-20 · branch: `featue/32-listing`_

## Where we are
Phase 1. **Identity (EPIC #2) is done and merged** (ADR-0006: JIT provisioning). **Listings (EPIC #3) host lifecycle — #32, by-hand — is implemented and in review as PR #33.**

#33 delivers the owner-facing listing lifecycle, owner-scoped:
- Endpoints: create draft, edit-draft, edit-published (price/desc/photos only), publish, archive, list-mine (paginated) under `/host/listings`.
- Domain: rich `Listing` with a guarded state machine (`Draft → Published → Archived`); publish gate names missing fields; `private set` throughout.
- Value objects: `Money` (owned type), `IanaTimeZone` (value converter) — both kept out of the wire (DTOs use primitives).
- Infra: `GlobalExceptionHandler` (`DomainException`→409, `RecordNotFoundException`→404), OpenAPI Bearer scheme for Scalar, fail-fast pending-migration check at startup.
- Tests: **26 unit** (state machine + Money) + **8 integration** (Testcontainers Postgres: owner-scoping, publish gate, lifecycle, list-mine). All green.

## In progress
- **PR #33** open (`Closes #32`). Code review done — 2 findings: timezone-primitive (fixed, `IanaTimeZone` VO) and unused `GetAsync` (kept intentionally for future non-provisioning reads).
- **Uncommitted on the branch:** renamed `Listing.TimeZoneId` → `TimeZone` (property + DTO + service + tests). **This renames the column `time_zone_id` → `time_zone`, so it needs a migration that isn't generated yet** — name it `RenameListingTimeZoneIdToTimeZone`, and verify `Up()` uses `RenameColumn` (not drop+add).

## Done (latest first)
- 2026-06-20 — #33 opened: Listings host lifecycle (#32); code-reviewed; timezone VO added
- ~2026-06-16 — Identity EPIC #2 closed; ADR-0006 (JIT provisioning) merged (#31)
- 2026-06-14 — #28/#27/#25 merged: frontend `/identity/me`, backend CORS + Clerk authority, strict tsconfig
- ~2026-06-12 — #23/#21 merged: Docker Node 24 + container-build on PRs; shadcn baseline; ADR-0004/0005

## Next
1. Generate the `RenameListingTimeZoneIdToTimeZone` migration, commit, push, get CI green, **merge #33**.
2. Then next #3 slice — public/guest listing browse (read side), or Search (EPIC #4) / Booking (EPIC #5, the GiST exclusion-constraint work is already stubbed in `BookingExclusionConstraintTests`).

## Cross-session gotchas (not obvious from code)
- **Integration tests:** the test factory must apply migrations via a **standalone `DbContext` before touching `Services`** — accessing `Services` starts the host, which runs the fail-fast pending-migration check. (See `IntegrationTestFactory.InitializeAsync`.)
- **Serilog + multiple test hosts:** `CreateBootstrapLogger()` freezes the global logger; a second `WebApplicationFactory` host re-freezes → "logger already frozen". Fixed via `preserveStaticLogger: true` in `Program.cs`. Don't revert that.
- **EF value-object mapping lives in `DbContext.ConfigureConventions`:** single-column VOs (`DateRange`, `IanaTimeZone`) use `Properties<T>().HaveConversion<>()`; multi-column VOs (`Money`) use `ComplexProperties<T>()`. Add new VOs there, not per-entity.
- `dotnet user-secrets` are **per-Windows-account**; `dotnet ef` needs `ASPNETCORE_ENVIRONMENT=Development` to load the secret connection string.
- Tickets carry **by-hand** vs **by-claude** labels: by-hand = the user implements, Claude only advises/reviews. #32 is by-hand.
- Agent shell runs as Windows user `vm`; the user's IDE runs as another account → different user-secrets stores.
