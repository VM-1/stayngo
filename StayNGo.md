# StayNGo

Stays / short-term-rentals booking platform. Solo side project simulating small-company workflow.

> This is the human-readable project brief. The authoritative spec (with merge audit trail and deferred-decisions log) lives at `docs/superpowers/specs/2026-04-19-staynGo-design.md`. `CLAUDE.md` is the Claude Code operational ruleset.

---

## 1. Identity & goals

- **Name:** StayNGo.
- **Type:** Stays / short-term-rentals booking platform.
- **Context:** Long-running solo side project that deliberately simulates a small-company workflow while working solo.
- **Primary goal:** Interview portfolio emphasizing **staff-level backend architecture judgment + Postgres internals depth** (commit history, ADRs, PR descriptions, phased decisions).
- **Secondary goals:** Learn frontend enough to build features unaided; grow hands-on DevOps.
- **Non-goal:** Real users, production SLA. Every architectural decision is justified by *learning value* or *domain pain*, not "users need this."
- **Budget:** ~10 hrs/week, 12–18 month arc (no specific interview date).
- **First deploy:** Local-only for Phase 0–1 dev; Fly.io as the Phase 1 demo milestone.

### 1.1 Learning tracks

Four explicit tracks the project serves. Assumes prior working experience with SQL Server / PostgreSQL / MySQL at the application-developer level — targeting *depth*, not basics.

| Track | Focus | Phase emphasis |
|---|---|---|
| **Backend architecture** | DDD, vertical slices → modular monolith → messaging, outbox, CQRS read models | Phase 1 → 5 |
| **Database internals** | Index types (B-tree, GiST, GIN, BRIN, hash, covering), query planning (`EXPLAIN ANALYZE`), MVCC, concurrency control, isolation levels, row-level + advisory locks, logical replication, partitioning, warehouse modeling | Phase 1, 2, 4, 5, 6 |
| **Frontend** | TS strict, TanStack Query patterns, forms/validation, optimistic updates, shadcn composition | Phase 1, 3 |
| **DevOps / platform** | Docker/Compose, GH Actions, OTel + Grafana, hosting evolution (local → Fly → VPS), secrets, TLS, reverse proxy | Phase 0, 2, 4 |

---

## 2. Guiding principles

These exist because the default failure mode of this kind of project is over-engineering early, under-shipping in the middle, and abandoning before the interesting parts. Principles 1–9 come from the three merged prompts; Principle 10 was added on user request.

1. **MVP first, ship every 2–3 weeks.** Invisible work kills solo momentum. Every milestone must end with something demo-able.

2. **Introduce complexity only when the current design hurts — OR when it's a named learning milestone.** "I want to learn Kafka" is a valid reason *if* you write it down as such in an ADR. It is never "architectural necessity."

3. **Keep the discipline, drop the theater.** ADRs, PR descriptions written for a future reader, Definition of Done, CHANGELOG → keep. Solo standups, velocity tracking, sprint retros with yourself, estimation rituals → drop.

4. **Domain first, architecture second.** The booking domain has real complexity (concurrency, availability ranges, idempotency, cancellations, timezones, pricing). Let that drive the architecture — don't impose architecture and fit the domain into it.

5. **Vertical slices in Phase 1, Clean Arch per-module in Phase 3.** Organize Phase 1 code by feature (`Features/Bookings/Create/`), not by layer. Migrate to Clean Architecture when modules make the boundaries real.

6. **Boring tech first.** Postgres, EF Core, React + Tailwind. Exotic tech only when a phase justifies it.

7. **Event-ready, not event-sourced.** Raise domain events from day 1 so adding RabbitMQ in Phase 4 is additive, not a rewrite.

8. **Don't conflate naming with practice.** Command/Query-named handlers against one DB = handler-based organization, not CQRS. True CQRS = split read/write stores (Phase 5). Be precise with yourself about what you're actually doing.

9. **3× timeline multiplier for solo evening work.** Any estimate from a tutorial triples.

10. **Warn on anti-patterns, unprompted.** If Claude Code (or user) proposes a decision, name, or pattern that clashes with principles 1–9 or with the red-flag list in `CLAUDE.md`, flag it **before** writing the ticket or code, not after. Silent compliance is a bug. Applies during discussion, ticket drafting, implementation, and PR review.

11. **Teach, don't just suggest.** When Claude Code introduces a concept, pattern, or technique the user hasn't asked about by name (e.g. "use DDD aggregates," "add a covering GIN index," "outbox pattern," "event sourcing vs event-driven," "exclusion constraints") — accompany the suggestion with **one high-quality learning reference** (article / video / book chapter / RFC / original paper). Don't fabricate URLs; if unsure of the exact URL, cite title + author + where to search. One canonical source beats ten mediocre ones. Applies equally in discussion, ticket descriptions, PR-review comments, and ADRs.

---

## 3. Tech stack

### 3.1 Phase 1 (MVP)

| Area | Choice | Notes |
|---|---|---|
| Runtime | **.NET 10 (LTS)** | Released Nov 2025; supported through Nov 2028 — covers the full project arc. |
| Architecture | **Vertical slices** — `Features/<Domain>/<Action>/` | Migrate to Clean Architecture *per module* in Phase 3. |
| Handler library | **Plain handler classes** + small `IRequestHandler<TReq, TRes>` interface | **Do not adopt MediatR — it went commercial in 2024.** Upgrade to source-gen [`Mediator`](https://github.com/martinothamar/Mediator) (MIT) in Phase 3 if dispatch ceremony hurts. FastEndpoints optional if endpoint-per-file is preferred. |
| ORM | **EF Core 10** | `DbContext.SaveChangesAsync` **is** the Unit of Work. No custom `IUnitOfWork`, no generic `IRepository<T>` — add only if testing forces it. |
| Validation | **FluentValidation** | — |
| Database | **PostgreSQL 16** (via `docker compose` locally) | `daterange` + GiST exclusion constraint = correct availability model. |
| Migrations | EF Core migrations | — |
| Auth | **Clerk** (managed) | Don't roll ASP.NET Identity. Clerk chosen over Supabase Auth (pulls you into their DB) and Auth0 (enterprise overkill). |
| API docs | **Scalar** | Better DX than Swagger UI; same OpenAPI backend. |
| Logging | **Serilog** + structured logging | Correlation-id middleware from day 1. |
| Tests | **xUnit + FluentAssertions + Testcontainers** | Real Postgres in integration tests from day 1; mocks would mask the concurrency bugs this project exists to teach. |
| Frontend | **React 18 + Vite + TypeScript strict** (no `any`) | — |
| Server state | **TanStack Query** | — |
| UI | **Tailwind + shadcn/ui** | Pairs with Figma-to-code. |
| Containers | **Docker + `docker compose`** | `up` = API + Postgres + FE. |
| CI | **GitHub Actions** | Build + test + container-build on PR. |
| Hosting | **Local `docker compose` (Phase 0–1 dev)** → **Fly.io** (Phase 1 demo milestone) | No paid infra until there's something to demo. |

### 3.2 Later phases (do NOT touch before the phase justifies it)

- **Phase 3** — split into modules (`StayNGo.Modules.Identity`, `.Catalog`, `.Booking`, `.Notifications`). Clean Arch *per module*. Own EF schema per module. Cross-module talk = public contracts + in-process events.
- **Phase 4** — RabbitMQ via MassTransit, **outbox** pattern, Redis (cache + distributed lock on booking concurrency). Concrete use case: `BookingConfirmed` → Notifications sends email + Reviews schedules review prompt. Infra migration from Fly.io to VPS + compose + Caddy.
- **Phase 5** — CQRS read models in MongoDB or Elasticsearch. *Now* CQRS actually means split read/write stores.
- **Phase 6** — Kafka + warehouse (dbt on Postgres, or BigQuery sandbox), dashboards (Grafana/Metabase). K3s on VPS **only if** multi-service operations become painful.

---

## 4. Phased roadmap

### Phase 0 — Foundations *(1–2 weekends, 10–20 hrs)*

- Monorepo created: `/backend`, `/frontend`, `/infra`, `/docs`.
- `StayNGo.md` + `CLAUDE.md` + `README.md` + `CHANGELOG.md` committed.
- `docker-compose.yml` — Postgres 16 + placeholder API + placeholder FE; `docker compose up` works.
- GitHub Actions CI: lint + test + container-build on PR.
- Issue template + PR template + automated PR-review workflow wired.
- ADR template + ADR-0001 (initial stack) + ADR-0002 (monorepo) committed.
- GitHub Project board: 8 epics + Phase 0 seed tickets loaded.
- **Done =** `docker compose up` works locally; CI green on a trivial PR; board populated.

### Phase 1 — MVP monolith *(3–4 months @ 10 hrs/week)*

Nine user flows (see §5.5). Deploy to Fly.io as the final milestone.
- **Two user personas (not roles):** every account is both a potential guest and potential host. No admin. No role enum. No permissions table. Authorization is **ownership-based**: "can I edit this listing? — am I the owner?"; "can I cancel this booking? — am I the booking guest?".
- **One frontend app** (React SPA). Guest-facing routes are public / authenticated; host-facing routes are under `/host/*`, guarded only by "user is logged in." Same app, same deploy, same components.
- **Domain enforced at DB level**, not app level: Postgres `EXCLUDE USING gist` on `(listing_id, during)` WHERE `status = 'Confirmed'`.
- **Done =** a stranger can register, publish a listing, and book someone else's listing via the live URL.

### Phase 2 — Quality & polish *(1 month)*

- `ProblemDetails` error responses.
- OpenTelemetry → Grafana Cloud free tier.
- Staging + prod environments on Fly.
- CI/CD with manual prod approval.
- Correlation IDs across requests.
- Rate limiting middleware.

### Phase 3 — Modular monolith + Clean Arch per module *(2–3 months)*

- Reorganize into `StayNGo.Modules.{Identity, Catalog, Booking, Notifications}`.
- Each module: own DB schema, Domain/Application/Infrastructure folders.
- Cross-module talk = public contracts + in-process event bus.
- Migrate plain handlers → source-gen `Mediator` if dispatch overhead hurts.
- **ADR on module boundaries** is the interview artifact.

### Phase 4 — Async messaging + caching *(2–3 months)*

- RabbitMQ via MassTransit.
- **Outbox pattern** — the teaching is in the *why*.
- Redis: cache for listing lookups + distributed lock on booking concurrency. ADR comparing Redis lock vs Postgres advisory lock vs existing GiST exclusion.
- First cross-module async flow: `BookingConfirmed` → Notifications + Reviews.
- **Infra milestone:** migrate from Fly.io → Hetzner/DO VPS + docker-compose + Caddy. First real infra-you-own learning.

### Phase 5 — CQRS read models *(1–2 months)*

- Project Catalog + Booking write-side events into MongoDB (or Elasticsearch) for search.
- Search endpoint moves from EF to the read store.
- Now it's actually CQRS.

### Phase 6 — Streaming + warehouse *(open-ended)*

- Kafka for one real use case (e.g. `PageViewed` stream → funnel).
- dbt on Postgres → Grafana/Metabase dashboards.
- Extract one module to a service **only if** ops pain is real.
- K3s on VPS **only if** multi-service orchestration hurts.

### Cross-cutting

- Every 2 phases → `docs/retros/phase-N.md` — honest retro on what worked, what didn't, what you'd change. These become interview stories.
- **Honesty rule:** if at Phase 3 you'd rather touch Kafka next than build modules, that's fine — write an ADR naming it a learning milestone and re-order. Never fake architectural necessity.

### Database learning milestones by phase

Concrete DB concepts each phase deliberately exercises. Each milestone = write a short `docs/db-notes/NNN-topic.md` summarizing what you learned (with `EXPLAIN ANALYZE` output where relevant). These become portable interview talking points.

| Phase | DB concepts introduced |
|---|---|
| **1** | `daterange` type; **GiST exclusion constraint** for no-double-booking; `btree_gist` extension; idempotency-key table design with TTL; Testcontainers for real-Postgres integration tests; EF Core migrations + hand-edited SQL for constraints EF can't express |
| **2** | `EXPLAIN (ANALYZE, BUFFERS)` reading; **B-tree vs GIN vs GiST vs BRIN** — when each wins; covering indexes with `INCLUDE`; partial indexes; statistics (`pg_stat_statements`, `pg_stat_user_indexes`); connection pooling (PgBouncer transaction mode vs session mode) |
| **3** | Schema-per-module (one physical DB, logical schemas: `identity`, `catalog`, `booking`, `notifications`); foreign keys across schemas vs cross-schema event contracts; migration coordination across modules |
| **4** | **Isolation levels** (`READ COMMITTED` default vs `REPEATABLE READ` vs `SERIALIZABLE`) and what anomalies each permits; row-level locking (`SELECT ... FOR UPDATE`, `FOR UPDATE SKIP LOCKED`); **advisory locks** (session vs transaction); outbox-table pattern with `SKIP LOCKED` worker; comparing Redis lock vs advisory lock vs exclusion constraint for booking concurrency (ADR) |
| **5** | Logical replication basics; **CDC** (Debezium conceptually, if not adopted); projection stores (MongoDB document model for denormalized search, or Elasticsearch for relevance); **projection lag** and eventual consistency UX; idempotent projection handlers |
| **6** | **Table partitioning** (range/list/hash); materialized views + refresh strategies; warehouse schemas (star, snowflake); OLTP vs OLAP separation; dbt models against Postgres |

---

## 5. Domain sketch

### 5.1 Aggregates (Phase 1)

- **User** — `Id, ClerkId, Email, DisplayName, CreatedAt`. **No role column.** "Host" is emergent: a user who owns at least one listing acts as a host in the context of that listing. No admin.
- **Listing** — owned by a User (implicit Host). Has title, description, image URLs (array of strings), nightly price cents, capacity, IANA timezone, address. Lifecycle state.
- **Booking** — a User's reservation of a Listing for a date range. Has lifecycle state.
- **Review, Payment** — Phase 2+.

### 5.2 Lifecycles

```
Listing:   [Draft] → [Active] → [Archived]

  Draft     — private, only the owner sees it; fully editable.
  Active    — public, appears in search; owner can edit price/description/photos.
  Archived  — hidden from search; read-only; preserves booking history.

  Transitions are owner-initiated only. No admin, no approval gate.

Booking:   [Pending] → [Confirmed] → [CheckedIn] → [Completed]
                ↓          ↓
           [Cancelled] [Cancelled]
```

### 5.3 Invariants (DB-enforced where possible)

| Invariant | Enforcement |
|---|---|
| No overlapping `Confirmed` bookings on same Listing | **Postgres `EXCLUDE USING gist` on `(listing_id WITH =, during WITH &&)` WHERE `status = 'Confirmed'`** |
| `CheckIn < CheckOut` | `CHECK (during IS NOT NULL)` (enforced by range validity) |
| `CheckIn >= today` at creation | App-level (can't reference `now()` in a `CHECK`) |
| Only listing owner can edit / publish / archive own listing | Handler authorization — ownership check |
| Only booking guest can cancel (before check-in) | Handler authorization — ownership check |
| Only listing owner can confirm a booking for that listing | Handler authorization — ownership check |
| User cannot book their own listing | Handler check: `booking.GuestUserId != listing.OwnerUserId` |
| Only `Active` listings appear in search | Query filter (`WHERE status = 'Active'`) |
| Archived listings are immutable | Handler check on edit attempts |

### 5.4 Hard questions — **answered**

| Question | Answer |
|---|---|
| **Concurrency** — two guests book same dates simultaneously | Postgres `daterange` + GiST **exclusion constraint** on confirmed bookings. DB raises `23P01` on conflict → handler returns `409 Conflict`. No Redis, no distributed lock in Phase 1. |
| **Idempotency** — guest double-clicks "Book" | `Idempotency-Key` header on `POST /bookings`. Store `(key, user_id, request_hash, response_body, created_at)` in `idempotency_keys` table with 24h TTL. Duplicate key + same user + same request → return cached response. Duplicate key + different request → `422`. |
| **Availability query** — find listings free for X–Y | `SELECT listings WHERE NOT EXISTS (SELECT 1 FROM bookings WHERE listing_id = listings.id AND status = 'Confirmed' AND during && daterange(X, Y, '[)'))`. The GiST index that enforces the exclusion constraint doubles as the lookup index. |
| **Timezones** — whose "night" is it? | **Property's local timezone.** Store `timezone TEXT NOT NULL` (IANA string, e.g. `Asia/Dubai`) on Listing. Check-in/out as `DATE` (not `TIMESTAMP`), interpreted in listing timezone. |
| **Cancellation** | Free cancellation until **24h before check-in** in the listing's timezone. Later → cancellation-policy-per-listing. |
| **Pricing** | Flat `price_per_night_cents BIGINT NOT NULL` on Listing for MVP. Total = `price_per_night_cents × nights`. Seasonal pricing, discounts, fees → Phase 2+. |

### 5.5 MVP user flows (Phase 1 — 9 total)

1. User signs up / logs in (Clerk-hosted).
2. User creates listing → goes to `Draft`. Fully editable.
3. User publishes listing → `Active` (appears in search). Or archives it → `Archived` (hidden, immutable).
4. User searches `Active` listings (location + date range + guests + price max). Searching does not require an account.
5. User views listing detail page.
6. User (as guest) creates booking on another user's listing → `Pending`. Cannot book own listing.
7. Listing owner (as host) confirms or rejects incoming booking → `Confirmed` / `Cancelled`.
8. User views their bookings — one page with tabs or filters for "as guest" and "as host."
9. Guest cancels their booking before check-in, subject to 24h rule (in listing timezone).

Everything else (reviews, payments, messaging, photo upload, seasonal pricing, cancellation policies per listing, calendar UI, moderation / admin) = **Phase 2+**.

---

## 6. Collaboration workflow

### 6.1 The four loops

```
Loop 1 — Discussion
  User brings idea/change to Claude Code (chat).
  Claude Code asks clarifying questions, flags anti-patterns (Principle 10),
  confirms phase-fit, proposes a ticket shape. When introducing any new
  concept/pattern/technique: pairs it with a learning reference (Principle 11).

Loop 2 — Ticket drafting
  Claude Code drafts strict ticket via template.
  `gh issue create --template=ticket.yml` → added to Project board.
  Labels applied: type, phase, size, executor (by-claude | by-hand).
  When the work introduces a new concept/pattern/DB feature, Claude Code
  fills the "Learning objective" field + one canonical reference (Principle 11).
  User approves label-only edits or requests rewrite.

Loop 3 — Execution (split by executor label)
  Every line of code that ships ties to a ticket. Executor label decides who writes it.

  by-claude (mechanical):
    Claude Code creates branch feat/<N>-<kebab-summary>
    Claude Code implements + tests
    Opens PR with "Closes #N" → Loop 4 runs → user reviews & merges

  by-hand (architectural):
    User creates branch feat/<N>-<kebab-summary>
    User implements + tests; may consult Claude Code in chat during implementation
    (that chat is covered by the parent ticket and doesn't need its own)
    Opens PR with "Closes #N" → Loop 4 runs → user addresses review & merges

Loop 4 — Review (automated)
  Every PR (regardless of author) triggers pr-review.yml (Claude Code GH Action).
  Review focus: principle violations, anti-patterns, invariants skipped,
  missing tests, commit/ADR discipline. No style nitpicks (formatter handles it).
  User PRs: review comments must be addressed or explicitly overridden.
  Claude Code PRs: user reviews + merges.
```

### 6.2 Three kinds of Claude Code interaction

Only one of these requires a ticket:

| Interaction | Needs a ticket? | Example |
|---|---|---|
| Code is being produced | **Yes** — always | "Implement booking cancel endpoint." |
| Sounding-board chat during a ticket already in flight | No — covered by the parent ticket | "Inside #42, should I use advisory lock or serializable isolation?" |
| Pure architectural discussion (no code yet) | No — but if it produces a decision, spawn an `adr` ticket so the decision lands in `docs/adr/` | "Let's discuss how async messaging arrives in Phase 4." |

**Decisions never live only in chat history** — if a discussion concludes something non-obvious, the outcome is captured as an ADR via a ticket.

### 6.3 Definition of Done (per ticket)

- Code merged to `main` via PR (even self-review).
- PR description explains *why*, not just *what*.
- Tests pass in CI.
- If backend/domain code: integration test against Testcontainers Postgres added.
- Deployed to staging (auto on merge) — Phase 1+ when Fly.io is live.
- Ticket has a screenshot, terminal output, or link to running feature proving it works.
- If architectural: ADR added under `docs/adr/NNNN-slug.md`.

### 6.4 Branching, commits, CHANGELOG

- **Branching:** GitHub Flow. `main` always deployable. Feature branches → PR → squash merge.
- **Branch naming:** `<type>/<ticket-number>-<kebab-summary>` (e.g. `feat/42-add-booking-cancel`).
- **Commits:** [Conventional Commits](https://www.conventionalcommits.org/) (`feat:`, `fix:`, `chore:`, `refactor:`, `docs:`, `test:`).
- **CHANGELOG.md:** updated as part of every PR that ships a user-visible change.

---

## 7. Using this doc with AI assistants

When starting a new conversation with an AI about StayNGo, paste:
- §1 (identity & goals)
- §2 (10 principles)
- §3.1 (current tech stack table)
- The current phase
- The specific task or question

That's enough context for aligned advice without reconstructing the whole plan.

---

## Source

Full spec with merge audit trail: [`docs/superpowers/specs/2026-04-19-staynGo-design.md`](./docs/superpowers/specs/2026-04-19-staynGo-design.md).
