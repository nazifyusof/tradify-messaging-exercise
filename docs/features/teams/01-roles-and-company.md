# Roles and company on users (lite spec)

## Goal
Every user has a role (Admin, FieldWorker, Customer) and a company, and the API returns both at login. Message endpoints stop people posting as someone else and handle unknown callers the same way `/jobs` does. Later SUs can then check staff vs customer and same-company on every new endpoint.

## In scope
- `UserRole` enum and `Role` and `CompanyId` on `User`, plus a helper `IsStaff` (Admin or FieldWorker).
- Seed: U1 Alex = Admin, U2 Sam and U3 Priya = FieldWorker, U4 Chris and U5 Dana = Customer. All in company `CO1`.
- `POST /auth/login` returns `role` and `companyId`.
- Shared caller lookup `AuthStub.GetUser(request, users)`, which returns the `User` or null for a missing or unknown header.
- Message endpoints: a missing or unknown caller gets 401 (today it's 403). Posting with a `senderId` that isn't the caller gets 403.
- Frontend: `SessionUser` gains `role` and `companyId`. A stored session without `role` is discarded, so the user logs in again.
- `docs/ARCHITECTURE.md`: update the User model, login payload, and the auth and spoofing gaps.

## Out of scope (deferred)
- Any UI that uses the role (the Teams section arrives in SU2).
- `CompanyId` on `Job`. Jobs are already isolated by assignment, and a company check is first needed for job linking (SU5).
- Fixing `JobThreadService.CanView`, the newest-50 paging, body validation, and logging the body. These are unrelated to roles and tracked in ARCHITECTURE section 9. Paging is fixed in SU2.
- Real auth or tokens.

## API contract
**`POST /auth/login`** (unchanged request)
```jsonc
// 200
{ "id": "U1", "username": "alex", "name": "Alex Morgan", "role": "Admin", "companyId": "CO1" }
// 401: unknown or blank username (unchanged)
```
`role` is one of `"Admin" | "FieldWorker" | "Customer"`. It's serialised as a string using `JsonStringEnumConverter`, registered in `AddControllers().AddJsonOptions(...)`.

**`GET /jobs/{jobId}/messages`** and **`POST /jobs/{jobId}/messages`** (shapes unchanged)

| Case | Before | After |
|------|--------|-------|
| `X-User-Id` missing or unknown | 403 | **401** |
| Known user, not assigned to the job | 403 | 403 |
| POST with `senderId` set and not equal to the caller | 201, posted as that user | **403**, nothing stored |
| POST with `senderId` omitted or equal to the caller | 201 | 201, sender is the caller |

`GET /jobs` is unchanged (it already returns 401 for a missing or unknown caller).

## Acceptance criteria
1. Given `alex`, when they log in, then the response has `role: "Admin"` and `companyId: "CO1"`.
2. Given `sam` or `priya`, when they log in, then `role` is `"FieldWorker"`. Given `chris` or `dana`, then `role` is `"Customer"`.
3. Given U1 is assigned to J1, when U1 POSTs to J1 with `senderId: "U4"`, then the response is 403 and a following GET on J1 still returns only the 4 seeded messages.
4. Given U1, when U1 POSTs to J1 with `senderId: "U1"` or no `senderId`, then the response is 201 and `senderId` is `"U1"`.
5. Given no `X-User-Id` header, or `X-User-Id: U999`, when calling GET or POST on `/jobs/J1/messages`, then the response is 401.
6. Given U3, who isn't assigned to J1, when U3 calls GET or POST on J1, then the response is still 403 (this permission case is unchanged).
7. Given a browser holding an old `tradify.session` without `role`, when the app loads, then the login screen is shown.
8. All 13 existing backend tests still pass, and `./start.sh` logs in and sends a message as before.

Cross-company: there are no cross-company resources yet, because all seed users are in `CO1` and jobs are isolated by assignment. The first cross-company test arrives in SU2.

## Tasks
### Backend
- [ ] `Tradify.Core/UserRole.cs`: `enum UserRole { Admin, FieldWorker, Customer }`.
- [ ] `User.cs`: `User(string Id, string Username, string Name, UserRole Role, string CompanyId)` with `bool IsStaff => Role is UserRole.Admin or UserRole.FieldWorker;`.
- [ ] `SeedData.cs`: set the roles and `CO1` as listed in In scope.
- [ ] `Program.cs`: register `JsonStringEnumConverter`.
- [ ] `AuthController`: add `Role` and `CompanyId` to `LoginResponse`.
- [ ] `AuthStub.GetUser(HttpRequest, IUserRepository)`: returns `User?`.
- [ ] `MessagesController`: inject `IUserRepository`, use `GetUser` (null gives 401), and reject a mismatched `SenderId` with 403 before storing anything.
### Frontend
- [ ] `api.ts`: add `UserRole` type, and add `role` and `companyId` to `SessionUser`.
- [ ] `session.ts`: in `loadSession`, if the parsed object has no `role`, clear storage and return null.
### Tests and checks
- [ ] Add the backend tests below.
- [ ] Run `dotnet test backend/Tradify.sln` and `npm run build` in `frontend/`.
- [ ] Update `docs/ARCHITECTURE.md` sections 3, 5 and 9.

## Test plan
- Backend tests:
  - `AuthAndJobsApiTests`: a login theory covering `alex`, `sam` and `chris` that checks role and `CO1` (criteria 1-2).
  - `MessagesApiTests`:
    - `PostMessage_SpoofedSenderId_Returns403AndNotStored` (criterion 3).
    - `PostMessage_OwnSenderId_Returns201` (criterion 4).
    - A theory `MessagesEndpoints_MissingOrUnknownUser_Returns401` covering GET and POST with null and `U999` (criterion 5).
  - The existing 403 tests cover criterion 6.
- Manual smoke test:
  1. Run `./start.sh` and log in as `alex`. In DevTools, the login response shows `role` and `companyId`.
  2. Send a message in J1. It works as before.
  3. Reload. The session persists.
  4. Delete `role` from `tradify.session` in localStorage and reload. The login screen appears.

## Rollout
- No feature flag. The change is additive to the login payload. The only behaviour changes close a spoofing hole the UI never used (the frontend never sends `senderId`) and correct the status code to 401. Safe to deploy on its own, and rollback is a plain revert.
- Size: about 2-3 hours.

## Open questions and assumptions
- Should a mismatched `senderId` be ignored or rejected? -> Rejected with 403, which is explicit and testable. An equal `senderId` is allowed so that existing clients keep working.
- Enum JSON casing? -> PascalCase strings (`"FieldWorker"`), the converter's default.
- Should customers have a `CompanyId`? -> Yes, the company they're a customer of (`CO1`). It isn't used for access yet.
