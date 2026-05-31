---
type: moc
status: evergreen
created: 2026-05-29
---

# 🏠 StayNGo — Knowledge Base

Entry point for the StayNGo second brain. This vault *is* the repo's `docs/` folder — ADRs and
db-notes are linkable notes, not a parallel copy. Conventions: see `CLAUDE.md` in this folder.

## Learning tracks
> The four tracks from `StayNGo.md` §1.1, plus the domain.

- [[Database internals]] — index types, query planning, MVCC, concurrency *(the staff-level focus)*
- [[Backend architecture]] — DDD, vertical slices → modular monolith → messaging → CQRS
- [[Domain model]] — aggregates, lifecycles, invariants
- [[Frontend]] — *to create*
- [[DevOps]] — *to create*

## Canonical artifacts (ticket-driven)
- ADRs → `adr/` — [[0001-initial-stack|0001 · Initial stack]], [[0002-monorepo|0002 · Monorepo]], [[0003-obsidian-vault-in-docs|0003 · Obsidian vault]]
- DB learning notes → `db-notes/` — [[100-local-dev-compose|100 · Local dev compose]]
- Retros → `retros/`
- Specs & plans → `superpowers/`

## Working set
- [[Reference library]] — canonical learning references (Principle 11)
- Latest journal: [[2026-05-29]]

## How to use
- **New concept you learned** → `Templates/Concept note` → drop in `Concepts/` → link it from a MOC.
- **New work session** → daily-note ribbon creates today's note in `Journal/`.
- **Decision worth keeping** → spawn an `adr` ticket; the ADR lands in `adr/` and gets linked here.
