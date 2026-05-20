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
  - task: "architecture analysis"
    duration: "00:09:00"
  - task: "architecture documentation"
    duration: "00:13:00"
  - task: "traceability updates"
    duration: "00:03:00"
total_duration: "00:25:00"
ai_log: "ai-logs/2026/05/19/create-project-documentation-20260519/conversation.md"
source: "github-copilot-chat"
---

# Architecture Overview

## Runtime composition

The API uses a standard ASP.NET Core composition in `Program.cs`:

- Controllers for HTTP endpoints.
- Service interfaces and implementations for business logic.
- EF Core `ApplicationDbContext` for persistence.
- ASP.NET Core Identity for user management.
- JWT Bearer authentication for protected routes.
- AutoMapper profiles for DTO mapping.

## Layered structure

- `Controllers/`: HTTP route handlers and response codes.
- `Services/Interfaces/`: service contracts.
- `Services/Implementations/`: business logic and data access orchestration.
- `Data/`: EF Core DbContext.
- `Models/`: persistence entities.
- `Dtos/`: request and response transport models.
- `Profiles/`: AutoMapper configuration.
- `Configuration/`: startup configuration helpers.
- `Exceptions/`: domain-specific exceptions used for API error mapping.

## Environment strategy

- Development:
  - InMemory EF Core database.
  - Swagger UI enabled.
- Non-development:
  - SQLite database using `ConnectionStrings:DefaultConnection`.
  - Swagger UI disabled by default.

## Authentication flow

1. Client calls register or login endpoint.
2. `UserService` validates credentials through ASP.NET Identity.
3. `UserService` issues JWT using settings from `JwtSettingsResolver`.
4. Client sends JWT in bearer header for protected endpoints.

## Error handling pattern

- Services throw `NotFoundException` when entities do not exist.
- Controllers catch `NotFoundException` and return `404 Not Found`.
- Input validation relies on DataAnnotations and ModelState checks.

## Testing strategy

The test project uses xUnit and ASP.NET Core test host support:

- Controller and authorization behavior tests.
- Configuration-focused tests (`JwtSettingsResolverTests`).

Run all tests:

```powershell
dotnet test .\PostHubAPI.Tests\PostHubAPI.Tests.csproj
```
