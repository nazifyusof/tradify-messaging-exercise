# AGENTS.md

## Read first
- Before any structural change (new endpoints, models, persistence, or UI architecture), read `docs/ARCHITECTURE.md`.

## Keeping ARCHITECTURE.md current
Update `docs/ARCHITECTURE.md` in the same change whenever you:
- Add, remove, or change an API endpoint, including its auth or role rules.
- Add or change a domain model or repository interface in `backend/src/Tradify.Core`.
- Change how real-time updates work (polling, SSE, etc.) or how data is stored.
- Add a new top-level frontend component, page, or data-flow pattern.
- Change role isolation rules (who can see which threads).
- Make a decision that future contributors would otherwise have to rediscover (briefly note the choice and why).

Guidelines:
- Edit only the sections your change affects. Keep it short and accurate, not exhaustive.
- Remove or correct anything your change makes false. A stale architecture doc is worse than none.
- If a change doesn't affect architecture (bug fix, copy tweak, styling, tests only), leave the file alone.
- If `docs/ARCHITECTURE.md` doesn't exist yet, create it with: overview, backend structure, frontend structure, data flow, role isolation, and key decisions.

## Repo layout
- `backend/` - .NET 10 API (`backend/Tradify.sln`)
  - `backend/src/Tradify.Core` - domain models and repository interfaces
- `frontend/` - React + TypeScript on Vite
- `docs/ARCHITECTURE.md` - system design and key decisions
- `start.sh` - starts the whole app

## Repo conventions
- Keep persistence behind the repository interfaces in `backend/src/Tradify.Core`.
- Do not add EF Core, Docker, or real database dependencies.
- Do not install external UI libraries or CSS frameworks. `package.json` must stay clean.
- Keep seed data synthetic.
- Do not commit secrets or real personal data.

## Before you finish (checklist)
1. Backend changes: `dotnet test backend/Tradify.sln` passes. Add unit tests for every new endpoint or model. The 13 existing tests must stay green.
2. Frontend changes: `npm run build` in `frontend/` passes (or `npx tsc --noEmit` for a quick type check).
3. Architecture-relevant changes: `docs/ARCHITECTURE.md` is updated (see above).
4. The app still starts with `./start.sh`.

## Product domain
- Objective: introduce "teams" into Tradify to support real-time two-way messaging between team members, and between administrators and field workers (tradies), linked to jobs or other parts of Tradify.
- Scale: 20,000 paying trades companies, 60,000 active tradies, and 1 to 12 staff per company.
- Design implication: optimize for small, high-trust teams. Use simple flat member lists and lightweight polling/SSE for real-time updates. No enterprise pagination or deep role hierarchies.
- Delivery: work in small, vertical Shippable Units (SUs) that de-risk technical and product unknowns and could be deployed to production daily (Shape Up).

## Existing baseline
- Auth: stub login with 5 demo users (3 staff, 2 customers). Any password works. The signed-in user is passed via the `X-User-Id` header.
- API: 4 endpoints (log in, list my jobs, list a job's messages, post a message). Data is in memory and resets on restart.
- UI (React + TypeScript on Vite):
  - Conversation list shows assigned jobs WhatsApp/iMessage-style, with last message and timestamp.
  - Thread view groups messages under day headings. Own messages are blue bubbles on the right. Others are white bubbles on the left with the sender's name.
  - Composer: `Enter` posts immediately (updating thread and sidebar), `Shift+Enter` inserts a newline.

## Guardrails
- Role isolation: internal staff team threads must never be exposed to customer users. Distinguish the 3 staff users (admins/field workers) from the 2 customers on every new endpoint.
- UI consistency: reuse the existing chat bubble components, day-heading grouping, and `Enter`/`Shift+Enter` composer behavior for team threads.