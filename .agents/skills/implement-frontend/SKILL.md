---
name: implement-frontend
description: Implement the frontend (React + TypeScript) tasks of a spec for one shippable unit. Use when asked to build or change UI components, screens, hooks, or API client code.
---

# Implement Frontend

Implement the **Frontend** tasks from the SU's spec in `docs/features/<feature>/`. If there is no spec, stop and ask for one (or run create-specs first).

## Before coding
1. Read the spec: goal, in scope, out of scope, UI notes, API contract, acceptance criteria. Do not build anything listed as out of scope.
2. Read `AGENTS.md`, `docs/ARCHITECTURE.md`, and the existing frontend: `api.ts` (typed fetch wrapper that sets `X-User-Id`), `session.ts`, `format.ts`, and the components `App`, `Login`, `Messenger` (state owner), `ConversationList`, `JobThread`.
3. Confirm the backend endpoints in the contract exist (or are mocked). Build against the contract; do not invent fields.
4. If the spec adds a new API route prefix (anything other than `/auth` or `/jobs`), it must also be added to the dev proxy in `frontend/vite.config.ts`, or calls fail in dev.
5. Plan the change in a few lines and list the files you will touch.

## Rules
- TypeScript strict: no `any`. Types for API responses live in `api.ts` and match the contract.
- No new UI or state libraries. Use React state and hooks, and keep state as simple as possible. `package.json` must stay clean.
- **Reuse** existing components (chat bubbles, conversation list, composer) and extend them; do not duplicate them.
- Keep all network calls in `api.ts`, not scattered through components.
- Handle every state: loading, empty, error (with a way to retry or a clear message), and success.
- Preserve existing behaviour: Enter sends, Shift+Enter adds a newline, own messages on the right in blue, others on the left with the sender's name, day headings.
- Admin-only controls are hidden or disabled for non-admins; the backend still enforces permissions.
- Polling or real-time updates must not duplicate messages, lose scroll position, or leak timers (clean up on unmount).
- Unfinished UI goes behind the feature flag named in the spec.
- Keep components small and readable; match the surrounding code style.

## Steps
1. Types and API client functions for the contract.
2. Components and hooks, reusing existing ones.
3. Wire into the existing screens, behind the flag if specified.
4. Tests: use the `write-frontend-tests` skill. No test setup exists today — do not install one without approval; test pure logic you added and list manual steps otherwise.
5. Run `npm run build` in `frontend/` (it runs `tsc --noEmit` first). Fix failures.
6. If the change added a top-level component, page, or data-flow pattern, update `docs/ARCHITECTURE.md` in the same change (see `AGENTS.md`). Note any contract mismatch found in the spec's Design section.

## Output
Reply with: files changed, components added or reused, states handled, tests added, anything deferred, and any deviation from the spec or contract.