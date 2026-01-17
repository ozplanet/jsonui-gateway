# ADR 0004: Caching Policy

## Status
Accepted

## Context
Caching improves responsiveness and reduces rate-limit load (e.g., TMDB/TVDB metadata). However, caching can leak data across users if not scoped correctly.

## Decision
- Caching is opt-in per action.
- Only GET requests are cached by default.
- TTL-based expiration; no stale-while-revalidate or negative caching in v1.
- Cache key includes integration ID, action ID/tool slug, HTTP method, full URL (path+query), relevant headers, and caller scope (so caches are per API key scope by default).
- Admin-only cache endpoints: stats, purge (by integration/tool), purge-all.

## Deferred
- Advanced caching features (ETag revalidation, SWR, warming, distributed cache) are post-v1.

## Verification
- Unit tests for cache key generation and scope isolation.
- Manual test: enabling cache on TMDB action reduces repeated upstream calls.
