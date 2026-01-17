# ADR 0003: Secrets Encryption

## Status
Accepted

## Context
JsonUI stores upstream API credentials (Sonarr keys, ntfy tokens, OpenAI keys). Compromise would leak access to user services.

## Decision
- Encrypt all stored secrets at rest in SQLite using a master key supplied via `JSONUI_MASTER_KEY` env var.
- Refuse to start if the master key is missing.
- Provide write-only secret APIs; plaintext values are never returned.
- Use authenticated encryption (ensures tamper detection).

## Consequences
- Master key must be backed up separately; losing it means secrets cannot be recovered.
- Cold-start requires key injection via env vars/secret store.

## Verification
- Unit tests: encrypt/decrypt roundtrip; tamper detection.
- Manual verification: secret values never appear in logs/UI responses.
