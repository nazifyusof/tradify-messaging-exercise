# Architecture and Engineering Spec (current codebase)

Describes how the app works today, before any team-messaging work.

## 1. Overview

A small job-chat app. Users sign in, see the jobs they are assigned to, and chat inside a job thread.

```
Browser (React + Vite, :5173)
   |  fetch /auth, /jobs   (Vite dev proxy -> API)
   v
ASP.NET Core API (.NET 10, :5294)
   |  controllers -> repository interfaces
   v
In-memory repositories (seeded from Tradify.Seed). Reset on every API restart.
```

Run everything with `./start.sh` (installs frontend deps, runs `dotnet watch` for the API and `vite` for the UI). Ctrl+C stops both.

## 2. Repo layout

| Path | Purpose |
|------|---------|
| `backend/src/Tradify.Api` | Web API: `Program.cs`, controllers, `AuthStub` |
| `backend/src/Tradify.Core` | Models, repository interfaces, in-memory implementations, `JobThreadService` |
| `backend/src/Tradify.Seed` | Synthetic demo data (`SeedData`) |
| `backend/tests/Tradify.Tests` | xUnit integration tests (`WebApplicationFactory<Program>`) |
| `frontend/src` | React UI: `App`, `Login`, `Messenger`, `ConversationList`, `JobThread`, `api.ts`, `session.ts`, `format.ts` |

Dependency direction: `Api -> Core`, `Api -> Seed`, `Seed -> Core`. Core knows nothing about HTTP.

## 3. Data model

There is no database. Data lives in memory. Logical schema:

**User** (`Tradify.Core/User.cs`)

| Field | Type | Notes |
|-------|------|-------|
| Id | string | `U1`..`U5` |
| Username | string | used to log in |
| Name | string | display name |

No role field. Staff versus customer is only described in seed comments.

**Job** (`Job.cs`)

| Field | Type |
|-------|------|
| Id | string (`J1`..`J3`) |
| Title | string |

**Assignment** (a `(UserId, JobId)` pair, a many-to-many link)

| Field | Type |
|-------|------|
| UserId | string |
| JobId | string |

**Message** (`Message.cs`)

| Field | Type | Notes |
|-------|------|-------|
| Id | Guid | |
| JobId | string | thread the message belongs to |
| SenderId | string | |
| Body | string | |
| CreatedAt | DateTimeOffset | UTC |

Relationships: User N--N Job (via Assignment). Job 1--N Message. User 1--N Message (sender).

### Seed data (`SeedData.cs`)

| User | Id | Assigned to |
|------|----|-------------|
| Alex Morgan (staff) | U1 | J1, J2 |
| Sam Lee (staff) | U2 | J2 |
| Priya Shah (staff) | U3 | none |
| Chris Walker (customer) | U4 | J1 |
| Dana Price (customer) | U5 | J2 |

J3 "Roof repair" has no assignees. J1 starts with 4 seeded messages between U1 and U4.

## 4. Backend components

### Repositories (`Tradify.Core`)

| Interface | Methods | Implementation |
|-----------|---------|----------------|
| `IUserRepository` | `Get(id)`, `FindByUsername(name)` | `InMemoryUserRepository` |
| `IJobRepository` | `Get(id)` | `InMemoryJobRepository` |
| `IJobAssignmentRepository` | `IsAssigned(user, job)`, `ListJobIdsForUser(user)`, `ListUserIdsForJob(job)` | `InMemoryJobAssignmentRepository` (HashSet of pairs) |
| `IMessageRepository` | `Add(message)`, `ListForJob(jobId)` | `InMemoryMessageRepository` (locked list, sorted by `CreatedAt`) |

All are registered as singletons in `Program.cs`. Message and assignment repos are filled from `SeedData`.

### Auth (stub)
- `POST /auth/login` looks up the username. Any password works. Returns the user, or 401.
- All other calls identify the user with the `X-User-Id` request header (`AuthStub.GetUserId`).
- There is no token, session, or signature. Anyone can send any user id. Exercise only.

### Other
- CORS allows `http://localhost:5173`.
- `JobThreadService.CanView` is registered but not used by any controller (it only checks the user id is not blank).
- Swashbuckle is referenced in the csproj, but Swagger is not wired up in `Program.cs`.

## 5. API endpoints

| Method | Path | Auth | Success | Errors |
|--------|------|------|---------|--------|
| POST | `/auth/login` | none | 200 `{id, username, name}` | 401 unknown or blank username |
| GET | `/jobs` | `X-User-Id` of a known user | 200 list of `JobSummary` | 401 missing or unknown user |
| GET | `/jobs/{jobId}/messages` | `X-User-Id`, must be assigned to the job | 200 list of `Message` | 403 missing header or not assigned |
| POST | `/jobs/{jobId}/messages` | `X-User-Id`, must be assigned to the job | 201 `Message` | 403 missing header or not assigned |

### Payloads

```jsonc
// POST /auth/login request
{ "username": "alex", "password": "anything" }

// GET /jobs response item (JobSummary)
{
  "id": "J1",
  "title": "Kitchen refit",
  "participants": [{ "id": "U1", "name": "Alex Morgan" }],
  "lastMessage": { /* Message or null */ }
}

// POST /jobs/{jobId}/messages request
{ "body": "Hello", "senderId": null }   // senderId optional

// Message
{ "id": "guid", "jobId": "J1", "senderId": "U1", "body": "Hello", "createdAt": "2026-10-01T09:00:00+00:00" }
```

### Logic notes
- `GET /jobs`: take the user's job ids, load each job, attach participants (sorted by name) and the last message. Sort by last message time (newest first, no messages last), then by title.
- `GET .../messages`: messages for the job in time order, capped with `Take(50)`. Because the list is oldest first, this returns the oldest 50, not the latest 50.
- `POST .../messages`: the sender is `request.SenderId ?? userId`, `Id` is a new Guid, `CreatedAt` is `UtcNow`. The body is logged by `ILogger`.

## 6. Main flows

### Sign in
1. `Login.tsx` calls `POST /auth/login`.
2. On success, `session.ts` saves the user to `localStorage` (`tradify.session`) and `api.ts` remembers the user id.
3. Every later request adds `X-User-Id`.
4. On page reload, `loadSession()` restores the user. Log out clears it.

### Load conversations
1. `Messenger` mounts and calls `GET /jobs`.
2. `ConversationList` shows each job: avatar, title, last message preview ("You:" or first name), and time.

### Open a thread
1. Clicking a job sets `selectedJobId`.
2. `Messenger` calls `GET /jobs/{id}/messages`, with a `cancelled` flag to ignore stale responses.
3. `JobThread` renders bubbles: mine on the right (blue), others on the left with the sender name. Messages are grouped under day headings and it scrolls to the bottom.

### Send a message
1. User types. `Enter` sends, `Shift+Enter` adds a newline.
2. `JobThread` trims the text and ignores empty text or a send already in progress.
3. `POST /jobs/{id}/messages`. On success the message is appended to the thread and `GET /jobs` is called again to refresh the sidebar.
4. On failure the error shows under the composer and the draft is kept.

There is no polling, SSE, or WebSocket. Other users' messages only appear after reopening a thread or sending.

## 7. Frontend structure

| File | Role |
|------|------|
| `App.tsx` | Chooses `Login` or `Messenger` based on the session |
| `Messenger.tsx` | State owner: jobs, selected job, messages, errors. Header and layout |
| `ConversationList.tsx` | Sidebar list of jobs |
| `JobThread.tsx` | Thread view and composer |
| `api.ts` | Typed fetch wrapper and `X-User-Id` header. Throws on non-2xx |
| `session.ts` | `localStorage` session helpers |
| `format.ts` | Initials, avatar colour, time and day formatting |

The Vite dev server proxies `/auth` and `/jobs` to `http://localhost:5294` (override with `VITE_API_PROXY_TARGET`).

## 8. Tests

13 xUnit integration tests (all passing), each running the real app in memory:
- Login: known user with any password, unknown user returns 401.
- Jobs: only assigned jobs, participants and last message included, user with no jobs gets empty list, missing or unknown user returns 401.
- Messages: assigned user can post (201) and list (in order), unassigned user gets 403 on both, empty thread returns 200 with an empty list.

Run: `dotnet test backend/Tradify.sln`. Frontend: `npm run build` in `frontend/`. There are no frontend tests.

## 9. Known gaps and risks (current state)

| Area | Gap |
|------|-----|
| Auth | `X-User-Id` can be forged. No real session. Message endpoints do not check the user exists, and return 403 instead of 401 when the header is missing |
| Spoofing | `POST` accepts a client-supplied `senderId`, so a user can post as someone else |
| Validation | Message body is not validated (empty or very long text is accepted) |
| Privacy | The message body is written to logs |
| Roles | No staff versus customer model. Internal team threads would need role checks |
| Paging | `Take(50)` returns the oldest 50 messages, not the newest |
| Real time | No push or polling. Messages from others appear only on reload |
| Unused code | `JobThreadService.CanView` is unused and does not check assignment |
| Persistence | In memory only. Data resets on restart |
| Swagger | `launchUrl` is `swagger`, but Swagger is not configured |
