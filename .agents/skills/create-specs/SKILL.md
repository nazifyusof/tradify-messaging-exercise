---
name: create-specs
description: Plan a feature into shippable units (SUs) or write a short spec for one SU, before any code is written. Use at the start of a feature, before each SU, and when a new request changes the plan.
---

# Create Specs

Plan work as small shippable units (SUs) and spec ONE SU at a time. Do not write code.

## Team context (apply to every plan and spec)
- We work in 6-week cycles on one bigger idea, similar to Shape Up. An SU is a thin slice of that idea, not the whole idea.
- We release to production at least daily, and the engineer who builds an SU drives its release. If an SU cannot be released on its own in a day or two, it is too big: split it.
- We break work down to derisk it technically and from a product view, and to make progress visible. Every SU states the risk it reduces and what a person can see or do afterwards.
- Unfinished work must be safe to ship (behind a feature flag, or additive and unused).
- Scale assumption: 20,000 companies, 60,000 active users, 1 to 12 staff per company. Prefer the simplest design that holds at this size; do not propose heavy infrastructure.
- Constraints: follow the stack and conventions in `AGENTS.md`. No new UI packages. Reuse existing components and endpoints before adding new ones.

## File layout
All files live in `docs/features/<feature>/` (for example `docs/features/teams/`).
- `plan.md`: the feature plan, the single source of truth for order and status.
- `NN-<su-name>.md`: one spec per SU, zero-padded, numbered in creation order (`01-teams-api.md`, `02-team-conversation.md`). Never renumber or rename existing files; `plan.md` owns the real order.
- Create a spec file only when that SU is about to be built. No placeholder files for later SUs.

## Modes
Choose one mode per run.

- **Plan mode** (default when the request is large or new): write `plan.md` only. Do not write any SU spec.
- **Spec mode** (when asked to spec a named SU, or once the plan is agreed): write the next numbered spec file for that ONE SU using the template. Update its row in `plan.md`.
- **Lite spec:** for an SU that fits in a few hours, use only Goal, In scope, Out of scope, API contract (if any), Acceptance criteria, Tasks, Test plan.
- **Re-slice mode** (when a new request changes the plan): update `plan.md` with what is reused, moved, dropped, and the new next SU, and add a dated line to the Re-slice log. Spec only the new next SU.

## Steps
1. Read the request, `AGENTS.md`, `plan.md` if it exists, and the relevant existing code (models, endpoints, components) so the output reflects what already exists.
2. Choose the mode above. Never write detailed specs for more than one SU at a time.
3. List open questions. For each, state the assumption you will proceed with, so work is not blocked.
4. Write the file(s).
5. Reply with a 3-5 line summary: the mode used, the files written, the next SU, and any assumption the person should confirm.

## plan.md template

```
# <Feature> plan

## Goal
What the feature should achieve for users, in two or three sentences.

## Roles and assumptions
- Roles: who the actors are (for example admin, field worker, end customer) and what each can do.
- Key assumptions, each with the question it answers.

## Themes and SUs
| # | SU | Spec | Visible outcome | Risk reduced | Depends on | Status |
|---|----|------|-----------------|--------------|------------|--------|
| 1 | ... | `01-....md` or (one-liner only) | ... | ... | none | Not started / In progress / Shipped |

Order rationale: one line per SU on why it sits where it does.

## Deferred
- What is deliberately left out, with one-line reasons.

## Open questions and assumptions
- Question -> assumption used for now.

## Re-slice log
- <date>: what changed, what was reused, moved or dropped, and why.
```

## SU spec template

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
What someone can demo or observe after release.

## In scope
- Behaviour included.

## Out of scope (deferred)
- What is left for later SUs, with one-line reasons.

## Design
- Data model: entities, fields, relationships, company scoping.
- UI: screens or components touched, existing components reused, states (loading, empty, error).

## API contract
The single source of truth for backend and frontend.
- Method and route, request shape, response shape, status codes, authorization rule.

## Acceptance criteria
- Given / When / Then, specific and testable.
- Include tenant isolation (cross-company) and permission (non-member) cases.

## Tasks
### Backend
- [ ] ...
### Frontend
- [ ] ...
### Tests and checks
- [ ] ...

## Test plan
- Backend tests to add, frontend checks, manual smoke steps.

## Rollout
- Feature flag name (or why none is needed), safe to deploy daily, rollback approach.
- Size estimate: fits in about one day? If not, split.

## Open questions and assumptions
- Question -> assumption used for now.
```

## Rules
- One SU per spec file; `plan.md` covers all SUs at one line each.
- Prefer the thinnest vertical slice that delivers visible value.
- Backend-only or additive SUs are fine if they are safe to release and unlock the next slice.
- Tackle the riskiest unknown early only if it does not block value; otherwise isolate it behind a flag and ship value first.
- Define the API contract before the tasks, so backend and frontend build to the same shapes.
- Every acceptance criterion must be verifiable by a test or a manual step.
- Keep each spec to about one page; cut anything the implementer does not need.
- When the plan changes, update `plan.md` first, then the spec.