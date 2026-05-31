# Claude Code operational rules — StayNGo

## The 11 principles

1. **MVP first, ship every 2–3 weeks.** Invisible work kills solo momentum. Every milestone must end with something demo-able.

2. **Introduce complexity only when the current design hurts — OR when it's a named learning milestone.** "I want to learn Kafka" is a valid reason *if* you write it down as such in an ADR. It is never "architectural necessity."

3. **Keep the discipline, drop the theater.** ADRs, PR descriptions written for a future reader, Definition of Done, CHANGELOG → keep. Solo standups, velocity tracking, sprint retros with yourself, estimation rituals → drop.

4. **Domain first, architecture second.** The booking domain has real complexity (concurrency, availability ranges, idempotency, cancellations, timezones, pricing). Let that drive the architecture — don't impose architecture and fit the domain into it.

5. **Vertical slices in Phase 1, Clean Arch per-module in Phase 3.** Organize Phase 1 code by feature (`Features/Bookings/Create/`), not by layer. Migrate to Clean Architecture when modules make the boundaries real.

6. **Boring tech first.** Postgres, EF Core, React + Tailwind. Exotic tech only when a phase justifies it.

7. **Event-ready, not event-sourced.** Raise domain events from day 1 so adding RabbitMQ in Phase 4 is additive, not a rewrite.

8. **Don't conflate naming with practice.** Command/Query-named handlers against one DB = handler-based organization, not CQRS. True CQRS = split read/write stores (Phase 5). Be precise with yourself about what you're actually doing.

9. **3× timeline multiplier for solo evening work.** Any estimate from a tutorial triples.

10. **Warn on anti-patterns, unprompted.** If Claude Code (or user) proposes a decision, name, or pattern that clashes with principles 1–9 or with the red-flag list below, flag it **before** writing the ticket or code, not after. Silent compliance is a bug. Applies during discussion, ticket drafting, implementation, and PR review.

11. **Teach, don't just suggest.** When Claude Code introduces a concept, pattern, or technique the user hasn't asked about by name (e.g. "use DDD aggregates," "add a covering GIN index," "outbox pattern," "event sourcing vs event-driven," "exclusion constraints") — accompany the suggestion with **one high-quality learning reference** (article / video / book chapter / RFC / original paper). Don't fabricate URLs; if unsure of the exact URL, cite title + author + where to search. One canonical source beats ten mediocre ones. Applies equally in discussion, ticket descriptions, PR-review comments, and ADRs.

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
- Tickets describe **business + requirements only — no code**. Architecture/infrastructure changes are **`[internal]`** tickets that state the value ("what this gives us") and are need-driven — see **Ticket content** below.

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

## Ticket content (business vs `[internal]`)
- **Feature tickets describe the business part only** — WHAT the user gets + acceptance criteria, in business terms. **No code, no file-by-file implementation detail.** The implementer owns the *how* (for `by-hand`, that's the user — the learning lives there).
- **Architecture / infrastructure changes are `[internal]` tickets** (label `internal`). State **the value — "what this gives us"** — not just the action, and keep them **need-driven**: a real, observed problem spawns the ticket. Examples: `/me` slow → `[internal]` "add Redis caching"; JIT-provisioned profile goes stale → `[internal]` "add profile-sync webhook"; outgrow a vendor → `[internal]` "replace Clerk with <X>".
- An `[internal]` change a future reader would question still earns an ADR (see Hard rules).

## PR review mode (GH Actions)
- Focus: principle violations, anti-patterns, invariants skipped, test gaps.
- Do not nitpick style — the formatter handles it.
- When flagging, cite the principle number or red-flag entry.
- Never auto-approve a PR — only comment.

## See also
- Project brief: ./StayNGo.md
- ADRs: ./docs/adr/
- Domain invariants: ./StayNGo.md §5.3
