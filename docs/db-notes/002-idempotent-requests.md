# 002. Idempotent requests — safe duplicate booking submissions

## The problem

`POST` is not idempotent by nature. If a guest double-clicks "Book", or the network drops the
response and the client retries, the backend can receive the *same* request twice and create two
bookings. Idempotency means: **the same request, repeated, has a single effect** — one booking, not
two.

## The mechanism

The client supplies an **idempotency key** with the request; the backend remembers what that key
produced and, on a repeat, **replays the original result instead of re-executing**. The conceptual
state machine has more than two outcomes:

| Key on arrival | Response |
|---|---|
| not seen | claim it, do the work, return the result |
| seen, completed, same request | replay the stored response |
| seen, in progress | the first call is still running → tell the client to retry |
| seen, different request | reject (the key was reused for a different operation) |

## As built in StayNGo

Simplified to the part that matters for one endpoint:

- The key is a required **`Idempotency-Key` header** on `POST /bookings`; missing/empty → `400`
  before any work (so it can never default to `Guid.Empty` and collide).
- A **per-user unique index `(guest_user_id, idempotency_key)`** is the atomic claim. The *database*
  arbitrates the race — exactly the same "let the DB decide, don't check-then-act" lesson as the
  [[GiST exclusion constraint]].
- On a duplicate, the insert raises `23505`; the service catches it and **replays the existing
  booking** (looked up by `(user, key)`). The "in-progress" concurrent case resolves for free: the
  second insert **blocks on the unique index** until the first transaction commits, then gets
  `23505` → replay.
- Per-**user** scope (not global) avoids cross-user key collisions and prevents one guest probing
  another's booking via a guessed key.
- We deliberately skipped the request-fingerprint / `422` branch — low value here, and a reused key
  just returns the first booking.

## Why it's a different concern from the exclusion constraint

The exclusion constraint stops **two different bookings** overlapping on a listing. Idempotency stops
**one booking** being created twice by one client. Different endpoints, different guarantees — they
are not substitutes.

> Rejecting an empty/absent key prevents a *silent collision*; it does **not** guarantee idempotency
> on its own — a client that sends a fresh key on every retry still defeats it. Supplying a stable
> key per logical request is the client's contract.

Reference: Stripe — ["Idempotent requests"](https://stripe.com/docs/api/idempotent_requests); IETF
draft "The Idempotency-Key HTTP Header Field".
