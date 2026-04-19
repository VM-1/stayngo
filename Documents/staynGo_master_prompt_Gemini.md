StayNGo Project Manifesto & Guidelines
1. Project Overview
Name: StayNGo

Vision: A high-performance booking platform built with enterprise-grade standards.

Goal: Start as a clean monolith, architected for future transition to a Modular Monolith and eventually an Event-Driven Microservices architecture.

2. Technical Stack
Backend: .NET 8/9, C#, Entity Framework Core (PostgreSQL).

Architecture: Clean Architecture + CQRS (MediatR) + Vertical Slices where appropriate.

Frontend: React (Vite), TypeScript, Tailwind CSS, TanStack Query.

Infrastructure: GitHub Actions (CI/CD), Railway/Vercel (Deployment).

Messaging (Future): RabbitMQ / Kafka / MassTransit.

3. Engineering Standards (Strict)
Backend (The "Clean" Rule)
Domain Layer: Must be the center. No dependencies on Infrastructure or API. Contains Entities and Domain Events.

Application Layer: Contains MediatR Commands/Queries. Logic lives in Handlers.

Infrastructure Layer: Data persistence and external services.

CQRS: Every state change must be a Command. Every data fetch must be a Query. No exceptions.

EF Core: Use DbContext as the Unit of Work. Avoid redundant custom Repository wrappers unless a specific abstraction is required for testing.

Frontend (The "Type-Safe" Rule)
Vite + TS: Use strict TypeScript. No any.

Components: Functional components with Tailwind for styling.

Data Fetching: Use TanStack Query (React Query) for all API interactions to manage caching and server state.

4. Operational Workflow (Simulated Company)
Task Management: Use GitHub Projects (accessible via MCP).

Branching: Trunk-based development. Work on feature branches (feat/ticket-id-description), then merge to main via PR.

Commits: Follow Conventional Commits (e.g., feat: add booking validation).

AI Collaboration: Claude Code is the "Lead Engineer." Before writing code, Claude should check the GitHub Project board for the next task and move it to "In Progress."

5. Evolution Strategy (YAGNI but Ready)
Modularity: Keep namespaces distinct (e.g., StayNGo.Modules.Bookings, StayNGo.Modules.Users).

Events: Design logic to trigger Domain Events so that adding a Message Broker later only requires adding a new Subscriber/Handler.

No Sprawl: Do not add Redis or Kafka yet. Write code that is ready for them, but use In-Memory implementations for the MVP.