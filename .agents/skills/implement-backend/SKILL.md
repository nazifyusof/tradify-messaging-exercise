---
name: implement-backend
description: Implement the backend (.NET API) tasks of a spec for one shippable unit. Use when asked to build or change API endpoints, models, or services.
---

# Implement Backend

Implement the **Backend** tasks from the SU's spec in `specs/`. If there is no spec, stop and ask for one (or run create-specs first).

## Before coding
1. Read the spec: goal, in scope, out of scope, API contract, acceptance criteria. Do not build anything listed as out of scope.
2. Read `AGENTS.md`, `docs/ARCHITECTURE.md`, and the existing API code: controllers and primary-constructor DI, DTO records beside the controller, `AuthStub.GetUserId` for `X-User-Id`, repository interfaces and in-memory implementations in `Tradify.Core`, registrations in `Program.cs`, seed data in `Tradify.Seed`.
3. Plan the change in a few lines and list the files you will touch. Keep the diff small.

## Rules
- Follow the existing structure and naming. Reuse existing helpers, models, and services before adding new ones.
- Keep persistence behind the repository interfaces in `backend/src/Tradify.Core`. No EF Core, database, or Docker dependencies. Keep seed data synthetic.
- **Scoping:** every read and write is limited to what the caller may see (assigned job, team membership). There is no company model yet; once a spec introduces one, also scope every query by the caller's company.
- **Never trust IDs from the request body.** The sender of a message is always the `X-User-Id` caller; a client-supplied user, sender, or company ID is ignored (the current `senderId` passthrough is a known spoofing gap, not a pattern to copy).
- **Authorization:** check the caller may access the resource (for example, is a member of the team), not just that they are signed in. Internal staff or team threads must never be visible to customer users (U4, U5 in seed data).
- **Status codes:** 401 unauthenticated (missing or unknown user), 403 signed-in but not allowed, 404 not found (do not leak that a resource exists outside the caller's scope), 400 invalid input, 201 on create. Note: existing message endpoints return 403 for a missing header; that is a known gap — new endpoints return 401.
- Validate inputs: empty or whitespace text, missing IDs, length limits.
- Shared in-memory state must be thread-safe (use the project's existing pattern, a lock, or a concurrent collection).
- Do not log message bodies or other user content (the current `POST` logs the body; that is a known gap, not a pattern to copy).
- Additive changes only where possible: do not change existing endpoint contracts or response shapes that the frontend already uses. If a breaking change is unavoidable, say so and ask.
- Unfinished or risky behaviour goes behind the feature flag named in the spec.
- No new packages unless the spec calls for it.

## Steps
1. Models, repository interfaces, and in-memory implementations in `Tradify.Core`; seed changes in `Tradify.Seed`; DI registrations in `Program.cs`.
2. Controllers and DTOs matching the contract in the spec exactly.
3. Tests: use the `write-backend-tests` skill for the happy path, unauthenticated, non-member, customer-isolation, validation, and not-found cases (cross-company once a company model exists).
4. Run `dotnet test backend/Tradify.sln`. Fix failures; never weaken a test to pass. The existing tests must stay green.
5. If the change touched endpoints, auth rules, models, repository interfaces, or storage, update `docs/ARCHITECTURE.md` in the same change (see `AGENTS.md`). Note any deviation from the spec's contract in the spec's Design section.

## Output
Reply with: files changed, endpoints added or changed (method, route, status codes), tests added, anything deferred, and any deviation from the spec.