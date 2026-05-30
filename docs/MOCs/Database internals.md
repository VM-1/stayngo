---
type: moc
track: [database]
status: growing
created: 2026-05-29
---

# 🗄️ Database internals — MOC

The staff-level focus of StayNGo. Concepts arrive phase by phase (`StayNGo.md` §4 "Database learning
milestones"). Each one earns a `db-notes/NNN-*.md` (~300 words, **own words**) **and** an atomic concept
note linked here. Unresolved links below = the backlog of notes to write.

## Phase 1 — availability & idempotency
- [[GiST exclusion constraint]] ✅ — no double-booking, DB-enforced
- [[daterange type]] — half-open `[)` ranges for nights
- [[btree_gist extension]] — equality + range in one GiST index
- [[Idempotency key table design]] — `(key, user, request_hash, response)` + TTL
- [[Testcontainers for real-Postgres tests]]
- [[EF Core migrations with hand-edited SQL]] — for constraints EF can't express

## Phase 2 — reading the planner
- [[EXPLAIN ANALYZE BUFFERS]]
- [[B-tree vs GIN vs GiST vs BRIN]] — when each wins
- [[Covering indexes with INCLUDE]]
- [[Partial indexes]]
- [[pg_stat_statements]] · [[pg_stat_user_indexes]]
- [[PgBouncer transaction vs session mode]]

## Phase 3 — schema-per-module
- [[Schema-per-module in one database]]
- [[Cross-schema FKs vs event contracts]]

## Phase 4 — concurrency & isolation
- [[Isolation levels and anomalies]] — Read Committed vs Repeatable Read vs Serializable
- [[SELECT FOR UPDATE SKIP LOCKED]]
- [[Advisory locks]] — session vs transaction
- [[Outbox pattern with SKIP LOCKED]]
- [[Redis lock vs advisory lock vs exclusion constraint]] — Phase 4 ADR

## Phase 5 — projections
- [[Logical replication]] · [[CDC with Debezium]]
- [[Projection lag and eventual consistency]]
- [[Idempotent projection handlers]]

## Phase 6 — scale & analytics
- [[Table partitioning]] — range / list / hash
- [[Materialized views and refresh]]
- [[Star vs snowflake schema]]
- [[OLTP vs OLAP separation]]

## Go-to references
See [[Reference library]] → *Database*. Primary: *Use The Index, Luke* (Winand) · PostgreSQL docs §11/§13 ·
*PostgreSQL 14 Internals* (Rogov).
