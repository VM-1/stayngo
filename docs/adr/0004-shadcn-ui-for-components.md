# 0004. UI component library: shadcn/ui

Date: 2026-06-11
Status: Accepted

## Context

ADR-0001 listed "shadcn/ui" in the Phase 1 frontend stack but never justified it against alternatives. The question surfaced directly during Phase 1 frontend work: rather than hand-roll inputs, dialogs, dropdowns, selects, date pickers — and their focus-management / keyboard / ARIA behavior — we want a component library. Two batteries-included options were raised, **MUI** and **Ant Design**, alongside the already-scaffolded shadcn/ui.

Forces:

- Stack is React 19 + Vite + **Tailwind CSS v4** (ADR-0001 — note 0001 said React 18; since bumped to 19).
- The frontend is **customer-facing with a bespoke design** (Figma: slate + indigo-600), not an internal admin tool.
- shadcn/ui is **already initialized** in `frontend/`: `components.json` (new-york / slate), `cn()` in `src/lib/utils.ts`, and the full dependency set (`@radix-ui/react-slot`, `class-variance-authority`, `clsx`, `tailwind-merge`, `lucide-react`).
- Clerk already renders its own auth UI (`<SignIn>`, `<UserButton>`).

## Decision

Standardize the frontend on **shadcn/ui**: copy-in component source under `src/components/ui/` = **Radix UI primitives** (accessible behavior) + Tailwind styling + `cva` variants. Components are added on demand via `npx shadcn@latest add <name>`; we own and may edit the resulting files.

For complex widgets outside shadcn's core, prefer **Tailwind-native headless libraries** over adopting a second design system:

- date ranges (bookings) → shadcn `Calendar` (wraps **react-day-picker**)
- data grids → **TanStack Table** (headless; sibling of the `@tanstack/react-query` already in use)

**Reject MUI and Ant Design.** Both are CSS-in-JS (Emotion / `@ant-design/cssinjs`) — a styling system that competes with Tailwind — and both impose an opinionated visual identity that fights a bespoke design. Ant Design additionally collides with Tailwind's Preflight reset (requires deliberate CSS cascade-layer ordering) and needs a React-19 compatibility patch for its static APIs. **DaisyUI** was rejected too: it is CSS-only and ships no behavior/a11y logic, which is the actual requirement. **Headless UI** (Tailwind Labs) is viable but offers a smaller primitive set than Radix.

## Consequences

**Positive**

- One styling system (Tailwind) end-to-end — no token duplication, no cascade-layer wrangling.
- Radix supplies the hard, error-prone behavior (focus traps, keyboard nav, popover positioning, ARIA) we explicitly did not want to hand-roll.
- We own the component code: editable, greppable, no black-box API lock-in; the bundle stays lean (only added components ship).
- Zero adoption friction — already initialized.

**Negative**

- shadcn components are **vendored source, not a versioned dependency**: upstream fixes don't arrive via `npm update`; adopting them means re-running the CLI and diffing. We own the a11y of any edits we make.
- No single "import everything" surface — each needed component is an explicit, tracked addition (a feature for bundle size, a small ceremony otherwise).

**Neutral**

- Elaborates — does not supersede — ADR-0001's frontend line; also records the React 18 → 19 drift in passing.
- Clerk's own UI coexists unchanged; shadcn is for our components, not for restyling Clerk internals.
- The slate + indigo-600 palette referenced above was a placeholder; the deliberate brand palette is recorded in **ADR-0005 ("Aubergine Soirée")**.

## References

- shadcn/ui — ui.shadcn.com/docs (copy-in model, `cn()`, `cva`)
- Radix UI Primitives — radix-ui.com/primitives (the accessible behavior layer; "why headless")
- Ant Design "Use with Tailwind CSS" — ant.design/docs/react/compatible-style (the cssinjs / cascade-layer / Preflight cost, recorded for the rejected option)
