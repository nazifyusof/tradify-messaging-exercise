---
name: create-specs
description: Turn a feature request or idea into a short spec for one shippable unit (SU) before any code is written. Use at the start of every SU or when a new request arrives.
---

# Create Specs

Produce a one-page spec for ONE shippable unit (SU). Keep it readable in two minutes. Do not write code.

## Team context (apply to every spec)
- We work in 6-week cycles on one bigger idea, similar to Shape Up. An SU is a thin slice of that idea, not the whole idea.
- We release to production at least daily, and the engineer who builds an SU drives its release. If an SU cannot be released on its own in a day or two, it is too big: split it.
- We break work down to derisk it technically and from a product view, and to make progress visible. Every SU must say which risk it reduces and what a person can see or do afterwards.
- Unfinished work must be safe to ship (behind a feature flag, or additive and unused).
- Scale assumption: 20,000 companies, 60,000 active users, 1 to 12 staff per company. Prefer the simplest design that holds at this size; do not propose heavy infrastructure.
- Constraints: follow the existing stack and conventions in `AGENTS.md`. No new UI packages. Reuse existing components and endpoints before adding new ones.

## Steps
1. Read the request and the relevant existing code (models, endpoints, components) so the spec reflects what already exists.
2. If the request is large, first propose a split: themes, then SUs, ordered, each with a one-line reason for its position. Then spec only the first SU unless asked otherwise.
3. List open questions. For each, state the assumption you will proceed with, so work is not blocked.
4. Write the spec to `specs/<short-name>.md` using the template below.

## Template

```
# <SU name>

## Goal
One or two sentences: what a user can do after this ships that they could not before.

## Why this slice, why now
Why it is first or next, what it unlocks.

## Risk reduced
- Technical risk: ...
- Product risk: ...

## Visible outcome
What someone can demo or observe after release (this is how we show progress).

## In scope
- Behaviour included.

## Out of scope (deferred)
- What is left for later SUs, with one-line reasons.

## Design
- Data model: entities, fields, relationships, company scoping.
- API: method, route, request, response, status codes, authorization rule.
- UI: screens or components touched, existing components reused, states (loading, empty, error).

## Acceptance criteria
- Given / When / Then, specific and testable.
- Include tenant isolation (cross-company) and permission (non-member) cases.

## Test plan
- Backend tests to add, frontend checks, manual smoke steps.

## Rollout
- Feature flag name (or why none is needed), safe to deploy daily, rollback approach.
- Size estimate: fits in about one day? If not, split.

## Open questions and assumptions
- Question -> assumption used for now.
```

## Rules
- One SU per spec. Prefer the thinnest vertical slice that delivers visible value.
- Backend-only or additive SUs are fine if they are safe to release and unlock the next slice.
- Order SUs so the riskiest unknown is tackled early only if it does not block value; otherwise isolate it behind a flag and ship value first.
- Every acceptance criterion must be verifiable by a test or a manual step.
- Cut anything the implementer does not need; keep to about one page.
- When a new request changes the plan, produce a short "re-slice" note: what is reused, what moves up or down, what is dropped, and the next SU.

## Modes
- **Plan mode** (default when the request is large or new): write `specs/plan.md` only.
  Contents: themes, an ordered SU table (goal, visible outcome, risk reduced, depends on),
  and what is deferred and why. Do not write detailed specs for any SU.
- **Spec mode** (when asked to spec a named SU, or after the plan is agreed):
  write `specs/<su-name>.md` for that ONE SU using the template below.
  Update its row in `specs/plan.md` if scope or order changed.
- **Lite spec:** for an SU that fits in a few hours, use only Goal, In scope,
  Out of scope, Acceptance criteria, Test plan.
- **Re-slice mode** (when a new request changes the plan): update `specs/plan.md`
  with what is reused, moved, dropped, and the new next SU. Spec only that SU.

## Steps
1. Read the request and the relevant existing code so the plan reflects what exists.
2. Choose the mode above. Never write detailed specs for more than one SU at a time.
3. List open questions with the assumption used for each, so work is not blocked.
4. Write the file(s) and summarise the plan or spec in 3-5 lines in your reply.