---
type: retro
phase: 0
status: evergreen
created: 2026-05-31
---

# Phase 0 retro — StayNGo

> Phase 0 ran ~2026-04-20 (plan written) → 2026-05-31 (closeout). Goal per the
> [plan](../superpowers/plans/2026-04-20-staynGo-phase-0.md): a monorepo with CI, a
> docker-compose local env, a Project board, issue/PR templates, ADRs, and 8 seeded epics —
> all green on a trivial PR, ready for Phase 1. Honest 1-pager per Principle 3: *real
> discipline, not theater.* Full session detail lives in [[2026-05-31]] (Journal).

---

## TL;DR

Phase 0 hit its goal: the foundation is demo-able, on `origin/main` (16 commits), with a
real domain invariant (`bookings_no_overlap_confirmed`) enforced in the schema and covered by
a passing Testcontainers test. Two planned acceptance items were **deliberately waived** (the
GitHub-Actions PR-review automation, and the exact `db-notes/001` path), and the genuine
value — and time sink — was the domain-modeling depth in Task 12, not the scaffolding. The
scope grew one thing the plan never listed: a full Obsidian vault under `docs/` (worth an
honest look below).

## What shipped (vs plan Tasks 1–17)

- **Tasks 1–7 (meta/docs):** README, CHANGELOG, placeholder dirs, `StayNGo.md`, `CLAUDE.md`,
  issue + PR templates, ADR template + ADR-0001 (stack) + ADR-0002 (monorepo). ✅
- **Tasks 8–9 (scaffold):** .NET 10 backend (`sln` + Api/Domain/Infrastructure + Unit/Integration
  tests + Dockerfile); Vite + React + TS-strict + Tailwind + shadcn frontend. ✅
- **Task 10–11 (env):** `.env.example`; docker-compose (postgres + api + frontend), `[by-hand]`,
  with `100-local-dev-compose` db-note. ✅
- **Task 12 (the heart):** EF Core schema — User/Listing/Booking, `Money` + `DateRange` value
  objects, `BookingStatus`/`ListingStatus`, snake_case conventions, `InitialScheme` migration with
  `btree_gist` + `citext`, the `EXCLUDE USING gist` constraint, and a passing integration test
  asserting SqlState `23P01`. ✅ See [[GiST exclusion constraint]].
- **Task 13 (CI):** build + test + container-build workflow. ✅
- **Task 15–17 (GitHub):** private remote `VM-1/stayngo`, Project board (5 cols + Phase/Size/Executor
  fields), 17 labels, 8 epic issues (#1–#8). ✅
- **Task 14 (PR-review automation):** **waived by decision** — see below.

## What changed vs the plan

| Item | Plan said | What we did | Why |
|---|---|---|---|
| Enum storage | `status text NOT NULL`, `WHERE (status = 'Confirmed')` | `int`; `WHERE (status = 2)` with explicit `(int)` cast | User's call — smaller storage; cost is readability + a discipline rule (keep explicit `= N` on every enum value). |
| Pricing | `price_per_night_cents bigint` column | `Money` value object (`long AmountCents` + `Currency`) via `ComplexProperties` | Value object over primitive (avoids a red-flag); richer model than planned. Iterated **3×** to land. |
| User/roles | (interim) `UserRole { Guest, Host, Admin }` enum | deleted enum → `is_admin` boolean; Guest/Host **implicit from relationships** | Ownership-based authz; no role column. JIT provisioning deferred to Phase 1. |
| `DateRange` | use `NpgsqlRange<DateOnly>` directly / raw column | `readonly record struct` + `HasConversion<DateRangeConverter>`, domain kept Npgsql-free | Cleaner domain purity than the plan's two suggested shortcuts. |
| Extensions | `migrationBuilder.Sql("CREATE EXTENSION …")` | Npgsql annotations (`HasPostgresExtension` / `citext` column type) | Cleaner generated migration; EXCLUDE still needs raw SQL (no EF API — Npgsql #25103). |
| db-note 001 | `docs/db-notes/001-gist-exclusion-constraint.md` (~300 words, own words) | satisfied via Concept note [[GiST exclusion constraint]] instead | Decision logged; the *own-words* learning still exists, just not at the planned path. |
| PR review (Task 14) | GH Action + `ANTHROPIC_API_KEY` + deliberately-violating test PR | **skipped** — reviews happen in Claude Code (`/code-review`) | No API-key secret to manage; interactive review is the actual workflow. |
| Migration name | `InitialSchema` | `InitialScheme` (typo, frozen in history) | Not worth a rename. |
| Test isolation | (unspecified) | per-class Testcontainer (`IAsyncLifetime`) — fresh DB per test | Isolation over speed for Task 12; Phase 1 moves to shared container + `WebApplicationFactory` (**not** MediatR). |
| **Obsidian vault** | **not in plan** | added full vault under `docs/` (MOCs, Concepts, Domain, References, Journal, Templates, vault `CLAUDE.md`) | Supports Principle 11 (teach). Scope-add — examine honestly below. |

## Bugs & catches worth remembering

- **Migration placement bug, caught pre-apply:** generated `Up()` put the `EXCLUDE` SQL *before*
  `CreateTable("bookings")` — would have failed with `relation "bookings" does not exist`. Moved to
  the bottom of `Up()`. (Good: reviewed generated SQL before running it.)
- **snake_case missing in test setup:** the naming convention has to be applied in
  `BookingExclusionConstraintTests.InitializeAsync` too, or EF emits PascalCase columns and the
  migration's hardcoded snake_case references fail cryptically.
- **`(int)` cast in interpolated SQL:** `$"… status = {BookingStatus.Confirmed}"` emits the *name*
  (`Confirmed`), not `2` — needs an explicit `(int)` cast.
- **`Money` API smell:** `Multiply(Money)` (money² is meaningless) → `Multiply(int factor)`.

## Definition of Done — honest status

Against the plan's end-of-phase checklist (plan §"End of Phase 0"):

- ✅ ≥16 commits on `main`; `docker compose up` works; `/health` ok; frontend builds; `dotnet test`
  green (incl. the GiST test); board has columns/fields/labels/8 epics.
- ⚠️ **"Trivial PR → CI green + Claude review posts a comment"** — CI half verified; the **PR-review
  comment** half is **waived** (Task 14 skipped by decision). The end-state is honest about this.
- ✅ **`db-notes/001`** — the own-words learning lives in the Concept note [[GiST exclusion constraint]];
  a pointer stub now exists at `db-notes/001-gist-exclusion-constraint.md` so the plan's expected path resolves.

Net: the *intent* of Phase 0 is met; two checklist items were consciously substituted, not silently dropped.

---

## Reflection

> **Draft bullets below are scaffolding from the evidence — confirm, cut, or rewrite each in your
> own words.** Per the vault learning rule, this section is yours; the value is your honest read,
> not my reconstruction. Tell me to blank it and you'll write from scratch if you'd rather.

### What worked *(draft)*

- Putting the no-double-booking invariant **in the schema** (EXCLUDE + Testcontainers) instead of
  handler logic — the canonical Phase-0 lesson, and it actually landed.
- Reviewing generated migration SQL before applying caught a bug that would've failed at runtime.
- `[by-hand]` on the domain-heavy task (12) — the learning friction was the point.

### What didn't *(draft)*

- `Money` took 3 swings — was that healthy iteration, or should a value-object shape have been
  pinned before touching EF? *(your call)*
- The plan assumed `text` enums and a bare `bigint` price; reality diverged early. Did the plan
  under-specify the domain, or is divergence just expected once you model for real? *(your call)*

### What I'd change *(draft)*

- Decide value-object shapes (Money, DateRange) on paper before wiring EF mapping.
- If a planned acceptance item is going to be waived (Task 14, db-note path), note the waiver *at the
  moment of the decision*, not at closeout.

### Cadence honesty check *(yours — Principle 9)*

Plan written 2026-04-20, closed 2026-05-31 ≈ 6 calendar weeks. Did the **3× evening-work multiplier**
hold for the tasks you actually did? Where did time really go (hint from the journal: Task 12 + the
vault)? *Only you have the hours — fill this in.*

### Open question: the Obsidian vault *(flagging per Principle 10)*

The vault wasn't in the plan. It serves Principle 11 (teach) — but it's also exactly the kind of
**invisible work** Principle 1 warns kills solo momentum, and the kind of complexity Principle 2 says
to add only when it hurts *or* as a **named learning milestone**. Honest question for you: was it a
justified learning-infrastructure investment, or scope creep dressed as discipline? If you keep it,
consider writing it down as a deliberate decision (ADR or a one-liner in the spec) so it's *named*,
not accidental.
---

## Carry into Phase 1

- **First ticket = Identity — Clerk (#2):** restore the stashed `Services/` scaffold + the exact
  JWT-bearer + `.csproj` diff recorded in [[2026-05-31]]; add the `Me` endpoint + integration test.
- **Test harness:** adopt shared-container `WebApplicationFactory` + `IClassFixture` + truncate cleanup —
  with **direct handler injection, not MediatR** (CLAUDE.md hard rule).
- **Discipline:** keep explicit `= N` on every enum value (the EXCLUDE constraint depends on `status = 2`).
- **Decide `FluentAssertions` future** — v8+ is commercial; on v6.12.2 now; switch to `AwesomeAssertions` if upgrading past v7.
- **Board polish + sub-tickets:** set `Status = Backlog` / `Phase = N` on the 8 epics; create the actual
  Phase 1 sub-tickets under #2–#8.
- **Memory layer:** the `memory/` dir is empty (the journal's claimed persisted memories aren't there) —
  rebuild from the journal when convenient.

## Links

- Journal: [[2026-05-31]] · Plan: [`2026-04-20-staynGo-phase-0.md`](../superpowers/plans/2026-04-20-staynGo-phase-0.md) · Spec: [`2026-04-19-staynGo-design.md`](../superpowers/specs/2026-04-19-staynGo-design.md)
- ADRs: [`0001-initial-stack.md`](../adr/0001-initial-stack.md) · [`0002-monorepo.md`](../adr/0002-monorepo.md)
- Concept: [[GiST exclusion constraint]] · [[Booking invariants]] · [[Lifecycles]]
