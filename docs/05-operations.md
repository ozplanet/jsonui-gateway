# Operations

## Deployment Model
- Primary deployment: Docker container running JsonUI gateway + admin UI.
- Optional: Redis container for caching.
- Optional read-only mounts: `/var/run/docker.sock` for discovery; `/var/lib/casaos/apps` for CasaOS compose inspection.

## Installation Steps (v1 target)
1. Prepare a persistent data directory (e.g., `./data`).
2. Generate a 32-byte master key (base64) and set `JSONUI_MASTER_KEY` environment variable.
3. Deploy via docker-compose as outlined in `README.md`.
4. On first run, perform admin bootstrap (mechanism TBD: CLI vs. local endpoint) to create initial admin API key.

## Configuration (Env Vars)
- `ASPNETCORE_URLS`: binding address.
- `JSONUI_MASTER_KEY`: required.
- `JSONUI_DB_PATH`: default `/app/data/jsonui.db`.
- `JSONUI_REDIS__ENABLED`: true/false.
- `JSONUI_REDIS__CONNECTIONSTRING`: e.g., `redis:6379`.
- `JSONUI_DOCKER_DISCOVERY__ENABLED`: true/false.
- `JSONUI_CASAOS_DISCOVERY__ENABLED`: true/false.
- `JSONUI_RATE_LIMITS__ENABLED`: true/false.
- `JSONUI_OPENAI__ENABLED`: true/false (LLM proxy).
- `JSONUI_OPENAI__SECRET_ID`: reference to stored secret.
- `JSONUI_OPENAI__MODEL_ALLOWLIST`: comma-separated models.

## Backup / Restore
- Backup SQLite DB file in `/app/data` plus master key (store separately).
- Restore by copying DB file and providing the same master key env var.
- Without the master key, encrypted secrets cannot be recovered.

## Upgrades
- Pull new image, restart container; app runs migrations automatically.
- Ensure backup before upgrades.

## Health Monitoring
- `/health/live`: process up.
- `/health/ready`: DB accessible, Redis reachable (if enabled), master key present.

## Troubleshooting
- **Startup failure**: check `JSONUI_MASTER_KEY` is set and `/app/data` writable.
- **Readiness failing**: DB lock issues, Redis unreachable, missing master key.
- **Discovery empty**: ensure docker.sock/casaos mount exists and container user has permission.
- **Admin UI errors**: check logs (redacted) and audit trail.

## Operational Safety Tips
- Avoid exposing JsonUI publicly; prefer Tailscale/wireguard for remote access.
- Restrict docker.sock mount to read-only and only when discovery needed.
- Regularly rotate API keys and secrets; document procedure for master key rotation (future).
