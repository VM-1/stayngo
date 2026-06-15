# StayNGo — Figma Design System & Screens Master Brief

**Purpose:** the single source-of-truth prompt that drives the Figma build. Every token,
component, and screen below is created in Figma in this exact order. Consistency rule:
**nothing is hardcoded** — every fill/stroke/spacing/radius binds to a variable, every text
run uses a text style. Screens are composed only from the component library defined here.

This brief is derived from the shipped frontend (`frontend/src/`, shadcn **new-york** / slate /
teal) and the Phase-1 product scope (`StayNGo.md` §5.5, 9 user flows). Light mode only; dark-mode
tokens defined as a second variable mode for a later flip.

---

## 0. Brand & tone

- **Product:** stays / short-term-rentals booking platform.
- **Feel:** calm, trustworthy, travel-warm but not loud. Slate neutrals + a teal/cyan primary
  ("modern booking" without copying Airbnb's coral).
- **Type:** system font stack (no webfont) — matches the shipped shell. Figma proxy: **Inter**
  (closest system-stack proxy available in Figma) for all text.
- **Radius:** one token, `0.5rem` (8px) base, per shadcn `--radius`.

---

## 1. Foundations — variable collections

### 1a. `Primitives` (mode: Value) — raw values, scopes hidden `[]`

Slate ramp + teal ramp + status hues + white/black. OKLCH targets from the shell doc converted to hex.

```
white            #FFFFFF
black            #0A0A0A
slate/50         #F8FAFC   slate/100 #F1F5F9   slate/200 #E2E8F0   slate/300 #CBD5E1
slate/400        #94A3B8   slate/500 #64748B   slate/600 #475569   slate/700 #334155
slate/800        #1E293B   slate/900 #0F172A   slate/950 #020617
teal/50  #F0FDFA  teal/100 #CCFBF1  teal/200 #99F6E4  teal/300 #5EEAD4  teal/400 #2DD4BF
teal/500 #14B8A6  teal/600 #0D9488  teal/700 #0F766E  teal/800 #115E59  teal/900 #134E4A
red/100 #FEE2E2   red/500 #EF4444   red/600 #DC2626   red/700 #B91C1C
amber/100 #FEF3C7 amber/500 #F59E0B amber/600 #D97706
emerald/100 #D1FAE5 emerald/500 #10B981 emerald/600 #059669
blue/100 #DBEAFE  blue/500 #3B82F6  blue/600 #2563EB
```

### 1b. `Color` (modes: Light, Dark) — semantic, aliased to primitives. shadcn slot names.

| Token | Light | Dark | Scopes |
|---|---|---|---|
| background | white | slate/950 | FRAME_FILL, SHAPE_FILL |
| foreground | slate/950 | slate/50 | TEXT_FILL |
| card | white | slate/900 | FRAME_FILL, SHAPE_FILL |
| card-foreground | slate/950 | slate/50 | TEXT_FILL |
| popover | white | slate/900 | FRAME_FILL |
| popover-foreground | slate/950 | slate/50 | TEXT_FILL |
| primary | teal/600 | teal/500 | FRAME_FILL, SHAPE_FILL |
| primary-foreground | white | slate/950 | TEXT_FILL |
| secondary | slate/100 | slate/800 | FRAME_FILL, SHAPE_FILL |
| secondary-foreground | slate/900 | slate/50 | TEXT_FILL |
| muted | slate/100 | slate/800 | FRAME_FILL, SHAPE_FILL |
| muted-foreground | slate/500 | slate/400 | TEXT_FILL |
| accent | slate/100 | slate/800 | FRAME_FILL, SHAPE_FILL |
| accent-foreground | slate/900 | slate/50 | TEXT_FILL |
| destructive | red/600 | red/500 | FRAME_FILL, SHAPE_FILL, TEXT_FILL |
| destructive-foreground | white | slate/50 | TEXT_FILL |
| success | emerald/600 | emerald/500 | FRAME_FILL, SHAPE_FILL, TEXT_FILL |
| warning | amber/500 | amber/500 | FRAME_FILL, SHAPE_FILL, TEXT_FILL |
| border | slate/200 | slate/800 | STROKE_COLOR |
| input | slate/200 | slate/800 | STROKE_COLOR |
| ring | teal/600 | teal/500 | STROKE_COLOR |

### 1c. `Spacing` (mode: Value) — 4px base. Scopes: GAP, WIDTH_HEIGHT, paddings.

`0=0, px=1, 0.5=2, 1=4, 1.5=6, 2=8, 3=12, 4=16, 5=20, 6=24, 8=32, 10=40, 12=48, 16=64, 20=80, 24=96`

### 1d. `Radius` (mode: Value) — Scopes: CORNER_RADIUS.

`none=0, sm=4, md=6, lg=8, xl=12, 2xl=16, full=9999`

---

## 2. Text styles (type ramp) — Inter

| Style | Size/px | Weight | Line-height | Tracking | Use |
|---|---|---|---|---|---|
| Display | 60 | SemiBold | 1.0 | -0.02em | hero h1 (lg) |
| H1 | 36 | SemiBold | 1.1 | -0.02em | hero h1 (base) |
| H2 | 24 | SemiBold | 1.2 | -0.01em | page h1 |
| H3 | 20 | SemiBold | 1.3 | normal | section h2 |
| H4 | 18 | Medium | 1.4 | normal | card titles |
| Body Large | 18 | Regular | 1.6 | normal | hero lead |
| Body | 16 | Regular | 1.6 | normal | default body |
| Body Medium | 16 | Medium | 1.5 | normal | labels, nav |
| Small | 14 | Regular | 1.5 | normal | secondary text |
| Small Medium | 14 | Medium | 1.4 | normal | buttons, badges |
| Caption | 12 | Regular | 1.4 | normal | footnotes |
| Mono | 12 | Regular (mono) | 1.5 | normal | IDs |

## 3. Effect styles

- `Shadow/sm` — y1 blur2 rgba(0,0,0,.05)
- `Shadow/md` — y2 blur8 rgba(0,0,0,.08) + y1 blur2 rgba(0,0,0,.04)
- `Shadow/lg` — y8 blur24 rgba(0,0,0,.10) + y2 blur6 rgba(0,0,0,.05)
- `Focus ring` — 0px spread2 ring color (for focus-visible demos)

---

## 4. Component library (atoms → molecules → organisms)

Built one per page, variants gridded, props wired, tokens bound.

**Atoms:** Button (variant: primary/secondary/outline/ghost/destructive/link × size: sm/md/lg ×
state: default/hover/disabled; optional leading icon via INSTANCE_SWAP), Icon Button, Badge
(variant: default/secondary/outline/success/warning/destructive), Input (default/focus/error/disabled),
Textarea, Label, Avatar (size sm/md/lg, image/initials), Skeleton, Separator, Checkbox, Switch,
Radio, Spinner, Logo (mark + wordmark), Star rating, Price tag, Status pill (booking/listing states).

**Molecules:** Form Field (label + input + helper/error), Search bar segment (location/dates/guests),
Listing Card (image, title, location, price, rating), Value-prop Card (icon + title + blurb),
Profile Card, Empty State, Error State, Alert, Tabs, Dropdown menu, Date-range picker (static),
Stepper/Counter (guests), Pagination, Toast, Dialog/Modal shell, Sheet (mobile nav), Breadcrumb.

**Organisms:** Header (desktop: signed-out / signed-in / booting; mobile bar), Footer,
Auth card frame, Booking summary card, Image gallery, Filters bar.

---

## 5. Screens (Phase-1 product — desktop 1440 + mobile 390 where layout switches)

Each screen built from instances only. States included where relevant (loading/empty/error).

1. **Landing `/`** — hero + inert search teaser + value props + host CTA banner. (signed-out & signed-in hero variant)
2. **Sign in `/sign-in`** — AuthLayout centered card (Clerk form mock).
3. **Sign up `/sign-up`** — AuthLayout variant.
4. **Search results** — filters bar + results grid of Listing Cards + map placeholder + pagination. (+ empty state, + loading skeletons)
5. **Listing detail** — gallery, title/location/rating, description, amenities, host blurb, sticky booking widget (price, date range, guests, total, Reserve).
6. **Booking flow / Reserve** — confirm dates + guests + price breakdown + Idempotency-safe Confirm. (+ 409 conflict error state)
7. **Booking confirmation** — success summary.
8. **Create / edit listing (Draft)** — multi-section form: basics, location+timezone, photos, capacity, price. Draft state banner.
9. **My trips & bookings** — tabbed: "As guest" / "As host"; booking rows with status pills; cancel action (guest, 24h rule note).
10. **Host: incoming bookings** — list of Pending bookings to Confirm/Reject.
11. **Host dashboard / listings** — owner's listings grid with Draft/Active/Archived status, publish/archive actions.
12. **Account `/account`** — profile card (avatar, name, email, user id mono + copy). (+ loading skeleton, + error card)
13. **Host placeholder `/host`** — "coming soon" (kept from shell).
14. **404 `*`** — in-chrome not-found.

Cross-cutting overlays: Cancel-booking dialog, Confirm-booking dialog, Mobile nav Sheet, Toast.

---

## 6. Figma file structure (pages)

```
Cover
Getting Started
Foundations  (colors, type, spacing, radius, elevation specimens)
———
Components   (one Figma page per component family)
———
Screens — Desktop
Screens — Mobile
Flows (FigJam-style arrows optional — skip if design file)
```

## 7. Non-negotiable consistency rules
- 8px spacing grid; container max-w 1280 (7xl), reading/forms max-w 672 (2xl).
- Primary actions = `primary` solid; secondary = `outline`; tertiary = `ghost`.
- Every interactive element shows a focus-ring demo on at least one variant.
- State conveyed by icon+text, never color alone.
- Card = `card` fill + `border` stroke + `radius/xl` + `Shadow/sm`.
- One `<h1>` equivalent (H2 style) per screen; headings don't skip levels.

---

## 8. Build result (v1 — 2026-06-14)

**Figma file:** https://www.figma.com/design/LwjzqLdVmpa0VbIZrjcMnH (StayNGo — Design System & Screens)

**Foundations:** 4 variable collections — `Primitives` (36 colors), `Color` (21 semantic
tokens, Light/Dark modes, aliased), `Spacing` (16), `Radius` (7). 12 text styles (Inter +
JetBrains Mono). 3 elevation effect styles. All variables scoped; semantics alias primitives.

**Components (token-bound, variant sets):** Button (5×3 + Label prop), Badge (6 + Label),
Input (4 states), Avatar (2×3), Logo, 19 lucide icon components, Listing Card (4 text props),
Header (signed-out / signed-in), Footer.

**Screens — Desktop (14):** Landing, Sign in, Sign up, Search results, Listing detail,
Reserve (with 409-conflict alert), Booking confirmed, Account, Create/edit listing (Draft),
My Trips (guest/host tabs), Host dashboard (listings), Host booking requests, Host placeholder, 404.

**Screens — Mobile (3):** Landing, Search results, Nav Sheet overlay.

**Cover** page + **Foundations** documentation page (swatches, type/spacing/radius/elevation specimens).

**File pages:** Cover · Foundations · Components · Screens — Desktop · Screens — Mobile.

Every screen is composed from component instances bound to design tokens — design-to-code stays
faithful to the shipped shadcn/slate/teal baseline. Build-resume state: `.figma-build-state.json`.
</content>
</invoke>
