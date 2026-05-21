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

## Development Guidelines

For guidance on implementing specific features, refer to the instruction files in `.github/instructions/`:

- [JWT Authentication Instructions](.github/instructions/jwt-authentication.instructions.md) - Token generation, validation, claims design, secret management, and security best practices
- [ASP.NET Core API Design Instructions](.github/instructions/aspnet-core-api-design.instructions.md) - RESTful API principles, routing, DTOs, validation
- [AutoMapper Instructions](.github/instructions/automapper.instructions.md) - DTO and entity mapping patterns
- [Evergreen Software Development](.github/instructions/evergreen-software-development.instructions.md) - Core principles for sustainable code

## Testing

Run the test project:

```powershell
dotnet test .\PostHubAPI.Tests\PostHubAPI.Tests.csproj
```

## Documentation index

- [Project requirements](docs/PROJECT-REQUIREMENTS.md) - Comprehensive requirements document covering business rules, workflows, tech stack, and architecture
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

## Development Guidance

Instruction files provide detailed guidance for implementing features consistently:

- [ASP.NET Core API Design Instructions](.github/instructions/aspnet-core-api-design.instructions.md): Comprehensive guidance for RESTful API design, controller patterns, HTTP semantics, validation, authorization, error handling, and testing. ([view chat log](ai-logs/2026/05/21/aspnet-core-api-design-20260521/conversation.md))
- [Entity Framework Core Instructions](.github/instructions/entity-framework-core.instructions.md): Data access guidance for DbContext setup, entity modeling, query patterns, mutation safety, relationship loading, and test strategy. ([view chat log](ai-logs/2026/05/21/create-entity-framework-core-instructions-20260521/conversation.md))
- [SQLite Database Instructions](.github/instructions/sqlite-database.instructions.md): SQLite persistence guidance for provider configuration, migrations, operations, troubleshooting, deployment, and test strategy. ([view chat log](ai-logs/2026/05/21/create-sqlite-database-instructions-20260521/conversation.md))
- [Swagger/OpenAPI Instructions](.github/instructions/swagger-openapi.instructions.md): Per-action Swagger documentation patterns covering `[ProducesResponseType]`, XML doc comments, `[Produces]`/`[Consumes]`, `AddSwaggerGen` configuration, JWT security definition, and Swagger middleware. ([view chat log](ai-logs/2026/05/21/8f22b8f1-109f-4036-b9f3-effc961047fb/conversation.md))
- [xUnit Testing Instructions](.github/instructions/xunit-testing.instructions.md): Guidance for unit/integration test structure, authorization testing, async reliability, and isolation patterns in `PostHubAPI.Tests`. ([view chat log](ai-logs/2026/05/21/create-xunit-testing-instructions-20260521/conversation.md))

## AI-Assisted Artifacts

- [README.md](README.md): Primary project entry point and navigation.
- [docs/getting-started.md](docs/getting-started.md): Local setup and run guide.
- [docs/api-reference.md](docs/api-reference.md): Endpoint contracts and payload examples.
- [docs/architecture.md](docs/architecture.md): High-level technical architecture.
- [.github/instructions/aspnet-core-api-design.instructions.md](.github/instructions/aspnet-core-api-design.instructions.md): API design instruction file with 16 comprehensive sections.
- [.github/instructions/entity-framework-core.instructions.md](.github/instructions/entity-framework-core.instructions.md): EF Core data-access instruction file for configuration, modeling, queries, and testing.
- [.github/instructions/sqlite-database.instructions.md](.github/instructions/sqlite-database.instructions.md): SQLite database instruction file for provider selection, schema evolution, operations, and resilience.
- [.github/instructions/swagger-openapi.instructions.md](.github/instructions/swagger-openapi.instructions.md): Swagger/OpenAPI instruction file for per-action response type attributes, XML documentation, Swashbuckle configuration, and JWT security definition.
- [.github/instructions/xunit-testing.instructions.md](.github/instructions/xunit-testing.instructions.md): xUnit testing instruction file for unit tests, integration tests, and authorization coverage patterns.
- [AI log (conversation - Swagger/OpenAPI instructions)](ai-logs/2026/05/21/8f22b8f1-109f-4036-b9f3-effc961047fb/conversation.md)
- [AI log (summary - Swagger/OpenAPI instructions)](ai-logs/2026/05/21/8f22b8f1-109f-4036-b9f3-effc961047fb/summary.md)
- [AI log (conversation - docs)](ai-logs/2026/05/19/create-project-documentation-20260519/conversation.md)
- [AI log (summary - docs)](ai-logs/2026/05/19/create-project-documentation-20260519/summary.md)
- [AI log (conversation - API design)](ai-logs/2026/05/21/aspnet-core-api-design-20260521/conversation.md)
- [AI log (summary - API design)](ai-logs/2026/05/21/aspnet-core-api-design-20260521/summary.md)
- [AI log (conversation - EF Core instructions)](ai-logs/2026/05/21/create-entity-framework-core-instructions-20260521/conversation.md)
- [AI log (summary - EF Core instructions)](ai-logs/2026/05/21/create-entity-framework-core-instructions-20260521/summary.md)
- [AI log (conversation - SQLite instructions)](ai-logs/2026/05/21/create-sqlite-database-instructions-20260521/conversation.md)
- [AI log (summary - SQLite instructions)](ai-logs/2026/05/21/create-sqlite-database-instructions-20260521/summary.md)
- [AI log (conversation - xUnit testing instructions)](ai-logs/2026/05/21/create-xunit-testing-instructions-20260521/conversation.md)
- [AI log (summary - xUnit testing instructions)](ai-logs/2026/05/21/create-xunit-testing-instructions-20260521/summary.md)
