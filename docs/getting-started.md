---
ai_generated: true
model: "openai/gpt-5.3-codex@unknown"
operator: "lawcarl"
chat_id: "create-project-documentation-20260519"
prompt: |
  @codebase create documentation for this project
started: "2026-05-19T00:00:00Z"
ended: "2026-05-19T00:25:00Z"
task_durations:
  - task: "documentation discovery"
    duration: "00:08:00"
  - task: "documentation authoring"
    duration: "00:14:00"
  - task: "traceability updates"
    duration: "00:03:00"
total_duration: "00:25:00"
ai_log: "ai-logs/2026/05/19/create-project-documentation-20260519/conversation.md"
source: "github-copilot-chat"
---

# Getting Started

## Prerequisites

- .NET 8 SDK
- PowerShell (recommended on Windows)
- A local JWT signing secret

## 1. Restore dependencies

```powershell
dotnet restore
```

## 2. Configure JWT secret for local development

The API requires `JWT:Secret`. Store it outside source control.

```powershell
dotnet user-secrets init --project .\PostHubAPI.csproj
dotnet user-secrets set "JWT:Secret" "replace-with-a-long-random-secret" --project .\PostHubAPI.csproj
```

Alternative: set environment variable `JWT__Secret`.

## 3. Run the API

```powershell
dotnet run --project .\PostHubAPI.csproj
```

In development, Swagger UI is available by default.

## 4. Run tests

```powershell
dotnet test .\PostHubAPI.Tests\PostHubAPI.Tests.csproj
```

## Environment behavior

- `Development`: uses EF Core InMemory database and enables Swagger.
- Non-development: uses SQLite (`ConnectionStrings:DefaultConnection`) and keeps Swagger disabled.

## Notes for local usage

- JWT bearer auth is required for creating, editing, and deleting posts and for all comment endpoints.
- Register or login first to get a token, then pass `Authorization: Bearer <token>` on protected endpoints.
