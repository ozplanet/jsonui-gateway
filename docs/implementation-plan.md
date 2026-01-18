# JsonUI Business Backend Implementation Plan (P1 Slice)

## Goal
Deliver the first functional slice of JsonUI MCP Gateway focused on the MVP in `project-015-mcp-gateway.md`. The slice enables managing Integrations, Secrets, and Actions, exposing them via MCP list/call endpoints, and running within CasaOS-friendly Docker packaging using SQLite + EF Core, with Redis optional.

## Scope
- **In-scope**
  - Domain models for Integrations, Secrets, Actions, API Keys (seed admin only).
  - EF Core SQLite DB context, migrations, and repository abstractions.
  - Secrets encryption service driven by `JSONUI_MASTER_KEY` env var.
  - Minimal admin APIs (REST) for Integrations, Secrets, Actions, Playground call.
  - MCP server endpoints for listing and calling tools scoped by API key.
  - SSRF guard that enforces base URL + allowlist; blocks loopback/metadata.
  - Auth middleware with API key hash verification, admin flag, and scopes.
  - Dockerfile + compose snippet + CasaOS manifest draft (marked experimental).
  - README updates covering setup, env vars, demo steps for ntfy + TMDB.
  - Tests for core services (domain validation, encryption, SSRF guard, auth).
- **Out-of-scope (future)**
  - Redis caching implementation (stub only).
  - Discovery (Docker/CasaOS) agents.
  - LLM proxy, rate limits, audit UI, schema-driven UI forms.
  - Full Blazor UI; short-term, use minimal Razor components or placeholder.

## Acceptance Criteria
1. `dotnet run` with `JSONUI_MASTER_KEY` launches Gateway, applies migrations, and exposes `/health/live` + `/health/ready`.
2. Admin API endpoints allow creating integrations, secrets, and actions with validation errors documented.
3. Secrets are encrypted at rest; startup fails without master key; secrets never returned in responses.
4. MCP `/mcp` endpoint lists tools for current API key scopes (admin sees all).
5. MCP call executes HTTP request limited to configured integration base URL; loopback/metadata blocked by default.
6. README documents Docker compose setup and step-by-step configuration for ntfy + TMDB tools.
7. CasaOS manifest + placeholder screenshots stored under `deploy/casaos/` and marked experimental.
8. `dotnet build` and `dotnet test` succeed; smoke test instructions captured in docs.

## Security & Reliability Considerations
- Master key required; refuse startup without it.
- Secrets stored using authenticated encryption; only IDs referenced elsewhere.
- API keys hashed with salted secret (per-key) and stored with prefix for logs.
- SSRF guard resolves hostnames, validates IP in allowed ranges, denies redirects out of range.
- Minimal structured logging with redaction of headers containing secrets.
- Graceful handling when Redis disabled (feature toggles documented).

## Verification Plan
1. Run `dotnet build` at repo root (compiles all projects).
2. Run `dotnet test` targeting solution.
3. Manual smoke:
   - `dotnet run --project src/JsonUi.Gateway` with env vars.
   - Use HTTP client (e.g., `httpie`) to create secret, integration, action.
   - Call `/mcp/tools` (or equivalent) using admin API key and verify `ntfy_publish` + `tmdb_search_movie` definitions.
   - Execute `/mcp/call` (or tool-specific) and verify upstream HTTP call (use mock/stub for offline testing).
4. Optional future: container build `docker build -t jsonui-gateway .` and run compose.

## Work Breakdown
1. **Domain Layer (`JsonUi.Core`)**
   - Entities: Integration, IntegrationAllowlist, Secret, ProxyAction, ApiKey, ToolScope.
   - Value Objects: IntegrationId, ApiKeyPrefix, SecretId.
   - Interfaces: IRepository abstractions, ISecretProtector, IApiKeyHasher, ISsrfGuard, IToolExecutor.
   - Validation: methods ensuring required fields, host parsing, schema placeholders.

2. **Infrastructure Layer (`JsonUi.Infrastructure`)**
   - DbContext + entity configurations (EF Core, SQLite provider).
   - Migrations folder seeded with initial schema.
   - SecretProtector implementation using `AesGcm` and master key derived via PBKDF2.
   - ApiKeyHasher using `Rfc2898DeriveBytes` with random salt.
   - SsrfGuard using `Dns.GetHostAddressesAsync` + CIDR checks, referencing allowlists.
   - HttpToolExecutor using `HttpClientFactory`, enforcing method/path constraints, injecting auth headers from integration.
   - Repository implementations (EF-based) for Integrations, Actions, Secrets, ApiKeys.

3. **Gateway Layer (`JsonUi.Gateway`)**
   - Program setup: configuration, logging, DI, `AddDbContext`, `AddControllers`, minimal endpoints.
   - Middleware: API key auth, scope enforcement, exception handling.
   - Controllers:
     - `AdminIntegrationsController`
     - `AdminSecretsController`
     - `AdminActionsController`
     - `PlaygroundController`
     - `McpController` (list + call)
     - `HealthController`
   - DTOs + request validators (FluentValidation or manual).
   - Options binding for CasaOS-specific toggles.

4. **UI Layer (`JsonUi.Ui`)**
   - For P1, maintain placeholder; future work will integrate with admin APIs.

5. **Deployment Assets**
   - `deploy/docker/Dockerfile` (ASP.NET Core, trim runtime).
   - `deploy/docker/docker-compose.yml` referencing README sample.
   - `deploy/casaos/manifest.json` (experimental) + placeholder PNG screenshot (already have background?).
   - Document env vars, volumes, ports.

6. **Docs & Templates**
   - Update `README.md` Quickstart, Demo, Env vars.
   - Add `docs/IMPLEMENTATION_NOTES.md` capturing manual smoke steps + verification log.
   - Ensure `.github` templates mention security review fields.

7. **Testing**
   - Unit tests for domain validators.
   - Infrastructure tests for encryption + hashing + SSRF guard (CIDR checks).
   - Gateway tests for API key middleware (use `WebApplicationFactory`).

## Timeline
1. Day 0–1: Domain + Infrastructure scaffolding, migrations, secret protector, tests.
2. Day 1–2: Gateway controllers + middleware + HTTP executor.
3. Day 2: MCP endpoints + manual smoke instructions.
4. Day 3: Docker assets + CasaOS manifest + README update + final verification.

## Open Questions / Follow-ups
- Admin UI technology: keep placeholder until we add actual Blazor pages (P1b).
- Redis caching toggles: implement stub interfaces now? (Yes, add `ICacheProvider` shim returning misses.)
- Rate limiting: include ASP.NET Core rate limiting middleware now or later? (Document as follow-up.)
- Master key rotation steps: add to operations doc later.
