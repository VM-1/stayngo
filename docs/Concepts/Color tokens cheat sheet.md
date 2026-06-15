---
type: reference
track: [frontend]
phase: 1
status: evergreen
created: 2026-06-14
---

# Color tokens cheat sheet

How to use the StayNGo design tokens in Tailwind — for borders, text, backgrounds, etc.
All classes below map to CSS variables in `frontend/src/index.css`, so they auto-follow the
active palette (currently **Aubergine Soirée**, plum `#8A3F80`). Change the palette in one place
and everything updates. See [[Design tokens]] and the master brief in `docs/design/`.

> **Golden rule:** never use raw Tailwind palette classes (`text-indigo-600`, `bg-slate-100`).
> Those bypass the theme. Always use the semantic tokens below.

## Tokens → utility classes

Every token works as `bg-*`, `text-*`, `border-*`, `ring-*`, `fill-*`, `stroke-*`, plus opacity
modifiers like `/10`, `/30`, `/90`.

| Token | `bg-` | `text-` | `border-` / `ring-` | Use for |
|---|---|---|---|---|
| **background** | `bg-background` | — | — | page background |
| **foreground** | — | `text-foreground` | — | body text, headings |
| **card** | `bg-card` | — | — | card / surface background |
| **card-foreground** | — | `text-card-foreground` | — | text on cards |
| **popover** / `-foreground` | `bg-popover` | `text-popover-foreground` | — | menus, dropdowns, tooltips |
| **primary** | `bg-primary` | `text-primary` | `border-primary` `ring-primary` | brand: CTAs, links, active state |
| **primary-foreground** | — | `text-primary-foreground` | — | text/icon **on** primary |
| **secondary** / `-foreground` | `bg-secondary` | `text-secondary-foreground` | — | secondary buttons / chips |
| **muted** | `bg-muted` | — | — | subtle fills, skeletons, hover |
| **muted-foreground** | — | `text-muted-foreground` | — | secondary text, captions, placeholders |
| **accent** / `-foreground` | `bg-accent` | `text-accent-foreground` | — | hover backgrounds (ghost btn, menu) |
| **destructive** | `bg-destructive` | `text-destructive` | `border-destructive` `ring-destructive` | errors, delete / cancel |
| **border** | — | — | `border-border` | default borders, dividers |
| **input** | — | — | `border-input` | form field borders |
| **ring** | — | — | `ring-ring` | focus rings |

## Background ↔ text pairings (always pair)

Rule of thumb: **`X` pairs with `X-foreground`.**

```
bg-background  + text-foreground            ← the page
bg-card        + text-card-foreground       ← cards
bg-primary     + text-primary-foreground    ← primary button / banner
bg-secondary   + text-secondary-foreground  ← secondary button
bg-muted       + text-muted-foreground      ← subtle panel
bg-accent      + text-accent-foreground     ← hover state
bg-destructive + text-white                 ← danger
```

Don't put `text-foreground` on `bg-primary` (low contrast) — use `text-primary-foreground`.

## Borders, focus, radius

```
border border-border        // default divider/border (bare `border` also uses border color)
border border-input         // form fields
border border-primary       // brand outline
border-2 border-primary     // thicker
border-b border-border      // bottom only
focus-visible:ring-2 focus-visible:ring-ring         // focus ring
focus-visible:ring-2 focus-visible:ring-destructive  // error focus
rounded-sm | rounded-md | rounded-lg | rounded-xl    // from --radius (0.625rem base)
```

## Soft / tinted surfaces (opacity modifier)

```
bg-primary/10  text-primary     // soft brand chip (value-prop icon boxes)
border-primary/30               // faint brand border
text-primary-foreground/90      // dimmed text on primary (banner subtitle)
hover:bg-primary/90             // primary button hover
hover:bg-muted                  // subtle row hover
```

## Icons & SVG

```
<ShieldCheck className="size-5 text-primary" />   // icons inherit color via text-*
fill-primary  stroke-primary                       // raw svg paths
```

## Prefer the components (no raw classes needed)

```tsx
// Button — variant + size
<Button>…</Button>                            // primary
<Button variant="secondary">…</Button>
<Button variant="outline">…</Button>          // neutral border
<Button variant="outline-primary">…</Button>  // plum border
<Button variant="ghost" size="icon">…</Button>
<Button variant="destructive">…</Button>
<Button variant="link">…</Button>
// sizes: xs sm default lg xl | icon icon-xs icon-sm icon-lg

// Text — variant (size) + tone (color)
<Text variant="h2">…</Text>
<Text variant="body" tone="muted">…</Text>
<Text variant="caption" tone="muted">…</Text>
<Text variant="h3" tone="inverted">…</Text>   // on primary/dark
// variants: display h1 h2 h3 h4 lead body small label caption mono
// tones: default muted primary secondary destructive inverted
```

## Re-skin everything from one place

Edit `frontend/src/index.css` `:root` (e.g. `--primary`, `--muted-foreground`, `--border`).
Every `*-primary`, `text-foreground`, etc. updates automatically — same idea as the Figma
primitive ramps. Dark mode lives in a `.dark` block (when added).

## See also
- `frontend/src/index.css` — token definitions (source of truth)
- `frontend/src/components/ui/button.tsx`, `text.tsx` — the variant components
- `docs/design/2026-06-14-figma-design-system-and-screens-brief.md` — Figma ↔ code design system
- [[Design tokens]] · [[Frontend conventions]]
