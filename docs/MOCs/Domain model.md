---
type: moc
track: [domain]
status: growing
created: 2026-05-29
---

# 🧩 Domain model — MOC

Domain first, architecture second (Principle 4). The booking domain's real complexity — concurrency,
availability ranges, idempotency, cancellations, timezones, pricing — drives the design.

## Aggregates (Phase 1)
- **User** — no role column; "host" is *emergent* (a user who owns ≥1 listing).
- **Listing** — owned by a user; has a lifecycle.
- **Booking** — a user's reservation of a listing for a date range; has a lifecycle.
- Review, Payment — Phase 2+.

## Notes
- [[Booking invariants]] — the rules, and where each is enforced
- [[Lifecycles]] — Listing & Booking state machines
- [[Hard questions]] — concurrency · idempotency · timezones · cancellation · pricing *(to create)*

## Source
Mirrors `StayNGo.md` §5 and the spec under `superpowers/specs/`. Those are canonical — these notes
**link, they don't duplicate.**
