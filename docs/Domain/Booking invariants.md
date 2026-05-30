---
type: domain
track: [domain, database]
phase: 1
status: growing
created: 2026-05-29
---

# Booking invariants

The Phase-1 rules and **where each is enforced** (DB-level wherever possible). Mirrors `StayNGo.md` §5.3 —
that spec is canonical; this note links each mechanism to its [[Concepts]].

| Invariant | Enforcement |
|---|---|
| No overlapping `Confirmed` bookings on a listing | [[GiST exclusion constraint]] — `EXCLUDE USING gist (listing_id WITH =, during WITH &&) WHERE status='Confirmed'` |
| `CheckIn < CheckOut` | range validity (`during` non-empty) |
| `CheckIn >= today` at creation | app-level (can't reference `now()` in a `CHECK`) |
| Only owner edits / publishes / archives a listing | handler [[Ownership-based authorization]] |
| Only booking guest cancels (before check-in) | handler ownership check |
| Only listing owner confirms a booking | handler ownership check |
| User cannot book own listing | handler check: `booking.GuestUserId != listing.OwnerUserId` |
| Only `Active` listings appear in search | query filter `WHERE status = 'Active'` |
| Archived listings are immutable | handler check on edit attempts |

## Why DB-level where possible
Concurrency invariants (overlap) **must** be enforced at the database — the app can't win the race against a
simultaneous request. Authorization invariants (ownership) live in handlers because they need request
context (who is calling). See [[GiST exclusion constraint]] for the race argument.

## Related
[[Lifecycles]] · [[Domain model]] · [[Database internals]]
