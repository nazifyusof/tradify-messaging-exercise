#!/usr/bin/env bash
# Starts the API (http://localhost:5294) and the UI (http://localhost:5173). Ctrl+C stops both.
set -e
cd "$(dirname "$0")"
(cd frontend && npm install --no-audit --no-fund)
dotnet watch --project backend/src/Tradify.Api --launch-profile http &
API_PID=$!
trap 'kill $API_PID 2>/dev/null' EXIT
cd frontend && npm run dev
