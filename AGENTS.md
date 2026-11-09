# Repo conventions

- Backend: .NET 10. Run `dotnet test backend/Tradify.sln` before you finish.
- Keep persistence behind the repository interfaces in `backend/src/Tradify.Core`.
- Do not add EF Core, Docker, or real database dependencies.
- Keep seed data synthetic.
- Do not commit secrets or real personal data.
- Frontend: run `npm run build` in `frontend/` after changes.
