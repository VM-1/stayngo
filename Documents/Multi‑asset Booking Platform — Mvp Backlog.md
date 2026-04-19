# StayNGo — MVP Backlog

## Epics (MVP)

### 1. Foundation / Repo / CI
Establish monorepo, baselines, tooling, Dockerized local env, and CI to build/test on PR and publish images on main.

### 2. Auth & Users
Secure email/password authentication, JWT issuance/validation, and basic RBAC (Guest/Host/Admin).

### 3. Listings & Units
CRUD for listings and units with validation, search fields, and DB indexes.

### 4. Availability & Holds
Calendar per unit and time-boxed holds with TTL and automatic expiry.

### 5. Booking Flow
Confirm booking with overlap checks and mock payment provider.

### 6. Admin Minimal
Admin pages to approve listings and view bookings.

### 7. Frontend SPA
React + TypeScript app with routing, auth, listings UI, search.

### 8. Testing & Quality
Unit/integration/e2e tests, coverage targets, linting.

### 9. Observability Basics
Structured logging, global error handling, health checks.

### 10. Seed & Fixtures
Seed data and fixtures for demo flows.

---

# Sprint 1 (2 weeks)

## Tasks

### Initialize Monorepo
Create /backend, /frontend, /infra with configs and standards.

### Docker Compose Setup
Postgres + backend + frontend containers.

### ADR-001
Document architecture decisions.

### EF Core Schema
Users, Roles, Listings, Units.

### CI Pipeline
GitHub Actions for build/test/deploy.

### RBAC Setup
User + Role tables, Admin seed.

### JWT Authentication
Register/Login + JWT tokens.

### Authorization Middleware
Role-based access control.

### Listings Domain
Entities + repositories.

### Listings API
CRUD endpoints.

### Search API
Filters: location, dates, guests.

### React Scaffold
Vite + TS + routing.

### Auth UI
Login/Register pages.

### Listings UI
List/Create/Edit.

### Observability
Logging + error handling + health checks.

### Swagger
API documentation.

### Testing
Backend + frontend test setup.

### Seed Data
Demo data.

### Env Config
.env.sample setup.

---

# Sprint 2 (2 weeks)

### Calendar & Holds
Tables for availability.

### Availability API
GET /units/{id}/availability

### Create Hold
POST /holds

### Expire Holds Job
Background worker.

### Booking Table
Booking entity.

### Confirm Booking
POST /bookings/confirm

### Admin UI
Approve listings.

### Calendar UI
Availability visualization.

### Hold UI
Countdown + cancel.

### Booking UI
Confirmation page.

### Testing
Edge cases + e2e.

### Coverage
70% CI enforcement.

---

# Notes

- Timezone: Asia/Dubai
- API: REST + JSON
- Images: URLs only
- Coverage target: 70%
