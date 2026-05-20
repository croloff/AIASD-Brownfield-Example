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

# PostHubAPI

PostHubAPI is an ASP.NET Core Web API for a lightweight blogging workflow. It supports user registration/login with JWT authentication, post management, and comment management.

## Features

- JWT-based authentication and authorization
- User registration and login
- CRUD operations for posts
- CRUD operations for comments
- Environment-based persistence strategy:
	- Development: EF Core InMemory
	- Non-development: SQLite

## Tech stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core (InMemory and SQLite)
- ASP.NET Core Identity
- AutoMapper
- Swashbuckle (Swagger)
- xUnit (tests)

## Quick start

1. Restore dependencies:

```powershell
dotnet restore
```

2. Configure JWT secret (required):

```powershell
dotnet user-secrets init --project .\PostHubAPI.csproj
dotnet user-secrets set "JWT:Secret" "replace-with-a-long-random-secret" --project .\PostHubAPI.csproj
```

3. Run the API:

```powershell
dotnet run --project .\PostHubAPI.csproj
```

4. Open Swagger UI in development:

- `https://localhost:<port>/swagger`

## Configuration notes

- `JWT:Secret` is required and should not be committed.
- `JWT:ValidIssuer` and `JWT:ValidAudience` are in `appsettings.json`.
- In `Development`, JWT `RequireHttpsMetadata` is disabled.
- In non-development environments, JWT `RequireHttpsMetadata` is enabled.
- In non-development environments, set `ConnectionStrings:DefaultConnection` for SQLite.

## Authentication and authorization

- Use `/api/User/Register` or `/api/User/Login` to obtain a JWT.
- Send the token in the `Authorization` header:

```http
Authorization: Bearer <token>
```

Protected routes:

- `POST`, `PUT`, `DELETE` on `/api/Post`
- All `/api/Comment` endpoints

## Testing

Run the test project:

```powershell
dotnet test .\PostHubAPI.Tests\PostHubAPI.Tests.csproj
```

## Documentation index

- [Getting started](docs/getting-started.md)
- [API reference](docs/api-reference.md)
- [Architecture overview](docs/architecture.md)

## Project structure

- `Controllers/`: HTTP endpoints
- `Services/`: business logic contracts and implementations
- `Dtos/`: request/response models
- `Models/`: persistence entities
- `Data/`: EF Core DbContext
- `Profiles/`: AutoMapper profiles
- `Configuration/`: startup config helpers
- `PostHubAPI.Tests/`: automated tests

## AI-Assisted Artifacts

- [README.md](README.md): Primary project entry point and navigation.
- [docs/getting-started.md](docs/getting-started.md): Local setup and run guide.
- [docs/api-reference.md](docs/api-reference.md): Endpoint contracts and payload examples.
- [docs/architecture.md](docs/architecture.md): High-level technical architecture.
- [AI log (conversation)](ai-logs/2026/05/19/create-project-documentation-20260519/conversation.md)
- [AI log (summary)](ai-logs/2026/05/19/create-project-documentation-20260519/summary.md)
