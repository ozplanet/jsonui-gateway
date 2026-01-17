# Agent Prompts (Ordered Chain)

## Prompt 0: Planning
Use when translating a requirement into an actionable issue.

"""
You are a staff/principal engineer working on JsonUI.
Given the requirement, produce:
- Goal
- In-scope vs. out-of-scope (stop drift)
- Acceptance criteria
- Security considerations (SSRF, secrets, auth/scopes, redaction, rate limiting)
- Verification loop (dotnet build/test + manual steps)
- Suggested file paths
Requirement:
<PASTE>
"""

## Prompt 1: Implementation (Loop)
Use when coding an issue.

"""
You are implementing a JsonUI feature.
Rules:
- Follow clean architecture boundaries.
- Do not log secrets.
- Enforce SSRF allowlists.
- Respect scopes.
After each meaningful change run:
  dotnet build
  dotnet test
Deliver: summary of changes, safety considerations, testing performed.
Issue:
<LINK>
"""

## Prompt 2: Cross-Model Review
Use after implementation before merge.

"""
Review this PR for JsonUI.
Check:
- SSRF/redirect/DNS issues
- Secret handling/log redaction
- Auth/scope enforcement
- Cache key safety
- Rate limiting coverage
- Boundary violations (Core vs Infrastructure)
Output: blockers, suggestions, missing tests.
PR:
<LINK>
"""

## Prompt 3: Drift Audit
Use weekly or before release.

"""
Audit JsonUI repo against charter, PRD, implementation plan.
Identify:
- Out-of-scope features
- Missing acceptance criteria/verification
- Untested security requirements
- Stale docs
Output: drift findings + corrective actions.
"""

## Prompt 4: Release Readiness
Use before tagging a release.

"""
Evaluate release readiness for JsonUI.
Confirm:
- docker-compose quickstart works
- health endpoints ok
- secrets encryption enforced
- SSRF guard + scopes + rate limits enforced
- cache admin endpoints restricted
- docs updated (operations/security)
Output: Go/No-Go + remaining tasks.
"""
