# Tradify Job Thread Messaging

## Quick start

1. Install the [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) and [Node 24 LTS](https://nodejs.org/).
2. Run `./start.sh` (macOS/Linux). On Windows, run the two commands under "Running the app" in two terminals.
3. Open http://localhost:5173 and sign in as `alex` with any password.

## Prerequisites

- .NET 10 SDK
- Node 24 LTS

## Commands

```bash
# Backend tests
dotnet test backend/Tradify.sln

# Frontend build check
cd frontend && npm install && npm run build
```

## Running the app

```bash
# Terminal 1: API on http://localhost:5294
dotnet run --project backend/src/Tradify.Api --launch-profile http

# Terminal 2: UI on http://localhost:5173 (proxies /auth and /jobs to the API)
cd frontend && npm install && npm run dev
```

Sign in with any demo username below. Any password is accepted.

| Username | User |
|---|---|---|---|
| `alex` | U1 Alex Morgan |
| `sam` | U2 Sam Lee |
| `priya` | U3 Priya Shah |
| `chris` | U4 Chris Walker |
| `dana` | U5 Dana Price |

## API

| Endpoint | Purpose |
|---|---|
| `POST /auth/login` | Body `{ "username", "password" }`. Returns the user. Any password works (stub). |
| `GET /jobs` | Jobs assigned to the signed-in user, with participants and the last message. |
| `GET /jobs/{jobId}/messages` | Messages in a job thread. |
| `POST /jobs/{jobId}/messages` | Post a message to a job thread. |

Authenticated requests send the signed-in user's ID in the `X-User-Id` header.

## Layout

```
backend/     C# solution: Api (controllers), Core (domain and repositories), Seed (synthetic data), Tests
frontend/    Vite + React + TypeScript (login, conversation list, job thread)
db/          schema.sql, reference only (not executed)
```

The backend uses in-memory repositories behind interfaces. You do not need a database or Docker.

## Working rules

- Keep test data synthetic. Do not paste real customer or Tradify data into any tool.
- Do not add new packages unless you explain why.
- Finish with `dotnet test` passing for the behaviour you built. Say clearly which acceptance criteria are not met, if any.
