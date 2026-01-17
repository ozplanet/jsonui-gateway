# Product Requirements Document: JsonUI v1

## Open Questions
1. Template connectors (Sonarr/n8n/ntfy): do we ship curated connectors in v1, or rely on manual proxy definitions and document connectors as P2? (Assumption: manual proxies in v1; curated connectors in later phase.)
2. First-time admin access: will v1 include a one-time setup workflow to create the first admin API key? (Assumption: yes, via a secure bootstrap endpoint or CLI, TBD.)

## Assumptions
- JsonUI is installable via Docker compose with optional Redis.
- Admin UI is Blazor (hosted alongside the gateway) and accessible via browser.
- Input schemas use JSON Schema 2020-12.

## Summary
JsonUI v1 allows users to expose local and remote HTTP APIs as MCP tools with schema-driven inputs, secure proxy execution, and admin operability features.

## Goals
- Provide a safe MCP tool gateway for existing services (Sonarr, ntfy, n8n, TMDB, etc.).
- Give admins an understandable UI/workflow to configure integrations and actions.
- Ensure secrets and policies are applied consistently (scopes, caching, LLM usage).

## Non-Goals
- Agent orchestration or workflow automation.
- Enterprise SSO/RBAC beyond API keys + scopes.
- Distributed caching or HA.

## Personas
- **Hobbyist Self-Hoster**: wants to expose local services securely to MCP clients without opening everything to the internet.
- **Startup Builder**: needs MCP governance + observability quickly.
- **Internal Tools Engineer**: wants reproducible, auditable tool catalogs.

## Functional Requirements
### Integrations
- CRUD for integrations (name, type, base URL, enabled, allowlist hosts/CIDRs).
- Auth modes: none, header key, bearer, basic, query key; references stored secrets.
- Discovery suggestions (Docker, optional CasaOS) to bootstrap entries.

### Actions / Proxy Tools
- CRUD for actions tied to integrations.
- Define HTTP method, path template, query/header/body templates.
- Store validated JSON Schema for inputs.
- Test action (admin) with request/response preview (redacted).
- Enable caching per action (GET-only default, TTL, vary-by scope).

### Secrets
- Create/update delete? (No delete in v1 or soft delete?).
- Store encrypted at rest; values write-only.
- Reference by identifier in actions/integrations.

### API Keys
- Create/revoke API keys.
- Show raw key once at creation.
- Assign scopes (integration/tool allowlist) and admin flag.
- Track last-used timestamp.

### Cache Management
- Opt-in caching per action.
- Admin-only endpoints: stats, purge by integration/tool, purge all.

### MCP Gateway
- `/mcp` (all enabled tools per scope) and `/mcp/{integrationId}` (scoped set).
- Tool listing respects API key scopes.
- Tool execution enforces SSRF guard, rate limiting, caching policy.

### LLM Proxy
- Optional OpenAI proxy endpoint(s).
- Server-side storage of LLM API keys.
- Model allowlist + rate limiting + metadata logging.

## Non-Functional Requirements
- Security-first posture (SSRF allowlist, secret handling, redacted logs, per-key rate limits).
- Self-hosted-first (works on homelab gear; no external dependencies beyond optional Redis).
- Clean architecture to support agent-assisted implementation.

## Acceptance Criteria
- A user can deploy JsonUI via compose, add Sonarr + TMDB integrations, call tools via MCP.
- Non-admin keys cannot access `/admin/*`; scopes prevent unauthorized tool listing/calling.
- Secrets are encrypted at rest; SSRF guard prevents metadata/loopback access by default.
- Caching works only when explicitly enabled and includes scope separation.

## Verification
- For each feature: issue template defines AC + security considerations + verification.
- Standard loops: `dotnet build`, `dotnet test`, manual smoke test of feature.

## Deferred (Stop Drift)
- AI-assisted endpoint builder (curl -> tool) is post-v1.
- Multi-tenant teams/organizations beyond simple API keys.
- Hosted control plane (jsonui.ca) beyond documentation planning.
