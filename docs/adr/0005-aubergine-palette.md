# 0005. Brand palette: "Aubergine Soirée"

Date: 2026-06-15
Status: Accepted

## Context

ADR-0004 adopted shadcn/ui and, incidentally, kept near-default tokens — a slate base with an indigo-600 primary. That was a placeholder, not a deliberate brand choice. As the Phase-1 design system was built out in Figma, StayNGo needed an intentional, cohesive palette that (a) reads as a distinct brand rather than the shadcn default, and (b) maps cleanly onto the shadcn/Tailwind token model (one `--primary`, semantic status colors, light + dark).

A design-exploration pass produced several cohesive candidate palettes; **"Aubergine Soirée"** was chosen.

## Decision

Adopt **"Aubergine Soirée"** as the brand palette, defined as shadcn design tokens in `frontend/src/index.css`:

- **Primary:** plum-600 (`oklch(0.489 0.132 332.7)`) — replaces indigo-600. `--ring` matches it.
- **Base / neutrals:** mauve-gray (`--foreground`, `--muted`, `--border`, …).
- **Status, not brand:** `--success` (emerald-600) and `--warning` (amber-700) tokens — used by `StatusBadge` and status surfaces, deliberately kept *separate* from the plum brand so "Confirmed/Active" reads as success rather than brand.
- Tokens are the single source of truth: components reference semantic tokens (`bg-primary`, `text-muted-foreground`, the `Text`/`Button`/`Badge` variants), not hardcoded colors. The Figma design system mirrors the same tokens.

This supersedes the incidental slate / indigo-600 color choice noted in ADR-0004; ADR-0004's component-library decision itself stands.

## Consequences

**Positive**

- A distinct, cohesive brand instead of the shadcn default — one palette across Figma and the app.
- Semantic tokens make a future re-theme a token edit, not a find-replace across components.
- Status (`success`/`warning`) is orthogonal to brand, so status colors stay meaningful regardless of the primary.

**Negative**

- Two sources to keep in sync (Figma + `index.css`); drift is possible until tokens are generated from one source.
- Dark-mode token values are defined but not yet exercised (no theme toggle).

**Neutral**

- Updates only ADR-0004's incidental color mention; the shadcn/ui decision is unchanged.

## References

- "Aubergine Soirée" palette + Phase-1 design system — `docs/design/2026-06-14-figma-design-system-and-screens-brief.md`; token cheat sheet — `docs/Concepts/Color tokens cheat sheet.md`.
- OKLCH color model — MDN `oklch()` (developer.mozilla.org/en-US/docs/Web/CSS/color_value/oklch): why tokens use oklch for perceptually-even lightness/chroma.
