# 0001. Initial stack for Phase 1

Date: 2026-04-19
Status: Accepted

## Context

StayNGo is a long-running solo side project (spec §1). Primary goals: interview portfolio emphasizing staff-backend + Postgres-internals depth; secondary: learn FE and DevOps. Budget: ~10 hrs/week, 12–18 month arc. Stack choices must:

- Be supported through the project arc (no mid-project LTS exit).
- Have minimal friction for a solo maintainer.
- Expose real domain complexity (booking concurrency, availability ranges) so the implementation *teaches*, not papers-over.
- Avoid commercial/license surprises.

## Decision

Phase 1 stack:

- **Runtime:** .NET 10 (LTS, released Nov 2025; supported through Nov 2028).
- **Architecture:** vertical slices (`Features/<Domain>/<Action>/`) in Phase 1 → Clean Architecture *per module* in Phase 3.
- **Handler library:** plain handler classes (bespoke `IRequestHandler<TReq, TRes>`). **Do not adopt MediatR** — it went commercial in 2024. Re-evaluate source-gen `Mediator` (MIT) in Phase 3 if dispatch ceremony hurts.
- **ORM:** EF Core 10. `DbContext.SaveChangesAsync` **is** the unit of work — no `IUnitOfWork` abstraction, no generic `IRepository<T>`.
- **Validation:** FluentValidation.
- **Database:** PostgreSQL 16 (via docker-compose locally; Fly.io managed Postgres at the Phase 1 demo milestone).
- **Concurrency invariant:** enforced via Postgres `EXCLUDE USING gist` on `(listing_id, during)` WHERE `status = 'Confirmed'`. Domain invariants live in the schema, not the handler.
- **Auth:** Clerk (managed). Not rolling ASP.NET Identity.
- **API docs:** Scalar (over Swagger UI).
- **Logging:** Serilog + structured logging.
- **Tests:** xUnit + FluentAssertions + **Testcontainers** (real Postgres) from day 1.
- **Frontend:** React 18 + Vite + TypeScript strict + Tailwind + shadcn/ui. One SPA serves both guest and host views (host routes guarded under `/host/*`).
- **Containers:** Docker + `docker compose` locally.
- **CI:** GitHub Actions.
- **Hosting:** local-only for Phase 0–1 development; Fly.io as the Phase 1 demo milestone.
- **Repo:** monorepo. Backend nested under `backend/StayNGo/` so Phase 3 can add `backend/Modules/*` siblings cleanly.

## Consequences

**Positive**
- .NET 10 LTS covers the full 12–18 month arc without forced mid-project migration.
- Low friction solo — minimum moving parts.
- Postgres GiST exclusion teaches the canonical "invariant in the schema" pattern.
- Testcontainers from day 1 means integration tests exercise real SQL behavior (exclusion constraint, ranges, timezone handling) — a mock-based stack would mask the bugs this project exists to teach.

**Negative**
- .NET 10 at ~5 months of age means some third-party packages may lag. Mitigation: pin versions in `Directory.Packages.props`; prefer packages with active .NET 10 releases.
- Clerk costs money if free tier is exceeded (unlikely for a demo app). Mitigation: monitor Clerk MAU; swap to Supabase Auth if friction grows.
- Monorepo means one CI config to maintain; build times grow linearly with both stacks.

**Neutral**
- The `backend/StayNGo/` nesting looks redundant in Phase 1 but unlocks clean Phase 3 modularization without a disruptive restructure. Documented in ADR-0002.
