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

## Homelab Setup (Step-by-Step)
You do not need to be a professional admin to run JsonUI. Follow these steps and copy/paste the commands as-is. Anything that looks like `UPPER_CASE_TEXT` is something you choose (for example, the master key or API URLs).

### 1. Prepare folders and a master key
```bash
mkdir -p ~/jsonui/data ~/jsonui/logs
head -c 32 /dev/urandom | base64 > ~/jsonui/master.key
```
Open `~/jsonui/master.key` and copy the line for later. JsonUI will refuse to start without this key.

### 2. Create a docker-compose file
Save the snippet below as `~/jsonui/docker-compose.yml`. It exposes JsonUI on port `8170`, stores the SQLite database + logs under `~/jsonui/data`, and enables optional Redis caching.
```yaml
services:
  jsonui:
    image: jsonui/jsonui-gateway:latest
    container_name: jsonui
    restart: unless-stopped
    ports:
      - "8170:8080"
    environment:
      - ASPNETCORE_URLS=http://0.0.0.0:8080
      - JSONUI_MASTER_KEY=$(cat ~/jsonui/master.key)
      - JSONUI_DB_PATH=/app/data/jsonui.db
      - JSONUI_LOGGING_CONFIG_PATH=/app/data/logging.settings.json
    volumes:
      - ~/jsonui/data:/app/data
      - ~/jsonui/logs:/app/data/logs

  redis:
    image: redis:7-alpine
    restart: unless-stopped
```
Start it:
```bash
cd ~/jsonui
docker compose up -d
```
Check logs once to capture the one-time admin key:
```bash
docker logs jsonui | grep "Initial admin key"
```
Copy the base64 key that appears—this is your admin API key.

### 3. Talk to the Admin API
Use `curl` (already on most systems). Replace `ADMIN_KEY` with the one you copied.

#### 3.1 Create a secret (example: TMDB token)
```bash
curl -s -X POST http://localhost:8170/admin/secrets \
  -H "X-Api-Key: ADMIN_KEY" \
  -H "Content-Type: application/json" \
  -d '{"name":"tmdb","value":"TMDB_TOKEN_HERE"}'
```
The response shows the `id`. Save it as `TMDB_SECRET_ID`.

#### 3.2 Create an integration
```bash
curl -s -X POST http://localhost:8170/admin/integrations \
  -H "X-Api-Key: ADMIN_KEY" \
  -H "Content-Type: application/json" \
  -d '{
        "name":"tmdb",
        "baseUrl":"https://api.themoviedb.org/3",
        "authMode":"header",
        "enabled":true,
        "secretId":"TMDB_SECRET_ID",
        "allowlists":[{"value":"api.themoviedb.org","kind":"host"}]
      }'
```
Copy the returned `id` → `TMDB_INTEGRATION_ID`.

#### 3.3 Add an action (tool)
```bash
curl -s -X POST http://localhost:8170/admin/actions \
  -H "X-Api-Key: ADMIN_KEY" \
  -H "Content-Type: application/json" \
  -d '{
        "integrationId":"TMDB_INTEGRATION_ID",
        "name":"tmdb_search_movie",
        "method":"GET",
        "pathTemplate":"/search/movie",
        "jsonSchema":"{\"type\":\"object\",\"properties\":{\"query\":{\"type\":\"string\"}}}",
        "enabled":true,
        "cacheEnabled":true,
        "cacheTtlSeconds":300
      }'
```
Now any MCP client using the admin key can list/call this tool via `GET /mcp/tools` and `POST /mcp/call/{actionId}`.

### 4. Configure logging without touching the filesystem
Default behavior writes compact JSON logs to `/app/data/logs/jsonui.log` (mapped to `~/jsonui/logs`). You can change level, file path, or enable Application Insights via the Admin API.

- Read current settings:
```bash
curl -s http://localhost:8170/admin/logging -H "X-Api-Key: ADMIN_KEY"
```
- Update (example bumps level to `Debug` and turns on App Insights):
```bash
curl -s -X PUT http://localhost:8170/admin/logging \
  -H "X-Api-Key: ADMIN_KEY" -H "Content-Type: application/json" \
  -d '{
        "minimumLevel":"Debug",
        "file":{"enabled":true,"path":"/app/data/logs/jsonui.log"},
        "applicationInsights":{
          "enabled":true,
          "connectionString":"InstrumentationKey=YOUR_KEY"
        }
      }'
```
JsonUI persists the change to `/app/data/logging.settings.json` so it survives restarts.

### 5. Import tools from a Swagger / OpenAPI file
If the API you want already ships a Swagger file (often named `openapi.json`), you can upload it and JsonUI will create one tool per endpoint.

```bash
curl -s -X POST http://localhost:8170/admin/integrations/TMDB_INTEGRATION_ID/swagger \
  -H "X-Api-Key: ADMIN_KEY" \
  -F file=@/path/to/openapi.json
```
Every `GET` path becomes a cached tool by default; other methods are created without caching.

### 6. Keep it safe
- Never check `~/jsonui/master.key` or `/app/data/logs/*.log` into git.
- Rotate the admin key by creating a new one (future UI will simplify this). For now, run a migration/seed script or regenerate manually after clearing the API key table.
- Use CasaOS or Portainer to monitor the container; it only needs ~200MB RAM for the base slice.
- Take regular backups (`~/jsonui/data/jsonui.db` and the master key) before upgrading.

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

## Syncing Private Repos
Use `push-private-repos.sh` (in `project-015/`) to refresh the Forgejo SSH host key and push all private repos (`business-backend`, `jsonui-ui`, `business-docs`) after a restart:
```bash
cd /home/data/Projects/projects/Business/project-015
./push-private-repos.sh
```
The script removes stale `[ozserver]:222` entries from `~/.ssh/known_hosts`, re-adds the current host key, and pushes each repo’s current branch if the working tree is clean.
