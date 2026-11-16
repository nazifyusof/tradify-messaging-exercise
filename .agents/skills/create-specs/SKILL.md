---
name: create-specs
description: Turn a feature request or idea into a short spec for one shippable unit before any code is written. Use at the start of every SU or when a new request arrives.
---

# Create Specs

Produce a one-page spec for ONE shippable unit (SU). Keep it short enough to read in two minutes. Do not write code.

## Steps
1. Read the request and the relevant existing code (models, endpoints, components) so the spec reflects what exists.
2. If the request is large, first propose a split into SUs (themes, then SUs, ordered, with reasons), then spec only the first one.
3. List open questions and state the assumption you are making for each, so work can proceed.
4. Write the spec to `specs/<short-name>.md` using the template below.

## Template

```
# <SU name>

## Goal
One or two sentences: what a user can do after this ships that they could not before.

## Why this slice, why now
Why it is first or next, what it unlocks, what risk it reduces.

## In scope
- Bullet list of behaviour included.

## Out of scope (deferred)
- What is deliberately left for later SUs, with one-line reasons.

## Design
- Data model changes (entities, fields, relationships, company scoping).
- API: method, route, request, response, status codes, authorization rule.
- UI: screens or components touched, reused components, states (loading, empty, error).

## Acceptance criteria
- Given / When / Then statements, testable and specific.
- Include tenant isolation and permission cases.

## Test plan
- Backend tests to add. Frontend checks. Manual smoke steps.

## Rollout
- Feature flag name (if any), safe to deploy daily?, rollback approach.

## Open questions and assumptions
- Question -> assumption used for now.
```

## Rules
- One SU per spec. If it will not ship in a day or two, split it.
- Prefer the thinnest vertical slice that delivers visible value.
- Reuse existing components and patterns before proposing new ones.
- Every acceptance criterion must be verifiable by a test or a manual step.
- Keep it under about one page; cut anything the implementer does not need.