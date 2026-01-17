# ADR 0001: Solution Structure

## Status
Accepted

## Context
JsonUI needs a maintainable architecture that keeps security-sensitive logic centralized and allows agent-assisted implementation without tight coupling.

## Decision
Adopt a clean-architecture-inspired layout:
- `src/JsonUi.Core`: domain models + interfaces; no infrastructure dependencies.
- `src/JsonUi.Infrastructure`: persistence, encryption, caching, discovery, HTTP client implementations; depends on Core.
- `src/JsonUi.Gateway`: ASP.NET Core host (admin API, MCP endpoints, health checks); depends on Core + Infrastructure.
- `src/JsonUi.Ui`: Blazor admin UI; calls Admin API and stays presentation-focused.
- `tests/`: unit and integration tests per module.

## Consequences
- Clear dependency direction simplifies security review.
- Infrastructure can be replaced (e.g., SQLite -> Postgres) without touching Core.
- UI remains thin; business logic is centralized.

## Verification
- Build analyzers (later) ensure Core has no forbidden references.
- PR review rejects boundary violations.
