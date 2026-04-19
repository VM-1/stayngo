# StayNGo — Consolidated Project Design

> **Date:** 2026-04-19
> **Status:** Accepted (brainstorming complete)
> **Owner:** valiko.mkhitaryan@newguys.io
> **Supersedes:** `Documents/staynGo_master_prompt_Claude.md`, `Documents/staynGo_master_prompt_Gemini.md`, `Documents/stayngo_master_prompt_ChatGPT.md`, `Documents/Multi‑asset Booking Platform — Mvp Backlog.md` (merged, flaws removed)

---

## 0. Purpose of this document

This is the **single authoritative spec** for StayNGo. It consolidates three prior master prompts and one MVP backlog document into one design, with known flaws removed (see §13 for what was dropped and why).

This doc is the input to the `writing-plans` skill which will produce the step-by-step implementation plan for Phase 0. After Phase 0 is executed, the canonical docs in the repo become:

- `StayNGo.md` — human-readable project brief (§1–§5 of this doc, lightly formatted)
- `CLAUDE.md` — Claude Code operational rules (§7.1)
- `.github/ISSUE_TEMPLATE/ticket.yml` — strict ticket template (§7.2)
- `.github/workflows/pr-review.yml` — automated PR review workflow (§7.3)
- `docs/adr/*` — ADRs

This spec file remains at `docs/superpowers/specs/2026-04-19-staynGo-design.md` as historical record.

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

## 7. Operational files — contents

These files are **created during Phase 0**. Full contents below.

### 7.1 `CLAUDE.md` (repo root, auto-loaded by Claude Code)

```markdown
# Claude Code operational rules — StayNGo

## The 10 principles
(copy-paste the list from StayNGo.md §2 verbatim — kept in sync via PR review)

## Red-flag anti-patterns (warn before writing, per Principle 10)
- Generic repositories (`IRepository<T>`) over EF Core
- Anemic domain models (all logic in handlers; entities are bags)
- Sync HTTP between modules — use in-process events instead (Phase 3+)
- CQRS-named handlers against one DB (that's handler-based org, not CQRS — Principle 8)
- Catch-log-swallow without rethrow/return
- Primitives over value objects for DateRange, Money, IANA timezone
- Missing idempotency on state-changing endpoints
- Missing DB constraint where one is possible (e.g. no uniqueness on email)
- Premature abstraction (extract-method-of-one; `IFoo` with one implementation)
- "Temporary" code without a dated ticket

## Hard rules
- Never start coding without a ticket on the Project board.
- Never bypass the PR review workflow on `main`.
- Never introduce Redis / Kafka / RabbitMQ before the phase permits it.
- Never add MediatR (commercial). Use plain handlers or source-gen Mediator.
- Every architectural choice a future reader would wonder about → ADR.
- When drafting a ticket that touches a new concept/pattern/DB feature, fill the **Learning objective** field and **Learning references** field in the template (Principle 11). Code tickets that teach a concept gain an acceptance bullet "write `docs/db-notes/NNN-<topic>.md` in own words (~300 words)".

## Learning-reference rule (Principle 11)
When introducing a concept/pattern/technique the user didn't name:
- Pair the suggestion with **one canonical reference** (article/video/book/RFC/paper).
- Format: "Introduces <concept>. Reference: <Title> — <Author/Source>, <URL> (why: <1 line>)"
- Prefer authoritative sources. Good defaults to reach for:
  - **DDD / patterns** — Martin Fowler's bliki (martinfowler.com/bliki), Vaughn Vernon's *Implementing DDD*, Eric Evans's *DDD* (book chapters)
  - **PostgreSQL — indexing & query tuning** — Use The Index, Luke (use-the-index-luke.com) · sql-performance.com (Markus Winand) · pganalyze.com/blog (Lukas Fittl on planner/indexes) · official docs §11 "Indexes" and §14 "Performance Tips"
  - **PostgreSQL — internals & concurrency** — "PostgreSQL 14 Internals" by Egor Rogov (free PDF, postgrespro.com/community/books/internals) · Bruce Momjian's talks (momjian.us/main/presentations) · official docs §13 "Concurrency Control" (MVCC, isolation, locks)
  - **Database systems (general depth)** — CMU 15-445 (Intro) and 15-721 (Advanced) by Andy Pavlo — full lectures on YouTube; *Designing Data-Intensive Applications* by Martin Kleppmann (book)
  - **Distributed systems / consistency** — Jepsen (jepsen.io), Fowler on outbox / saga / eventual consistency
  - **.NET architecture** — Microsoft Learn architecture guides (learn.microsoft.com/dotnet/architecture) · Andrew Lock's blog (andrewlock.net) · Jimmy Bogard's posts (jimmybogard.com)
  - **Systems / perf broadly** — Dan Luu (danluu.com) · Marc Brooker (brooker.co.za/blog)
- Don't fabricate URLs. If unsure of the exact URL, cite title + author + where to search (e.g. "search 'Martin Fowler outbox pattern'").
- One good source > ten mediocre ones. Two only if the concept has clear "intro" vs "depth" tiers.

## Ticket handoff
- `by-claude` tickets: pick up, implement, open PR with "Closes #N".
- `by-hand` tickets: do not implement — offer sounding-board advice if asked.
- If a `by-hand` ticket requests implementation from Claude Code, refuse and ask user to re-label.

## PR review mode (GH Actions)
- Focus: principle violations, anti-patterns, invariants skipped, test gaps.
- Do not nitpick style — the formatter handles it.
- When flagging, cite the principle number or red-flag entry.
- Never auto-approve a PR — only comment.

## See also
- Project brief: ./StayNGo.md
- ADRs: ./docs/adr/
- Domain invariants: ./StayNGo.md §5.3
```

### 7.2 `.github/ISSUE_TEMPLATE/ticket.yml`

```yaml
name: Work item
description: StayNGo ticket
title: "[feature|chore|bug|refactor|adr|spike] <imperative summary>"
labels: ["triage"]
body:
  - type: dropdown
    id: phase
    attributes:
      label: Phase
      options: ["0", "1", "2", "3", "4", "5", "6"]
    validations:
      required: true
  - type: dropdown
    id: size
    attributes:
      label: Size
      options: ["S (<2h)", "M (2-4h)", "L (4-8h)"]
    validations:
      required: true
  - type: dropdown
    id: executor
    attributes:
      label: Executor
      options: ["by-claude", "by-hand"]
    validations:
      required: true
  - type: textarea
    id: context
    attributes:
      label: Context
      description: "1-3 sentences. Why this ticket exists; what flow/problem it serves."
    validations:
      required: true
  - type: textarea
    id: acceptance
    attributes:
      label: Acceptance criteria
      description: "Checklist bullets. For tickets that introduce a new concept, include a bullet like: 'docs/db-notes/NNN-<topic>.md written in own words (~300 words)'."
      value: |
        - [ ]
        - [ ]
    validations:
      required: true
  - type: textarea
    id: learning_objective
    attributes:
      label: Learning objective
      description: "If this ticket exercises or teaches a new concept (pattern, technique, DB feature, framework capability), state it in one sentence. Leave blank for pure plumbing tickets."
      placeholder: "e.g., Postgres `daterange` + GiST exclusion constraint for no-double-booking."
  - type: textarea
    id: references
    attributes:
      label: Learning references
      description: "Per Principle 11. Canonical sources only (title + author + URL). Don't fabricate URLs."
      placeholder: |
        - Postgres docs §8.17 "Range Types" — postgresql.org/docs/current/rangetypes.html
        - Use The Index, Luke — use-the-index-luke.com (search 'exclusion constraint')
  - type: textarea
    id: invariants
    attributes:
      label: Invariants to respect
      description: "Domain rules this ticket must not violate. Reference StayNGo.md §5.3."
  - type: textarea
    id: out-of-scope
    attributes:
      label: Out of scope
      description: "Explicit non-goals. Prevents scope creep."
    validations:
      required: true
```

**Type conventions:**
- `feature` — user-visible new behavior.
- `chore` — scaffolding, config, non-behavioral work.
- `bug` — fix for a broken behavior.
- `refactor` — restructuring with no behavior change.
- `adr` — decision-recording ticket; produces a file under `docs/adr/`.
- `spike` — pure study / throwaway exploration; produces a note under `docs/db-notes/` or `docs/notes/`, not production code.

### 7.3 `.github/workflows/pr-review.yml`

Exact action name + parameters to be confirmed at implementation time against current [`anthropics/claude-code-action`](https://github.com/anthropics/claude-code-action) docs. Indicative shape:

```yaml
name: PR review (Claude Code)

on:
  pull_request:
    types: [opened, synchronize, reopened]

jobs:
  review:
    runs-on: ubuntu-latest
    permissions:
      contents: read
      pull-requests: write
      issues: read
    steps:
      - uses: actions/checkout@v5
        with:
          fetch-depth: 0
      - uses: anthropics/claude-code-action@v1
        with:
          anthropic_api_key: ${{ secrets.ANTHROPIC_API_KEY }}
          mode: review
          prompt_file: .github/claude-review-prompt.md
```

Acceptance for ticket #11: the workflow runs on PR open, posts at least one review comment on a PR that deliberately violates a principle (seed test PR).

### 7.4 `.github/claude-review-prompt.md`

```markdown
You are reviewing a PR for StayNGo. Read `CLAUDE.md` and `StayNGo.md` at repo root first.

Scope of review:
1. Principle violations (cite principle number from StayNGo.md §2).
2. Red-flag anti-patterns (cite entry from CLAUDE.md).
3. Invariants from StayNGo.md §5.3 — ensure not bypassed.
4. Missing tests (especially integration tests for state-changing endpoints).
5. Missing ADR for architectural changes.
6. Commit message discipline (Conventional Commits).
7. CHANGELOG.md updated if user-visible change.

Do NOT review:
- Code style / formatting (formatter handles it).
- Preference-level bikeshedding (variable names, minor structure).
- Scope of the PR (ticket's "Out of scope" field already settled that).

Output: review comments on specific lines where issues exist.
When recommending an alternative pattern/technique, include a learning reference per Principle 11 (canonical article/video/RFC — don't fabricate URLs).
Never approve — only comment.
```

### 7.5 `.github/PULL_REQUEST_TEMPLATE.md`

```markdown
## Closes
Closes #<ticket-number>

## Why
<1-3 sentences — the motivation, not the diff>

## What changed
<bullet list of notable changes>

## Testing
- [ ] Unit tests
- [ ] Integration tests (Testcontainers)
- [ ] Manual smoke (paste terminal output or screenshot)

## Invariants checked
<reference StayNGo.md §5.3 invariants relevant to this change>

## ADR
- [ ] Added / updated under `docs/adr/` (if architectural)
- [ ] N/A

## Checklist
- [ ] CHANGELOG.md updated (if user-visible)
- [ ] PR title follows Conventional Commits
```

### 7.6 `docs/adr/template.md`

```markdown
# NNNN. <Title>

Date: YYYY-MM-DD
Status: Proposed | Accepted | Superseded by NNNN

## Context
<what situation / forces are at play>

## Decision
<the choice — active voice, declarative>

## Consequences
<positive, negative, neutral — honest>
```

### 7.7 `docs/adr/0001-initial-stack.md` (sketch)

```markdown
# 0001. Initial stack for Phase 1

Date: 2026-04-19
Status: Accepted

## Context
StayNGo is a long-running solo side project targeting interview portfolio
+ hands-on learning. Stack choices must optimize for staff-backend skills
demo + low friction for a solo maintainer.

## Decision
- Backend: .NET 10 (LTS, through Nov 2028)
- Architecture: vertical slices Phase 1 → Clean Arch per module Phase 3
- Handler library: plain handler classes (MediatR is commercial)
- ORM: EF Core 10; DbContext = UoW
- DB: PostgreSQL 16 with `daterange` + GiST exclusion constraint
- Auth: Clerk (managed)
- Frontend: React 18 + Vite + TS strict + Tailwind + shadcn/ui
- Hosting: local docker-compose → Fly.io at Phase 1 demo milestone
- Repo: monorepo

## Consequences
+ Stack covers the whole 12–18 month arc without forced migration.
+ Low friction solo — fewest moving parts.
+ Teaches the domain (GiST exclusion) at the DB level, not app level.
- .NET 10 at 5 months of age means some third-party packages may lag.
- Clerk costs if free tier is exceeded (unlikely for a demo app).
- Monorepo means one CI config to maintain; atomic cross-stack commits.
```

### 7.8 `docs/adr/0002-monorepo.md` (sketch)

```markdown
# 0002. Monorepo structure

Date: 2026-04-19
Status: Accepted

## Context
Solo developer. Backend (.NET), frontend (React), infra (docker compose, fly.toml)
all evolve in lockstep during Phase 1–3. Three separate repos would triple the
context-switch and CI-config overhead; cross-stack changes couldn't be atomic.

## Decision
Single monorepo with top-level `/backend`, `/frontend`, `/infra`, `/docs`.
One `.github/` config governs the whole repo.

## Consequences
+ Atomic PRs across stacks.
+ One Claude Code context covers everything.
+ Single CI pipeline.
- If/when one subsystem grows large enough to warrant isolation (Phase 6+),
  extraction is a deliberate move with its own ADR.
```

---

## 8. Repo layout (target at end of Phase 0)

```
stayngo/
├── StayNGo.md                        # consolidated project brief (narrative)
├── CLAUDE.md                         # Claude Code operational rules
├── README.md                         # entry point, dev setup
├── CHANGELOG.md
├── docker-compose.yml                # api + postgres + frontend
├── .env.example
├── .gitignore
│
├── .github/
│   ├── ISSUE_TEMPLATE/ticket.yml
│   ├── PULL_REQUEST_TEMPLATE.md
│   ├── workflows/
│   │   ├── ci.yml                    # build + test on PR
│   │   └── pr-review.yml             # Claude Code auto-review on PR
│   └── claude-review-prompt.md
│
├── backend/
│   └── StayNGo/                          # Phase 1: the monolith codebase. Phase 3: stays as the main module / host; sibling folders (`Modules/Identity`, `Modules/Catalog`, etc.) get added beside it.
│       ├── StayNGo.sln
│       ├── Directory.Packages.props       # central package versions
│       ├── src/
│       │   ├── StayNGo.Api/              # endpoints + Features/ vertical slices (Phase 1). Becomes the composition-root Host project in Phase 3.
│       │   │   ├── Features/
│       │   │   │   ├── Bookings/{Create, Cancel, ListMine, ConfirmByHost}/
│       │   │   │   ├── Listings/{Create, Edit, Publish, Archive, GetById, Search}/
│       │   │   │   └── Identity/{Me, OnSignUp}/
│       │   │   ├── Program.cs
│       │   │   └── appsettings.json
│       │   ├── StayNGo.Domain/            # entities, value objects, domain events — zero deps
│       │   └── StayNGo.Infrastructure/    # EF Core DbContext, migrations, Clerk client
│       ├── tests/
│       │   ├── StayNGo.UnitTests/
│       │   └── StayNGo.IntegrationTests/  # Testcontainers + real Postgres from day 1
│       └── Dockerfile
│
├── frontend/                          # ONE React app serves both guest and host views
│   ├── src/
│   │   ├── routes/
│   │   │   ├── (public)/              # /, /search, /listings/:id
│   │   │   ├── (authenticated)/       # /my/bookings, /account
│   │   │   └── host/                  # /host/listings, /host/listings/:id, /host/bookings
│   │   ├── features/                  # mirror backend features where possible
│   │   ├── components/ui/             # shadcn/ui
│   │   └── lib/                       # api client, auth, utils
│   ├── package.json
│   ├── vite.config.ts
│   ├── tsconfig.json
│   └── Dockerfile
│
├── infra/                             # empty at end of Phase 0; populated in Phase 1 (fly.toml) and Phase 4 (Caddy, compose-prod, etc.)
│
└── docs/
    ├── adr/{template.md, 0001-initial-stack.md, 0002-monorepo.md}
    ├── retros/                        # populated Phase 2+
    └── superpowers/specs/             # this spec lives here as historical record
```

**Phase 3 evolution** keeps `backend/StayNGo/` as the composition-root host (Program.cs wires modules; owns migrations orchestration) and adds sibling folders:

```
backend/
├── StayNGo/                          # host: Program.cs, module wiring, shared kernel
├── Modules/
│   ├── Identity/                     # StayNGo.Modules.Identity.{Api,Domain,Infrastructure}
│   ├── Catalog/
│   ├── Booking/
│   └── Notifications/
└── backend.sln                       # master solution referencing all projects
```

Clean Arch applied *per module*. Inter-module calls via public contracts + in-process event bus (see §2 Principle 7). Each module owns its DB schema.

---

## 9. Phase 0 execution plan — seed tickets

All 15 tickets created on the board at Phase 0 kickoff. `by-hand` marks steps where user wants to own the learning moment (typically the first interaction with a new tool).

| # | Title | Executor | Size | Epic |
|---|---|---|---|---|
| 1 | `chore: init monorepo, .gitignore, README.md, CHANGELOG.md` | by-claude | S | Foundation |
| 2 | `chore: commit StayNGo.md + CLAUDE.md` | by-claude | S | Foundation |
| 3 | `chore: scaffold backend under backend/StayNGo/ — .sln + 3 src projects (Api, Domain, Infrastructure) + 2 test projects (UnitTests, IntegrationTests with Testcontainers + xUnit + FluentAssertions) + Directory.Packages.props. The StayNGo/ subfolder is intentional: in Phase 3 it becomes the host/composition-root sitting alongside backend/Modules/*.` | by-claude | M | Foundation |
| 4 | `chore: scaffold frontend — Vite + React + TS strict + Tailwind + shadcn/ui` | by-claude | M | Foundation |
| 5 | `chore: docker-compose.yml — postgres + api + frontend; docker compose up works` | **by-hand** | M | Foundation |
| 6 | `chore: .env.example with all required vars documented` | by-claude | S | Foundation |
| 7 | `feat: EF Core DbContext + initial migration — users, listings, bookings with daterange + GiST exclusion constraint (migration first runs `CREATE EXTENSION IF NOT EXISTS btree_gist;` so the setup is portable to Fly.io without init-db scripts)` | **by-hand** | M | Foundation |
| 8 | `chore: .github/workflows/ci.yml — build + test on PR, container-build on main` | by-claude | M | Foundation |
| 9 | `chore: .github/ISSUE_TEMPLATE/ticket.yml` | by-claude | S | Foundation |
| 10 | `chore: .github/PULL_REQUEST_TEMPLATE.md` | by-claude | S | Foundation |
| 11 | `chore: .github/workflows/pr-review.yml + claude-review-prompt.md` | **by-hand** | M | Foundation |
| 12 | `docs: docs/adr/template.md + seed 0001-initial-stack.md` | by-claude | S | Foundation |
| 13 | `docs: seed ADR 0002-monorepo.md` | by-claude | S | Foundation |
| 14 | `chore: GitHub Project board v2 — columns, custom fields, labels` | **by-hand** | S | Foundation |
| 15 | `chore: create 8 epic issues on the board (Phase 0–1)` | by-claude | S | Foundation |

The 5 `by-hand` items are the genuine learning moments — docker-compose, the GiST exclusion constraint (the domain-modeling heart of Phase 1), the PR-review workflow (the DevOps heart), the Project board setup, and the ADR for the initial stack.

### 9.1 Example: ticket #7 expanded through the template

This is what a by-hand learning ticket looks like when Claude Code drafts it. Use this shape for all tickets that introduce a new concept.

```
Title: [feature] EF Core initial migration: users, listings, bookings with daterange + GiST exclusion constraint

Phase: 0
Size: M (2-4h)
Executor: by-hand

Context:
  Phase 1 hinges on the no-double-booking invariant being enforced at the
  database level, not app level. This ticket wires up the Postgres building
  blocks the rest of Phase 1 depends on: the btree_gist extension, a
  `bookings.during DATERANGE` column, and an EXCLUDE constraint that
  prevents overlapping Confirmed bookings on the same listing.

Acceptance criteria:
  - [ ] Migration creates btree_gist extension via
        `CREATE EXTENSION IF NOT EXISTS btree_gist` (portable to Fly.io).
  - [ ] `users` (id, clerk_id unique, email unique ci, display_name, created_at)
  - [ ] `listings` (id, owner_user_id FK, title, description, image_urls text[],
        price_per_night_cents bigint, capacity int, timezone text NOT NULL,
        status text NOT NULL, created_at, updated_at)
  - [ ] `bookings` (id, listing_id FK, guest_user_id FK, during daterange NOT NULL,
        status text NOT NULL, created_at)
  - [ ] EXCLUDE constraint: no two Confirmed bookings on same listing overlap.
        Tested with an integration test that inserts two overlapping Confirmed
        bookings and asserts the second raises `23P01`.
  - [ ] `docs/db-notes/001-gist-exclusion-constraint.md` written in own words
        (~300 words): what a GiST index is, why `btree_gist` is required,
        how the EXCLUDE syntax reads, and one EXPLAIN ANALYZE of the
        index in action.

Learning objective:
  Postgres `daterange` type + GiST index + EXCLUDE constraint, enforced
  at the database level — the canonical example of "domain invariants
  belong in the schema, not the handler."

Learning references:
  - Postgres docs §8.17 "Range Types" — postgresql.org/docs/current/rangetypes.html
  - Postgres docs §11.2 "Index Types" (GiST section) — postgresql.org/docs/current/indexes-types.html
  - "PostgreSQL 14 Internals" by Egor Rogov, chapter on index access methods —
    postgrespro.com/community/books/internals
  - Use The Index, Luke — use-the-index-luke.com (search "exclusion constraint")

Invariants to respect (ref StayNGo.md §5.3):
  - No overlapping `Confirmed` bookings on same Listing
  - `CheckIn < CheckOut` (enforced by daterange validity)

Out of scope:
  - Idempotency key table (separate ticket in Phase 1)
  - Reviews table, Payments table
  - Performance tuning or additional indexes beyond what the constraint requires
```

Every `by-hand` ticket in Phase 1 that touches a DB concept (idempotency keys, availability query with GiST, timezone-aware check-in) follows this shape. Claude Code drafts; user reviews + executes.

### 9.2 Suggested pre-Phase-1 study spikes (optional)

Short, throwaway-output reading tickets to prime the Phase 1 DB learning track. Each produces one file under `docs/db-notes/`. None of them block Phase 0 completion — run in any slot where you want reading time instead of typing time.

| Title | Size | Output |
|---|---|---|
| `[spike] Read & summarize: Use The Index, Luke §1–3 (Preface, Anatomy of an Index, Where Clause)` | S | `docs/db-notes/001a-btree-anatomy.md` |
| `[spike] Read & summarize: Postgres docs §11 Indexes — focus on types (B-tree, GiST, GIN, BRIN, hash)` | S | `docs/db-notes/002-pg-index-types.md` |
| `[spike] Read & summarize: Postgres docs §8.17 Range Types + Fowler "Range" bliki` | S | `docs/db-notes/003-daterange-patterns.md` |
| `[spike] Read & summarize: Fowler on Idempotency (or Stripe blog "Designing robust and predictable APIs with idempotency")` | S | `docs/db-notes/004-idempotency-keys.md` |
| `[spike] Watch & notes: CMU 15-445 Lecture 1–3 (Relational model, Data storage, B+ trees) — 3×80min` | L | `docs/db-notes/005-cmu-fundamentals.md` |

Each spike's acceptance = "the note exists, ~300 words, in own words, cites the source read." They become interview talking points.

---

## 10. GitHub Projects board

### 10.1 Columns (Status field)

`Backlog` → `Ready` → `In Progress` → `In Review` → `Done`

### 10.2 Custom fields

- **Phase** — single-select: `0`, `1`, `2`, `3`, `4`, `5`, `6`
- **Size** — single-select: `S (<2h)`, `M (2-4h)`, `L (4-8h)`
- **Executor** — single-select: `by-claude`, `by-hand`
- **Epic** — linked issue field

### 10.3 Labels

- **type:** `feature`, `bug`, `chore`, `refactor`, `adr`, `spike`
- **executor:** `by-claude`, `by-hand`
- **phase:** `phase-0`, `phase-1`, `phase-2`, `phase-3`, `phase-4`, `phase-5`, `phase-6`
- **flags:** `blocked`, `learning` (set when the ticket has a Learning objective field populated — makes learning tickets easy to filter at retro time)

### 10.4 Seeded epics (8)

1. **EPIC: Foundation / Repo / CI** — Phase 0.
2. **EPIC: Identity — Clerk** — Phase 1.
3. **EPIC: Listings** — Phase 1. CRUD + lifecycle (`Draft` → `Active` → `Archived`).
4. **EPIC: Search** — Phase 1.
5. **EPIC: Booking** — Phase 1. Concurrency, idempotency, availability, host confirm/reject.
6. **EPIC: Frontend Shell** — Phase 1. Routing (public / authenticated / `/host/*`), auth flows, layout.
7. **EPIC: Observability Basics** — Phase 1–2. Structured logging, ProblemDetails.
8. **EPIC: Deploy to Fly.io** — Phase 1 demo milestone.

Phase 2+ epics are **not** seeded — they're created when that phase begins, per the principle "don't plan further than the current horizon."

---

## 11. Using this doc with AI assistants

When starting a new conversation with an AI about StayNGo, paste:
- §1 (identity & goals)
- §2 (10 principles)
- §3.1 (current tech stack table)
- The current phase
- The specific task or question

That's enough context for aligned advice without reconstructing the whole plan.

---

## 12. Deferred / parked decisions

Decisions explicitly not settled in this spec. When Phase reaches them, spawn an ADR:

- **Moderation / admin / RBAC** — no admin role or permissions in Phase 1. If moderation (approve/remove listings, ban users) is added later, that's a new feature with its own ADR + evolution from ownership-auth to role-based auth. Interview story would then become "how I introduced RBAC once a real need emerged," which is stronger than "I added RBAC because tutorials do."
- Cancellation policy evolution (per-listing rules, partial refunds)
- Pricing (seasonal, discounts, fees, taxes)
- Photo upload (object storage choice, image processing)
- Review & rating model
- Payment provider (Stripe vs others; test mode only at first)
- Messaging between host and guest
- Email provider (Phase 4+)
- Search infrastructure choice — MongoDB vs Elasticsearch (Phase 5 ADR)
- Kafka use case specifics (Phase 6)
- VPS provider — Hetzner vs DigitalOcean (Phase 4 ADR)

---

## 13. What was merged/dropped from the source prompts

### Kept from `staynGo_master_prompt_Claude.md`
- "Keep discipline, drop theater" frame (Principle 3).
- Vertical slices → Clean Arch at Phase 3 (Principle 5).
- MediatR-is-commercial warning.
- Managed auth instead of rolling ASP.NET Identity.
- Domain sketch + invariants + 6 hard questions.
- Phased roadmap 0–6 structure.
- Michael Nygard ADR template.
- 3× timeline multiplier (Principle 9).

### Kept from `stayngo_master_prompt_ChatGPT.md`
- Advisor "role" framing (senior-level, pushback, boring solutions first).
- "v1 avoid" list (microservices, Kafka, RabbitMQ, Redis, NoSQL in Phase 1).
- Booking-domain-specific clarity-forcing questions.
- Structured advice format (Recommendation / Why / Do-now-later-not-now / Risks / Next steps) — baked into CLAUDE.md PR review instructions.

### Kept from `staynGo_master_prompt_Gemini.md`
- `StayNGo.Modules.<Name>` namespace convention for Phase 3+.
- "Event-ready so that adding a broker later is additive" (Principle 7).
- "No sprawl" discipline (embedded in Principle 2).

### Kept from `Multi‑asset Booking Platform — Mvp Backlog.md`
- 10 epics structure (adapted to 9 — `EPIC: Testing & Quality` dissolved into DoD, `EPIC: Seed & Fixtures` dropped as a Phase 1 ticket not an epic).
- Timezone default `Asia/Dubai`.
- Coverage target 70% (kept as internal aim, not CI-enforced gate in Phase 1 — it gates in Phase 2).
- Images = URLs only in Phase 1.

### Dropped (with reasons)

| Source | What | Why dropped |
|---|---|---|
| Gemini | "CQRS (MediatR) — every state change must be a Command, no exceptions" | Dogmatic; conflates naming with practice (Principle 8). MediatR is commercial (2024). |
| Gemini | "Railway/Vercel" hosting | User preference is local-first → Fly.io. |
| Gemini | "GitHub Projects via MCP" | Using `gh` CLI directly; MCP unnecessary. |
| Claude | "Render" as hosting default | User chose Fly.io (better learning). |
| Claude | "Roles: Guest / Host / Admin enum" from day 1 | **No roles at all in Phase 1** (user decision). Only ownership-based authorization: listing owner can edit/publish/archive; booking guest can cancel. No admin, no `IsAdmin bool`, no permissions. Moderation/admin/RBAC deferred entirely (see §12) — if ever added, it arrives as a documented evolution story. |
| ChatGPT | "Admin can manage bookings / listings" | No admin panel in Phase 1 — dropped entirely per user request (§12). |
| Claude | "Host submits listing → PendingApproval → Admin approves" gate | **Removed.** Listings go `Draft → Active → Archived`, owner-controlled, no approval step. |
| MABP Backlog | `mabp-*.csv` Jira exports | GitHub Projects chosen; CSVs kept as reference only. |
| MABP Backlog | Naming "MABP / Multi-asset Booking Platform" | Project name standardized to **StayNGo**. |
| All | Solo standups / velocity / sprint retros | Theater — explicitly dropped (Principle 3). |
| All | Generic `IRepository<T>` wrappers | Anti-pattern — listed in `CLAUDE.md` red-flag list. |
| All | Microservices/Kafka/RabbitMQ/Redis in Phase 1 | Violates Principle 2. Scheduled into Phases 4–6 with explicit justification. |

---

## 14. Changelog of this document

- **2026-04-19** — Initial consolidated version from brainstorming session. Merged three master prompts and one MVP backlog; dropped commercial-MediatR default, dogmatic-CQRS framing, premature roles/permissions, and solo-workflow theater. Added Principle 10 (warn on anti-patterns) and Principle 11 (teach — pair new concepts with canonical learning references), plus the four-loop collaboration workflow with executor-label split. Added §1.1 Learning tracks (with database internals as a first-class track), §4 "Database learning milestones by phase" table, and expanded Postgres/DB reference list in CLAUDE.md. **Removed admin/RBAC from Phase 1 entirely** — no admin, no `IsAdmin`, no roles, no permissions; authorization is ownership-based only. Listing lifecycle simplified to `Draft → Active → Archived` (no approval gate). 11 MVP flows → 9. 9 epics → 8 (dropped `EPIC: Admin Panel`). Moderation/RBAC moved to §12 deferred decisions. Committed to a **single frontend app** (one React SPA) with `/host/*` route tree for host views.
- **2026-04-20** — Made learning objectives first-class in the ticket template. Added `Learning objective` + `Learning references` fields to `.github/ISSUE_TEMPLATE/ticket.yml`. Added `spike` as a ticket type (pure study / throwaway exploration; produces a note under `docs/db-notes/` or `docs/notes/`). Added §9.1 "Example: ticket #7 expanded" showing the full ticket shape for learning-bearing work, and §9.2 "Suggested pre-Phase-1 study spikes" (5 optional reading tickets). Added `learning` flag label and `spike` type label in §10.3. Updated `CLAUDE.md` hard rules to mandate populating the Learning-objective field when a ticket touches a new concept.
- **2026-04-20** — Restructured backend layout: moved the monolith codebase under `backend/StayNGo/` instead of `backend/`. Phase 3 modularization will keep `backend/StayNGo/` as the composition-root host and add sibling folders under `backend/Modules/` (Identity, Catalog, Booking, Notifications). Updated §8 repo layout + Phase 3 evolution sketch + ticket #3 description.
