---
name: run-checks
description: Run all verification (backend tests, frontend type-check/lint/build, smoke test) and report pass/fail. Use after any code change and before committing.
---

# Run Checks

Run the checks below in order, from the repo root.

## Steps
1. **Backend build and tests**
   - `dotnet build backend/Tradify.sln` then `dotnet test backend/Tradify.sln` (needs the .NET 10 SDK; if `dotnet --list-runtimes` has no 10.x, report that instead of rolling forward).
   - Record total, passed, failed. The 13 starter tests must stay green; new work should add to that number.
2. **Frontend checks** (in `frontend/`)
   - `npm run build` (it already runs `tsc --noEmit` first). There is no lint script, so skip lint.
3. **Smoke test** (only if code touching runtime behaviour changed)
   - There is no health endpoint. Start the API only: `dotnet run --project backend/src/Tradify.Api --launch-profile http &` (port 5294). Do not run `./start.sh`, it never exits.
   - `curl -s -o /dev/null -w "%{http_code}" -H "X-User-Id: U1" http://localhost:5294/jobs` must print 200; without the header it must print 401.
   - Kill the background process afterwards. If port 5294 is already in use, say so and stop rather than killing the user's process.

## Rules
- Run everything even if one step fails, so the report is complete.
- Never skip, disable, or delete a failing test to get green. Report it instead.
- If a failure looks pre-existing, confirm with `git stash` or on the base branch before saying so.
- Do not fix failures unless asked; report the first useful error line, not the full log.

## Output format
| Check | Result | Notes |
|---|---|---|
| Backend build | Pass/Fail | |
| Backend tests | X passed / Y failed | failing test names |
| Frontend type-check | Pass/Fail | |
| Frontend build | Pass/Fail | |
| Smoke test | Pass/Fail/Skipped | |

End with one line: **All green** or **Not ready: <reason>**.