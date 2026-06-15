# 0006. Bridge Clerk users via JIT provisioning, not an OnSignUp webhook

Date: 2026-06-15
Status: Accepted

## Context

The Identity epic (#2) proposed an `OnSignUp` handler to copy Clerk users into the local `users` table. An eager webhook means standing up a public, signature-verified endpoint and handling Clerk's delivery/retry semantics — infrastructure that exists before any user value does.

## Decision

Bridge users **lazily (just-in-time)** instead. `CurrentUserService.GetOrProvisionAsync` creates the local `users` row on the user's first authenticated request (e.g. `GET /identity/me`), keyed by the JWT subject (`sub`). No webhook in Phase 1. This supersedes the "OnSignUp handler" wording in epic #2; the epic's actual goal — a local user record bridged from Clerk — is met.

## Consequences

- Simpler: no webhook endpoint, no signature verification, no Clerk delivery/retry handling. A local user exists exactly when they first touch an authenticated endpoint.
- Trade-off: the local row can drift from Clerk (a name/email changed in Clerk isn't pushed to us). When that staleness actually bites, add a **profile-sync webhook** as a need-driven `[internal]` ticket — which would supersede this decision in part.
- `/identity/me` is therefore a write-on-read on first call; benign, but worth knowing it isn't a pure read.

## References

- Just-in-time (JIT) provisioning — Clerk's "Sync user data to your backend" guidance (clerk.com/docs); the general pattern of lazily materializing an identity-provider user into the application's own store on first authenticated use.
