---
type: moc
track: [backend]
status: growing
created: 2026-05-29
---

# 🏗️ Backend architecture — MOC

Evolution: vertical slices → modular monolith → async messaging → CQRS read models. Complexity is added
only when the design hurts **or** as a named learning milestone (Principle 2). Be precise about what you're
actually doing vs. what you're naming it (Principle 8).

## Phase 1 — vertical slices
- [[Vertical slice architecture]] — `Features/<Domain>/<Action>/`
- [[Handler-based organization vs CQRS]] — naming ≠ practice (Principle 8)
- [[SaveChanges as Unit of Work]] — no generic `IRepository<T>`
- [[Domain events from day one]] — event-ready, not event-sourced
- [[Ownership-based authorization]] — no role enum, no permissions table

## Phase 3 — modular monolith
- [[Module boundaries]] — Identity / Catalog / Booking / Notifications
- [[In-process event bus]] — no sync HTTP between modules
- [[Clean Architecture per module]]
- [[Source-gen Mediator vs MediatR]] — why not MediatR (commercial since 2024)

## Phase 4 — async messaging
- [[Outbox pattern]] — atomic state-change + message
- [[Saga vs choreography]]
- [[BookingConfirmed event flow]] — → Notifications + Reviews

## Phase 5 — CQRS
- [[CQRS read models]] — split read/write stores (the real thing)

## Anti-patterns to avoid
See repo `CLAUDE.md` red-flag list: generic repositories, anemic domain models, sync inter-module HTTP,
CQRS-in-name-only, catch-log-swallow, primitives over value objects, premature abstraction.

## Go-to references
See [[Reference library]] → *.NET architecture*. Primary: Microsoft Learn architecture guides ·
Andrew Lock · Jimmy Bogard · *Implementing DDD* (Vernon).
