---
name: write-backend-tests
description: Write or extend backend tests for new or changed API behaviour. Use whenever an endpoint, service, or model is added or modified.
---

# Write Backend Tests

## Before writing
1. Read `backend/tests/Tradify.Tests/MessagesApiTests.cs` and `AuthAndJobsApiTests.cs`. Copy their style: xUnit, `IDisposable` class with a `WebApplicationFactory<Program>` per test (fresh in-memory state), a `Request(method, url, userId)` helper that sets `X-User-Id`, `JsonDocument` assertions. Do not introduce a new test library.
2. Use the seed users and jobs in `backend/src/Tradify.Seed/SeedData.cs` (staff U1-U3, customers U4-U5). Do not add real data.
3. Never rely on test order; each test gets its own host.

## What to cover for every endpoint or behaviour
- **Happy path:** the expected result and status code, and that state actually changed.
- **Unauthenticated:** missing or unknown `X-User-Id` gives 401.
- **No access:** a user who is not a member of the job or team gets 403 and never the data. There is no company concept yet; once one exists, add a cross-company case (403 or 404, never the data).
- **Role:** customers (U4, U5) must never see internal staff-only or team threads.
- **Validation:** empty, missing, or too-long input returns 400.
- **Not found:** unknown IDs return 404.
- **Edge cases:** duplicates (adding the same member twice), removing a non-member, ordering of results, empty lists.

## Conventions
- Name tests `Method_Scenario_ExpectedResult` (or match the existing style if different).
- Arrange / Act / Assert, one behaviour per test.
- Assert on status code AND response body or resulting state.
- Keep tests fast and deterministic: no sleeps, no real network, no dependence on the clock without a controllable time source.
- A bug fix starts with a failing test that reproduces it.

## After writing
- Run `dotnet test backend/Tradify.sln` and confirm the new tests pass.
- Break the code deliberately (or reason through it) to confirm at least the access-control test would fail if the membership check were removed.
- Report: tests added, scenarios covered, and any scenarios intentionally not covered.