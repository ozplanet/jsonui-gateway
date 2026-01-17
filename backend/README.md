# JsonUI

Tools for AI integrations: schemas, UI, security, access.

JsonUI is a self-hosted-first toolkit that exposes existing HTTP APIs as MCP tools with validated inputs, strong security guardrails, and operability features (caching, discovery, auditing). A hosted option on `jsonui.ca` is planned later; the OSS core remains genuinely useful on its own.

## What JsonUI Is
- MCP gateway for local + remote APIs.
- Schema-driven input validation and UI.
- Security-first proxy execution (SSRF guardrails, encrypted secrets, scoped API keys, redacted logging).
- Admin console to manage integrations, actions, secrets, API keys, cache, discovery, LLM proxy settings.

## What JsonUI Is Not
- Not a workflow engine or orchestrator.
- Not a prompt library or "agent builder".
- Not a general-purpose cache.

## Quickstart (Planned UX)
JsonUI will run via Docker with optional Redis:

```yaml
services:
  jsonui:
    image: jsonui/jsonui-gateway:latest
    ports:
      - "8170:8080"
    environment:
      - ASPNETCORE_URLS=http://0.0.0.0:8080
      - JSONUI_MASTER_KEY=base64:REPLACE_ME_32_BYTES
      - JSONUI_DB_PATH=/app/data/jsonui.db
      - JSONUI_REDIS__ENABLED=true
      - JSONUI_REDIS__CONNECTIONSTRING=redis:6379
      - JSONUI_DOCKER_DISCOVERY__ENABLED=true
    volumes:
      - ./data:/app/data
      - /var/run/docker.sock:/var/run/docker.sock:ro # optional discovery
      - /var/lib/casaos/apps:/var/lib/casaos/apps:ro # optional CasaOS heuristics
    extra_hosts:
      - "host.docker.internal:host-gateway"

  redis:
    image: redis:7-alpine
```

## Planned v1 Demo
1. Add Sonarr integration, store secret, expose curated/manual tools.
2. Add ntfy integration with `ntfy_publish` tool.
3. Add TMDB GET tool with caching enabled and demonstrate cache stats/purge.

## Documentation Map
- Charter: `docs/00-project-charter.md`
- PRD v1: `docs/01-prd-v1.md`
- Implementation Plan: `docs/02-implementation-plan-v1.md`
- Architecture: `docs/03-architecture.md`
- Security: `docs/04-security.md`
- Operations: `docs/05-operations.md`
- ADRs: `docs/decisions/`
- Agent prompts: `docs/99-prompts/agent-prompts.md`

## Contribution Process
- Issues follow `.github/ISSUE_TEMPLATE/*` (feature / bug).
- PRs use `.github/pull_request_template.md` and must document security considerations.
- Coding agents run verification loops (plan -> implement -> cross-review -> drift audit) as defined in `docs/99-prompts/agent-prompts.md`.
