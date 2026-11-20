# Teams messaging plan

## Goal
Let a trades company group its staff into teams and message between members and teams in real time, with conversations recorded against jobs. Staff-only (internal) conversations must never be visible to customers, and data must never cross companies.

## Roles and assumptions
- Roles:
  - **Admin** (office staff): everything a field worker can do, plus create teams and manage team membership.
  - **Field worker** (tradie): sees and posts in teams they belong to and DMs with colleagues in their company; can link a team message to a job they are assigned to.
  - **Customer**: unchanged. Sees only their assigned job threads. Never sees teams, DMs, or internal job notes.
- Key assumptions:
  - Who is admin vs field worker? -> Seed: U1 Alex = Admin, U2 Sam and U3 Priya = FieldWorker, U4 Chris and U5 Dana = Customer.
  - How is a company modelled? -> New `CompanyId` on `User`. All 5 demo users are in company `CO1`. A second company is created only in test fixtures for cross-tenant tests (the demo seed stays at 5 users).
  - What does "real time" mean at our scale? -> Short polling (about 3 s on the open thread, 10 s on the sidebar) with an `after` cursor. At 60k users with roughly 20% online, that's about 2-4k cheap requests per second, fine for stateless reads. SSE can come later behind the same client hook if needed.
  - What does "linked to a job" mean? -> A team message can carry an optional `jobId`. Staff on that job see it in an **Internal** tab on the job. Customers never receive it. Other Tradify entities (quotes, invoices) are deferred.

## Themes and SUs
| # | SU | Spec | Visible outcome | Risk reduced | Depends on | Status |
|---|----|------|-----------------|--------------|------------|--------|
| 1 | Roles and company on users | `01-roles-and-company.md` | Login response shows `role` and `companyId`; posting as another user is rejected | Role isolation foundation; closes the `senderId` spoofing gap before new endpoints copy it | none | Not started |
| 2 | Seeded team conversation | (one-liner only) | Staff see a "Teams" section with a seeded "Field crew" team and can read and post using the existing bubbles and composer; customers see nothing new | Core value; proves the thread UI can be reused for non-job threads; enforces customer exclusion end to end; returns the newest 50 messages, not the oldest | 1 | Not started |
| 3 | Admin manages teams | (one-liner only) | Admin creates a team and adds or removes staff members; field workers can't | Product risk: is a flat member list enough? Permission model for admin vs field worker | 2 | Not started |
| 4 | Direct messages between staff | (one-liner only) | Staff start a 1:1 chat with a colleague from a company staff picker | Generalises threads to members as well as teams, with no new UI patterns | 2 | Not started |
| 5 | Link team messages to jobs | (one-liner only) | Staff tag a team message with a job; it shows in the job's Internal tab for assigned staff only | Riskiest privacy surface (internal content next to customer threads); validates the "lodged against jobs" requirement | 2 | Not started |
| 6 | Live updates via polling | (one-liner only) | A message sent in one browser appears in another within about 3 s; sidebar previews refresh | "Feels real-time" without new infra | 2 | Not started |

Order rationale:
1. A small backend-only change that every later endpoint relies on for role and tenant checks. It ships in hours and is safe because it's additive.
2. The thinnest vertical slice of the idea. A seeded team avoids building management UI before we know people message in teams.
3. Management only matters once team chat has proven useful. Until then, seeded teams are enough.
4. DMs reuse the thread model from SU2, so they come after it, but they're independent of SU3 and SU5.
5. Comes after the thread SUs because it combines team threads with job access rules. It needs the isolation tests from SU1 and SU2 in place first.
6. Last, because the focus is on features people can see first. Until then, new messages from others appear when a thread is reopened or after sending, as they do today. Polling stays isolated in one client hook, so it can be added to every thread type at once.

## Deferred
- Unread counts and read receipts: helpful, but not needed to prove the value. Add after SU3.
- Push notifications (mobile or email): needs infrastructure outside this repo.
- SSE or WebSockets: polling is enough at this scale. Revisit if poll load or latency becomes a problem.
- Linking to quotes, invoices, or other entities: same pattern as SU6. Wait until job linking is validated.
- Customers in team threads, or customer-to-staff DMs: breaks the internal-only guardrail. Out of scope.
- Real auth or token sessions: an existing stub limitation, tracked separately.
- Message editing, deletion, attachments, and search.
- Persistence beyond memory: repo convention. Everything stays behind repository interfaces so it can be swapped later.

## Open questions and assumptions
- Can a field worker create a team? -> No, admins only (SU4).
- Can a staff member see teams they don't belong to? -> No. Membership only, including for admins, who can see the team list in management but not message history unless they're members.
- Are DMs limited to 1:1? -> Yes. Group conversations use teams.
- How are feature flags done? -> No flag system exists. Use a config flag `Features:Teams` in `appsettings.json` (defaults on in Development). When it's off, team and DM endpoints return 404 and the UI hides the Teams section. SU1 needs no flag.
- Should an admin be able to post on someone else's behalf? -> No. A `senderId` that doesn't match the caller is rejected with 403, so the sender is always the caller (SU1).
- Who can see a job's Internal tab? -> Staff assigned to that job, plus members of the team the message was posted in.
- Polling interval -> 3 s for the open thread, 10 s for the sidebar, paused when the tab is hidden.

## Re-slice log
- 2026-11-17: Initial plan.
- 2026-11-17: Moved live updates via polling from SU3 to SU6 (last), because visible features come first. Admin management, DMs and job linking each move up one place and now depend only on SU2. The newest-50 paging fix moved from polling into SU2, since long threads need it to display correctly. Nothing dropped. The next SU is still SU1.
