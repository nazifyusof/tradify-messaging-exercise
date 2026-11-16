---
name: review-pr
description: Review the current branch's diff before committing or opening a PR. Use when asked to review changes, check my work, or before any commit.
---

# Review PR

Review the diff against the base branch (`git diff main...HEAD`, plus uncommitted changes). Report findings only; do not edit code unless asked.

## Steps
1. Read the diff and summarise the intent of the change in one sentence.
2. Check each area below and note anything that fails.
3. Output the report in the format at the bottom.

## Checklist

**Scope**
- Is this one small shippable unit? Flag unrelated changes or refactors.
- Can it ship on its own without breaking existing behaviour? Is unfinished work behind a flag?

**Security and access**
- Every query and endpoint is scoped to what the caller may see (job assignment or team membership today; also company once that concept exists). Staff-only and team threads are never exposed to customers (U4, U5).
- The caller is resolved from `X-User-Id`; the user or sender is never taken from the request body.
- Authorization is checked, not just authentication (for example: is the user a member of this team or job?).
- Correct status codes: 401 missing or unknown user, 403 not allowed, 404 unknown resource.
- Message bodies and other personal data are not written to logs.

**Correctness**
- Validation on inputs (empty messages, missing IDs, oversized text).
- Edge cases: empty lists, duplicates, removing the last member, ordering of messages.
- No race conditions on shared in-memory state (locking or thread-safe collections).

**Tests**
- New behaviour has tests, including a non-member case and a customer-cannot-access case (plus cross-company once companies exist).
- `dotnet test backend/Tradify.sln` and `npm run build` (in `frontend/`) pass.
- Existing tests still make sense and were not weakened to pass.

**Frontend**
- TypeScript types are accurate (no `any`), no new UI packages.
- Loading, empty, and error states are handled.
- Existing components are reused rather than duplicated.

**Hygiene**
- No debug logging, commented-out code, TODOs without a reason, or secrets.
- Naming matches the surrounding code.

## Output format
- **Summary:** one sentence.
- **Blockers:** must fix before merge (with file and line).
- **Should fix:** important but not blocking.
- **Nits:** optional.
- **Looks good:** what was done well, briefly.
- **Verdict:** Ready / Needs changes.

Be specific and concise. If a category has no issues, say so in one line.