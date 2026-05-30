---
type: domain
track: [domain]
phase: 1
status: growing
created: 2026-05-29
---

# Lifecycles

State machines for the Phase-1 aggregates (`StayNGo.md` §5.2). All transitions are **owner/guest-initiated** —
no admin, no approval gate.

## Listing
```
[Draft] → [Active] → [Archived]
```
- **Draft** — private, owner-only, fully editable.
- **Active** — public, appears in search; price / description / photos editable.
- **Archived** — hidden from search, read-only; preserves booking history.

## Booking
```
[Pending] → [Confirmed] → [CheckedIn] → [Completed]
     ↓           ↓
[Cancelled]  [Cancelled]
```
- Guest creates → **Pending**.
- Listing owner confirms → **Confirmed** (now subject to the [[GiST exclusion constraint]]) or rejects → **Cancelled**.
- Guest may cancel before check-in (free until 24h before, in the listing's timezone).

## Related
[[Booking invariants]] · [[Domain model]]
