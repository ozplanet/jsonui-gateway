# ADR 0002: Authentication & Scopes

## Status
Accepted (v1)

## Context
JsonUI exposes powerful operations (proxying arbitrary HTTP actions, managing secrets). We need a simple, reliable auth model suitable for self-hosted deployments and future hosted offerings.

## Decision
Use API keys for all endpoints.
- Admin API: requires API key with `IsAdmin` flag.
- Non-admin keys: must specify allowlist scopes (integration IDs and/or tool names).
- Scopes default to deny.
- Rate limits enforced per API key.
- API keys stored as hashes; raw keys shown once.

## Consequences
- Straightforward for hobbyists to reason about.
- Provides a path to team/org features later.
- Requires UI/CLI for key creation and scope assignment.

## Verification
- Unit tests for API key hashing/verification.
- Tests for scope evaluation (list + call).
- Manual test: non-admin key cannot access `/admin/*`.
