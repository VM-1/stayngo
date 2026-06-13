I'll fold the critic's points into the draft and produce the final doc. The two critical fixes (C1: `<Show>` returns null while loading, so `useAuth().isLoaded` / `<ClerkLoading>/<ClerkLoaded>` must drive boot state; C2: resolve modal-vs-redirect with explicit props) and the material gaps (G1–G5) all need to be woven in throughout.

# StayNGo — Phase-1 Frontend Shell: Design Specification

**Epic #6 · Phase-1 · SHELL ONLY** · by-hand blueprint (design + spec, *not* full implementation)
**Scope:** the app chrome + four route surfaces — global layout, landing (`/`), auth (`/sign-in/*`, `/sign-up/*`), and account (`/account`) — plus the cross-cutting conventions that bind them. No feature pages, no real search, no booking domain.

---

## 0. Cross-section decisions (resolved inconsistencies)

The four source sections disagreed on several points. Resolved once, here, and applied throughout:

1. **Clerk conditional-render API — use `<Show when="signed-in" | "signed-out">` for the *post-load* swap, and `useAuth().isLoaded` / `<ClerkLoading>/<ClerkLoaded>` for the *boot* state.**
   The brief named `SignedIn`/`SignedOut`, but the installed package is `@clerk/react` v6 and the live `src/App.tsx` already standardizes on `<Show when="...">` (confirmed: `import { Show, SignInButton, SignUpButton, UserButton } from '@clerk/react'`). `SignedIn`/`SignedOut` **do not exist** in `@clerk/react` v6 — do not use them.
   **Critical semantic (verified in `@clerk/react` d.ts):** `<Show>` *returns `null` while auth is loading* — it has **no** `!isLoaded` branch you can render into. Therefore `<Show>` alone **cannot** "kill the flash" or render a boot skeleton. The boot/loading state must come from `useAuth().isLoaded` (in `RequireAuth` and the Account query gate) or from `<ClerkLoading>/<ClerkLoaded>` (for reserving header-cluster width). Division of labor:
   - **Boot / `!isLoaded`** → `<ClerkLoading>` (header skeleton pill) and `<ClerkLoaded>` (real cluster), or `useAuth().isLoaded` in component logic.
   - **Post-load signed-in/out swap** → `<Show when="signed-in">` / `<Show when="signed-out">`.
   Do not mix `<Show>` into the loading-state logic (Principle 10). `<SignedIn>/<SignedOut>` are **not** a migration option here — they don't exist in v6.

2. **Remove the duplicate Clerk package.** `package.json` ships **both** `@clerk/clerk-react@^5.x` (the v5 idiom, has `SignedIn/SignedOut`) and `@clerk/react@^6.x` (the v6 idiom, has `Show`). A stray auto-import will pull `SignedIn` from the v5 package, compile, and **break at runtime** against the v6 provider. Mitigation is a build step, not a guideline: **uninstall `@clerk/clerk-react`** (build-order #17) and add an ESLint `no-restricted-imports` rule banning `@clerk/clerk-react`. The "one idiom" rule (§7) is necessary but insufficient without this.

3. **Layout wraps routes via `<Outlet/>` layout route, not `children` prop.**
   The stub `MainLayout.tsx` **does** import `@/components/Header` and `@/components/Footer` (default exports — verified, lines 1–2). The real defect is that it takes and renders a `children` prop instead of an `<Outlet/>`. Refactor to an `<Outlet/>`-based layout route — the canonical React Router v7 mechanism for "chrome wraps every route." This doc calls the composed shell **`RootLayout`** (= `Header` / `<main flex-1><Outlet/></main>` / `Footer`); keep the existing `MainLayout.tsx` filename or rename — just make it `Outlet`-based. (No "missing imports" to add; do not go hunting for absent imports.)

4. **Header entry buttons REDIRECT, not modal (resolves former open decision).** `<SignInButton>`/`<SignUpButton>` default to modal behavior. The whole design is built around in-app path-routed `/sign-in` / `/sign-up` (mandatory for Clerk's multi-step sub-paths), so the header buttons must be explicit: **`<SignInButton mode="redirect">` / `<SignUpButton mode="redirect">`**, which honor the `signInUrl`/`signUpUrl` set on `<ClerkProvider>`. One primary path, consistently. This is now **decided**, not open (was §12 #2).

5. **Auth routes render inside the app shell? — No. They use their own minimal `AuthLayout`.**
   The Layout section implied auth pages sit inside `MainLayout`; the Auth and Cross-Cutting sections specified a dedicated centered `AuthLayout` with **no** app header/footer. Resolved in favor of the dedicated `AuthLayout` (centered card, wordmark-links-home only) — the stronger, more deliberate spec; avoids a half-chrome auth screen. Auth routes sit **as siblings of `RootLayout`**, not children.

**Repo facts (verified):** `components.json` → `new-york` / `slate` / `cssVariables: true` / `lucide`, `@` alias wired. `src/components/ui/` does **not** exist yet (every shadcn primitive is a fresh `npx shadcn@latest add`). `index.css` already pins `#root` to `flex flex-col / min-h-100dvh / isolation: isolate` and enables Preflight via `@import "tailwindcss"`. No `@theme` tokens defined yet. `cn()` helper present in `src/lib/utils.ts`. `ClerkProvider` is mounted in `main.tsx`; `QueryClientProvider` and a router are **not** yet added. `Header.tsx` and `Footer.tsx` **both exist as trivial `<div>` stubs** — replace, don't create. `@tanstack/react-query`, `react-router-dom@7`, `@radix-ui/react-slot` are present (so `Button asChild` + router need no new deps). **Verify `lucide-react` version resolves** before relying on it (`package.json` shows a suspiciously old `^1.8.0`; current lucide-react is `~0.5xx` — confirm the import path `lucide-react` actually exports `Menu`/`X`/`AlertTriangle`/`Copy` during #17, swap package if it's the wrong/typo'd dependency).

---

## 1. Information architecture & route map

```
ClerkProvider                         (main.tsx — already mounted; add signInUrl/signUpUrl)
  └─ QueryClientProvider              (add in #17, inside ClerkProvider)
       └─ BrowserRouter
            └─ <Routes>
                ├─ element=<RootLayout/>            Header / main flex-1 <Outlet/> / Footer
                │    ├─ "/"            PUBLIC        → LandingPage
                │    ├─ "/account"     PROTECTED     → <RequireAuth><AccountPage/></RequireAuth>
                │    ├─ "/host/*"      PROTECTED     → <RequireAuth><HostPlaceholder/></RequireAuth>
                │    └─ "*"            PUBLIC        → NotFound   (inside RootLayout chrome)
                │
                ├─ "/sign-in/*"  PUBLIC (Clerk auto-redirects if already signed-in)  → AuthLayout + <SignIn/>
                └─ "/sign-up/*"  PUBLIC (Clerk auto-redirects if already signed-in)  → AuthLayout + <SignUp/>
```

**Route ordering:** the auth splats (`/sign-in/*`, `/sign-up/*`) are **siblings of `RootLayout`** and must be declared **before** any catch-all so they aren't swallowed. The global `*` → `NotFound` lives **inside** `RootLayout` (so a 404 still shows app chrome) and is the **last** child of the layout route.

| Path | Access | Layout | Renders | Notes |
|---|---|---|---|---|
| `/` | Public | `RootLayout` | `LandingPage` | Identical signed-in/out except hero CTA + optional "welcome back". No guard, no fetch. |
| `/account` | Signed-in only | `RootLayout` | `AccountPage` via `RequireAuth` | Only data-fetching page in the shell (`GET /identity/me`). |
| `/host/*` | Signed-in only (**no role check**) | `RootLayout` | `HostPlaceholder` via `RequireAuth` | Splat keeps future nested host routes additive. |
| `/sign-in/*` | Public | `AuthLayout` | Clerk `<SignIn/>` | Splat is **mandatory** for Clerk multi-step sub-paths (verification, SSO callback, MFA). |
| `/sign-up/*` | Public | `AuthLayout` | Clerk `<SignUp/>` | Same splat requirement. |
| `*` | Public | `RootLayout` | `NotFound` | Inside chrome; **last** child of the layout route; auth splats precede it as siblings. |

**`RequireAuth`** — wraps the protected element. Drives state from **`useAuth()`** → `isLoaded`, `isSignedIn` (NOT `<Show>`, which renders null while loading):
- `!isLoaded` → render skeleton/neutral placeholder, **never redirect** (avoids bouncing signed-in users on refresh).
- signed-out → redirect to sign-in **carrying the intended path as a `redirect_url` query param** (see §7 / G4 — *not* React Router `state`):
  `<Navigate to={`/sign-in?redirect_url=${encodeURIComponent(location.pathname + location.search)}`} replace />`. `replace` prevents a Back-loop.
- signed-in → render children.

> **Flag (Principle 10):** `/host/*` is signed-in-only with **no role gate** in Phase 1. Do not add a claims/role predicate now — premature abstraction. When host onboarding lands, both this link and the `/host` guard need a role predicate; leave a `// TODO(#<host-roles-ticket>)` so it isn't silent "temporary" code.

> **Reference (Principle 11):** React Router v7 *Layout Routes* / `<Outlet>` — reactrouter.com, "Routing → Layout Routes" (why: the canonical mechanism for chrome-wraps-all-routes, vs prop-drilling a page into each route).

---

## 2. Shared layout wireframe (`RootLayout`)

### Desktop (≥ md, ~768px+) — sticky header

```
┌───────────────────────────────────────────────────────────────────────────┐
│ HEADER  (sticky top-0, h-16, border-b, backdrop-blur)                       │
│ ┌─────────────┐                          ┌──────────────────────────────┐  │
│ │ ◆ StayNGo   │   Stay   Host            │ [signed-out] Sign in  Sign up │  │
│ │  (Link "/") │   (nav, ml-8 gap-6)      │ [signed-in]  Account (Host)   │  │
│ └─────────────┘   (Host gated signed-in) │              ( ◐ UserButton ) │  │
│   brand left              nav left        │ [booting]    skeleton pill    │  │
│                                           └──────────────────────────────┘  │
│                                                          auth cluster right  │
├───────────────────────────────────────────────────────────────────────────┤
│ MAIN  <main id="main" class="flex-1">   ← flex-1 fills, pushes footer down  │
│        ┌───────────────────────────────────────────────────────┐          │
│        │  <Outlet/> — route content (landing / account / host)  │          │
│        │  Container: mx-auto max-w-7xl px-4 sm:px-6 lg:px-8      │          │
│        │  (Account/Host narrow to max-w-2xl)                     │          │
│        └───────────────────────────────────────────────────────┘          │
├───────────────────────────────────────────────────────────────────────────┤
│ FOOTER (border-t, py-8, muted)                                              │
│  ◆ StayNGo · © 2026      About · Help · Terms · Privacy        (text-sm)    │
└───────────────────────────────────────────────────────────────────────────┘
```

The **booting** state (`<ClerkLoading>`) renders a fixed-width skeleton pill in the right cluster so the header does not reflow when auth resolves. `<ClerkLoaded>` then renders the `<Show>`-gated signed-in/out cluster.

### Mobile (< md) — header collapses to a Sheet

```
SIGNED-OUT                              SIGNED-IN
┌─────────────────────────────┐        ┌─────────────────────────────┐
│ ◆ StayNGo            [ ☰ ]   │        │ ◆ StayNGo      ( ◐ )  [ ☰ ] │
└─────────────────────────────┘        └─────────────────────────────┘
  tap ☰ → Sheet (slides in):             tap ☰ → Sheet:
  ┌─────────────────────────┐            ┌─────────────────────────┐
  │ ✕                       │            │ ✕                       │
  │  Stay                   │  ← nav     │  Stay                   │
  │  ───────────────        │  always    │  Host        (signed-in)│
  │  Sign in                │            │  ───────────────        │
  │  Sign up   (primary)    │  ← signed- │  Account                │
  │                         │    out only│  (UserButton stays in   │
  └─────────────────────────┘            │   the BAR, not sheet)   │
                                         └─────────────────────────┘
```

**Mobile decision (resolved):** `<UserButton/>` stays **in the top bar** (always one tap away — it's the sign-out path), the **hamburger Sheet holds nav links + (signed-out) Sign in / Sign up**. This avoids nesting Clerk's own popover inside our Sheet popover and avoids burying sign-out two taps deep.

**Crowding check (M2):** at `< sm` the signed-in bar holds three interactive zones (brand link + `UserButton` + hamburger) plus the wordmark. Verify at 320px: the brand may need `truncate`/shrink below `sm` (e.g. mark-only or `text-base`) so the two ~44px right-side targets keep their hit areas. Not a blocker; bake a `truncate max-w-[8rem] sm:max-w-none` guard into the `Logo`.

**`<main flex-1>`** against the existing `#root` flex column is what sticks the footer to the bottom on short pages — no `mt-auto` needed.

---

## 3. Per-page wireframes

### 3a. Landing `/`

**Desktop (≥ md):**
```
<main flex-1>
  ┌─ HERO (text-center, py-16 lg:py-28, max-w-2xl text col) ────────────┐
  │   Find your next stay.                                  (h1)         │
  │   Book unique homes and rooms — simple, fast, no surprises. (lead)   │
  │   ┌─ SEARCH TEASER (inert, all controls disabled) ──────────────┐   │
  │   │ [ 📍 Where to? ] [ Dates ] [ Guests ] [  Search →  ]         │   │
  │   │   caption: "Search coming soon"  (aria-describedby)          │   │
  │   └──────────────────────────────────────────────────────────────┘   │
  │   [ Get started → ]   [ Browse stays ]   (primary CTA / ghost)       │
  └──────────────────────────────────────────────────────────────────────┘
  ┌─ VALUE PROPS (grid-cols-1 md:grid-cols-3, <ul>/<li>) ───────────────┐
  │  ◻ Verified homes     ◻ Instant booking     ◻ Flexible cancels      │
  │    short blurb           short blurb           short blurb           │
  └──────────────────────────────────────────────────────────────────────┘
  ┌─ HOST CTA BANNER (bg primary/dark, rounded-2xl) ────────────────────┐
  │  Have a place to share?            [ Become a host → ]  (→ /host)    │
  └──────────────────────────────────────────────────────────────────────┘
```

**Mobile (< md):** single column — hero text, then search-teaser fields stacked full-width (disabled), full-width primary then secondary CTA, value-prop cards 1-up, host banner stacks text over a full-width button.

**Secondary CTA target (resolves M1):** "Browse stays" is a **route `<Link>`** (to `/` for signed-in "Go to account" variant, or a future listings route placeholder) — **not** an in-page `#anchor`. There are no in-page anchors on landing in Phase 1. `scroll-padding-top` (§5) is kept as a harmless global default for future anchored content; it has no landing target today.

### 3b. Auth `/sign-in/*` (and `/sign-up/*`, structurally identical)

**Desktop & Mobile share the centered `AuthLayout` (NO app header/footer):**
```
┌───────────────────────────────────────────────┐
│  AuthLayout: min-h-dvh grid place-items-center  │
│              p-4, subtle bg (bg-muted/30)       │
│                ◆ StayNGo  → "/"   (wordmark)    │
│            ┌───────────────────────┐           │
│            │  Welcome back   (h1)  │  ← OUR     │
│            │  Sign in to manage    │    heading │
│            │  your trips.          │    block   │
│            │ ┌───────────────────┐ │           │
│            │ │  CLERK <SignIn/>  │ │  ← Clerk   │
│            │ │  email / Continue │ │    owns    │
│            │ │  — or —           │ │    form,   │
│            │ │  [Google][GitHub] │ │    OAuth,  │
│            │ │  No account?      │ │    OTP,    │
│            │ │  Sign up ──►      │ │    MFA     │
│            │ └───────────────────┘ │           │
│            └───────────────────────┘           │
│                 (w-full max-w-sm)              │
└───────────────────────────────────────────────┘
```
`/sign-up` differs only in copy ("Create your account" / "Start booking in minutes"), the Clerk element (`<SignUp/>`), and cross-link direction ("Already have an account? Sign in"). One shared `AuthLayout`.

**Heading / one-`<h1>` guarantee (resolves G5):** the design *prefers* exactly one `<h1>` per page, with our heading block as that `<h1>`. To suppress Clerk's internal title, set on the Clerk element:
```
appearance={{ elements: { headerTitle: 'hidden', headerSubtitle: 'hidden' } }}
```
**These element keys are version-sensitive — verify against installed `@clerk/react` v6 during #18** (Clerk renames `elements` keys across majors; inspect the rendered DOM/classNames if `headerTitle` doesn't take). **Fallback rule if suppression can't be verified:** relax the guarantee to "*our* custom heading block is the page `<h1>`; Clerk's internal title, if present, is `<h2>`-or-lower or visually-hidden." Do **not** block the a11y DoD on an unverified `appearance` key — the relaxed rule still passes a heading-order check.

### 3c. Account `/account` (signed-in only)

**Desktop — SUCCESS:**
```
<main flex-1>  (Container max-w-2xl)
   Account                                   (h1)
   Your StayNGo profile.                     (muted subtitle)
   ┌─ Card ─────────────────────────────────────────────────┐
   │ ( ◐ avatar 64px )  Display Name           [ Admin ]?    │  ← name = h2, badge optional
   │                    email@host.io                        │
   │                    ● Signed in                          │  ← dot decorative + TEXT label
   │  ────────────────────────────────────────────────────  │
   │   User ID   usr_2abc…XYZ  [copy]   (font-mono, <dl>)    │
   │   Email     email@host.io                               │
   └─────────────────────────────────────────────────────────┘
```

**Mobile:** avatar stacks above text; badge wraps under name; `<dl>` rows stack label-over-value.

**LOADING:** same Card shell, `<Skeleton>` bars mirroring final layout; page title renders immediately. Container `role="status" aria-busy="true"` + sr-only "Loading your profile…".

**ERROR:** Card with lucide `AlertTriangle`, "We couldn't load your profile", muted body, `[ Try again ]` → `refetch()`. Container `role="alert"`. Page title still renders (layout never collapses to blank).

### 3d. NotFound `*` (resolves G2)

Renders **inside `RootLayout` chrome** (header + footer present), centered in the container:
```
<main flex-1>  (Container, py-24 text-center)
   404                                        (small muted eyebrow, text-sm)
   This page doesn't exist.                   (h1, text-2xl font-semibold)
   The link may be broken or the page moved.  (muted body)
   [ Go home ]   → <Link to="/">              (primary Button asChild)
```
**a11y/copy spec:** one `<h1>` ("This page doesn't exist."); the "404" eyebrow is decorative `text-sm text-muted-foreground` (not the `<h1>`); a single descriptive `<Link to="/">` styled as primary `Button asChild`. No fetch, no state. Matrix agrees: `*` is Public, `RootLayout`, renders normally in all auth states.

### 3e. HostPlaceholder `/host/*` (resolves G2)

Static, guarded (signed-in only, no role check). Centered in `max-w-2xl`:
```
<main flex-1>  (Container max-w-2xl, py-24 text-center)
   Host dashboard                              (h1)
   Coming soon — list and manage your places.  (muted body)
   [ Back to home ]   → <Link to="/">          (Button asChild, outline)
```
**Copy/a11y:** one `<h1>` ("Host dashboard"); muted "coming soon" body; one descriptive back `<Link>`. No loading/empty/error states (purely static).

---

## 4. Auth-state matrix (what each surface renders)

| State | Mechanism | Header brand | Header nav (desktop) | Header right cluster | Mobile | Landing hero CTA | `/account` | `/sign-in`,`/sign-up` |
|---|---|---|---|---|---|---|---|---|
| **`!isLoaded`** (Clerk booting) | `<ClerkLoading>` / `useAuth().isLoaded` (**not `<Show>`**) | shown | none | **fixed-width skeleton pill** (reserves width, no flash) | hamburger shown, items deferred | static hero renders; CTA label deferred | `RequireAuth` (via `useAuth().isLoaded`) renders skeleton, **no redirect** | render normally |
| **Signed-out** | `<Show when="signed-out">` (after load) | shown | `Stay` only | `Sign in` (ghost) + `Sign up` (primary), both `mode="redirect"` | links + Sign in/up in Sheet | "Get started" → `SignUpButton mode="redirect"` | redirect → `/sign-in?redirect_url=…` | show the form |
| **Signed-in** | `<Show when="signed-in">` (after load) | shown | `Stay`, `Host` | `Account` link, `(Host)`, `<UserButton afterSignOutUrl="/">` | `UserButton` in bar; `Stay`/`Host`/`Account` in Sheet | "Browse stays" / "Go to account" → `<Link>` | render profile | Clerk auto-redirects away (no manual guard needed) |

**Two distinct mechanisms — do not conflate (C1):**
- **Boot vs loaded:** `<ClerkLoading>/<ClerkLoaded>` (for the header cluster) or `useAuth().isLoaded` (in `RequireAuth` and the Account query gate). `<Show>` renders `null` while loading and **cannot** express the boot state.
- **Signed-in vs signed-out (post-load):** `<Show when="signed-in">…</Show>` / `<Show when="signed-out">…</Show>`.

**Avoiding the auth flash:** wrap the header right-cluster in `<ClerkLoading>`(skeleton pill, fixed min-width) + `<ClerkLoaded>`(the `<Show>` swap), so the header reserves the cluster's width and doesn't jump signed-out → signed-in on first paint.

---

## 5. Responsive strategy

Mobile-first; Tailwind default scale. Only **`md` (768px)** is a real layout switch for the shell.

| Token | Min width | Used for |
|---|---|---|
| base | 0 | single column, stacked CTAs, hamburger + Sheet nav, `px-4` gutters |
| `sm` | 640px | CTAs go `w-auto` inline; gutters `px-6`; brand un-truncates |
| `md` | **768px** | **primary nav switch** — desktop nav links + inline auth buttons appear, hamburger hidden; header `h-14 → h-16` |
| `lg` | 1024px | gutters `px-8`; hero spacing grows |
| `xl`/`2xl` | 1280px+ | content capped at `max-w-7xl`; no new layout |

Rules baked into the spec:
- **One `Container`** component: `max-w-7xl` default, `max-w-2xl` for reading/forms (Account, Host placeholder) via prop. Gutters `mx-auto px-4 sm:px-6 lg:px-8`.
- The signed-in/out **right cluster** (auth state) and the **breakpoint** switch (inline nav vs Sheet) are **orthogonal** — design independently.
- Use `dvh` (already in `index.css`) for the full-height `AuthLayout` to dodge mobile URL-bar jank.
- **Sticky-header anchor offset:** add `scroll-padding-top` on `html` (or `scroll-mt-16` on headings) in `index.css` so future in-page anchor jumps clear the `h-16` sticky bar. (Harmless now — landing has no anchors; see M1.)
- **Backdrop-blur fallback:** guard with `supports-[backdrop-filter]` so non-supporting browsers get an opaque bar, not transparent-over-scroll.
- **Mobile crowding:** brand `truncate max-w-[8rem] sm:max-w-none` so signed-in bar (brand + UserButton + hamburger) holds at 320px (M2).

---

## 6. Region-by-region component spec

Legend — **C** = Clerk, **S** = shadcn/ui, **X** = custom, **HTML** = semantic element only.

### 6a. Header / global chrome

| Region | Content | Component | Data / state | Responsive | a11y | Tailwind hints |
|---|---|---|---|---|---|---|
| `#root` (exists) | wraps everything | HTML | none | n/a | first child = skip-link `<a href="#main">` | already `flex flex-col min-h-[100dvh] isolation-isolate` |
| Header shell | sticky bar | HTML `<header>` | none | `h-14 md:h-16`, `sticky top-0 z-40` | landmark; one `<nav aria-label="Primary">` inside | `sticky top-0 z-40 w-full border-b bg-background/80 backdrop-blur supports-[backdrop-filter]:bg-background/60` |
| Header inner | 3-zone row | HTML `<div>` | none | flex, space-between | — | `mx-auto flex h-full max-w-7xl items-center justify-between gap-4 px-4 sm:px-6 lg:px-8` |
| Brand / logo | ◆ + "StayNGo" → `/` | **X** `Logo` over RR `<Link>` | none | always visible; `truncate max-w-[8rem] sm:max-w-none` | `aria-label="StayNGo home"` if mark-only | `flex items-center gap-2 font-semibold tracking-tight text-lg shrink-0` |
| Desktop nav | `Stay` (`/`), `Host` (`/host`) | **X** `NavLink` (RR) | route `isActive`; **Host gated** `<Show when="signed-in">` | `hidden md:flex` | `aria-current="page"` via NavLink; use `end` on `/` (and scope `/host`) | `hidden md:flex items-center gap-6 text-sm font-medium`; idle `text-muted-foreground hover:text-foreground` |
| Cluster boot wrap | reserve width while loading | **C** `<ClerkLoading>` → skeleton pill | Clerk loading | `hidden md:flex` (+ mobile variant) | `aria-hidden` skeleton | fixed `min-w-[8rem] h-9` skeleton |
| Auth cluster (signed-out) | Sign in (ghost) + Sign up (primary) | **C** `<ClerkLoaded>` → `SignInButton mode="redirect"`/`SignUpButton mode="redirect"` wrapping **S** `Button`, inside `<Show when="signed-out">` | Clerk via `<Show>` (post-load) | `hidden md:flex` | real `<button>`; focus rings | `hidden md:flex items-center gap-3` |
| Auth cluster (signed-in) | Account link, (Host), `UserButton` | **C** `<ClerkLoaded>` → `<Show when="signed-in">` → `UserButton afterSignOutUrl="/"` + RR `<Link>` | `<Show>` (post-load) | `hidden md:flex` | Clerk a11y; ≥44px hit area | `gap-3` |
| Mobile UserButton | avatar when signed-in | **C** `<ClerkLoaded><Show when="signed-in">UserButton` | Clerk | `flex md:hidden` | Clerk-labeled | `flex md:hidden items-center` |
| Hamburger | open menu | **S** `Button` (icon, ghost) + **S** `Sheet` trigger | local `open` state | `inline-flex md:hidden` | `aria-label="Open menu"`, `aria-expanded`, `aria-controls="mobile-menu"`; icon `aria-hidden` | `size-9 md:hidden`; lucide `Menu`/`X` |
| Mobile Sheet panel | nav + (signed-out) Sign in/up | **S** `Sheet` (Radix Dialog) | `open`; close on route change | `md:hidden` | focus trap + restore (Radix); `id="mobile-menu"`; links `min-h-11` | `flex flex-col gap-1 p-4`; links `flex min-h-11 items-center rounded-md px-3` |

### 6b. Main / page outlet (shared)

| Region | Content | Component | Data / state | Responsive | a11y | Tailwind hints |
|---|---|---|---|---|---|---|
| Main landmark | route outlet | HTML `<main>` + RR `<Outlet/>` | route match | `flex-1` | `<main id="main" tabIndex={-1}>` skip-link target; one `<h1>` per route (page's job) | `flex-1 w-full`; inner `Container` |
| Container | width + gutters | **X** `Container` | none | `max-w-7xl` (or `max-w-2xl` via prop) | none | `mx-auto w-full px-4 sm:px-6 lg:px-8 py-6 md:py-10` |
| Skip link | "Skip to content" | **X** | none | visible on focus | first focusable; `href="#main"` | `sr-only focus:not-sr-only focus:absolute focus:z-50 focus:p-2` |

### 6c. Landing `/`

| Region | Content | Component | Data / state | Responsive | a11y | Tailwind hints |
|---|---|---|---|---|---|---|
| Hero | h1 + lead + CTAs + search teaser | **X** `Hero` | static; CTA gated by `<Show>` (post-load) | centered, `max-w-2xl` text col | one `<h1>`; lead is `<p>` | `flex flex-col items-center text-center gap-6 py-16 lg:py-28`; h1 `text-4xl sm:text-5xl lg:text-6xl font-semibold tracking-tight` |
| Primary CTA | "Get started"(out) / "Browse stays"(in) | signed-out: **C** `SignUpButton mode="redirect"`; signed-in: **S** `Button asChild` → RR `<Link>` | Clerk `<Show>` | stack `w-full` <sm, `w-auto` sm+ | real `<a>`/`<button>`; descriptive label | primary solid `bg-primary text-primary-foreground` |
| Secondary CTA | "Browse stays" → route `<Link>` (not anchor) | **S** `Button variant="outline" asChild` → `<Link>` | static | same stack→inline; primary first on mobile | focus/contrast | `border bg-transparent hover:bg-muted` |
| Search teaser | inert faux search + caption | **X** `SearchTeaser` | **no state, no submit, no Query** | desktop horizontal pill, mobile stacked | each field a `<label>` (visually-hidden OK); native `disabled` (announced, not removed); group `aria-describedby` caption | `rounded-2xl border bg-card shadow-sm p-2 flex flex-col md:flex-row md:divide-x`; button `opacity-60 cursor-not-allowed` |
| Value props | 3 cards icon+title+blurb | **S** `Card` ×3 | static array | `grid grid-cols-1 md:grid-cols-3 gap-6` | `<ul>/<li>`; titles `<h3>`; icons `aria-hidden` | `rounded-xl border p-6 flex flex-col gap-2` |
| Host CTA banner | prompt + "Become a host" → `/host` | **X** `HostBanner` + **S** `Button asChild` → `<Link to="/host">` | static, shown to all | desktop row, mobile stacked | `<section aria-labelledby>`; h2 prompt | `rounded-2xl bg-primary text-primary-foreground p-8 flex flex-col gap-4 md:flex-row md:items-center md:justify-between` |

### 6d. Auth pages `/sign-in/*`, `/sign-up/*`

| Region | Content | Component | Data / state | Responsive | a11y | Tailwind hints |
|---|---|---|---|---|---|---|
| AuthLayout | centered frame, no app chrome | **X** `AuthLayout` | none | `min-h-dvh grid place-items-center p-4` | wordmark links home | `bg-muted/30` |
| Heading block | "Welcome back"/"Create your account" + subcopy | **X** (`<h1 id="auth-heading">` + `<p>`) | static copy | `text-2xl md:text-3xl` | the page's `<h1>`; suppress Clerk's title via `appearance` (verify keys — G5) | `text-2xl font-semibold tracking-tight` + `text-sm text-muted-foreground` |
| Clerk card | full form: email/pw, OAuth, OTP, MFA, reset, errors | **C** `<SignIn/>` / `<SignUp/>` | Clerk owns all async/error/redirect | fluid, constrained by `max-w-sm` parent | Clerk-managed internals; `appearance={{ elements: { headerTitle:'hidden', headerSubtitle:'hidden' } }}` (verify v6 keys) | wrap, don't restyle in Phase 1; full `appearance` theming later (ADR) |
| In-card cross-link | "No account? Sign up" etc. | **C** internal link via `signUpUrl`/`signInUrl` | static prop | inside card | Clerk-rendered | `signUpUrl="/sign-up"` on SignIn; `signInUrl="/sign-in"` on SignUp |

### 6e. Account `/account`

| Region | Content | Component | Data / state | Responsive | a11y | Tailwind hints |
|---|---|---|---|---|---|---|
| Route guard | gate before render | **X** `RequireAuth` (uses **`useAuth()`**, not `<Show>`) + RR `<Navigate>` | `isLoaded`,`isSignedIn`; `!isLoaded`→skeleton; out→redirect with `?redirect_url=` | n/a | `replace` redirect | — |
| Page header | h1 "Account" + subtitle | HTML | static — renders in **all** states | single col | one `<h1>`; subtitle `<p>` | `text-2xl font-semibold tracking-tight`; subtitle `text-sm text-muted-foreground mt-1` |
| Profile card | avatar + name + email + id | **S** `Card` | wraps success/loading/error | `max-w-2xl mx-auto`, `p-5 md:p-6` | `<section aria-labelledby>` → name | `rounded-xl border bg-card shadow-sm` |
| Avatar | image or initials | **S** `Avatar`/`AvatarImage`/`AvatarFallback` | image from **Clerk `user.imageUrl`**; initials from `/me` `displayName` | `size-14 md:size-16`; `flex-col md:flex-row` | `alt={displayName}`; fallback `aria-hidden` | `shrink-0 rounded-full`; fallback `bg-muted` |
| Display name | `displayName` | **X** text (`<h2>`) | **`/identity/me`** → `displayName` (success only) | `text-lg md:text-xl` | `aria-labelledby` target | `text-lg font-semibold leading-none` |
| Admin badge (optional) | "Admin" pill | **S** `Badge` | **Clerk `user.publicMetadata.role`** (NOT `/me` — see §7/§12) | wraps under name (mobile), inline (desktop) | text label suffices | `ml-2 inline-flex` |
| Email | `email` | **X** text | **`/identity/me`** → `email` | `text-sm` | plain text (not mailto in shell) | `text-sm text-muted-foreground` |
| Signed-in confirm | dot + "Signed in" | **X** (`<span aria-hidden>` dot + label) | implied by success | inline | state via **text**, not color alone | dot `h-2 w-2 rounded-full bg-emerald-500`; label `text-xs text-muted-foreground` |
| Detail rows | User ID, Email | **X** `<dl>`/`<dt>`/`<dd>` | **`/identity/me`** → `id`, `email` | `grid grid-cols-[6rem_1fr]` desktop → 1-col mobile | real `<dl>` | `dt text-muted-foreground`; `dd font-mono text-xs break-all` |
| Copy ID (optional) | copy `id` | **S** `Button` (icon/ghost) + lucide `Copy` | reads `/me` `id`; transient "Copied" | icon-only | `aria-label="Copy user ID"`; `aria-live="polite"` | `h-7 w-7 text-muted-foreground` |
| Loading | skeleton mirroring layout | **S** `Skeleton` | Query `isPending` | matches loaded shape | `role="status" aria-busy` + sr-only text | `animate-pulse rounded-md bg-muted` |
| Error | ⚠ + message + Try again | **X** panel + **S** `Button` + lucide `AlertTriangle` | Query `isError`; button → `refetch()` (disabled while `isFetching`) | centered | `role="alert"`; no raw error strings | `flex flex-col items-center text-center gap-3 py-8`; icon `text-destructive` |

### 6f. Host `/host/*`

| Region | Content | Component | Data / state | Responsive | a11y | Tailwind hints |
|---|---|---|---|---|---|---|
| Placeholder | "Host dashboard" h1 + "coming soon" body + back link → `/` | **X** `HostPlaceholder` + **S** `Button asChild` → `<Link to="/">` | none (static) | centered `max-w-2xl` | one `<h1>`; descriptive link | `py-24 text-center text-muted-foreground` |

### 6g. NotFound `*`

| Region | Content | Component | Data / state | Responsive | a11y | Tailwind hints |
|---|---|---|---|---|---|---|
| 404 page | "404" eyebrow + h1 "This page doesn't exist." + body + Go home | **X** `NotFound` + **S** `Button asChild` → `<Link to="/">` | none (static) | centered, in `RootLayout` chrome | one `<h1>` (not the eyebrow); descriptive link | `py-24 text-center`; eyebrow `text-sm text-muted-foreground` |

### 6h. Footer

| Region | Content | Component | Data / state | Responsive | a11y | Tailwind hints |
|---|---|---|---|---|---|---|
| Footer | brand + copyright + links | HTML `<footer>` + RR `<Link>` | year via `new Date().getFullYear()` | row `md`, stacked/centered <md | landmark; `<nav aria-label="Footer">` | `border-t py-8 text-sm text-muted-foreground`; inner `mx-auto max-w-7xl px-4 … flex flex-col md:flex-row md:items-center md:justify-between gap-4` |

---

## 7. Interaction & auth notes

### Routing & Clerk mounting (load-bearing)
- **Catch-all auth routes are mandatory.** `"/sign-in/*"` / `"/sign-up/*"` — Clerk drives multi-step flows (`/sign-in/factor-one`, `/sign-in/sso-callback`, password reset, MFA) via sub-paths; without the splat they 404. **Order auth splats (siblings of `RootLayout`) before** any catch-all; the global `*` lives inside `RootLayout` and is its last child.
- **Clerk props mirror the route:**
  - `<SignIn routing="path" path="/sign-in" signUpUrl="/sign-up" fallbackRedirectUrl="/account" appearance={{ elements: { headerTitle:'hidden', headerSubtitle:'hidden' } }} />`
  - `<SignUp routing="path" path="/sign-up" signInUrl="/sign-in" fallbackRedirectUrl="/account" appearance={{ … }} />`
  - `routing="path"` (not hash/virtual) so sub-steps live in the URL and survive refresh; `path` **must** agree with the route + splat or redirects loop.
- On `<ClerkProvider>` (in `main.tsx`): set `signInUrl="/sign-in"` `signUpUrl="/sign-up"`. Add `<BrowserRouter>` + `<QueryClientProvider>` around `<App/>`.
- **Header entry buttons (decided, §0 #4):** `<SignInButton mode="redirect">` / `<SignUpButton mode="redirect">` — they redirect to the `signInUrl`/`signUpUrl` routes rather than opening a modal, keeping one consistent in-app auth path.

### Post-auth redirect & deep-link round-trip (resolves G4)
- **Default target = `/account`** via `fallbackRedirectUrl="/account"`. `fallbackRedirectUrl` means "go here *unless* a redirect was already requested" — it does **not** stomp a `redirect_url` captured when an anon user was bounced off a guarded route. **Do not use `forceRedirectUrl`** (it overrides the captured destination and breaks deep-linking).
- **The capture mechanism is a `redirect_url` query param, NOT React Router `location.state`.** Clerk's redirect machinery reads `?redirect_url=`; it does **not** read `<Navigate state={{from}}>`. So `RequireAuth` must redirect with the param:
  `<Navigate to={`/sign-in?redirect_url=${encodeURIComponent(location.pathname + location.search)}`} replace />`.
  Clerk's `<SignIn>` then picks up `redirect_url` automatically and sends the user there post-auth, falling back to `/account` if absent. (If you prefer to keep `state.from`, you must have `SignInPage` read it and translate it into Clerk's redirect prop — pick **one** mechanism; the query-param route is Clerk-native and recommended.)
- **Round-trip:** anon hits `/account` (or `/host/...`) → `RequireAuth` redirects to `/sign-in?redirect_url=/host/...` → after auth, Clerk returns the user to `/host/...`, falling back to `/account`.

### Signed-in vs signed-out (and boot)
- **Header** reserves cluster width with `<ClerkLoading>` (skeleton pill) and swaps the right cluster inside `<ClerkLoaded>` via `<Show when="signed-out">` / `<Show when="signed-in">`. **`<Show>` returns null while loading — it is not the anti-flash mechanism**; `<ClerkLoading>/<ClerkLoaded>` (or `useAuth().isLoaded`) is.
- **Landing CTA** swaps via `<Show>` (post-load): signed-out → `SignUpButton mode="redirect"`; signed-in → `<Link>` ("Browse stays" / account). Optional signed-in "Welcome back" line pulls the name from **Clerk's user object** (`useUser`, client-side) — **do not** call `/identity/me` on landing (keeps it instant/cacheable; truncate long names `truncate max-w-[12rem]`).
- **Already-signed-in user hits `/sign-in`/`/sign-up`:** Clerk auto-redirects to the target — no manual guard needed. (A belt-and-suspenders `<Show when="signed-in"><Navigate/></Show>` is acceptable but redundant — don't do both.)
- **Sign-out** is owned entirely by `<UserButton afterSignOutUrl="/">` — no custom sign-out button.
- **Sheet lifecycle:** close on route change (effect on `useLocation().pathname`), Esc, and outside-click; **restore focus to the trigger** on close (Radix `Sheet` provides trap + scroll-lock + restore — don't hand-roll).

### Account data layer (the core of #19)
- **One query:** `useQuery({ queryKey: ['identity','me'], queryFn, enabled: isLoaded && isSignedIn })` — `isLoaded`/`isSignedIn` from `useAuth()`.
- `queryFn` is **async**: `await getToken()` from `useAuth()`, then `fetch('/identity/me', { headers: { Authorization: \`Bearer ${token}\` } })`. **Throw on non-2xx** so TanStack routes it to the error state (never `return res` on 4xx/5xx; never catch-log-swallow — Principle 10).
- Defaults: `staleTime: 60_000`, `retry: 1`. State → UI: `isPending`/`!isLoaded` → skeleton · `isError` → error panel · `isSuccess` → card. Page header stays static across all three.
- **Two name sources, intentional:** `id`/`email`/`displayName` from **`/identity/me`** (proves the API loop); avatar **image** from Clerk `user.imageUrl`. Don't double-fetch the name.
- **`displayName` fallback chain (define once):** `displayName || email.split('@')[0] || 'Your account'`.

### Edge cases
- **Token valid but `/identity/me` 401/403** (API refused, e.g. user not provisioned backend-side): this is the **error** state, *not* a redirect — the user IS Clerk-authenticated. Error panel + Retry is correct and more debuggable than bouncing to sign-in.
- **`getToken()` returns null** (session expired mid-view): treat as error → Retry; the guard/Clerk surfaces `signed-out` on next render. Don't swallow.
- **Short pages:** `main flex-1` against `#root` flex column keeps the footer down — no `mt-auto`.
- **Refresh mid-auth-flow** (e.g. email-verification step): `routing="path"` keeps the sub-step in the URL → re-renders correctly.
- **Direct hit to a bare sub-path** (`/sign-in/factor-one` cold): Clerk has no in-memory flow state → redirects to flow start. Acceptable; no extra handling.
- **Reduced motion / no-JS:** landing is content-first and meaningful without JS; any animation respects `motion-reduce:`; Sheet slide respects `prefers-reduced-motion` (Radix + `motion-reduce:`).
- **404:** unmatched under root → `NotFound` inside chrome with link home (§3d).

### Anti-pattern flags (Principle 10)
- **No bespoke auth form** — Clerk `<SignIn/>`/`<SignUp/>` own the form, validation, and security flows. Hand-rolling duplicates auth logic.
- **No form library / React Query for the search teaser** — it's an inert placeholder; wiring data fetching in is premature abstraction. Real search is a separate epic.
- **No `/identity/me` dependency on landing** — coupling marketing to an authed endpoint adds a failure mode + spinner to a page that must be instant.
- **No role gate on `/host`** — signed-in-only by design in Phase 1.
- **One Clerk package, one idiom** — import only from `@clerk/react`; `<Show when="...">` for the post-load swap, `<ClerkLoading>/<ClerkLoaded>` / `useAuth().isLoaded` for boot. **Uninstall `@clerk/clerk-react`** and add `no-restricted-imports` so `SignedIn/SignedOut` (v5-only) are unimportable, not just discouraged.
- **No interceptor machinery** for 401 retry in the shell — surface error + retry; build a shared `apiFetch` wrapper, not an interceptor layer.

---

## 8. Component inventory

### 8a. shadcn/ui (`npx shadcn@latest add <name>` — `src/components/ui/` is created by the first add)

| Component | Used by | Ticket | Notes |
|---|---|---|---|
| `button` | header, hero CTAs, error retry, host, 404, copy-id | #17 | verify `Button asChild` works with RR `<Link>` (`@radix-ui/react-slot` present) |
| `card` | landing value cards, Account profile, error | #17 | |
| `skeleton` | Account loading, header cluster boot pill | #17 add / #19 use | global loading primitive |
| `sheet` | mobile nav | #18 | Radix Dialog → focus trap + restore free |
| `avatar` | Account profile | #19 | image + initials fallback |
| `badge` | Account admin pill (optional) | #19 | display-only |
| `sonner` (toast) | optional retry/copy feedback | defer (#19 if used) | nice-to-have |
| `dropdown-menu` | — | **defer** | UserButton ships its own menu — don't duplicate |
| `input` / `label` | — | **skip** | brief lists as "e.g."; no shell form outside Clerk's widgets — install when a feature page needs them |

### 8b. Clerk (`@clerk/react` v6 — **uninstall `@clerk/clerk-react`**)

| Item | Where | Purpose |
|---|---|---|
| `<ClerkProvider>` | `main.tsx` (already) + `signInUrl`/`signUpUrl` props | root auth context; drives `mode="redirect"` buttons |
| `<ClerkLoading>` / `<ClerkLoaded>` | header right cluster | boot-state gating (reserve width / kill flash) — the **correct** anti-flash mechanism |
| `<Show when="signed-in"\|"signed-out">` | header (inside `ClerkLoaded`), landing CTA | **post-load** conditional render (renders null while loading — not for boot state) |
| `<SignInButton mode="redirect">` / `<SignUpButton mode="redirect">` | header signed-out, landing | open auth via redirect to `/sign-in`/`/sign-up` (not modal) |
| `<UserButton afterSignOutUrl="/">` | header signed-in | avatar + menu + sign-out |
| `<SignIn>` / `<SignUp>` | `/sign-in/*`, `/sign-up/*` | in-app path-routed forms; `appearance` to hide internal title (verify keys) |
| `useAuth()` → `isLoaded`, `isSignedIn`, `getToken()` | `RequireAuth`, Account `queryFn`/`enabled`, CTA target | guard + token for API (the loading-state source `<Show>` cannot provide) |
| `useUser()` → `imageUrl`, `publicMetadata` | Avatar image, admin badge, "welcome back" name | client-only display data |

### 8c. Custom

`RootLayout`, `AuthLayout`, `Header`, `Footer`, `Logo`, `Container`, `NavLink`, `MobileNav`/Sheet, `RequireAuth`, `SkipLink`, `Hero`, `SearchTeaser`, `HostBanner`, page components (`LandingPage`, `AccountPage`, `HostPlaceholder`, `NotFound`), `apiFetch` wrapper + `useIdentityMe()` query hook.

> **Slice cohesion (Principle 5):** keep `RootLayout`/`Header`/`Footer`/`Logo`/`MobileNav`/`RequireAuth` together as the shell slice under `src/layouts/` (or `src/components/layout/`) — don't scatter. Don't over-extract `AuthLayout` if it stays trivial (Principle 9 — avoid extract-method-of-one).

---

## 9. Design tokens

Define as CSS variables in `index.css` under shadcn's convention (`cssVariables: true`, base `slate`). Keep to **one brand hue + slate neutrals** so the shell is genuinely re-brandable by swapping `--primary`.

**Brand feel:** calm, trustworthy, travel-warm but not loud — slate neutrals + a **teal/cyan primary** ("modern booking" without copying Airbnb's coral).

**Color (semantic shadcn slots):**

| Token | Light (suggested) | Role |
|---|---|---|
| `--background` / `--foreground` | white / slate-950 | page |
| `--primary` / `--primary-foreground` | teal-600 (`~oklch(0.6 0.12 200)`) / white | CTAs, active nav, brand mark, host banner |
| `--muted` / `--muted-foreground` | slate-100 / slate-500 | secondary text, footer, skeletons |
| `--card` / `--card-foreground` | white / slate-950 | cards |
| `--border` / `--input` / `--ring` | slate-200 / slate-200 / teal-600 | borders + focus ring |
| `--destructive` | red-600 | error states |

Use shadcn's default slate block from `add`; only override `--primary` and `--ring` to teal. **Dark mode is out of scope** for the shell — but keep the `.dark {}` block shadcn generates so it's a later flip, not a rewrite.

**Typography (system stack; no webfont yet):**

| Use | Class |
|---|---|
| Hero h1 | `text-4xl sm:text-5xl lg:text-6xl font-semibold tracking-tight` |
| Page h1 | `text-2xl font-semibold tracking-tight` |
| Section h2 | `text-xl font-semibold` |
| Body | `text-base leading-relaxed` (~16px) |
| Secondary / footer | `text-sm text-muted-foreground` |
| Mono (IDs) | `font-mono text-xs` |

**Spacing & radius:** 4px base (Tailwind default). Section rhythm `py-16 md:py-24` (hero), `py-8` (footer); card padding `p-6`; stack gaps `gap-4`/`gap-6`; gutters `px-4 sm:px-6 lg:px-8`. One radius token (shadcn `--radius`, default `0.5rem`).

**Focus ring:** rely on shadcn's `focus-visible:ring-2 ring-ring ring-offset-2` (ships with `button`); apply the same util to custom links so the whole shell shares one visible-focus language.

> **Reference (Principle 11):** Tailwind v4 theme variables — tailwindcss.com docs, "Theme" / `@theme` directive (why: v4 moved tokens from `tailwind.config.js` into CSS, which this repo already uses via `@import "tailwindcss"`). Pair with shadcn's "Theming" docs for the semantic slot names above.

---

## 10. Global conventions — loading / empty / error / a11y

**Loading**
- Data regions (Account): **Skeleton mirroring final layout**, not a centered spinner; `aria-busy="true"` + sr-only label.
- Auth booting (`!isLoaded`): **`<ClerkLoading>`** renders a fixed-width skeleton pill in the header right-cluster; `RequireAuth` (via `useAuth().isLoaded`) renders a route-level skeleton, never a flash of the wrong auth state. **Not** `<Show>` (which renders null while loading).

**Empty** — not hit in the shell, but establish the pattern: centered icon + one-line explanation + one primary action inside the container. Reused by future search/listing epics.

**Error**
- Query errors: inline **error card**, `role="alert"`, plain-language message, single `Try again` (`refetch`). No raw error strings/stack traces.
- **Never catch-log-swallow** (Principle 10): `queryFn` throws on non-2xx so React Query owns the error state; don't `try/catch → return null`.
- Global fallback: wrap routes in an **ErrorBoundary** (RR `errorElement` or small custom boundary) so a render crash shows a friendly page, not a white screen (add #18).

**a11y baseline (Phase-1 sanity, not full audit)**
- Landmarks: one `<header>`, one `<main id="main">`, one `<footer>`; nav regions labeled.
- **Skip-to-content** link as first focusable.
- Exactly one `<h1>` per page; headings don't skip levels (Account name = `<h2>` under the page `<h1>`). **Auth pages:** target is our heading block as the `<h1>` with Clerk's internal title suppressed via `appearance`; **if the `appearance` keys can't be verified (G5), the relaxed acceptance is** "our block is `<h1>`, Clerk's title is visually-hidden-or-`<h2>`+" — don't block DoD on it.
- All interactive elements are real `<button>`/`<a>` with visible focus rings; icon-only buttons have `aria-label`.
- Sheet/menu: focus trap + restore (Radix), `aria-expanded`/`aria-controls` on trigger; respect `prefers-reduced-motion`.
- State conveyed by **text**, not color alone ("Signed in", error icon + heading).
- Color contrast ≥ 4.5:1 body — **verify teal-600 + `--primary-foreground` pairings**.
- Account `id` in a `<dl>` so it reads as a labeled value; loading `role="status"`, error `role="alert"`.
- `NotFound` / `HostPlaceholder` each have exactly one `<h1>` (the message; the "404" eyebrow is decorative).

---

## 11. Build order (mapped to #17 / #18 / #19)

### #17 — Foundation (no auth UI yet)
1. **Uninstall `@clerk/clerk-react`** (keep only `@clerk/react`); add ESLint `no-restricted-imports` banning `@clerk/clerk-react`. **Verify `lucide-react` resolves** `Menu`/`X`/`AlertTriangle`/`Copy` (the `^1.8.0` version looks wrong — swap to the correct `lucide-react` if imports fail).
2. Add `QueryClientProvider` in `main.tsx` (inside `ClerkProvider`); create `src/lib/queryClient.ts` (`staleTime`, no retry on 4xx). Add `signInUrl`/`signUpUrl` to `ClerkProvider`; add `<BrowserRouter>`.
3. `npx shadcn add button card skeleton` → confirm `src/components/ui/` generates and slate tokens land in `index.css`. Override `--primary`/`--ring` to teal. Add `scroll-padding-top` to `html`.
4. Build layout primitives: `RootLayout` (`Header` / `<main flex-1><Outlet/></main>` / `Footer` over the existing `#root` flex column), `Container`, `SkipLink`, **replace the `Footer.tsx` stub**, **replace the `Header.tsx` stub** with a **non-auth** skeleton (brand + placeholder right slot).
5. Router skeleton: `RootLayout` with `<Outlet/>`, `LandingPage` at `/`, `NotFound` at `*` (inside chrome). **Reconcile the stubs:** make `MainLayout`/`RootLayout` `Outlet`-based (it already imports `Header`/`Footer`; the defect is the `children` prop — replace with `<Outlet/>`); convert `App.tsx` from the bare `<header>` stub into the `<Routes>` tree.
6. **DoD:** `/` renders header + hero + footer responsively; `*` shows the 404 page in chrome; `npm run build` + `lint` clean; no `@clerk/clerk-react` in the dependency tree; no auth logic yet.
7. *Learning note:* short `docs/` note "why QueryClient defaults (staleTime / retry) for an auth'd SPA." Reference: **TanStack Query v5 — "Important Defaults"** (tanstack.com/query/latest/docs/framework/react/guides/important-defaults) — default `retry: 3` + refetch surprises people on 401s.

### #18 — Auth + layout integration
1. `npx shadcn add sheet`.
2. Header auth states: wrap the right cluster in `<ClerkLoading>`(skeleton pill) / `<ClerkLoaded>`, then inside `<ClerkLoaded>` swap via `<Show>`: signed-out → `SignInButton mode="redirect"`/`SignUpButton mode="redirect"` (Buttons); signed-in → nav links + `<UserButton afterSignOutUrl="/">`. **This is what kills the flash — not `<Show>` alone.**
3. Mobile `Sheet` nav; `UserButton` stays in the bar; brand `truncate` guard for 320px.
4. Auth routes: `AuthLayout` + `<SignIn routing="path" path="/sign-in" signUpUrl="/sign-up" fallbackRedirectUrl="/account">` / `<SignUp …>` with catch-alls, declared **before** the catch-all. Suppress Clerk's internal title via `appearance={{ elements: { headerTitle:'hidden', headerSubtitle:'hidden' } }}` — **verify the v6 element keys**; if they don't take, accept the relaxed one-`<h1>` rule (G5).
5. `RequireAuth` (driven by `useAuth().isLoaded`/`isSignedIn`) + protected group for `/account`, `/host/*`; anon → `/sign-in?redirect_url=<intended path>` (query param, **not** RR state — G4); signed-in users auto-redirected off auth pages (Clerk).
6. Add an `ErrorBoundary` fallback.
7. **Confirm the single Clerk idiom** — only `@clerk/react`; `<ClerkLoading>/<ClerkLoaded>` for boot, `<Show>` for the swap; ensure no `SignedIn/SignedOut` slipped in.
8. **DoD:** correct header per auth state at every breakpoint, including the boot skeleton (no flash); guards + redirect-param round-trip verified (anon→`/host/x` lands back on `/host/x` after auth); auth pages have one effective `<h1>`.

### #19 — Account page (the data slice)
1. `npx shadcn add avatar` (+ `badge`; optional `sonner`).
2. `apiFetch` wrapper calling `useAuth().getToken()` → `Authorization: Bearer …`; `useIdentityMe()` hook (`queryKey: ['identity','me']`, `enabled: isLoaded && isSignedIn`, throws on non-2xx).
3. `AccountPage`: Card with Avatar (image from `useUser().imageUrl`) + displayName + email + id (`<dl>`), **Skeleton** loading, **error card** with `Try again`. Optional admin badge from `useUser().publicMetadata.role` (display-only). Page header static across all states.
4. `HostPlaceholder` (guarded, no role check) per §3e.
5. **DoD:** signed-in user sees real `/identity/me` data; loading shows skeleton; forced error (e.g. 401 from API) shows retry, **not** a redirect; anon → redirect with `redirect_url`.
6. *Learning note:* write-up "attaching Clerk JWT to API calls + why `enabled` gating matters." Reference: **Clerk docs — `getToken()` with fetch / TanStack Query** (clerk.com/docs, search "getToken fetch") — the exact token-on-fetch pattern every future authed call copies.

---

## 12. Open decisions to confirm before coding

1. **Admin badge source (Principle 10):** `/identity/me` shape is `{ id, email, displayName }` — **no role field**. Options: **(a)** read Clerk `useUser().publicMetadata.role` client-side (zero backend, fine for a *cosmetic* shell badge — **recommended now**), or **(b)** extend `/identity/me` to return `role`/`isAdmin` (ADR-worthy, changes the API contract). The badge must **never gate anything** — client claims are trivially spoofable; real authz is server-enforced.
   *Reference:* Clerk "Roles and Permissions / Authorization" docs (clerk.com/docs, search "Clerk authorization checks publicMetadata") + **OWASP Top 10 2021 A01: Broken Access Control** (owasp.org/Top10/A01_2021-Broken_Access_Control) — why "the client said it's an admin" is never sufficient.
2. **Avatar source:** Clerk `useUser().imageUrl` with initials fallback (recommended; API provides only text fields). Initials derive from the `displayName` fallback chain.
3. **`displayName` empty:** `displayName || email.split('@')[0] || 'Your account'` — define once.
4. **`/host` index vs `/host/*`:** keep the splat so future nested host routes are additive.
5. **Clerk `appearance` element keys for title suppression (G5):** confirm `headerTitle`/`headerSubtitle` are the live keys in installed `@clerk/react` v6 during #18; if renamed, find the equivalents or fall back to the relaxed one-`<h1>` acceptance.

> **Note:** the former "modal vs redirect" open decision is now **resolved** in §0 #4 (redirect, via `mode="redirect"` + provider `signInUrl`/`signUpUrl`).

---

## Files referenced (absolute)

- `C:\VM\Projects\StayNGo\frontend\src\main.tsx` — `ClerkProvider` root; add `QueryClientProvider`, `signInUrl`/`signUpUrl`, `<BrowserRouter>` (#17).
- `C:\VM\Projects\StayNGo\frontend\src\App.tsx` — currently a bare `<header>` using `<Show>`; becomes the `<Routes>` tree wrapping `RootLayout` (#17/#18).
- `C:\VM\Projects\StayNGo\frontend\src\layouts\MainLayout.tsx` — stub that **already imports** `@/components/Header` and `@/components/Footer` (default exports); the defect is the `children` prop — refactor to `<Outlet/>`-based `RootLayout` (#17). No missing imports.
- `C:\VM\Projects\StayNGo\frontend\src\components\Header.tsx` — trivial `<div>` stub to **replace** per §6a (shell #17 / auth #18).
- `C:\VM\Projects\StayNGo\frontend\src\components\Footer.tsx` — trivial `<div>` stub to **replace** per §6h (#17). (Not a new file.)
- `C:\VM\Projects\StayNGo\frontend\src\index.css` — `#root` flex-column + `100dvh` + Preflight already set; add `--primary`/`--ring` overrides and `scroll-padding-top` (#17).
- `C:\VM\Projects\StayNGo\frontend\components.json` — `new-york`/`slate`/`lucide`/`cssVariables:true`; drives all `shadcn add`.
- `C:\VM\Projects\StayNGo\frontend\src\lib\utils.ts` — `cn()` helper (present).
- `C:\VM\Projects\StayNGo\frontend\package.json` — **ships both `@clerk/clerk-react` (v5) and `@clerk/react` (v6) — uninstall the v5 package (#17)**; `@tanstack/react-query`, `react-router-dom@7`, `@radix-ui/react-slot` present (no new deps for router/`asChild`); **verify `lucide-react` (listed `^1.8.0`) resolves the needed icons** (#17).
- `C:\VM\Projects\StayNGo\frontend\src\components\ui\` — does **not** exist yet; created by the first `shadcn add` in #17.
- New shell files to add: refactored `MainLayout`/`RootLayout`, `AuthLayout.tsx`, `Logo.tsx`, `Container.tsx`, `MobileNav.tsx`, `RequireAuth.tsx`, `SkipLink.tsx`, `routes/auth/SignInPage.tsx`, `routes/auth/SignUpPage.tsx`, `AccountPage`, `HostPlaceholder`, `NotFound`, `lib/queryClient.ts`, `lib/apiFetch.ts`, `hooks/useIdentityMe.ts`. (`Header.tsx`/`Footer.tsx` already exist as stubs to replace, not add.)
