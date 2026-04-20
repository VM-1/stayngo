# 0002. Monorepo structure + backend/StayNGo/ nesting

Date: 2026-04-19
Status: Accepted

## Context

Solo developer. Backend (.NET), frontend (React), infra (docker compose, later fly.toml) all evolve in lockstep during Phase 1–3. Three separate repos would triple the context-switch and CI-config overhead; cross-stack changes couldn't be atomic.

Separately: Phase 3 of the roadmap (spec §4, §8) splits the monolith into a modular monolith with sibling folders `backend/Modules/{Identity,Catalog,Booking,Notifications}/`. The natural home for the composition-root host is a sibling folder alongside the modules.

## Decision

1. **Single monorepo** with top-level `backend/`, `frontend/`, `infra/`, `docs/`, `.github/`. One `.github/` config governs the whole repo.
2. **Backend nested at `backend/StayNGo/`** (not flat under `backend/`). In Phase 1, `backend/StayNGo/` contains `StayNGo.sln`, `src/{Api,Domain,Infrastructure}/`, `tests/`. In Phase 3, sibling folders `backend/Modules/*` are added; `backend/StayNGo/` becomes the composition-root host (Program.cs wires modules, owns migrations orchestration, hosts the shared kernel).
3. **Root-level `backend.sln`** will be introduced at Phase 3 kickoff referencing all module projects; until then, `backend/StayNGo/StayNGo.sln` is the only solution file.

## Consequences

**Positive**
- Atomic PRs across stacks (backend + frontend + infra in one diff).
- One Claude Code context covers everything; `CLAUDE.md` at repo root auto-loads for every session.
- Single CI pipeline.
- Phase 3 modularization is additive — `backend/StayNGo/` doesn't move, just gets siblings. No painful repo restructure.

**Negative**
- `backend/StayNGo/` nesting looks redundant in Phase 1 (one subfolder, one solution). Cost is paid up-front for Phase 3 readiness.
- Monorepo means CI build time grows with both stacks. Mitigation: split CI jobs per stack; add path-based filters in Phase 2 if build time hurts.

**Neutral**
- If/when one subsystem grows large enough to warrant isolation (Phase 6+ extraction), doing so will be a deliberate move with its own ADR.
