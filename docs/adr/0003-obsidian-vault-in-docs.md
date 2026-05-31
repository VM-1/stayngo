# 0003. Obsidian vault co-located in docs/

Date: 2026-05-31
Status: Accepted

## Context
The portfolio deliverable includes learning notes, ADRs, and db-notes (spec §1); a separate notes app would drift from the repo and split that record.

## Decision
Make `docs/` itself the Obsidian vault — not the repo root (avoids indexing `frontend/node_modules/**`) — committing portable `.obsidian/` core config, gitignoring per-machine workspace churn and the local `Journal/` scratchpad.

## Consequences
ADRs/db-notes become a linked knowledge graph at zero extra cost; the tradeoff is a second (vault-scoped) `docs/CLAUDE.md` and Obsidian's whole-folder indexing constraining where the vault root can sit.
