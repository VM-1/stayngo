# StayNGo — Phase 0 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Execute Phase 0 of StayNGo — get a monorepo with CI, docker-compose local env, GitHub Project board, issue/PR templates, automated PR review, and 8 seeded epics, all green on a trivial PR. End state: ready to start Phase 1 feature work.

**Architecture:** Single monorepo (`.NET 10` backend under `backend/StayNGo/`, React 18 SPA under `frontend/`, empty `infra/` until Phase 1, docs under `docs/`). Local-first development (`docker compose up` = API + Postgres + frontend). GitHub Actions CI + automated PR review via `anthropics/claude-code-action`. GitHub Projects v2 for work tracking, with strict issue template and four-loop collaboration workflow (discussion → ticket → split-execute → review).

**Tech Stack:** .NET 10 LTS, EF Core 10, PostgreSQL 16 (via Docker), React 18 + Vite + TypeScript strict + Tailwind + shadcn/ui, Testcontainers for integration tests, Serilog for logging, xUnit + FluentAssertions for tests, GitHub Actions, `gh` CLI, Clerk (auth — wired in Phase 1).

**Authoritative spec:** `/mnt/c/VM/Projects/StayNGo/docs/superpowers/specs/2026-04-19-staynGo-design.md`. Every design decision that a task touches is justified there — cite section numbers when in doubt.

---

## Executor conventions

- **`[by-claude]`** — Claude Code executes. Subagent or inline either way. Default.
- **`[by-hand]`** — **User executes.** Claude Code stands by for design-chat (tradeoff questions, anti-pattern warnings, WIP review) but does not write the code. The plan still provides reference commands, starter content, and acceptance criteria so the user isn't blocked.
- **`[SPIKE]`** — Optional pure-study task from spec §9.2. Produces a note under `docs/db-notes/`, not production code. User does these on their own cadence.

All tasks end with a **commit** step using Conventional Commits (`feat:`, `chore:`, `docs:`, `refactor:`, `test:`).

---

## Prerequisites (verify before starting)

- [ ] **.NET 10 SDK** installed. Verify: `dotnet --version` → `10.0.x`.
- [ ] **Node.js 20+** installed. Verify: `node --version` → `v20.x` or higher.
- [ ] **Docker + docker compose** installed and daemon running. Verify: `docker compose version`.
- [ ] **Git** configured. Verify: `git config --global user.name` and `user.email` both set.
- [ ] **`gh` CLI** installed + authenticated. Verify: `gh auth status` → logged in.
- [ ] **GitHub account** ready (for the remote repo; Task 15).
- [ ] **Anthropic API key** ready (for Task 14, PR review workflow). Can be deferred until after Task 15.
- [ ] **Clerk account** (deferred until Phase 1; not needed for Phase 0).
- [ ] Working directory: `/mnt/c/VM/Projects/StayNGo` (repo root; already has `.git/`, `.gitignore`, `Documents/`, `docs/superpowers/specs/`).

---

## File structure at end of Phase 0

New files this plan creates:

```
/mnt/c/VM/Projects/StayNGo/
├── StayNGo.md                                     # Task 2 — narrative project brief (from spec §1–6, §11)
├── CLAUDE.md                                      # Task 3 — Claude Code operational rules (spec §7.1)
├── README.md                                      # Task 1 — repo entry point, dev quickstart
├── CHANGELOG.md                                   # Task 1 — "Unreleased" section stub
├── docker-compose.yml                             # Task 11 [by-hand]
├── .env.example                                   # Task 10
│
├── .github/
│   ├── ISSUE_TEMPLATE/ticket.yml                  # Task 4 (spec §7.2)
│   ├── PULL_REQUEST_TEMPLATE.md                   # Task 5 (spec §7.5)
│   ├── workflows/
│   │   ├── ci.yml                                 # Task 13
│   │   └── pr-review.yml                          # Task 14 [by-hand] (spec §7.3)
│   └── claude-review-prompt.md                    # Task 14 [by-hand] (spec §7.4)
│
├── backend/StayNGo/
│   ├── StayNGo.sln                                # Task 8
│   ├── Directory.Packages.props                   # Task 8
│   ├── src/
│   │   ├── StayNGo.Api/                           # Task 8 (ASP.NET minimal project)
│   │   ├── StayNGo.Domain/                        # Task 8 (classlib)
│   │   └── StayNGo.Infrastructure/                # Task 8 (classlib + EF Core in Task 12)
│   ├── tests/
│   │   ├── StayNGo.UnitTests/                     # Task 8 (xUnit)
│   │   └── StayNGo.IntegrationTests/              # Task 8 (xUnit + Testcontainers)
│   └── Dockerfile                                 # Task 8
│
├── frontend/
│   ├── (Vite scaffolded layout)                   # Task 9
│   ├── package.json                               # Task 9
│   ├── tailwind.config.js                         # Task 9
│   └── Dockerfile                                 # Task 9
│
├── infra/                                         # intentionally empty at Phase 0 end
│   └── .gitkeep                                   # Task 1
│
└── docs/
    ├── adr/
    │   ├── template.md                            # Task 6 (spec §7.6)
    │   ├── 0001-initial-stack.md                  # Task 6 (spec §7.7)
    │   └── 0002-monorepo.md                       # Task 7 (spec §7.8)
    ├── db-notes/                                  # Tasks 18–22 (optional spikes)
    │   └── .gitkeep                               # Task 1
    └── retros/                                    # empty — populated Phase 2+
        └── .gitkeep                               # Task 1
```

---

## Task 1: Repo meta files (README, CHANGELOG, placeholder dirs) `[by-claude]`

**Files:**
- Create: `/mnt/c/VM/Projects/StayNGo/README.md`
- Create: `/mnt/c/VM/Projects/StayNGo/CHANGELOG.md`
- Create: `/mnt/c/VM/Projects/StayNGo/infra/.gitkeep`
- Create: `/mnt/c/VM/Projects/StayNGo/docs/db-notes/.gitkeep`
- Create: `/mnt/c/VM/Projects/StayNGo/docs/retros/.gitkeep`

- [ ] **Step 1.1: Create README.md**

Write `/mnt/c/VM/Projects/StayNGo/README.md`:

```markdown
# StayNGo

Stays / short-term-rentals booking platform. Solo side project, long-running.

See [`StayNGo.md`](./StayNGo.md) for the project brief, principles, tech stack, domain, and roadmap.
See [`CLAUDE.md`](./CLAUDE.md) for Claude Code operational rules.
See [`docs/adr/`](./docs/adr/) for architectural decision records.
See [`docs/superpowers/specs/`](./docs/superpowers/specs/) for the authoritative design spec.

## Quickstart (local development)

Prerequisites: .NET 10 SDK, Node 20+, Docker + docker compose.

```bash
# Bring up Postgres + API + frontend
docker compose up

# API:       http://localhost:8080
# Frontend:  http://localhost:5173
# Postgres:  localhost:5432 (see .env.example for credentials)
```

Backend tests (inside `backend/StayNGo/`):

```bash
dotnet test
```

Frontend (inside `frontend/`):

```bash
npm install
npm run dev
```

## Contributing

All work tracked on the GitHub Project board. Every PR closes a ticket:
`Closes #N` in the PR body. See `CLAUDE.md` for ticket handoff rules and
the automated PR review workflow.
```

- [ ] **Step 1.2: Create CHANGELOG.md**

Write `/mnt/c/VM/Projects/StayNGo/CHANGELOG.md`:

```markdown
# Changelog

All notable user-visible changes to StayNGo. Format loosely follows
[Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [Unreleased]

### Added
- Phase 0 scaffolding — see `docs/superpowers/specs/2026-04-19-staynGo-design.md`.
```

- [ ] **Step 1.3: Create placeholder directories via `.gitkeep`**

Create empty files:
- `/mnt/c/VM/Projects/StayNGo/infra/.gitkeep`
- `/mnt/c/VM/Projects/StayNGo/docs/db-notes/.gitkeep`
- `/mnt/c/VM/Projects/StayNGo/docs/retros/.gitkeep`

Each file's content is empty (zero bytes). Git won't track empty dirs without a placeholder.

- [ ] **Step 1.4: Verify**

Run: `ls /mnt/c/VM/Projects/StayNGo/{README.md,CHANGELOG.md,infra/.gitkeep,docs/db-notes/.gitkeep,docs/retros/.gitkeep}`
Expected: all 5 files listed, no errors.

- [ ] **Step 1.5: Commit**

```bash
git -C /mnt/c/VM/Projects/StayNGo add README.md CHANGELOG.md infra/.gitkeep docs/db-notes/.gitkeep docs/retros/.gitkeep
git -C /mnt/c/VM/Projects/StayNGo commit -m "chore: add README, CHANGELOG, and placeholder directories"
```

---

## Task 2: Create `StayNGo.md` (narrative project brief) `[by-claude]`

**Files:**
- Create: `/mnt/c/VM/Projects/StayNGo/StayNGo.md`

**Source:** Spec sections 1, 1.1, 2, 3, 4, 5, 6, 11 — copied into a standalone narrative doc. Drop the spec's title/date/status/supersedes header; replace with StayNGo-specific intro.

- [ ] **Step 2.1: Write StayNGo.md**

Read spec sections §1, §1.1, §2, §3, §4, §5, §6, §11 from `/mnt/c/VM/Projects/StayNGo/docs/superpowers/specs/2026-04-19-staynGo-design.md`. Compose `/mnt/c/VM/Projects/StayNGo/StayNGo.md` with this structure:

```markdown
# StayNGo

Stays / short-term-rentals booking platform. Solo side project simulating small-company workflow.

> This is the human-readable project brief. The authoritative spec (with merge audit trail and deferred-decisions log) lives at `docs/superpowers/specs/2026-04-19-staynGo-design.md`. `CLAUDE.md` is the Claude Code operational ruleset.

---

## 1. Identity & goals

<verbatim content of spec §1>

### 1.1 Learning tracks

<verbatim content of spec §1.1>

---

## 2. Guiding principles

<verbatim content of spec §2>

---

## 3. Tech stack

### 3.1 Phase 1 (MVP)

<verbatim content of spec §3.1>

### 3.2 Later phases

<verbatim content of spec §3.2>

---

## 4. Phased roadmap

<verbatim content of spec §4, including the "Database learning milestones by phase" table>

---

## 5. Domain sketch

<verbatim content of spec §5, including §5.1–§5.5>

---

## 6. Collaboration workflow

<verbatim content of spec §6, including §6.1–§6.4>

---

## 7. Using this doc with AI assistants

<verbatim content of spec §11>

---

## Source

Full spec with merge audit trail: [`docs/superpowers/specs/2026-04-19-staynGo-design.md`](./docs/superpowers/specs/2026-04-19-staynGo-design.md).
```

**Rule:** copy the spec section content **verbatim** into each slot — don't paraphrase. If the spec has a table, keep the table. If it has a code fence, keep the code fence. If the spec refers to "§5.3" internally, those cross-references become "§5.3" in StayNGo.md too (they still resolve because the section numbering matches).

- [ ] **Step 2.2: Verify length and structure**

Run: `wc -l /mnt/c/VM/Projects/StayNGo/StayNGo.md`
Expected: ~400–500 lines.

Run: `grep -c '^## ' /mnt/c/VM/Projects/StayNGo/StayNGo.md`
Expected: 7 (the seven `##`-level sections).

- [ ] **Step 2.3: Commit**

```bash
git -C /mnt/c/VM/Projects/StayNGo add StayNGo.md
git -C /mnt/c/VM/Projects/StayNGo commit -m "docs: add StayNGo.md project brief (from spec §1–6, §11)"
```

---

## Task 3: Create `CLAUDE.md` (Claude Code operational rules) `[by-claude]`

**Files:**
- Create: `/mnt/c/VM/Projects/StayNGo/CLAUDE.md`

**Source:** Spec §7.1 — verbatim, but resolve the "copy-paste the list from StayNGo.md §2 verbatim" marker by actually copying the 11 principles.

- [ ] **Step 3.1: Write CLAUDE.md**

Read spec §7.1 from `docs/superpowers/specs/2026-04-19-staynGo-design.md`. Write `/mnt/c/VM/Projects/StayNGo/CLAUDE.md` with the spec §7.1 content, **but** resolve the `(copy-paste the list from StayNGo.md §2 verbatim — kept in sync via PR review)` marker by pasting the actual 11 principles from spec §2 there.

Final structure:

```markdown
# Claude Code operational rules — StayNGo

## The 11 principles

<full list of 11 numbered principles verbatim from spec §2>

## Red-flag anti-patterns (warn before writing, per Principle 10)

<verbatim from spec §7.1>

## Hard rules

<verbatim from spec §7.1>

## Ticket handoff

<verbatim from spec §7.1>

## PR review mode (GH Actions)

<verbatim from spec §7.1>

## Learning-reference rule (Principle 11)

<verbatim from spec §7.1>

## See also

<verbatim from spec §7.1>
```

- [ ] **Step 3.2: Verify**

Run: `wc -l /mnt/c/VM/Projects/StayNGo/CLAUDE.md`
Expected: ~120–180 lines.

Run: `grep -E '^## ' /mnt/c/VM/Projects/StayNGo/CLAUDE.md | wc -l`
Expected: 7 sections.

- [ ] **Step 3.3: Commit**

```bash
git -C /mnt/c/VM/Projects/StayNGo add CLAUDE.md
git -C /mnt/c/VM/Projects/StayNGo commit -m "docs: add CLAUDE.md operational rules (from spec §7.1 + §2)"
```

---

## Task 4: GitHub issue template `[by-claude]`

**Files:**
- Create: `/mnt/c/VM/Projects/StayNGo/.github/ISSUE_TEMPLATE/ticket.yml`
- Create: `/mnt/c/VM/Projects/StayNGo/.github/ISSUE_TEMPLATE/config.yml`

**Source:** Spec §7.2 verbatim for `ticket.yml`.

- [ ] **Step 4.1: Write ticket.yml**

Create directory if needed: `mkdir -p /mnt/c/VM/Projects/StayNGo/.github/ISSUE_TEMPLATE`.

Write `/mnt/c/VM/Projects/StayNGo/.github/ISSUE_TEMPLATE/ticket.yml` with the exact YAML content from spec §7.2 (the `name: Work item ... validations: required: true` block — includes Learning objective + Learning references textareas).

- [ ] **Step 4.2: Write config.yml to disable blank issues**

Write `/mnt/c/VM/Projects/StayNGo/.github/ISSUE_TEMPLATE/config.yml`:

```yaml
blank_issues_enabled: false
```

Rationale: enforces the ticket template — no-one (including Claude Code) can bypass it by opening a blank issue.

- [ ] **Step 4.3: Verify YAML is valid**

Run: `python3 -c "import yaml; yaml.safe_load(open('/mnt/c/VM/Projects/StayNGo/.github/ISSUE_TEMPLATE/ticket.yml'))"`
Expected: no output (successful parse).

Run: `python3 -c "import yaml; yaml.safe_load(open('/mnt/c/VM/Projects/StayNGo/.github/ISSUE_TEMPLATE/config.yml'))"`
Expected: no output.

- [ ] **Step 4.4: Commit**

```bash
git -C /mnt/c/VM/Projects/StayNGo add .github/ISSUE_TEMPLATE/
git -C /mnt/c/VM/Projects/StayNGo commit -m "chore: add strict ticket issue template (from spec §7.2)"
```

---

## Task 5: Pull request template `[by-claude]`

**Files:**
- Create: `/mnt/c/VM/Projects/StayNGo/.github/PULL_REQUEST_TEMPLATE.md`

**Source:** Spec §7.5 verbatim.

- [ ] **Step 5.1: Write PR template**

Write `/mnt/c/VM/Projects/StayNGo/.github/PULL_REQUEST_TEMPLATE.md` with the exact markdown content from spec §7.5 — the "## Closes / ## Why / ## What changed / ## Testing / ## Invariants checked / ## ADR / ## Checklist" template.

- [ ] **Step 5.2: Commit**

```bash
git -C /mnt/c/VM/Projects/StayNGo add .github/PULL_REQUEST_TEMPLATE.md
git -C /mnt/c/VM/Projects/StayNGo commit -m "chore: add pull request template (from spec §7.5)"
```

---

## Task 6: ADR template + ADR-0001 (initial stack) `[by-claude]`

**Files:**
- Create: `/mnt/c/VM/Projects/StayNGo/docs/adr/template.md`
- Create: `/mnt/c/VM/Projects/StayNGo/docs/adr/0001-initial-stack.md`

**Source:** Spec §7.6 verbatim for template; spec §7.7 sketch as base for ADR-0001 (flesh out slightly).

- [ ] **Step 6.1: Write ADR template**

Write `/mnt/c/VM/Projects/StayNGo/docs/adr/template.md` with the exact content from spec §7.6 (Michael Nygard format: NNNN. Title / Date / Status / Context / Decision / Consequences).

- [ ] **Step 6.2: Write ADR-0001 (initial stack)**

Write `/mnt/c/VM/Projects/StayNGo/docs/adr/0001-initial-stack.md`:

```markdown
# 0001. Initial stack for Phase 1

Date: 2026-04-19
Status: Accepted

## Context

StayNGo is a long-running solo side project (spec §1). Primary goals: interview portfolio emphasizing staff-backend + Postgres-internals depth; secondary: learn FE and DevOps. Budget: ~10 hrs/week, 12–18 month arc. Stack choices must:

- Be supported through the project arc (no mid-project LTS exit).
- Have minimal friction for a solo maintainer.
- Expose real domain complexity (booking concurrency, availability ranges) so the implementation *teaches*, not papers-over.
- Avoid commercial/license surprises.

## Decision

Phase 1 stack:

- **Runtime:** .NET 10 (LTS, released Nov 2025; supported through Nov 2028).
- **Architecture:** vertical slices (`Features/<Domain>/<Action>/`) in Phase 1 → Clean Architecture *per module* in Phase 3.
- **Handler library:** plain handler classes (bespoke `IRequestHandler<TReq, TRes>`). **Do not adopt MediatR** — it went commercial in 2024. Re-evaluate source-gen `Mediator` (MIT) in Phase 3 if dispatch ceremony hurts.
- **ORM:** EF Core 10. `DbContext.SaveChangesAsync` **is** the unit of work — no `IUnitOfWork` abstraction, no generic `IRepository<T>`.
- **Validation:** FluentValidation.
- **Database:** PostgreSQL 16 (via docker-compose locally; Fly.io managed Postgres at the Phase 1 demo milestone).
- **Concurrency invariant:** enforced via Postgres `EXCLUDE USING gist` on `(listing_id, during)` WHERE `status = 'Confirmed'`. Domain invariants live in the schema, not the handler.
- **Auth:** Clerk (managed). Not rolling ASP.NET Identity.
- **API docs:** Scalar (over Swagger UI).
- **Logging:** Serilog + structured logging.
- **Tests:** xUnit + FluentAssertions + **Testcontainers** (real Postgres) from day 1.
- **Frontend:** React 18 + Vite + TypeScript strict + Tailwind + shadcn/ui. One SPA serves both guest and host views (host routes guarded under `/host/*`).
- **Containers:** Docker + `docker compose` locally.
- **CI:** GitHub Actions.
- **Hosting:** local-only for Phase 0–1 development; Fly.io as the Phase 1 demo milestone.
- **Repo:** monorepo. Backend nested under `backend/StayNGo/` so Phase 3 can add `backend/Modules/*` siblings cleanly.

## Consequences

**Positive**
- .NET 10 LTS covers the full 12–18 month arc without forced mid-project migration.
- Low friction solo — minimum moving parts.
- Postgres GiST exclusion teaches the canonical "invariant in the schema" pattern.
- Testcontainers from day 1 means integration tests exercise real SQL behavior (exclusion constraint, ranges, timezone handling) — a mock-based stack would mask the bugs this project exists to teach.

**Negative**
- .NET 10 at ~5 months of age means some third-party packages may lag. Mitigation: pin versions in `Directory.Packages.props`; prefer packages with active .NET 10 releases.
- Clerk costs money if free tier is exceeded (unlikely for a demo app). Mitigation: monitor Clerk MAU; swap to Supabase Auth if friction grows.
- Monorepo means one CI config to maintain; build times grow linearly with both stacks.

**Neutral**
- The `backend/StayNGo/` nesting looks redundant in Phase 1 but unlocks clean Phase 3 modularization without a disruptive restructure. Documented in ADR-0002.
```

- [ ] **Step 6.3: Commit**

```bash
git -C /mnt/c/VM/Projects/StayNGo add docs/adr/template.md docs/adr/0001-initial-stack.md
git -C /mnt/c/VM/Projects/StayNGo commit -m "docs: add ADR template and ADR-0001 (initial stack)"
```

---

## Task 7: ADR-0002 (monorepo + backend nesting) `[by-claude]`

**Files:**
- Create: `/mnt/c/VM/Projects/StayNGo/docs/adr/0002-monorepo.md`

**Source:** Spec §7.8 sketch as base.

- [ ] **Step 7.1: Write ADR-0002**

Write `/mnt/c/VM/Projects/StayNGo/docs/adr/0002-monorepo.md`:

```markdown
# 0002. Monorepo structure + backend/StayNGo/ nesting

Date: 2026-04-19
Status: Accepted

## Context

Solo developer. Backend (.NET), frontend (React), infra (docker compose, later fly.toml) all evolve in lockstep during Phase 1–3. Three separate repos would triple the context-switch and CI-config overhead; cross-stack changes couldn't be atomic.

Separately: Phase 3 of the roadmap (spec §4, §8) splits the monolith into a modular monolith with sibling folders `backend/Modules/{Identity,Catalog,Booking,Notifications}/`. The natural home for the composition-root host is a sibling folder alongside the modules.

## Decision

1. **Single monorepo** with top-level `backend/`, `frontend/`, `infra/`, `docs/`, `.github/`. One `.github/` config governs the whole repo.
2. **Backend nested at `backend/StayNGo/`** (not flat under `backend/`). In Phase 1, `backend/StayNGo/` contains `StayNGo.sln`, `src/{Api,Domain,Infrastructure}/`, `tests/`. In Phase 3, sibling folders `backend/Modules/*` are added; `backend/StayNGo/` becomes the composition-root host (Program.cs wires modules, owns migrations orchestration, hosts the shared kernel).
3. **Root-level `backend.sln`** will be introduced at Phase 3 kickoff referencing all module projects; until then, `backend/StayNGo/StayNGo.sln` is the only solution file.

## Consequences

**Positive**
- Atomic PRs across stacks (backend + frontend + infra in one diff).
- One Claude Code context covers everything; `CLAUDE.md` at repo root auto-loads for every session.
- Single CI pipeline.
- Phase 3 modularization is additive — `backend/StayNGo/` doesn't move, just gets siblings. No painful repo restructure.

**Negative**
- `backend/StayNGo/` nesting looks redundant in Phase 1 (one subfolder, one solution). Cost is paid up-front for Phase 3 readiness.
- Monorepo means CI build time grows with both stacks. Mitigation: split CI jobs per stack; add path-based filters in Phase 2 if build time hurts.

**Neutral**
- If/when one subsystem grows large enough to warrant isolation (Phase 6+ extraction), doing so will be a deliberate move with its own ADR.
```

- [ ] **Step 7.2: Commit**

```bash
git -C /mnt/c/VM/Projects/StayNGo add docs/adr/0002-monorepo.md
git -C /mnt/c/VM/Projects/StayNGo commit -m "docs: add ADR-0002 (monorepo + backend/StayNGo/ nesting)"
```

---

## Task 8: Scaffold backend `[by-claude]`

**Files:**
- Create: `backend/StayNGo/StayNGo.sln`
- Create: `backend/StayNGo/Directory.Packages.props`
- Create: `backend/StayNGo/global.json`
- Create: `backend/StayNGo/src/StayNGo.Api/*` (ASP.NET minimal API scaffold)
- Create: `backend/StayNGo/src/StayNGo.Domain/*` (classlib)
- Create: `backend/StayNGo/src/StayNGo.Infrastructure/*` (classlib)
- Create: `backend/StayNGo/tests/StayNGo.UnitTests/*` (xUnit)
- Create: `backend/StayNGo/tests/StayNGo.IntegrationTests/*` (xUnit + Testcontainers)
- Create: `backend/StayNGo/Dockerfile`

- [ ] **Step 8.1: Create the solution folder and solution**

```bash
mkdir -p /mnt/c/VM/Projects/StayNGo/backend/StayNGo
dotnet new sln -n StayNGo -o /mnt/c/VM/Projects/StayNGo/backend/StayNGo
```

Expected: `backend/StayNGo/StayNGo.sln` created.

- [ ] **Step 8.2: Pin the .NET SDK**

Write `/mnt/c/VM/Projects/StayNGo/backend/StayNGo/global.json`:

```json
{
  "sdk": {
    "version": "10.0.100",
    "rollForward": "latestFeature"
  }
}
```

Adjust `version` to the actual installed SDK if `dotnet --version` reports different (keep major=10, minor=0). `rollForward: latestFeature` tolerates patch updates.

- [ ] **Step 8.3: Create the Directory.Packages.props (central package versions)**

Write `/mnt/c/VM/Projects/StayNGo/backend/StayNGo/Directory.Packages.props`:

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
    <CentralPackageTransitivePinningEnabled>true</CentralPackageTransitivePinningEnabled>
  </PropertyGroup>
  <ItemGroup>
    <!-- EF Core -->
    <PackageVersion Include="Microsoft.EntityFrameworkCore" Version="10.0.0" />
    <PackageVersion Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.0" />
    <PackageVersion Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="10.0.0" />

    <!-- Validation -->
    <PackageVersion Include="FluentValidation" Version="11.11.0" />
    <PackageVersion Include="FluentValidation.DependencyInjectionExtensions" Version="11.11.0" />

    <!-- Logging -->
    <PackageVersion Include="Serilog.AspNetCore" Version="9.0.0" />
    <PackageVersion Include="Serilog.Sinks.Console" Version="6.0.0" />

    <!-- API docs -->
    <PackageVersion Include="Scalar.AspNetCore" Version="2.0.0" />

    <!-- Testing -->
    <PackageVersion Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
    <PackageVersion Include="xunit" Version="2.9.2" />
    <PackageVersion Include="xunit.runner.visualstudio" Version="2.8.2" />
    <PackageVersion Include="FluentAssertions" Version="6.12.2" />
    <PackageVersion Include="Testcontainers.PostgreSql" Version="4.0.0" />
    <PackageVersion Include="coverlet.collector" Version="6.0.2" />
  </ItemGroup>
</Project>
```

**Note:** version numbers are current as of 2026-04-20. `dotnet restore` in Task 8.9 will surface any version that doesn't exist — bump to the nearest available release if so.

- [ ] **Step 8.4: Create Domain project**

```bash
cd /mnt/c/VM/Projects/StayNGo/backend/StayNGo
dotnet new classlib -n StayNGo.Domain -o src/StayNGo.Domain -f net10.0
dotnet sln StayNGo.sln add src/StayNGo.Domain/StayNGo.Domain.csproj
rm src/StayNGo.Domain/Class1.cs
```

Expected: project created, added to solution, stub class removed.

- [ ] **Step 8.5: Create Infrastructure project**

```bash
dotnet new classlib -n StayNGo.Infrastructure -o src/StayNGo.Infrastructure -f net10.0
dotnet sln StayNGo.sln add src/StayNGo.Infrastructure/StayNGo.Infrastructure.csproj
dotnet add src/StayNGo.Infrastructure/StayNGo.Infrastructure.csproj reference src/StayNGo.Domain/StayNGo.Domain.csproj
rm src/StayNGo.Infrastructure/Class1.cs
```

Edit `src/StayNGo.Infrastructure/StayNGo.Infrastructure.csproj` to add the EF Core packages (central versions — no `Version` attribute):

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.EntityFrameworkCore" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.Design" />
  <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" />
</ItemGroup>
```

- [ ] **Step 8.6: Create Api project**

```bash
dotnet new web -n StayNGo.Api -o src/StayNGo.Api -f net10.0
dotnet sln StayNGo.sln add src/StayNGo.Api/StayNGo.Api.csproj
dotnet add src/StayNGo.Api/StayNGo.Api.csproj reference src/StayNGo.Domain/StayNGo.Domain.csproj
dotnet add src/StayNGo.Api/StayNGo.Api.csproj reference src/StayNGo.Infrastructure/StayNGo.Infrastructure.csproj
```

Edit `src/StayNGo.Api/StayNGo.Api.csproj` to add packages:

```xml
<ItemGroup>
  <PackageReference Include="Serilog.AspNetCore" />
  <PackageReference Include="Serilog.Sinks.Console" />
  <PackageReference Include="FluentValidation" />
  <PackageReference Include="FluentValidation.DependencyInjectionExtensions" />
  <PackageReference Include="Scalar.AspNetCore" />
</ItemGroup>
```

Replace `src/StayNGo.Api/Program.cs` with a minimal health-check host:

```csharp
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration).WriteTo.Console());

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
```

Create empty feature folders for Phase 1 (placeholders — they signal the vertical-slice convention):

```bash
mkdir -p src/StayNGo.Api/Features/Bookings
mkdir -p src/StayNGo.Api/Features/Listings
mkdir -p src/StayNGo.Api/Features/Identity
touch src/StayNGo.Api/Features/Bookings/.gitkeep
touch src/StayNGo.Api/Features/Listings/.gitkeep
touch src/StayNGo.Api/Features/Identity/.gitkeep
```

- [ ] **Step 8.7: Create UnitTests project**

```bash
dotnet new xunit -n StayNGo.UnitTests -o tests/StayNGo.UnitTests -f net10.0
dotnet sln StayNGo.sln add tests/StayNGo.UnitTests/StayNGo.UnitTests.csproj
dotnet add tests/StayNGo.UnitTests/StayNGo.UnitTests.csproj reference src/StayNGo.Domain/StayNGo.Domain.csproj
rm tests/StayNGo.UnitTests/UnitTest1.cs
```

Edit `tests/StayNGo.UnitTests/StayNGo.UnitTests.csproj` to use central package versions and add FluentAssertions:

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.NET.Test.Sdk" />
  <PackageReference Include="xunit" />
  <PackageReference Include="xunit.runner.visualstudio" />
  <PackageReference Include="FluentAssertions" />
  <PackageReference Include="coverlet.collector" />
</ItemGroup>
```

Create a smoke test `tests/StayNGo.UnitTests/SmokeTest.cs`:

```csharp
using FluentAssertions;
using Xunit;

namespace StayNGo.UnitTests;

public class SmokeTest
{
    [Fact]
    public void TestHarnessIsWired()
    {
        (1 + 1).Should().Be(2);
    }
}
```

- [ ] **Step 8.8: Create IntegrationTests project**

```bash
dotnet new xunit -n StayNGo.IntegrationTests -o tests/StayNGo.IntegrationTests -f net10.0
dotnet sln StayNGo.sln add tests/StayNGo.IntegrationTests/StayNGo.IntegrationTests.csproj
dotnet add tests/StayNGo.IntegrationTests/StayNGo.IntegrationTests.csproj reference src/StayNGo.Infrastructure/StayNGo.Infrastructure.csproj
dotnet add tests/StayNGo.IntegrationTests/StayNGo.IntegrationTests.csproj reference src/StayNGo.Api/StayNGo.Api.csproj
rm tests/StayNGo.IntegrationTests/UnitTest1.cs
```

Edit `tests/StayNGo.IntegrationTests/StayNGo.IntegrationTests.csproj` to add Testcontainers + FluentAssertions:

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.NET.Test.Sdk" />
  <PackageReference Include="xunit" />
  <PackageReference Include="xunit.runner.visualstudio" />
  <PackageReference Include="FluentAssertions" />
  <PackageReference Include="Testcontainers.PostgreSql" />
  <PackageReference Include="coverlet.collector" />
</ItemGroup>
```

Create a Testcontainers smoke test `tests/StayNGo.IntegrationTests/PostgresSmokeTest.cs`:

```csharp
using FluentAssertions;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace StayNGo.IntegrationTests;

public class PostgresSmokeTest : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .Build();

    public Task InitializeAsync() => _container.StartAsync();
    public Task DisposeAsync() => _container.DisposeAsync().AsTask();

    [Fact]
    public async Task Postgres_container_is_reachable()
    {
        await using var conn = new NpgsqlConnection(_container.GetConnectionString());
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand("select 1", conn);
        var result = await cmd.ExecuteScalarAsync();
        result.Should().Be(1);
    }
}
```

- [ ] **Step 8.9: Restore and build**

```bash
cd /mnt/c/VM/Projects/StayNGo/backend/StayNGo
dotnet restore
dotnet build --no-restore
```

Expected: `Build succeeded. 0 Warning(s) 0 Error(s)`. If package versions in Directory.Packages.props are wrong, NuGet will error — bump to the nearest existing version and re-run.

- [ ] **Step 8.10: Run tests**

```bash
dotnet test --no-build
```

Expected: both test projects pass — 1 test in UnitTests, 1 test in IntegrationTests (the Testcontainers test pulls `postgres:16-alpine` on first run; takes 10–30s).

If Docker daemon isn't running, the integration test will fail — that's expected; document as a prerequisite.

- [ ] **Step 8.11: Write Dockerfile**

Write `/mnt/c/VM/Projects/StayNGo/backend/StayNGo/Dockerfile`:

```dockerfile
# syntax=docker/dockerfile:1.7
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Directory.Packages.props ./
COPY StayNGo.sln ./
COPY src/ ./src/
COPY tests/ ./tests/
RUN dotnet restore StayNGo.sln
RUN dotnet publish src/StayNGo.Api/StayNGo.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "StayNGo.Api.dll"]
```

- [ ] **Step 8.12: Commit**

```bash
git -C /mnt/c/VM/Projects/StayNGo add backend/
git -C /mnt/c/VM/Projects/StayNGo commit -m "chore: scaffold backend/StayNGo (net10, sln + 3 src + 2 tests + Dockerfile)"
```

---

## Task 9: Scaffold frontend `[by-claude]`

**Files:**
- Create: `frontend/` (Vite + React + TS strict + Tailwind + shadcn/ui + Dockerfile)

- [ ] **Step 9.1: Scaffold Vite + React + TS**

```bash
cd /mnt/c/VM/Projects/StayNGo
npm create vite@latest frontend -- --template react-ts
cd frontend
npm install
```

Expected: Vite scaffold created; dependencies installed.

- [ ] **Step 9.2: Enable TypeScript strict mode**

Edit `/mnt/c/VM/Projects/StayNGo/frontend/tsconfig.json` — set `"strict": true` and `"noUncheckedIndexedAccess": true`:

```json
{
  "compilerOptions": {
    "target": "ES2022",
    "useDefineForClassFields": true,
    "lib": ["ES2022", "DOM", "DOM.Iterable"],
    "module": "ESNext",
    "skipLibCheck": true,
    "moduleResolution": "bundler",
    "allowImportingTsExtensions": true,
    "resolveJsonModule": true,
    "isolatedModules": true,
    "noEmit": true,
    "jsx": "react-jsx",
    "strict": true,
    "noUncheckedIndexedAccess": true,
    "noUnusedLocals": true,
    "noUnusedParameters": true,
    "noFallthroughCasesInSwitch": true
  },
  "include": ["src"],
  "references": [{ "path": "./tsconfig.node.json" }]
}
```

- [ ] **Step 9.3: Install Tailwind + shadcn/ui prerequisites**

```bash
cd /mnt/c/VM/Projects/StayNGo/frontend
npm install -D tailwindcss@latest postcss autoprefixer
npx tailwindcss init -p
npm install clsx tailwind-merge class-variance-authority lucide-react
npm install @radix-ui/react-slot
```

- [ ] **Step 9.4: Configure Tailwind**

Replace `/mnt/c/VM/Projects/StayNGo/frontend/tailwind.config.js`:

```js
/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: { extend: {} },
  plugins: [],
};
```

Replace `/mnt/c/VM/Projects/StayNGo/frontend/src/index.css`:

```css
@tailwind base;
@tailwind components;
@tailwind utilities;
```

Smoke-test by editing `src/App.tsx` to render a tailwind-styled element:

```tsx
function App() {
  return (
    <main className="flex min-h-screen items-center justify-center bg-slate-100">
      <h1 className="text-3xl font-semibold text-slate-800">StayNGo</h1>
    </main>
  );
}

export default App;
```

- [ ] **Step 9.5: Install TanStack Query + routing**

```bash
npm install @tanstack/react-query
npm install react-router-dom
```

We'll wire providers in the first Phase 1 ticket — not now.

- [ ] **Step 9.6: Install shadcn/ui CLI and init**

```bash
npx shadcn@latest init -d
```

Accept defaults (style: default, base color: slate, css variables: yes). This adds `src/lib/utils.ts`, updates `tailwind.config.js`, and adds `components.json`.

If the CLI errors (shadcn API changes periodically), fall back to manually creating `src/lib/utils.ts`:

```ts
import { type ClassValue, clsx } from "clsx";
import { twMerge } from "tailwind-merge";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}
```

- [ ] **Step 9.7: Create Dockerfile for frontend**

Write `/mnt/c/VM/Projects/StayNGo/frontend/Dockerfile`:

```dockerfile
# syntax=docker/dockerfile:1.7
FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM node:20-alpine AS dev
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
EXPOSE 5173
CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0"]
```

Two stages: `build` for prod builds; `dev` for the local docker-compose hot-reload setup.

- [ ] **Step 9.8: Verify build works**

```bash
cd /mnt/c/VM/Projects/StayNGo/frontend
npm run build
```

Expected: `vite v5.x.x building for production...` + `✓ built in Xs`. No TypeScript errors.

- [ ] **Step 9.9: Commit**

```bash
git -C /mnt/c/VM/Projects/StayNGo add frontend/
git -C /mnt/c/VM/Projects/StayNGo commit -m "chore: scaffold frontend (Vite + React + TS strict + Tailwind + shadcn)"
```

---

## Task 10: `.env.example` `[by-claude]`

**Files:**
- Create: `/mnt/c/VM/Projects/StayNGo/.env.example`

- [ ] **Step 10.1: Write .env.example**

Write `/mnt/c/VM/Projects/StayNGo/.env.example`:

```bash
# Copy to .env.local (gitignored) and fill in actual values.
# Env vars are loaded by docker-compose and dotted through to each service.

# --- Postgres ---
POSTGRES_DB=stayngo
POSTGRES_USER=stayngo
POSTGRES_PASSWORD=stayngo_dev_only_replace_in_prod
POSTGRES_PORT=5432

# --- Backend (StayNGo.Api) ---
# Connection string built from POSTGRES_* above; overriding directly takes precedence.
ConnectionStrings__Postgres=Host=postgres;Port=5432;Database=stayngo;Username=stayngo;Password=stayngo_dev_only_replace_in_prod
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:8080

# --- Clerk auth (Phase 1 — leave blank in Phase 0) ---
# Clerk__Authority=
# Clerk__Audience=
# Clerk__ApiKey=

# --- Frontend (Vite) ---
VITE_API_BASE_URL=http://localhost:8080
# VITE_CLERK_PUBLISHABLE_KEY=
```

- [ ] **Step 10.2: Commit**

```bash
git -C /mnt/c/VM/Projects/StayNGo add .env.example
git -C /mnt/c/VM/Projects/StayNGo commit -m "chore: add .env.example with documented variables"
```

---

## Task 11: `docker-compose.yml` `[by-hand]`

**User executes this task. Claude Code stands by for design-chat.**

The docker-compose wiring is a genuine learning moment — networking, service dependencies, health checks, volume lifecycle. Don't let Claude Code hand you a finished yaml; write it yourself.

**Files:**
- Create: `/mnt/c/VM/Projects/StayNGo/docker-compose.yml`

**Reference starter** (not gospel — adjust as you learn):

```yaml
services:
  postgres:
    image: postgres:16-alpine
    environment:
      POSTGRES_DB: ${POSTGRES_DB}
      POSTGRES_USER: ${POSTGRES_USER}
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD}
    ports:
      - "${POSTGRES_PORT}:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U ${POSTGRES_USER} -d ${POSTGRES_DB}"]
      interval: 5s
      timeout: 3s
      retries: 10

  api:
    build:
      context: ./backend/StayNGo
      dockerfile: Dockerfile
    environment:
      ConnectionStrings__Postgres: ${ConnectionStrings__Postgres}
      ASPNETCORE_ENVIRONMENT: ${ASPNETCORE_ENVIRONMENT}
      ASPNETCORE_URLS: ${ASPNETCORE_URLS}
    ports:
      - "8080:8080"
    depends_on:
      postgres:
        condition: service_healthy

  frontend:
    build:
      context: ./frontend
      dockerfile: Dockerfile
      target: dev
    environment:
      VITE_API_BASE_URL: ${VITE_API_BASE_URL}
    ports:
      - "5173:5173"
    volumes:
      - ./frontend/src:/app/src
      - ./frontend/public:/app/public
    depends_on:
      - api

volumes:
  postgres_data:
```

**Things to decide yourself (call out in your commit message):**
- Is `depends_on: condition: service_healthy` enough, or do you need a migration step before API starts? (Probably add a `migrate` one-shot service in Phase 1.)
- Frontend hot-reload — is bind-mount of `src/` + `public/` sufficient, or do you need `CHOKIDAR_USEPOLLING` on Windows/WSL? (Test on your machine.)
- Do you want to also expose Postgres outside the network (for DBeaver/psql)? Convenient but unusual in prod — worth an ADR if you keep it.

**Acceptance criteria:**
- [ ] `docker compose up --build` brings all three services up without errors.
- [ ] `curl http://localhost:8080/health` → `{"status":"ok"}`.
- [ ] `curl http://localhost:5173` → the Vite React index page.
- [ ] `docker compose down` cleanly stops; `docker compose down -v` removes the Postgres volume.
- [ ] `docker compose logs postgres | grep "ready to accept connections"` confirms Postgres started.

**Canonical reference (per Principle 11):**
- Docker Compose spec: [`compose-spec.io`](https://compose-spec.io/)
- "Best practices for writing Dockerfiles" — Docker docs (search).

**Learning output:**
Write a short `docs/db-notes/100-local-dev-compose.md` (~200 words) summarizing: what `depends_on.condition: service_healthy` does, what a healthcheck is, why named volumes beat bind-mounts for Postgres data. This becomes a retro talking point.

- [ ] **Step 11.1 (user): Write the compose file.**
- [ ] **Step 11.2 (user): Verify all 5 acceptance bullets.**
- [ ] **Step 11.3 (user): Write `docs/db-notes/100-local-dev-compose.md`.**
- [ ] **Step 11.4 (user): Commit.**

```bash
git -C /mnt/c/VM/Projects/StayNGo add docker-compose.yml docs/db-notes/100-local-dev-compose.md
git -C /mnt/c/VM/Projects/StayNGo commit -m "chore: docker-compose.yml — postgres + api + frontend

Notes: <your decisions on healthcheck/migrate/bind-mounts>"
```

---

## Task 12: EF Core DbContext + initial migration with GiST exclusion constraint `[by-hand]`

**User executes this task. This is the Phase 1 domain-modeling heart. Claude Code stands by for design-chat.**

This ticket exists in expanded form in spec §9.1 — read that before starting. It's the canonical example of "domain invariants belong in the schema."

**Files:**
- Create: `backend/StayNGo/src/StayNGo.Domain/Entities/User.cs`
- Create: `backend/StayNGo/src/StayNGo.Domain/Entities/Listing.cs`
- Create: `backend/StayNGo/src/StayNGo.Domain/Entities/Booking.cs`
- Create: `backend/StayNGo/src/StayNGo.Domain/Enums/ListingStatus.cs`
- Create: `backend/StayNGo/src/StayNGo.Domain/Enums/BookingStatus.cs`
- Create: `backend/StayNGo/src/StayNGo.Infrastructure/Persistence/StayNGoDbContext.cs`
- Create: `backend/StayNGo/src/StayNGo.Infrastructure/Persistence/Configurations/UserConfiguration.cs`
- Create: `backend/StayNGo/src/StayNGo.Infrastructure/Persistence/Configurations/ListingConfiguration.cs`
- Create: `backend/StayNGo/src/StayNGo.Infrastructure/Persistence/Configurations/BookingConfiguration.cs`
- Create: `backend/StayNGo/src/StayNGo.Infrastructure/Persistence/Migrations/*` (generated by `dotnet ef migrations add`)
- Create: `backend/StayNGo/tests/StayNGo.IntegrationTests/BookingExclusionConstraintTests.cs`
- Create: `docs/db-notes/001-gist-exclusion-constraint.md`

**Acceptance criteria (from spec §9.1):**
- [ ] Migration creates `btree_gist` extension via `CREATE EXTENSION IF NOT EXISTS btree_gist` (portable to Fly.io).
- [ ] `users` table: id, clerk_id UNIQUE, email UNIQUE (citext), display_name, created_at.
- [ ] `listings` table: id, owner_user_id FK, title, description, image_urls `text[]`, price_per_night_cents `bigint`, capacity `int`, timezone `text NOT NULL`, status `text NOT NULL`, created_at, updated_at.
- [ ] `bookings` table: id, listing_id FK, guest_user_id FK, `during daterange NOT NULL`, status `text NOT NULL`, created_at.
- [ ] EXCLUDE constraint: no two Confirmed bookings on the same listing overlap. Tested with an integration test that inserts two overlapping Confirmed bookings and asserts the second raises `23P01`.
- [ ] `docs/db-notes/001-gist-exclusion-constraint.md` written in own words (~300 words).

**Hard parts you'll have to figure out (this is where the learning lives):**

1. **EF Core doesn't natively express `CREATE EXTENSION` or `EXCLUDE USING gist`.** You'll need `migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS btree_gist")` at the top of Up, and hand-edited SQL for the EXCLUDE constraint (EF Core 10 may have limited support via `HasIndex(...).HasAnnotation("Npgsql:IndexMethod", "gist")` for regular indexes, but EXCLUDE constraints still need raw SQL).

2. **`daterange` mapping.** Npgsql 10 maps `NpgsqlRange<DateOnly>` to `daterange`. You'll import `NpgsqlTypes`. Alternately, treat `during` as a raw column and use `HasColumnType("daterange")` + bypass the type system in queries for Phase 0; refine in Phase 1.

3. **Running the migration.** Install EF CLI tooling once: `dotnet tool install --global dotnet-ef`. Then, from `backend/StayNGo/`:
   ```bash
   dotnet ef migrations add InitialSchema --project src/StayNGo.Infrastructure --startup-project src/StayNGo.Api
   dotnet ef database update --project src/StayNGo.Infrastructure --startup-project src/StayNGo.Api
   ```
   The API needs a connection string; pull from `appsettings.Development.json` or env var `ConnectionStrings__Postgres`.

4. **The EXCLUDE constraint SQL** (edit the generated migration's Up to append):
   ```csharp
   migrationBuilder.Sql(@"
       ALTER TABLE bookings
       ADD CONSTRAINT bookings_no_overlap_confirmed
       EXCLUDE USING gist (
           listing_id WITH =,
           during WITH &&
       ) WHERE (status = 'Confirmed')
   ");
   ```
   Matching `Down`:
   ```csharp
   migrationBuilder.Sql("ALTER TABLE bookings DROP CONSTRAINT bookings_no_overlap_confirmed");
   ```

5. **The integration test.** Spin up Testcontainers Postgres, run the migration against it, insert two overlapping Confirmed bookings, catch `PostgresException` with SqlState == "23P01". Full shape:

```csharp
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using StayNGo.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace StayNGo.IntegrationTests;

public class BookingExclusionConstraintTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _pg = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .Build();

    private StayNGoDbContext _db = null!;

    public async Task InitializeAsync()
    {
        await _pg.StartAsync();
        var options = new DbContextOptionsBuilder<StayNGoDbContext>()
            .UseNpgsql(_pg.GetConnectionString())
            .Options;
        _db = new StayNGoDbContext(options);
        await _db.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _db.DisposeAsync();
        await _pg.DisposeAsync();
    }

    [Fact]
    public async Task Overlapping_Confirmed_bookings_are_rejected_by_exclusion_constraint()
    {
        // Arrange — seed one user, one listing, one confirmed booking Jul 1–5
        // Act    — insert a second confirmed booking Jul 3–7 (overlaps)
        // Assert — SaveChanges throws PostgresException with SqlState == "23P01"

        var act = async () => await /* ... */;

        var ex = await act.Should().ThrowAsync<DbUpdateException>();
        ex.WithInnerException<PostgresException>()
          .Which.SqlState.Should().Be("23P01");
    }
}
```

Fill in the Arrange section with concrete entity inserts; the point is to exercise the real constraint.

**Canonical references (per Principle 11, paste these into the learning-references field when drafting this ticket on the board):**
- Postgres docs §8.17 "Range Types" — [postgresql.org/docs/current/rangetypes.html](https://www.postgresql.org/docs/current/rangetypes.html)
- Postgres docs §11.2 "Index Types" (GiST) — [postgresql.org/docs/current/indexes-types.html](https://www.postgresql.org/docs/current/indexes-types.html)
- Postgres docs §5.4.1 "Exclusion constraints" — [postgresql.org/docs/current/ddl-constraints.html](https://www.postgresql.org/docs/current/ddl-constraints.html)
- "PostgreSQL 14 Internals" by Egor Rogov, index access methods chapter — [postgrespro.com/community/books/internals](https://postgrespro.com/community/books/internals) (free PDF)
- Use The Index, Luke — [use-the-index-luke.com](https://use-the-index-luke.com/) (search "exclusion constraint")

**Learning output:**
`docs/db-notes/001-gist-exclusion-constraint.md` (~300 words) in own words, covering: what a GiST index is, why `btree_gist` extension is required to mix `listing_id` (scalar) with `during` (range) in the same constraint, how the EXCLUDE syntax reads, and one `EXPLAIN ANALYZE` of the index in action (run a SELECT that would conflict with the constraint and show how Postgres uses the index to enforce it).

- [ ] **Step 12.1 (user): Author domain entities + enums.**
- [ ] **Step 12.2 (user): Author DbContext + EF configurations.**
- [ ] **Step 12.3 (user): Run `dotnet ef migrations add InitialSchema`.**
- [ ] **Step 12.4 (user): Hand-edit the generated migration to add `CREATE EXTENSION` + EXCLUDE constraint SQL.**
- [ ] **Step 12.5 (user): Write the integration test.**
- [ ] **Step 12.6 (user): Run `dotnet test` — new test passes; existing smoke tests still pass.**
- [ ] **Step 12.7 (user): Write `docs/db-notes/001-gist-exclusion-constraint.md`.**
- [ ] **Step 12.8 (user): Commit.**

```bash
git -C /mnt/c/VM/Projects/StayNGo add backend/ docs/db-notes/001-gist-exclusion-constraint.md
git -C /mnt/c/VM/Projects/StayNGo commit -m "feat(db): initial schema with GiST exclusion on bookings

Adds users, listings, bookings tables. Enforces no-double-booking
as a Postgres EXCLUDE USING gist constraint, not app logic.
Requires btree_gist extension — created in the migration so the
setup is portable to Fly.io without init-db scripts.

Integration test exercises the real constraint against Testcontainers.
See docs/db-notes/001-gist-exclusion-constraint.md for a summary."
```

---

## Task 13: CI workflow (`.github/workflows/ci.yml`) `[by-claude]`

**Files:**
- Create: `/mnt/c/VM/Projects/StayNGo/.github/workflows/ci.yml`

- [ ] **Step 13.1: Write ci.yml**

Write `/mnt/c/VM/Projects/StayNGo/.github/workflows/ci.yml`:

```yaml
name: CI

on:
  pull_request:
    branches: [main]
  push:
    branches: [main]

concurrency:
  group: ci-${{ github.ref }}
  cancel-in-progress: true

jobs:
  backend:
    runs-on: ubuntu-latest
    defaults:
      run:
        working-directory: backend/StayNGo
    steps:
      - uses: actions/checkout@v5
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
      - name: Restore
        run: dotnet restore
      - name: Build
        run: dotnet build --no-restore --configuration Release
      - name: Test
        run: dotnet test --no-build --configuration Release --logger "trx;LogFileName=test-results.trx"
      - name: Upload test results
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: dotnet-test-results
          path: backend/StayNGo/**/TestResults/*.trx

  frontend:
    runs-on: ubuntu-latest
    defaults:
      run:
        working-directory: frontend
    steps:
      - uses: actions/checkout@v5
      - uses: actions/setup-node@v4
        with:
          node-version: '20'
          cache: 'npm'
          cache-dependency-path: frontend/package-lock.json
      - name: Install
        run: npm ci
      - name: Typecheck
        run: npx tsc --noEmit
      - name: Build
        run: npm run build

  container-build:
    runs-on: ubuntu-latest
    needs: [backend, frontend]
    if: github.event_name == 'push' && github.ref == 'refs/heads/main'
    steps:
      - uses: actions/checkout@v5
      - name: Build API image
        run: docker build -t stayngo-api:ci ./backend/StayNGo
      - name: Build frontend image (prod target)
        run: docker build --target build -t stayngo-frontend:ci ./frontend
```

Rationale:
- `backend` and `frontend` jobs are independent — fail fast, parallel.
- `container-build` runs only on `push` to `main` (not on PRs — saves minutes).
- `concurrency` cancels superseded runs.
- Testcontainers works on `ubuntu-latest` out of the box (Docker is preinstalled). Expect longer run times than local.

- [ ] **Step 13.2: Validate yaml syntax**

Run: `python3 -c "import yaml; yaml.safe_load(open('/mnt/c/VM/Projects/StayNGo/.github/workflows/ci.yml'))"`
Expected: no output.

- [ ] **Step 13.3: Commit**

```bash
git -C /mnt/c/VM/Projects/StayNGo add .github/workflows/ci.yml
git -C /mnt/c/VM/Projects/StayNGo commit -m "chore(ci): add build + test + container-build workflow"
```

---

## Task 14: PR review workflow + claude-review-prompt `[by-hand]`

**User executes this task. Involves secrets management (ANTHROPIC_API_KEY) — do it yourself so you know where the secret lives.**

**Files:**
- Create: `/mnt/c/VM/Projects/StayNGo/.github/workflows/pr-review.yml`
- Create: `/mnt/c/VM/Projects/StayNGo/.github/claude-review-prompt.md`

**Source:** Spec §7.3 (workflow — indicative, verify against current action docs) and §7.4 (prompt — verbatim).

- [ ] **Step 14.1 (user): Write pr-review.yml**

Copy spec §7.3 content into `/mnt/c/VM/Projects/StayNGo/.github/workflows/pr-review.yml`. Before relying on it, check the current [`anthropics/claude-code-action` docs](https://github.com/anthropics/claude-code-action) — the action's inputs may have changed since 2026-04-19. Adjust `mode`, `prompt_file`, or equivalents to match the current API.

- [ ] **Step 14.2 (user): Write claude-review-prompt.md**

Copy spec §7.4 content verbatim into `/mnt/c/VM/Projects/StayNGo/.github/claude-review-prompt.md`.

- [ ] **Step 14.3 (user): Add acceptance seed — a deliberately-violating test PR**

Per the spec-updated acceptance for this ticket: after the repo is on GitHub (Task 15) and `ANTHROPIC_API_KEY` is set as a repo secret, open a test PR that deliberately violates a principle (e.g., add an `IRepository<T>` interface — red-flag from CLAUDE.md). Claude Code review should post at least one comment citing the principle/red-flag. Close the PR without merging.

**This last sub-step waits until after Task 15 (repo is on GitHub) and the secret is set. Move on to Task 15 now; return here once the remote is live.**

- [ ] **Step 14.4 (user): Commit the local files.**

```bash
git -C /mnt/c/VM/Projects/StayNGo add .github/workflows/pr-review.yml .github/claude-review-prompt.md
git -C /mnt/c/VM/Projects/StayNGo commit -m "chore(ci): add Claude Code PR review workflow + prompt"
```

---

## Task 15: Create GitHub remote + push `[by-hand]`

**User executes. Owning the GitHub account setup + secrets lives here.**

- [ ] **Step 15.1 (user): Decide repo visibility.**

Public if you want the portfolio visible immediately; private otherwise (can flip to public later — ADR-worthy moment if you delay).

- [ ] **Step 15.2 (user): Create the remote repo.**

```bash
gh repo create stayngo --source=/mnt/c/VM/Projects/StayNGo --remote=origin --<public|private>
```

(Replace `<public|private>`. `gh auth status` must be green; see prerequisites.)

- [ ] **Step 15.3 (user): Push main.**

```bash
git -C /mnt/c/VM/Projects/StayNGo push -u origin main
```

- [ ] **Step 15.4 (user): Set `ANTHROPIC_API_KEY` secret.**

```bash
gh secret set ANTHROPIC_API_KEY --repo <owner>/stayngo
# Paste the key when prompted.
```

- [ ] **Step 15.5 (user): Return to Task 14.3** and run the deliberately-violating test PR to verify the review workflow fires.

- [ ] **Step 15.6 (user): (No local commit needed — Task 15 is a GitHub-side action.)**

---

## Task 16: GitHub Project board — columns, custom fields, labels `[by-hand]`

**User executes. Projects v2 is a GraphQL API — automation via `gh` is possible but clunky; web UI is faster first time.**

**Board shape (from spec §10):**
- **Columns (Status):** `Backlog`, `Ready`, `In Progress`, `In Review`, `Done`.
- **Custom fields:**
  - `Phase` — single-select: `0`, `1`, `2`, `3`, `4`, `5`, `6`.
  - `Size` — single-select: `S (<2h)`, `M (2-4h)`, `L (4-8h)`.
  - `Executor` — single-select: `by-claude`, `by-hand`.
  - `Epic` — linked issue.
- **Labels (on the repo, not the project):**
  - type: `feature`, `bug`, `chore`, `refactor`, `adr`, `spike`
  - executor: `by-claude`, `by-hand`
  - phase: `phase-0`, `phase-1`, `phase-2`, `phase-3`, `phase-4`, `phase-5`, `phase-6`
  - flags: `blocked`, `learning`

- [ ] **Step 16.1 (user): Create the Project.**

Web: github.com → your profile → Projects → New project → Board template → name it `StayNGo`. Link it to the `stayngo` repo via the Project settings ("Manage access").

- [ ] **Step 16.2 (user): Set up columns.**

Default board has `Todo`/`In Progress`/`Done` — rename and add. Target: `Backlog` → `Ready` → `In Progress` → `In Review` → `Done`.

- [ ] **Step 16.3 (user): Add custom fields.**

In Project settings → Fields → `+ New field`:
- `Phase` (single-select) → add options 0–6.
- `Size` (single-select) → add `S (<2h)`, `M (2-4h)`, `L (4-8h)`.
- `Executor` (single-select) → add `by-claude`, `by-hand`.

(`Epic` field is auto-created when linking parent issues — leave for now.)

- [ ] **Step 16.4 (user): Create the labels on the repo.**

```bash
# Type labels
gh label create feature   --repo <owner>/stayngo --color 1d76db
gh label create bug       --repo <owner>/stayngo --color d93f0b
gh label create chore     --repo <owner>/stayngo --color cccccc
gh label create refactor  --repo <owner>/stayngo --color fbca04
gh label create adr       --repo <owner>/stayngo --color 5319e7
gh label create spike     --repo <owner>/stayngo --color b60205

# Executor labels
gh label create by-claude --repo <owner>/stayngo --color 0e8a16
gh label create by-hand   --repo <owner>/stayngo --color bfdadc

# Phase labels
for n in 0 1 2 3 4 5 6; do
  gh label create "phase-$n" --repo <owner>/stayngo --color c5def5
done

# Flag labels
gh label create blocked  --repo <owner>/stayngo --color e11d21
gh label create learning --repo <owner>/stayngo --color 7057ff
```

- [ ] **Step 16.5 (user): (No commit — this lives on GitHub, not in the repo.)**

---

## Task 17: Seed 8 epic issues on the board `[by-claude]`

**Files (no repo changes; creates GitHub issues via `gh`):**

Spec §10.4 lists 8 epics. Each opens as an issue with a `feature` + `phase-<N>` label; the Project custom field `Epic` references them as parents for future tickets.

- [ ] **Step 17.1: Author and create all 8 epic issues.**

Run each command (substitute `<owner>/stayngo`). Each `gh issue create` prints the issue URL; capture them for reference.

```bash
OWNER=<owner>  # <-- set your GitHub username/org
REPO=$OWNER/stayngo

# 1. Foundation / Repo / CI (Phase 0)
gh issue create --repo $REPO --title "EPIC: Foundation / Repo / CI" \
  --label feature,phase-0 \
  --body "Phase 0 foundation work — monorepo, docker-compose local env, CI workflow, PR review automation, issue templates, ADRs, GitHub Project board with 8 epics. End state: \`docker compose up\` works locally; CI green on a trivial PR; board populated.

Sub-tickets: the 15 Phase 0 seed tickets from the spec (spec §9).

Closes when: every Phase 0 seed ticket is \`Done\` on the board and a trivial PR ships green through CI + PR review."

# 2. Identity — Clerk (Phase 1)
gh issue create --repo $REPO --title "EPIC: Identity — Clerk" \
  --label feature,phase-1 \
  --body "Integrate Clerk for managed auth. Clerk issues JWTs; API validates via JWT middleware; a \`Me\` endpoint + \`OnSignUp\` handler bridge Clerk users into the local \`users\` table.

Out of scope for Phase 1:
- Roles, permissions, IsAdmin (we have none — authorization is ownership-based)
- MFA, SSO, organizations
- Sessions across devices"

# 3. Listings (Phase 1)
gh issue create --repo $REPO --title "EPIC: Listings" \
  --label feature,phase-1 \
  --body "Listing CRUD + lifecycle (\`Draft\` → \`Active\` → \`Archived\`). All transitions owner-initiated; no admin, no approval gate.

Sub-flows (spec §5.5):
- Create listing → Draft
- Edit listing (Draft freely; Active: price/description/photos)
- Publish listing → Active
- Archive listing → Archived (terminal, immutable)

Out of scope:
- Photo upload to object storage (use URLs only for Phase 1)
- Seasonal pricing
- Cancellation policies per listing"

# 4. Search (Phase 1)
gh issue create --repo $REPO --title "EPIC: Search" \
  --label feature,phase-1 \
  --body "Guest search over \`Active\` listings: location + date range + guests + price max. Pagination.

Uses the GiST index that enforces the exclusion constraint as the availability-lookup index (spec §5.4).

Out of scope:
- Full-text search relevance (Phase 5 via MongoDB/Elasticsearch)
- Map-based search
- Filters beyond the four above"

# 5. Booking (Phase 1)
gh issue create --repo $REPO --title "EPIC: Booking" \
  --label feature,phase-1 \
  --body "Concurrency, idempotency, availability, host confirm/reject, guest cancel.

Hard questions answered in spec §5.4:
- Concurrency: Postgres EXCLUDE constraint (already in Phase 0 migration)
- Idempotency: \`Idempotency-Key\` header + table with 24h TTL
- Availability: range predicate backed by the GiST exclusion index
- Timezones: property's local timezone (stored on Listing)
- Cancellation: free up to 24h before check-in
- Pricing: flat nightly rate

Flows:
- Guest creates booking → Pending (cannot book own listing)
- Host confirms → Confirmed (DB enforces no-overlap)
- Host rejects → Cancelled
- Guest cancels → Cancelled"

# 6. Frontend Shell (Phase 1)
gh issue create --repo $REPO --title "EPIC: Frontend Shell" \
  --label feature,phase-1 \
  --body "Routing (public / authenticated / \`/host/*\`), auth flows via Clerk React SDK, layout (header, footer, nav), shadcn components in use for buttons/inputs/cards, TanStack Query provider wired.

One React app serves both guest and host views. Host routes guarded by 'user is logged in' — no role check.

Out of scope:
- Complex loading states (suspense choreography)
- Optimistic updates (Phase 3 refinement)
- Accessibility audit (sanity-only for Phase 1)"

# 7. Observability Basics (Phase 1–2)
gh issue create --repo $REPO --title "EPIC: Observability Basics" \
  --label feature,phase-1 \
  --body "Structured logging (Serilog), \`ProblemDetails\` error responses, correlation-id middleware, \`/health\` + \`/ready\` endpoints.

Phase 2 extension: OpenTelemetry export to Grafana Cloud free tier.

Out of scope for Phase 1:
- Distributed tracing (Phase 2)
- APM dashboards
- Alerting rules"

# 8. Deploy to Fly.io (Phase 1 demo milestone)
gh issue create --repo $REPO --title "EPIC: Deploy to Fly.io" \
  --label feature,phase-1 \
  --body "Final Phase 1 milestone: public URL where a stranger can register, publish a listing, and book.

Sub-tickets:
- \`infra/fly.toml\` written
- Managed Postgres provisioned on Fly
- Staging + prod apps (Phase 2 refinement)
- CORS + Clerk production keys
- Custom domain (optional; subdomain under your apex is fine)

Done when: cold-start the public URL, sign up, publish, book — all green."
```

- [ ] **Step 17.2: Attach all 8 issues to the Project board.**

For each of the 8 URLs from Step 17.1, use the web UI (Project → `+` at bottom of any column → paste URL) or `gh`:

```bash
# Discover the Project number
gh api graphql -f query='{user(login:"'$OWNER'"){projectsV2(first:20){nodes{id,number,title}}}}' --jq '.data.user.projectsV2.nodes[] | select(.title=="StayNGo")'

# For each epic issue (example with issue #2):
gh project item-add <project-number> --owner $OWNER --url https://github.com/$REPO/issues/2
```

- [ ] **Step 17.3: (No commit — these exist on GitHub.)**

---

## Optional study spikes (Tasks 18–22) `[SPIKE][by-hand]`

From spec §9.2. User decides pacing. None block Phase 1 kickoff.

### Task 18: `[spike]` Use The Index Luke §1–3

**Output:** `/mnt/c/VM/Projects/StayNGo/docs/db-notes/001a-btree-anatomy.md`

- [ ] Read [use-the-index-luke.com](https://use-the-index-luke.com/) Preface, Anatomy of an Index, The Where Clause.
- [ ] Write ~300 words in your own words covering: what a B-tree actually stores at each node, the "leftmost prefix" rule for composite indexes, and one concrete query from this repo that would benefit from an index.
- [ ] Commit: `git -C /mnt/c/VM/Projects/StayNGo add docs/db-notes/001a-btree-anatomy.md && git -C /mnt/c/VM/Projects/StayNGo commit -m "docs(notes): b-tree anatomy notes from Use The Index Luke"`

### Task 19: `[spike]` Postgres docs §11 Indexes

**Output:** `docs/db-notes/002-pg-index-types.md`

- [ ] Read [postgresql.org/docs/current/indexes-types.html](https://www.postgresql.org/docs/current/indexes-types.html).
- [ ] Write ~300 words on: when B-tree wins, when GiST wins, when GIN wins, when BRIN wins, when hash wins. One StayNGo query example per type (even if hypothetical).
- [ ] Commit: `... && git commit -m "docs(notes): pg index type selection cheat-sheet"`

### Task 20: `[spike]` Postgres docs §8.17 Range Types + Fowler "Range"

**Output:** `docs/db-notes/003-daterange-patterns.md`

- [ ] Read [postgresql.org/docs/current/rangetypes.html](https://www.postgresql.org/docs/current/rangetypes.html) + search "martinfowler.com Range" for the bliki entry.
- [ ] Write ~300 words on: `daterange` bounds inclusivity (`[)` default), operators (`@>`, `&&`, `<@`), and why we chose `[checkIn, checkOut)` semantics for bookings.
- [ ] Commit: `... && git commit -m "docs(notes): daterange semantics for bookings"`

### Task 21: `[spike]` Idempotency (Fowler / Stripe)

**Output:** `docs/db-notes/004-idempotency-keys.md`

- [ ] Read Fowler's page on idempotency, or the Stripe engineering blog post "Designing robust and predictable APIs with idempotency".
- [ ] Write ~300 words on: the difference between idempotency and de-duplication, what goes in an idempotency key table schema, and how to handle "same key, different body" (usually `422`).
- [ ] Commit: `... && git commit -m "docs(notes): idempotency-key table design"`

### Task 22: `[spike]` CMU 15-445 lectures 1–3

**Output:** `docs/db-notes/005-cmu-fundamentals.md`

- [ ] Watch [CMU 15-445](https://www.youtube.com/playlist?list=PLSE8ODhjZXjaKScG3l0nuOiDTTqpfnWFf) lectures 1–3 (Relational model, Data storage, B+ trees).
- [ ] Write ~300 words summarizing each lecture's one key idea. Note any point where your prior understanding was wrong.
- [ ] Commit: `... && git commit -m "docs(notes): CMU 15-445 fundamentals (lectures 1-3)"`

---

## End of Phase 0 — verification checklist

After Tasks 1–17 are merged to `main`:

- [ ] `git -C /mnt/c/VM/Projects/StayNGo log --oneline | wc -l` ≥ 16 commits (initial + ~15 tasks).
- [ ] `docker compose up` brings all 3 services up without errors.
- [ ] `curl http://localhost:8080/health` → `{"status":"ok"}`.
- [ ] `curl http://localhost:5173` → Vite React page.
- [ ] `cd backend/StayNGo && dotnet test` → all tests pass (including the GiST exclusion constraint test from Task 12).
- [ ] `cd frontend && npm run build` → succeeds.
- [ ] Open a trivial PR on GitHub (e.g. tweak README) → CI workflow green; Claude Code PR review posts at least one comment.
- [ ] GitHub Project board has 5 columns, 3 custom fields, 13 labels, 8 epic issues.

All green → you are ready to start Phase 1. First Phase 1 ticket: `[feature] Clerk integration — JWT validation in API + /api/me endpoint` (or whichever epic you want to open).

---

## Self-review checklist (plan author, post-write)

- [x] Every Phase 0 seed ticket from spec §9 mapped to a task: tickets 1 (README/CHANGELOG/placeholder dirs) + 2 (StayNGo.md+CLAUDE.md split into Tasks 2+3) + 3 (Task 8) + 4 (Task 9) + 5 (Task 11) + 6 (Task 10) + 7 (Task 12) + 8 (Task 13) + 9 (Task 4) + 10 (Task 5) + 11 (Task 14) + 12 (Task 6) + 13 (Task 7) + 14 (Task 16) + 15 (Task 17). Plus Task 15 (GitHub push) which is implicit in the original ticket 14's prerequisites.
- [x] All 5 optional spikes from §9.2 mapped: Tasks 18–22.
- [x] Every `by-hand` ticket from the spec is planned as user-executes with Claude Code in support mode: Tasks 11, 12, 14, 15, 16 — all marked `[by-hand]` and written with user-as-executor framing.
- [x] No placeholder text like "TBD" or "similar to Task N" — each step has either actual content or a direct instruction ("copy spec §X verbatim, with this delta").
- [x] No fabricated URLs — references are to canonical authority pages (Postgres docs, Martin Fowler's bliki, CMU 15-445 YouTube playlist, use-the-index-luke.com) or searchable titles.
- [x] Exact file paths throughout.
- [x] Task 12 has the most complete learning-output pattern (domain model + migration + integration test + `docs/db-notes/001-...md`) since it's the canonical example from spec §9.1.
- [x] Commit messages follow Conventional Commits (`chore:`, `docs:`, `feat(db):`, etc.).
