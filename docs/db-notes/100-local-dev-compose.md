# 100. Local dev with docker-compose

## Healthcheck and `service_healthy`
`pg_isready` checks whether Postgres is ready to accept and process
queries — not just whether the port is open (`nc -z localhost 5432`
would pass earlier and lie). When Postgres is starting up or recovering
from a crash, the postmaster (the supervisor process that manages the
cluster) binds port 5432 first, but for the next 1–5 seconds it
**rejects** incoming connections with `FATAL: the database system is
starting up`. `pg_isready` returns exit code 1 during this window and
0 only once the server can actually serve queries. Compose'swha
`depends_on.condition: service_healthy` blocks the api container from
starting until this check passes, so EF Core never connects too early.

## Named volume for postgres data
Data lives inside a persistent named volume, so on service restart it
isn't deleted. `docker compose down` stops services but leaves volumes
intact; to wipe the data you need `docker compose down -v`.

## Frontend bind-mount for HMR
`src/` and `public/` are bind-mounted so Vite picks up file changes on
the host and triggers HMR inside the container. `node_modules` is
deliberately *not* mounted: native modules can be platform-specific
(Linux vs. Windows binaries differ), so the host's `node_modules` —
built on the developer's OS — would crash or misbehave when the
container tries to use them. Let the container build its own.
