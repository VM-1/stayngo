---
type: concept
track: [database]
phase: 1
status: seedling
created: 2026-05-29
---

# GiST exclusion constraint

> GiST - is a framework for building Tree-based indexes for complex comparison: such as geometric data, dateRange, fullText like structures, nearest-neighbor , etc. 
> Btree_gist extension allows to use primitive date types in index comparison 
> Exclude - allows to check whether for any 2 row all constrains to be true. 

## What it is
An **exclusion constraint** generalizes `UNIQUE`. `UNIQUE` forbids two rows whose columns are *equal*; an
exclusion constraint forbids two rows whose columns satisfy **any operator you pick** — including range
overlap (`&&`). It's backed by a **GiST index**, which (unlike a B-tree) can answer "does any existing row
*overlap* this range?".

## Why StayNGo uses it
The core invariant — *no two `Confirmed` bookings overlap on the same listing* (see [[Booking invariants]]) —
is a **concurrency** problem. App-level "check-then-insert" has a race: two requests both pass the check,
both insert. Pushing the rule into the database closes the race — Postgres serializes the check at commit and
raises SQLSTATE `23P01` on conflict, which the handler maps to `409 Conflict`. No Redis, no distributed lock
in Phase 1.

## The shape (Phase 1)
```sql
CREATE EXTENSION IF NOT EXISTS btree_gist;  -- lets = (listing_id) and && (range) live in one GiST index

ALTER TABLE bookings ADD CONSTRAINT no_overlapping_confirmed
  EXCLUDE USING gist (
    listing_id WITH =,
    during     WITH &&
  ) WHERE (status = 'Confirmed');           -- partial: only Confirmed bookings block each other
```
- `during` is a [[daterange type]] (`[)` half-open — check-out day is free to re-book).
- [[btree_gist extension]] is required to put the scalar `listing_id` into a GiST index beside the range.
- The same index **doubles as the availability lookup** index
  (`... NOT EXISTS (... WHERE during && daterange(X, Y, '[)'))`).

## Connects to
- Enforces → [[Booking invariants]]
- Built on → [[daterange type]] · [[btree_gist extension]]
- Contrast later → [[Redis lock vs advisory lock vs exclusion constraint]] (Phase 4 ADR)
- Track → [[Database internals]]

## Reference
Introduces **exclusion constraints**. Reference: *PostgreSQL Documentation — "Constraints → Exclusion
Constraints"* and *"Range Types"* (postgresql.org/docs/current). See [[Reference library]] → *Database*.
(why: the canonical definition plus the operator/index mechanics, straight from the source.)
