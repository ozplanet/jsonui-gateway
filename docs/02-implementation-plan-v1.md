# Implementation Plan v1 (Spec-Kit)

## Open Questions
1. **SSRF defaults**: Should JsonUI block RFC1918 ranges unless explicitly allowed per integration? (Recommendation: yes; require allowlisting with presets for homelab ranges.)
2. **MCP SDK integration**: Do we rely on the official MCP SDK for ASP.NET Core in v1 or implement minimal HTTP transport ourselves? (Recommendation: use the SDK but wrap it with our interfaces so we can swap if needed.)
3. **Admin bootstrap**: How is the initial admin API key created securely? (TBD: CLI vs. local-only setup endpoint.)

## Assumptions
- No implementation code during scaffold; only documentation and placeholders.
- .NET 8 solution will be created later following this plan.
- Windows support via Docker Desktop; Linux is primary host.
- SQLite is authoritative datastore; Redis optional for caching.
- Tailscale integration is optional and disabled by default.

## Principles
- Clean architecture dependency direction (Core -> Infrastructure -> Gateway/Ui).
- Security controls (SSRF, secrets, scopes, rate limits, logging redaction) are non-negotiable.
- Prefer deterministic, inspectable behavior.
- Enforce verification loops (build/test/manual smoke) during implementation.

## Repository Structure (planned)
- `README.md` (overview + quickstart + demo description).
- `docs/` for charter, PRD, implementation plan, architecture, security, operations, ADRs, prompts.
- `.github/` issue templates + PR template.
- `src/JsonUi.Gateway/README.md` (placeholder, will later host ASP.NET Core minimal API).
- `src/JsonUi.Core/README.md` (domain/contracts placeholder).
- `src/JsonUi.Infrastructure/README.md` (infrastructure placeholder).
- `src/JsonUi.Ui/README.md` (Blazor UI placeholder).
- `tests/README.md` (testing strategy placeholder).

## Modules & Responsibilities (spec)
### JsonUi.Core
- Domain models (Integration, ProxyAction, Secret, ApiKey, AuditEvent, CachePolicy).
- Interfaces for repositories, secret protector, API key service, SSRF guard, proxy executor, cache, discovery service.

### JsonUi.Infrastructure
- EF Core/SQLite implementations, encryption, hashing, Redis caching, SSRF guard, discovery, HTTP client factory.

### JsonUi.Gateway
- ASP.NET Core host with API key auth, rate limiting, admin API routes, MCP endpoints, health endpoints.

### JsonUi.Ui
- Blazor admin UI, schema-driven forms, tool playground.

## Data Model (spec)
Tables (planned):
- Integrations (allowlist hosts/cidrs, base URL, enabled, metadata).
- IntegrationAuth (auth mode, header/query names, secret references).
- ProxyActions (method, path template, templates JSON, schema JSON, cache policy, enabled).
- Secrets (encrypted value + metadata).
- ApiKeys (prefix, hash, scopes, admin flag, revoked/lastUsed, created).
- AuditEvents (actor, action, target, timestamp, metadata JSON).

## API Surface (planned)
- `/health/live`, `/health/ready`.
- `/mcp` (aggregate) + `/mcp/{integrationId}` routes (managed by MCP SDK).
- `/admin/*` for Integrations, Actions, Secrets, API Keys, Cache, Discovery, Tests.
- `/proxy/openai/*` for LLM proxy (optional).

## Phased Roadmap (P0-P5)
### P0 — Foundation
Outcome: repo scaffold + docs + standards.
- Deliverables:
  - Document set (charter, PRD, plan, architecture, security, operations, ADRs, prompts).
  - Placeholder module READMEs.
  - Issue/PR templates.
- Verification: manual doc review.

### P1 — MVP Gateway
Outcome: create integration/action via UI, call via MCP.
- Epics: Admin CRUD, Proxy engine + SSRF guard, MCP dynamic tools, basic UI.
- Verification: `dotnet build/test`, manual tool call demo, security smoke.

### P2 — Discovery + Cache
Outcome: faster onboarding + GET caching.
- Epics: Docker/CasaOS discovery, Redis caching + admin endpoints, UI polish.
- Verification: integration tests for caching; manual discovery/test action.

### P3 — Schema->UI + LLM Proxy
Outcome: schema-driven forms + policy-driven LLM proxy.
- Epics: JSON Schema -> Blazor forms, UI hints, LLM proxy allowlist/rate limits.
- Verification: tests for schema validation + LLM policy.

### P4 — Tailscale + SDKs
Outcome: secure remote access + cross-platform consumption.
- Epics: Documented Tailscale pattern, contracts repo + SDKs (.NET, JS).
- Verification: integration tests + sample clients.

### P5 — Hosted jsonui.ca
Outcome: hosted offering without paywalling OSS fundamentals.
- Epics: managed runtime, usage analytics, team access, support flows.
- Verification: operational runbooks + SLA coverage.

## Security Requirements (testable)
- SSRF guard denies loopback/link-local/metadata unless allowlisted.
- No arbitrary full URL overrides.
- Secrets encrypted at rest; plaintext never logged or returned.
- Admin endpoints require admin API key.
- Tool list/call respects scopes.
- Rate limiting on proxy/MCP/LLM endpoints.
- Cache keys vary by scope and avoid storing secrets.

## Verification Loops
- Each issue uses feature template specifying AC + security considerations.
- Implementation prompt mandates build/test after changes.
- Cross-model review prompt focuses on SSRF, secrets, scopes, caching, logging.
- Drift audit prompt ensures docs + repo stay aligned.
