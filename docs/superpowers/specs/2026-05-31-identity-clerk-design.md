---
type: spec
track: [backend]
phase: 1
status: growing
created: 2026-05-31
---

# StayNGo — Identity (Clerk) design — Phase 1, epic #2

> **Date:** 2026-05-31
> **Status:** Accepted (brainstorming complete) — input to `writing-plans`
> **Owner:** valiko.mkhitaryan@newguys.io
> **Executor:** **`[by-hand]`** — the user implements all code. Claude Code provides this spec + the
> implementation plan as the blueprint (reference snippets, acceptance criteria, learning refs) and
> stands by for design-chat, anti-pattern flags (Principle 10), and WIP review. Per CLAUDE.md, Claude
> Code does **not** write the implementation.
> **Parent spec:** [`2026-04-19-staynGo-design.md`](./2026-04-19-staynGo-design.md) — §3 stack, §5 domain, §5.5 flow #1.
> **Epic:** #2 *EPIC: Identity — Clerk*. Delivers MVP flow #1 ("User signs up / logs in") on the backend.

---

## 1. Goal

Authenticated requests carry a **Clerk-issued JWT**; the API validates it; the **first authenticated
request just-in-time (JIT) provisions** a local `users` row from JWT claims; **`GET /me`** returns the
current user. This is the backend half of "sign up / log in" — the Clerk-hosted UI and the React
wiring belong to the Frontend Shell epic (#6).

## 2. Scope & non-goals

**In scope:** JWT-bearer validation against Clerk; JIT provisioning (already drafted in
`CurrentUserService`); `GET /me`; and the **reusable integration-test harness**
(`WebApplicationFactory` + Testcontainers + `TestAuthHandler`) that every later epic will reuse.

**Non-goals (Phase 1, per epic #2):**
- Roles / permissions. Only the existing `IsAdmin` bool exists; it is **never JIT-granted**.
- MFA, SSO, organizations, multi-device session management.
- Profile editing through our API — Clerk owns the profile (no `PATCH /me`).
- `OnSignUp` webhook — **deferred** (no consumer yet; would add Svix signature verification + a local tunnel).
- Profile sync — JIT writes the row **once**, so it can go **stale** if the user later changes their
  name/email in Clerk. Accepted limitation for MVP; sync arrives with a future webhook.

## 3. The flow

> Browser (Clerk React SDK) obtains a Clerk **session JWT** (with `email` + `name` added via a JWT
> template) → calls `GET /me` with `Authorization: Bearer <jwt>` → API's **JWT-bearer middleware**
> validates signature/issuer/audience/lifetime against Clerk's JWKS (`Authority`/`Audience` from
> config) → `CurrentUserService.GetOrProvisionAsync()` reads `sub`/`email`/`name`, looks up by
> `ClerkId`, creates the row if missing (race-safe via the `23505` unique-violation refetch) →
> endpoint maps the `User` to a `MeResponse` → `200`.

## 4. Design decisions

| Decision | Choice | Rationale |
|---|---|---|
| Provisioning model | **JIT** via Clerk JWT template | Simplest; no webhook / Svix / tunnel; matches the existing `CurrentUserService`. Defer `OnSignUp` until a consumer exists (Principle 2). |
| `email`/`name` source | Clerk **JWT template** (custom session-token claims) | Clerk's default session token carries `sub` but not `email`/`name`. |
| Endpoint shape | **Thin `/me`**, no handler | `/me` has no query logic beyond `ICurrentUserService`. A no-logic `IRequestHandler` here is premature abstraction (Principle 10). The `IRequestHandler<TReq,TRes>` pattern is introduced with the first feature that has real logic — **Listings/Create**. |
| Domain events | **Deferred** | No consumer; entities stay POCO. First real events arrive with **Booking** (`BookingConfirmed`). "Event-ready" ≠ unused plumbing (Principles 2 & 7). |
| Integration-test auth | **`TestAuthHandler`** + Testcontainers | Exercise the real `/me` + JIT + DB (incl. the unique-violation refetch) against real Postgres; don't re-test framework JWT-bearer middleware (code you don't own). |

## 5. Components & changes (all under `backend/StayNGo/`)

**1. Restore JWT-bearer auth**
- `Directory.Packages.props` — add `PackageVersion` for `Microsoft.AspNetCore.Authentication.JwtBearer` (aligned to .NET 10, `10.0.x`).
- `src/StayNGo.Api/StayNGo.Api.csproj` — add `<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" />` (version comes from central management).
- `src/StayNGo.Api/Program.cs` — `AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(...)` with `Authority`/`Audience` from `"Clerk:Authority"`/`"Clerk:Audience"` and `TokenValidationParameters { ValidateIssuer/ValidateAudience/ValidateLifetime = true }`; `AddAuthorization()`; **and the middleware** `app.UseAuthentication(); app.UseAuthorization();` before mapping protected endpoints. ⚠️ The journal's restore diff (see [[2026-05-31]]) shows the *service* registration but **omits the two `Use…` middleware lines** — they are required.

**2. Wire the application services** (the stashed `Services/`)
- `Services/DependencyInjection.AddApplication` — add `services.AddHttpContextAccessor();` (`CurrentUserService` depends on `IHttpContextAccessor`, currently unregistered). Keep `AddScoped<ICurrentUserService, CurrentUserService>()`.
- `Program.cs` — call `builder.Services.AddApplication(builder.Configuration);` (today only `AddInfrastructure` is called).

**3. Config**
- `appsettings.json` / `appsettings.Development.json` — add `"Clerk": { "Authority": "...", "Audience": "..." }`.
- Opportunistic: fix the `ConnectionStrings:StayNGO` casing nit to match the `"StayNgo"` lookup (cosmetic; works today only because config keys are case-insensitive).

**4. `GET /me` endpoint** — `src/StayNGo.Api/Features/Identity/Me/`
- `MeEndpoint` — `MapGet("/me", async (ICurrentUserService cu, CancellationToken ct) => …).RequireAuthorization();` → `GetOrProvisionAsync` → map to `MeResponse` → `Results.Ok(...)`. Map it via a small `MapIdentityEndpoints(this IEndpointRouteBuilder)` extension in the slice, called from `Program.cs`.
- `MeResponse` — `record MeResponse(Guid Id, string Email, string DisplayName, bool IsAdmin, DateTime CreatedAt);`

**5. `CurrentUserService` touch-ups (minor)**
- Fix the typo `ThrowMisingClaim` → `ThrowMissingClaim` and the stray quote in the exception message.
- Map a missing required claim to a clear `401` (or `ProblemDetails`) rather than an unhandled `500` (see §7).

**6. Test infrastructure** — `tests/StayNGo.IntegrationTests/`
- `StayNgoWebApplicationFactory : WebApplicationFactory<Program>` — repoints the `DbContext` at the Testcontainers Postgres and replaces the auth scheme with `TestAuthHandler`.
- Shared container via `IClassFixture`/collection fixture + reset (truncate or Respawn) between tests.
- `TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>` — builds a `ClaimsPrincipal` from per-test seeded claims (`sub`/`email`/`name`), e.g. read from request headers the test sets.
- ⚠️ Top-level `Program.cs` makes `Program` internal — add `public partial class Program;` at the end so the test project can reference `WebApplicationFactory<Program>` (or use `InternalsVisibleTo`).

## 6. Prerequisite — `[by-hand]`, Clerk dashboard

1. Create (or reuse) a Clerk application.
2. **Customize the session token / add a JWT template** so the token includes `email` and `name` claims.
3. Record: **`Authority`** = your Clerk issuer / Frontend API URL (e.g. `https://<subdomain>.clerk.accounts.dev`); **`Audience`** = your configured audience.
4. Put these in `appsettings.Development.json` / `.env.local` (the `.env.example` already stubs `Clerk__Authority` / `Clerk__Audience`).

Reference: **Clerk docs — "Manual JWT verification" / "Customize your session token"** (clerk.com/docs).

## 7. Error handling

- Unauthenticated / invalid token → **`401`** (`RequireAuthorization` + JWT bearer).
- Authenticated but JWT missing `email`/`name` (JWT-template misconfig) → **fail clearly** (a `401`/`ProblemDetails` with "token missing required claim"), never create a half-populated user. Global `ProblemDetails` formatting is the Observability epic (#7); a clear status + message suffices now.
- Provisioning race (two concurrent first-requests, same `sub`) → already handled: catch `23505` unique-violation on `clerk_id`, detach, refetch.

## 8. Testing (TestAuthHandler + Testcontainers, real Postgres)

1. `GET /me` **without** auth → `401`.
2. `GET /me` with a **new** `sub` (seeded claims) → `200`; body matches the claims; a `users` row now exists with that `clerk_id`.
3. `GET /me` **twice** with the same `sub` → `200` both times, same user `id`, **exactly one** row (no duplicate).
4. *(optional)* Missing `email`/`name` claim → fails per §7 (no `500` stack-trace leak).

The `23505` race is hard to exercise deterministically end-to-end; a focused test of `GetOrProvisionAsync` under forced contention is optional — **log it if skipped** (no silent gaps).

## 9. Acceptance criteria (Definition of Done)

- [ ] JWT bearer wired (services **and** middleware); anonymous request to `/me` → `401`.
- [ ] `AddApplication` + `AddHttpContextAccessor` registered; app boots.
- [ ] `Clerk:Authority`/`Audience` read from config; the `[by-hand]` Clerk JWT-template step done and noted in own words.
- [ ] `GET /me` provisions-on-first-call and returns `MeResponse`; second call → no duplicate row.
- [ ] `WebApplicationFactory` + Testcontainers + `TestAuthHandler` harness in place (reusable by later epics).
- [ ] Tests 1–3 (§8) pass under `dotnet test`.
- [ ] No new domain-event infra; entities remain POCO (decision recorded here).
- [ ] *(Learning, Principle 11)* a short Concept/db-note in own words on JWT/JWKS validation + JIT provisioning.

## 10. Deferred / future

- `OnSignUp` webhook (Svix-verified) + profile sync (`user.updated`/`user.deleted`) → when a consumer needs it.
- `PATCH /me` / profile editing.
- `IRequestHandler<TReq,TRes>` pattern → **Listings** epic (#3).
- Domain events + dispatch-on-`SaveChanges` → **Booking** epic (#5).
- Global `ProblemDetails` → **Observability** epic (#7).

## 11. Learning objectives & references (Principle 11)

- **JWT validation / JWKS / OIDC discovery** — how `AddJwtBearer` pulls signing keys from the
  Authority's `/.well-known/openid-configuration`. Ref: *Microsoft Learn — "Configure JWT bearer
  authentication in ASP.NET Core"* + *Clerk — "Manual JWT verification"* (clerk.com/docs).
- **JIT provisioning + unique-violation race** — idempotent create under concurrency. Ref: Andrew
  Lock's blog on EF Core + unique constraints (andrewlock.net); relates to [[GiST exclusion constraint]].
- **Integration testing with `WebApplicationFactory` + `TestAuthHandler`** — Ref: *Microsoft Learn —
  "Integration tests in ASP.NET Core"* (learn.microsoft.com/aspnet/core/test/integration-tests) +
  the **eShop** reference app's factory/auth setup.

## Links

- Parent: [`2026-04-19-staynGo-design.md`](./2026-04-19-staynGo-design.md) (§5 domain, §5.5 flows) · `CLAUDE.md` (executor conventions, hard rules).
- Session handoff with the exact JWT restore diff: [[2026-05-31]] (Journal).
- Existing scaffold: `src/StayNGo.Api/Services/{CurrentUserService,DependencyInjection}.cs`, `Services/Interfaces/ICurrentUserService.cs`.
