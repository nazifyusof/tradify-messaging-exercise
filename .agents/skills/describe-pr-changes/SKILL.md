---
name: describe-pr-changes
description: Write a short, plain-language PR description for the current branch by reading its changes against main. USE WHEN the user says "describe pr", "describe-pr-changes", "pr description", "pr summary", "what changed", or asks you to open, create, update, or finish a PR. Non-technical by default; technical only if the user asks. Optional argument 'technical' for more detail.
---

# Describe PR Changes

When this skill is invoked, do the work immediately. Do NOT ask the user what they want. The task is always: describe the current branch's changes for a pull request.

## Steps

1. Find the base branch (default `main`; use `master` if `main` doesn't exist).
2. Look at what changed on this branch:
   - `git log <base>..HEAD --oneline`
   - `git diff <base>...HEAD --stat`
   - `git diff <base>...HEAD` (read the actual changes, not just file names)
3. Work out what changed from a user or product point of view.
4. Output the description in the format below. Nothing else.

If there are no differences from the base branch, say so in one line and stop.

## Output format

Use exactly this shape:

```
Changes:

* <change 1>
* <change 2>
* <change 3>
```

## Rules

1. **Short.** 2 to 5 bullets. One line per bullet. No paragraphs.
2. **Non-technical by default.** Describe what the user or product experiences, not how the code works.
   - Good: "Users who gain access while on the unauthorized page are now sent to the catalog automatically"
   - Bad: "Added explicit branch in routes-guard.ts to bypass cached checkScopeAccess closure"
3. **No jargon.** Avoid file names, function names, library names, and terms like "refactor", "guard", "endpoint", "schema", or "dependency".
4. **Start each bullet with a verb.** Added, Fixed, Updated, Removed, Improved.
5. **Group related changes.** Combine small edits into one bullet instead of listing every file.
6. **No filler.** No intro, sign-off, emojis, or extra sections like "Testing" unless asked.
7. **Be accurate.** Only describe what is actually in the diff. Never invent changes.

## Technical mode

If the user passes `technical` or asks for details, keep the same `Changes:` bullet format but include specifics: files, functions, approach, and trade-offs. Still concise.

## Example

```
Changes:

* Added a "Forgot password" link to the login page
* Fixed a bug where the dashboard showed the wrong total for some accounts
* Updated the welcome email wording
```