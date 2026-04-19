# StayNGo — Project Brief & Working Document

> A single source of truth for the project. Use this file as:
> 1. Context to paste into AI assistants when working on the project.
> 2. A reference when making architectural decisions.
> 3. A living doc — update it when decisions change.

---

## 1. Project Identity

**Name:** StayNGo
**Type:** Booking website (stays / short-term rentals).
**Goal:** A long-running side project to practice full-stack + DevOps end-to-end, deliberately simulating a small-company workflow while working solo.

**Non-goal:** Production product for real users. Every architectural decision should be justified by *learning value* or *domain pain*, not by "real users need this."

---

## 2. Guiding Principles (read these before every phase)

These exist because the default failure mode of this kind of project is over-engineering early, under-shipping in the middle, and abandoning before the interesting parts.

1. **Introduce complexity only when the current design hurts.** Kafka, Redis, modular monolith, CQRS read models, K8s — none of these go in until you can write down the specific problem in *your* app that they solve. "I want to learn X" is a valid reason, but name it honestly: it's a learning milestone, not an architectural necessity.

2. **Ship something deployable every 2–3 weeks.** Solo projects die in month 3 when the work becomes invisible. Every milestone ends with something visibly working at a URL, even if ugly.

3. **Keep the discipline, drop the theater.** ADRs, PR descriptions written for a future reader, a Definition of Done, a CHANGELOG — keep. Standups with yourself, velocity tracking, sprint retros solo — drop. You're one person; the goal is engineering discipline, not roleplay.

4. **Domain first, architecture second.** The booking domain has real complexity (concurrent reservations, availability ranges, idempotency, cancellations, timezones, pricing). Let that complexity drive the architecture — don't impose architecture and fit the domain into it.

5. **Vertical slices over horizontal layers at the start.** Organize Phase 1 code by feature (folders like `Features/Bookings/Create`), not by layer (`Domain/Application/Infrastructure`). Migrate to Clean Architecture in Phase 3 when modules make the boundaries real.

6. **Don't conflate naming with practice.** Command/query handlers against one DB is handler-based organization, not CQRS. True CQRS (split read/write stores) comes in Phase 5. Be precise with yourself about what you're actually doing.

7. **Realistic timelines.** Multiply any estimate by 3 for solo evenings/weekends work. The whole roadmap is a 12–18 month arc — plan motivation accordingly.

---

## 3. Tech Stack Decisions

### Phase 1 (MVP)

| Area | Choice | Why |
|------|--------|-----|
| Backend | .NET 8 or 9 | Chosen by user; stable, great tooling. |
| Architecture | Vertical Slice (folders by feature) | Migrate to Clean Architecture in Phase 3. |
| Handler library | **No MediatR** (it went commercial in 2024). Use plain handler classes, or `Mediator` (source-gen, MIT), or FastEndpoints. | Don't adopt a commercial default accidentally. |
| ORM | EF Core | Fine. `DbContext.SaveChangesAsync` *is* your Unit of Work — don't write a custom UoW abstraction yet. |
| Validation | FluentValidation | OK. |
| DB | PostgreSQL | Chosen. Works on every free tier. |
| Auth | **Managed (Clerk / Supabase Auth / Auth0 free tier)** | Rolling ASP.NET Core Identity is weeks of yak-shaving. Skip it. |
| API docs | Swagger / Scalar | Either. |
| Logging | Serilog + structured logging | Yes. |
| Tests | xUnit + FluentAssertions + Testcontainers | Testcontainers from day 1 — integration tests against real Postgres. |
| Frontend | React + Vite + TypeScript | Chosen. |
| Server state | TanStack Query | Yes. |
| UI | Tailwind + shadcn/ui | Pairs well with Figma-generated designs. |
| Containers | Docker + `docker compose` for local | `up` brings API + Postgres. |
| CI | GitHub Actions | Build + test on PR. |
| Hosting | **Render** | Chosen. Caveats below. |

### Render caveats (know these before you commit)
- Free web services spin down after inactivity → 30+ sec cold starts. Annoying during demos.
- Free Postgres expires after 90 days.
- Budget ~$7–14/month from month 3, or plan a migration path.

### Later phases (do NOT touch until the phase justifies it)
- Phase 3: reorganize into modules (modular monolith), introduce Clean Architecture *per module*.
- Phase 4: RabbitMQ + MassTransit, outbox pattern, Redis (caching + distributed locks on booking concurrency).
- Phase 5: CQRS read models in MongoDB or Elasticsearch.
- Phase 6: Kafka, warehouse (Postgres-as-warehouse + dbt, or BigQuery sandbox), dashboards. K8s only if any module is genuinely strained.

---

## 4. Phased Roadmap (realistic)

### Phase 0 — Foundations (1–2 weekends)
- Write the **domain sketch** (Section 6 of this doc).
- Create Jira project with epics matching the domain.
- Create repos: `stayngo-backend`, `stayngo-frontend`, `stayngo-infra`.
- Create Figma file with a minimal design system page (colors, typography, spacing, button, input, card).
- Decide auth provider, create account.
- Write `README.md` and `docs/adr/0001-initial-stack.md` in backend repo.

### Phase 1 — MVP monolith (3–4 months realistic)
Scope: register/login (via managed auth), list property as host, search properties, create booking with date-range availability check, view my bookings. Deployed to Render with a real URL.

**Done = deployed, tested end-to-end, has at least one ADR explaining the non-obvious choices.**

### Phase 2 — Quality & polish (3–6 weeks)
ProblemDetails error handling, OpenTelemetry → Grafana Cloud free tier, staging vs prod environments on Render, secrets via Render env vars (not committed), CI/CD pipeline with manual approval to prod.

### Phase 3 — Modular monolith + Clean Architecture (2–3 months)
Reorganize into modules (Identity, Catalog, Booking, Notifications). Each module gets Domain/Application/Infrastructure folders and its own DB schema. Modules talk through public contracts + in-process events.

### Phase 4 — Async messaging (2–3 months)
RabbitMQ via MassTransit. Outbox pattern. Redis for caching + distributed locks. Real use case: `BookingConfirmed` → Notifications sends email + Reviews schedules a review prompt.

### Phase 5 — CQRS read models (1–2 months)
Project property/booking data into MongoDB or Elasticsearch for search. Now CQRS actually means split read/write stores.

### Phase 6 — Streaming + warehouse (open-ended)
Kafka, warehouse, dbt, dashboards. Extract one module to a service if it's genuinely strained. K8s only if multi-service operations become painful.

---

## 5. Workflow Conventions

### Definition of Done (per ticket)
- [ ] Code merged to `main` via PR (even self-review).
- [ ] PR description explains *why*, not just *what*.
- [ ] Tests pass in CI.
- [ ] Deployed to staging (auto on merge).
- [ ] Ticket has a screenshot or terminal output proving it works.
- [ ] If the change is architectural: ADR added under `docs/adr/`.

### Branching
GitHub Flow. `main` is always deployable to staging. Feature branches → PR → squash merge.

### ADRs
`docs/adr/NNNN-short-title.md` — Michael Nygard template:
- Context
- Decision
- Consequences

Write one whenever you make a choice a future reader (or future you) would wonder about.

### What to keep from "company workflow"
- Jira tickets for everything (even 10-min tasks — trains the muscle).
- PR descriptions.
- ADRs.
- CHANGELOG.md.
- Staging → prod promotion.

### What to drop
- Solo standups.
- Velocity / story points.
- Sprint retros with yourself.
- Estimation rituals.

---

## 6. Domain Sketch (FILL THIS IN BEFORE JIRA)

This is the part that matters most. Don't skip it.

### 6.1 Core concepts (aggregates)

- **User** — has a role: Guest, Host, or both.
- **Property** — owned by a Host. Has address, photos, price per night, capacity, availability calendar.
- **Booking** — a Guest's reservation of a Property for a date range. Has a lifecycle (states).
- **Review** — later.
- **Payment** — later.

### 6.2 Booking lifecycle (sketch — expand this)

```
[Pending] → [Confirmed] → [CheckedIn] → [Completed]
    ↓            ↓
[Cancelled]  [Cancelled]
```

### 6.3 Invariants (non-negotiable rules the code must enforce)

- A Property cannot be double-booked for overlapping date ranges in `Confirmed` state.
- CheckIn date must be strictly before CheckOut date.
- Booking dates must be in the future at creation time.
- Only the Host who owns the Property can confirm bookings for it.
- Only the Guest who made the Booking can cancel it (before check-in).

### 6.4 Hard questions to answer (TODO — answer before Phase 1 starts)

- [ ] **Concurrency:** two guests book the same dates at the same millisecond. How do I prevent double-booking? (DB unique constraint on overlapping ranges? Pessimistic lock? Optimistic with retry?)
- [ ] **Idempotency:** guest clicks "Book" twice. How do I prevent two bookings? (Idempotency key header? DB constraint?)
- [ ] **Availability query:** how do I efficiently find properties available for dates X–Y? (Postgres `daterange` + GiST index is the right answer — write it down.)
- [ ] **Timezones:** is a "night" defined in the property's local timezone or the guest's? (Spoiler: property's. Write the rule down.)
- [ ] **Cancellation policy:** free cancellation window? Refund rules? (Keep simple in MVP: free cancellation until 24h before check-in.)
- [ ] **Pricing:** flat nightly rate for MVP. Seasonal pricing and discounts later.

### 6.5 Top 8 user flows (MVP)

1. Guest signs up / logs in.
2. Host signs up, switches to host mode, creates a property listing (title, description, photos, price, capacity).
3. Guest searches properties by location + date range + guests.
4. Guest views a property detail page.
5. Guest creates a booking for a date range → goes to Pending.
6. Host sees incoming bookings → confirms or rejects → goes to Confirmed.
7. Guest views their bookings list.
8. Guest cancels a booking (before check-in).

Everything else is Phase 2+.

---

## 7. Jira Epic Structure (once the domain sketch above is done)

Proposed epics — derived from aggregates + flows above:

- **EPIC: Foundations** — repos, CI, Docker compose, Postgres locally, Render setup, first deploy of empty API.
- **EPIC: Identity & Auth** — managed auth integration, JWT validation in API, user profile endpoint.
- **EPIC: Property Catalog** — create/update/delete listing, photo upload, list mine.
- **EPIC: Search** — search by location + date + guests, filters, pagination.
- **EPIC: Booking** — create pending booking, host confirms/rejects, guest cancels, my bookings list. Concurrency handling.
- **EPIC: Frontend Shell** — routing, layout, auth flows, design system components.
- **EPIC: Observability** — structured logging, error handling, basic metrics.

Each epic breaks into stories sized for one sitting (2–4 hours max). If a story is bigger, split it.

---

## 8. Next Concrete Actions

1. [ ] Answer the 6 "hard questions" in Section 6.4 — paste answers back into this doc.
2. [ ] Decide auth provider (Clerk vs Supabase Auth vs Auth0). Write a one-paragraph ADR.
3. [ ] Create the Jira project and the 7 epics in Section 7.
4. [ ] Create the three repos + a minimal README in each.
5. [ ] Set up the Figma file with a design system page.
6. [ ] Write the first sprint's stories for the **Foundations** epic only. Don't plan further ahead yet.

---

## 9. How to use this file with AI assistants

When starting a new conversation with an AI about this project, paste:
- Section 1 (identity)
- Section 2 (principles)
- Section 3 (current tech stack)
- The current phase
- The specific task

That gives the assistant enough context to give aligned advice without reconstructing the whole plan each time.

---

## 10. Changelog of this document

- `YYYY-MM-DD` — Initial version after planning conversation. Key revisions from first draft: vertical slice (not Clean Arch) for Phase 1; dropped MediatR default (now commercial); managed auth instead of ASP.NET Identity; tripled timelines; domain sketch before Jira; honest "keep discipline, drop theater" section.
