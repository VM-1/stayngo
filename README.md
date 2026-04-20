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
