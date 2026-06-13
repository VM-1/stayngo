# StayNGo — working status

One-screen "where are we." Read at session start; update at session end. Link tickets/PRs/ADRs — don't restate them.

_Updated: 2026-06-14 · branch: main_

## Where we are
Phase 1 → **Identity slice (Clerk) is complete end-to-end on `main`**: backend `/identity/me` + JIT provisioning + CORS, frontend Account page fetches it via TanStack Query. EPIC #2 (Identity) is essentially done.

## In progress
- **PR #29** — docs: this `STATUS.md` + design/learning notes — open.
- **Uncommitted backend domain WIP** — `Booking`/`Listing` entities + EF `ModelSnapshot` (early groundwork for Listings/Booking; left out of the Identity PRs on purpose).

## Done (latest first)
- 2026-06-14 — #28 merged: frontend profile via `GET /identity/me` (#19)
- 2026-06-14 — #27 merged: backend CORS + Clerk authority (#26)
- 2026-06-14 — #25 merged: restore strict tsconfig options (#24)
- ~2026-06-12 — #23 merged: frontend Dockerfile → Node 24 (#22); `container-build` now runs on PRs
- ~2026-06-12 — #21 merged: frontend shell + shadcn baseline (#20, EPIC #6); ADR-0004 (shadcn over MUI/antd)
- earlier — #16 merged: backend Identity — Clerk JWT, `/identity/me`, JIT provisioning (EPIC #2)

## Next
1. Merge #29 (docs).
2. Start the next Phase-1 epic — recommend **Listings (EPIC #3)** (foundation for Search #4 / Booking #5; the uncommitted `Booking`/`Listing` entities are early groundwork). Alt: sign-in/up polish (#18).

## Cross-session gotchas (not obvious from code)
- `dotnet user-secrets` are **per-Windows-account** — set the secret and run the app under the same account.
- `dotnet ef` needs `ASPNETCORE_ENVIRONMENT=Development` to load the user-secret connection string (else it falls back to the appsettings `test` placeholder).
- Tickets carry **by-hand** vs **by-claude** labels: by-hand = the user implements, Claude only advises.
- The agent shell runs as Windows user `vm`; the user's IDE runs as another account — so their user-secrets stores differ.
- appsettings still carries a `test/test` placeholder connection string + `StayNGO`/`StayNGo` casing mix — harmless, worth normalizing.
