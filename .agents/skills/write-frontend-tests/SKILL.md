---
name: write-frontend-tests
description: Write or extend frontend tests for new or changed React components, hooks, and API client code. Use whenever UI behaviour is added or modified.
---

# Write Frontend Tests

## Before writing
1. Check `frontend/package.json` for an existing test setup (Vitest, Jest, React Testing Library, jsdom) and read any existing `*.test.tsx` files. If they exist, copy their style, helpers, and naming exactly.
2. Currently NO test setup exists, and the repo rule is that `package.json` stays clean of extra packages. So:
   - Do not install anything without the user's explicit approval. Ask first, explaining why.
   - If approved, propose Vitest + React Testing Library + jsdom as dev dependencies only (no runtime or UI packages), pinned to exact versions at least 7 days old, with the minimum setup: a `test` script, a Vite `test` block, and one setup file.
   - Add the setup as its own small commit, separate from feature work.
   - If not approved, list the manual test steps instead of writing tests.
3. The API client is `frontend/src/api.ts` (a `fetch` wrapper). Mock at that module boundary only.

## What to test
Test behaviour a user can see, not implementation details.

- **Rendering:** conversation list shows each conversation's name, last message, and time. Thread shows messages grouped under day headings.
- **Own vs others:** own messages are styled as sent (right, blue); others show the sender's name (left, white).
- **Posting:** typing and pressing Enter sends the message; Shift+Enter inserts a newline and does NOT send; empty or whitespace-only input does not send; input clears after send.
- **Optimistic update:** the message appears immediately and the list's last-message preview updates.
- **States:** loading, empty (no conversations or messages), and error (failed fetch or failed send, with retry or message shown).
- **Permissions in UI:** admin-only controls (for example, create team) are hidden or disabled for non-admins.
- **New features:** team threads appear in the list, selecting one loads its messages, and polling or real-time updates add new messages without duplicates or losing scroll position.

## Conventions
- Query by role, label, or visible text (`getByRole`, `getByText`). Avoid test IDs and CSS selectors unless there is no alternative.
- Use `userEvent` over `fireEvent` for typing and key presses.
- Mock only the network boundary. Never mock the component under test or its children.
- Use fake timers for polling tests; never real `setTimeout` sleeps.
- Each test arranges its own data; no shared mutable state between tests.
- Name tests as behaviour: `shows sender name on messages from other users`.
- Keep types strict: no `any` in tests or mocks.

## What NOT to do
- Do not test styling details, snapshots of large trees, or library internals.
- Do not add UI or component libraries.
- Do not weaken or delete a failing test to get green.

## After writing
- Run the test script and `npm run build` (includes `tsc --noEmit`) in `frontend/`. Both must pass.
- Confirm at least one test fails when the behaviour is broken (for example, remove the Enter-to-send handler mentally or in a scratch edit).
- Report: tests added, behaviours covered, and what is intentionally left to manual testing.