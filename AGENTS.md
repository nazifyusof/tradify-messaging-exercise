# Repo conventions

- Backend: .NET 10. Run `dotnet test backend/Tradify.sln` before you finish.
- Keep persistence behind the repository interfaces in `backend/src/Tradify.Core`.
- Do not add EF Core, Docker, or real database dependencies.
- Keep seed data synthetic.
- Do not commit secrets or real personal data.
- Frontend: run `npm run build` in `frontend/` after changes.

# Tradify Team Messaging - Agent Context & Rules

## 1. Product Domain & Scale Assumptions
- Core Objective: Introduce 'teams' into Tradify to support real-time two-way team messaging between team members and between administrators and field workers (tradies), linked/recorded against jobs or other parts of Tradify.
- Scale Profile: 20,000 paying trades companies, 60,000 active tradies total, and strictly 1 to 12 staff members per company.
- Design Implication of Scale: Because a company has at most 12 staff members, optimize UI and data structures for small, high-trust teams (e.g., simple flat member lists, lightweight polling/SSE for real-time updates, zero need for complex enterprise pagination or deep role hierarchies).
- Delivery Philosophy (Shape Up / Daily Releases): Work must be broken down into small, vertical, incremental Shippable Units (SUs) that de-risk technical and product unknowns and could be safely deployed to production daily.

## 2. Existing Starter Codebase Baseline
- Sign in: Stub login with 5 demo users (3 staff, 2 customers). Any password works.
- Auth Mechanism: The signed-in user is passed via the `X-User-Id` HTTP header.
- Existing Endpoints (.NET 10 API): 4 endpoints currently exist: log in, list my jobs, list a job's messages, and post a message. Data is held in memory and resets on API restart.
- Existing Tests: 13 backend tests exist and must remain green.
- Existing UI Patterns (React + TypeScript on Vite):
  - Conversation list shows assigned jobs styled like WhatsApp/iMessage, displaying the last message and its timestamp.
  - Thread view renders messages grouped under day headings. Current user's messages are blue bubbles on the right; others' messages are white bubbles on the left with the sender's name.
  - Composer input: `Enter` posts the message immediately (updating both the thread and the sidebar conversation list); `Shift+Enter` inserts a newline.

## 3. Strict Technical & UX Guardrails
- Role Isolation: Strictly distinguish between the 3 staff demo users (admins/field workers) and the 2 customer demo users. Internal staff team threads must never be exposed to customer users.
- UI Consistency: Reuse existing WhatsApp/iMessage-style chat bubble components, day-heading grouping, and `Enter`/`Shift+Enter` composer behavior for team threads.
- Zero Extra UI Packages: Do NOT install any external UI libraries or CSS frameworks (`package.json` must remain clean).
- Verification Loop:
  - Startup runs via `./start.sh`.
  - Run `dotnet test` after every backend change and add unit tests for new endpoints/models.
  - Run `npx tsc --noEmit` (or `npm run build`) after frontend changes to guarantee type safety.
