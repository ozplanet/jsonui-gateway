# JsonUI Project Charter

## Open Questions
1. **SSRF defaults**: Should private RFC1918 ranges be allowed automatically for "local" integrations, or must each integration explicitly allow them? (Recommendation: require explicit allowlisting per integration; provide presets.)
2. **MCP SDK**: Use the official .NET MCP SDK (2025-11-25 spec) immediately, or build our own HTTP transport wrapper first? (Recommendation: use the official SDK, wrapped by our adapter.)
3. **Admin bootstrap**: How should the first admin API key be provisioned securely (one-time setup endpoint vs. CLI vs. log output)?

## Assumptions
- JsonUI is self-hosted-first, Docker-first, with Windows users running via Docker Desktop.
- SQLite is the authoritative store; Redis is optional for caching.
- Tailscale integration is optional and disabled by default.
- Hosted offering (jsonui.ca) builds on the OSS core without paywalling fundamentals.

## Vision
JsonUI makes AI integrations predictable, inspectable, and safe by wrapping existing HTTP APIs as MCP tools with schema-driven inputs, security controls, and operational guardrails.

## Positioning
- JsonUI is infrastructure, not an orchestrator or workflow engine.
- It provides tools for AI systems: MCP gateway, schema-driven UI, secure proxy, caching, discovery, policy-focused LLM proxy.

## Target Users
- Homelab builders (CasaOS/Unraid/Proxmox) wanting safe AI tool exposure.
- Startups using MCP that need governance, security, and observability without building the stack themselves.
- Internal tools teams that want a catalog of validated tools + auditability.

## Non-Goals (v1)
- Workflow orchestration or agent planning.
- Multi-tenant enterprise RBAC beyond scoped API keys.
- Distributed cache/HA clusters.
- Auto-publishing AI-generated endpoints without human review.

## Scope (v1)
- MCP endpoint(s) for aggregated and per-integration tool lists.
- Admin API + Blazor UI for managing integrations/actions/secrets/api keys/cache/discovery.
- Proxy execution engine with deterministic templates and SSRF guardrails.
- Secret encryption at rest (master-key gated startup).
- Redis-backed caching (opt-in, GET-only default, TTL-based) with admin purge/stats.
- Discovery (Docker socket optional; CasaOS compose optional) for onboarding local services.
- LLM proxy (OpenAI-first) with key storage, allowlists, and rate limits.

## Lifecycle Coverage
- Intake: capture integrations and dependencies via specs.
- Planning: status tracking in `projects/project-root.md` + this charter.
- Execution: admin UI + audit logging.
- Review: Drift audits + verification loops.
- Closeout: finish v1 scope, prepare hosted path.

## Definition of Done (v1)
A feature is "done" only if:
- Works in Docker with documented steps for Linux + Windows (Docker Desktop).
- Enforces security constraints: encrypted secrets, SSRF guard, scoped API keys, rate limits, logging redaction.
- Has clear acceptance criteria + verification loop (build/test/manual smoke).
- Includes documentation updates (README, ops, security) when relevant.
- Adds/updates tests for security-critical logic where feasible.
