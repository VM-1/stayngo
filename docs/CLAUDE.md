# Vault rules — StayNGo knowledge base (`docs/`)

This folder is **both the repo's documentation tree and an Obsidian vault.** These rules are
scoped to `docs/` and complement the repo-root `CLAUDE.md` — the 11 principles still apply,
especially **#3 keep the discipline, drop the theater** and **#11 teach, don't just suggest**.

## What lives here

| Folder | Role | Canonical? |
|---|---|---|
| `adr/` | Architecture Decision Records | **Yes** — ticket-driven record |
| `db-notes/` | Postgres learning notes, ~300 words **in own words** | **Yes** |
| `retros/` | Per-2-phase honest retros | **Yes** |
| `superpowers/` | Specs + plans | **Yes** |
| `MOCs/` | Maps of Content — index / dashboard notes | connective layer |
| `Concepts/` | Atomic learning notes (one idea each) | connective layer |
| `Domain/` | Domain notes (invariants, lifecycles) mirrored from the spec | connective layer |
| `References/` | The Principle-11 canonical reference library | connective layer |
| `Journal/` | Dated session logs | connective layer |
| `Templates/` | Note templates | — |

**Canonical artifacts** (`adr/`, `db-notes/`, `retros/`, `superpowers/`) are produced through the
normal ticket/PR workflow and *are* the record. The **connective layer** is the second-brain that
links them together — it needs no ticket, but must never contradict or duplicate a canonical
artifact: **link to them, don't restate them.**

## Note conventions

- **Atomic:** one idea per note. If the title needs an "and", split it.
- **Title-case names with spaces** (`GiST exclusion constraint.md`) — idiomatic Obsidian; wikilinks read like prose.
- **Link liberally** with `[[wikilinks]]`. An unresolved `[[link]]` is not an error — it's a note worth
  writing later (a visible backlog in the graph view).
- **Frontmatter on every note:**
  ```yaml
  ---
  type: concept        # concept | moc | domain | reference | journal | adr
  track: [database]    # database | backend | frontend | devops | domain
  phase: 1             # optional — which StayNGo phase introduces it
  status: seedling     # seedling → growing → evergreen
  created: 2026-05-29
  ---
  ```
- **status** (digital-garden convention): `seedling` = rough/scaffolded · `growing` = being refined ·
  `evergreen` = you'd defend it in an interview.

## The learning rule (Principle 11, enforced here)

- Every **Concept** note cites **≥1 canonical reference**, recorded in [[Reference library]]. Never fabricate URLs.
- Claude **scaffolds, links, and finds gaps**; the **user writes the load-bearing understanding** in their
  own words. Same reason `db-notes` are "in own words" — the learning *is* the deliverable. A note full of
  Claude prose the user never internalized is worse than no note.
- When Claude introduces a concept the user didn't name, create/extend its Concept note **and** add the
  reference — don't just mention it in chat.

## Don't

- **Don't edit `.obsidian/`** contents (vault config) unless asked.
- **Don't bulk-rewrite notes unsupervised** — propose a plan first.
- **Don't duplicate** ADRs / db-notes into Concepts — summarize in one line and `[[link]]`.
