# What was wrong and what changed

## Bugs found and fixed

| # | What was wrong | Fix | File |
|---|----------------|-----|------|
| 1 | Test package was version 8 but the project is .NET 10, so every test got a 500 error | Updated package to 10.0.0 | `backend/tests/Tradify.Tests/Tradify.Tests.csproj` |
| 2 | "List my jobs" ignored the user and returned all jobs to everyone (customers could see other jobs) | Now only returns jobs assigned to that user | `backend/src/Tradify.Core/InMemoryJobAssignmentRepository.cs` |
| 3 | Messages with the same timestamp came back in random order | Messages now keep the order they were added | `backend/src/Tradify.Core/InMemoryMessageRepository.cs` |
| 4 | Seed data gave user U3 a job (J3), but it should have none | Removed the `("U3", "J3")` assignment | `backend/src/Tradify.Seed/SeedData.cs` |

## Still to do (not changed)
- `JobThreadService.CanView` is not used anywhere.
- It only checks that the user id is not blank, but its comment says only assigned users may view a thread.
- If you use it, it should call `assignments.IsAssigned(userId, jobId)`.

## Result
- Before: 13 of 13 backend tests failed.
- Now: 13 of 13 backend tests pass.
