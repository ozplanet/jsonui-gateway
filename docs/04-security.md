# Security

## Threat Model Summary
JsonUI executes HTTP requests based on user-supplied configuration and tool inputs. Threats include SSRF, secret leakage, privilege escalation, cross-scope data leakage via caching, and abuse/DoS of proxy endpoints.

## Security Principles
- Deny by default: no arbitrary outbound calls, no default cached responses, no unauthenticated endpoints.
- Explicit allowlists: integrations must specify allowed hosts/CIDRs.
- Secrets never appear in plaintext outside the admin UI write-only flows.
- Logs are metadata-focused with redaction of sensitive fields.
- Rate limiting per API key for proxy/MCP/LLM routes.

## SSRF Protections
1. **No full URL overrides**: tool inputs cannot provide full URLs; requests are `BaseUrl + PathTemplate` only.
2. **Allowlist-first**: each integration defines allowed hosts/CIDRs. If unspecified, default to blocking all but the BaseUrl host (recommendation: require explicit allowlists).
3. **Deny special ranges**: by default block loopback (`127.0.0.0/8`, `::1`), link-local (`169.254.0.0/16`, `fe80::/10`), metadata ranges, and other sensitive networks unless explicitly allowed.
4. **Redirect handling**: either disable following redirects or re-validate targets using the same allowlist logic.
5. **DNS safety**: resolve hostnames and ensure resolved IPs remain in allowed ranges.
6. **Protocol restrictions**: HTTP/HTTPS only in v1.

## Secret Handling
- Encrypted at rest in SQLite using a master key provided via `JSONUI_MASTER_KEY` env var.
- Application refuses to start without the master key, preventing accidental plaintext storage.
- Admin APIs provide write-only secret operations; plaintext values are never returned.
- Secrets are referenced via IDs/aliases, not embedded in config.

## Authentication & Authorization
- API key required for all routes.
- Admin APIs require `IsAdmin` flag.
- Non-admin keys have scoped access: allowlist of integration/tool IDs.
- API keys stored as hashes; raw key displayed once at creation.
- Rate limits apply per key.

## Logging & Audit
- Log metadata: route, method, status, latency, key prefix (not full key).
- Redact headers: Authorization, Cookie, Set-Cookie, X-Api-Key, plus any configured secret header names.
- Avoid logging request/response bodies by default.
- Record audit events for admin mutations with actor, action, target, timestamp.

## Caching Controls
- Cache opt-in per action; GET-only default with TTL.
- Cache key includes integration/action, method, URL (path+query), relevant headers, and caller scope to avoid cross-key leakage.
- Admin-only endpoints for stats, purge, purge-all.

## LLM Proxy Security
- Server-side storage of provider keys; clients never see them.
- Model allowlist + request size limits.
- Rate limits and metadata logging (no body logging by default).

## Acceptance Criteria (Security)
- Non-admin key cannot access `/admin/*` routes.
- A key cannot list/call tools outside its scopes.
- Secrets are never persisted/logged in plaintext; startup fails without master key.
- SSRF guard blocks loopback/link-local/metadata targets absent allowlist.
- Cache key includes caller scope to avoid leakage.
- Rate limits enforced on proxy/MCP/LLM endpoints.
