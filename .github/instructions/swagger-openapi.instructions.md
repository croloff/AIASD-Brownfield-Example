---
ai_generated: true
model: "anthropic/claude-sonnet-4-6@2026-05-21"
operator: "johnmillerATcodemag-com"
chat_id: "8f22b8f1-109f-4036-b9f3-effc961047fb"
prompt: |
  Follow instructions in #prompt:SKILL.md with these arguments: create-swagger-openapi-instructions.prompt.md
started: "2026-05-21T00:00:00Z"
ended: "2026-05-21T00:08:00Z"
task_durations:
  - task: "codebase exploration"
    duration: "00:03:00"
  - task: "instruction authoring"
    duration: "00:04:00"
  - task: "validation and polish"
    duration: "00:01:00"
total_duration: "00:08:00"
ai_log: "ai-logs/2026/05/21/8f22b8f1-109f-4036-b9f3-effc961047fb/conversation.md"
source: ".github/prompts/create-swagger-openapi-instructions.prompt.md"
name: swagger-openapi
description: "Use when adding or modifying Swagger/OpenAPI documentation in this ASP.NET Core project. Covers ProducesResponseType attributes, XML documentation comments, Swashbuckle configuration, JWT Bearer security definition, OpenApiInfo setup, and Swagger UI options."
applyTo: ["Controllers/*.cs", "Program.cs", "**/*.csproj"]
version: "1.0.0"
author: "johnmillerATcodemag-com"
tags: ["swagger", "openapi", "swashbuckle", "api-documentation", "aspnet-core"]
owner: "Development Team"
reviewedDate: "2026-05-21"
nextReview: "2026-08-21"
---

# Swagger / OpenAPI Instructions

## Overview

Apply these rules when adding, updating, or validating Swagger/OpenAPI documentation in this ASP.NET Core project. This file complements the high-level setup described in section 12 of [aspnet-core-api-design.instructions.md](aspnet-core-api-design.instructions.md) with granular, action-level patterns and Swashbuckle configuration details.

This project uses **Swashbuckle.AspNetCore 6.5.0** with dynamic spec generation (no static `.json`/`.yaml` files checked in).

---

## 1. `[ProducesResponseType]` on Every Action

Every public action method **must** declare all possible HTTP response codes using `[ProducesResponseType]`. Use the typed overload when returning a body, the untyped overload for empty responses.

```csharp
// GET — typed response + 404
[HttpGet("{id}")]
[ProducesResponseType(typeof(ReadPostDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> GetPostById(int id) { ... }

// POST — 201 Created with body + 400
[HttpPost]
[ProducesResponseType(typeof(ReadPostDto), StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<IActionResult> CreatePost([FromBody] CreatePostDto dto) { ... }

// PUT — 204 No Content + 400 + 401 + 404
[HttpPut("{id}")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> EditPost(int id, EditPostDto dto) { ... }

// DELETE — 204 No Content + 401 + 404
[HttpDelete("{id}")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> DeletePost(int id) { ... }
```

**Rules**:
- Always include `400 BadRequest` for POST/PUT actions that accept a request body.
- Always include `401 Unauthorized` for actions decorated with `[Authorize]`.
- Always include `403 Forbidden` when role-based checks are applied.
- Always include `404 NotFound` for actions that look up a resource by ID.
- Use `StatusCodes.Status*` constants — never raw integer literals.

---

## 2. XML Documentation Comments on Every Action

Enable XML comments to populate Swagger operation summaries and parameter descriptions.

### 2a. Enable XML Output in `.csproj`

```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```

`1591` suppresses warnings for undocumented public members outside controllers.

### 2b. Comment Format

Every action must have at minimum a `<summary>` tag. Include `<param>` for route/query parameters and `<response>` tags that mirror each `[ProducesResponseType]`.

```csharp
/// <summary>
/// Retrieves a post by its ID.
/// </summary>
/// <param name="id">The unique post identifier.</param>
/// <returns>The matching post.</returns>
/// <response code="200">Post found and returned.</response>
/// <response code="404">No post exists with the given ID.</response>
[HttpGet("{id}")]
[ProducesResponseType(typeof(ReadPostDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> GetPostById(int id) { ... }
```

**Rules**:
- `<summary>` must be present; start with a verb (Retrieves, Creates, Updates, Deletes).
- `<response>` codes must match the `[ProducesResponseType]` attributes exactly.
- Do not repeat the HTTP method or route in the summary — it is already shown by Swagger.

---

## 3. Controller-Level `[Produces]` and `[Consumes]`

Declare content-type expectations at the controller class level to produce accurate Swagger media type entries.

```csharp
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class PostController : ControllerBase { ... }
```

- `[Produces]` declares the response content type for all actions in the controller.
- `[Consumes]` declares the accepted request body content type for POST/PUT actions.
- Omit `[Consumes]` on controllers that have no request body actions (e.g., a read-only controller).

---

## 4. `AddSwaggerGen` Configuration in `Program.cs`

Replace the default bare `AddSwaggerGen()` call with a configured version that includes metadata and JWT authentication support.

```csharp
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PostHub API",
        Version = "v1",
        Description = "A social posting platform API supporting users, posts, and comments."
    });

    // Wire up XML documentation comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // JWT Bearer security definition
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token. The 'Bearer ' prefix is added automatically."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
```

**Rules**:
- Always call `IncludeXmlComments` — omitting it means XML comments have no effect.
- `AddSecurityDefinition` + `AddSecurityRequirement` must both be present to enable the Swagger UI **Authorize** button.
- Keep `Title`, `Version`, and `Description` fields populated in `OpenApiInfo`.

---

## 5. Swagger Middleware in `Program.cs`

Swagger middleware must remain **development-only**. Never expose it in production.

```csharp
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "PostHub API v1");
        options.RoutePrefix = "swagger"; // Access at /swagger
    });
}
```

- Do not move `app.UseSwagger()` outside the `IsDevelopment()` guard.
- Set `RoutePrefix` explicitly to make the access path predictable.

---

## 6. `[Authorize]` Interactions in Swagger

When an action is protected with `[Authorize]`, Swagger UI must reflect that the user needs a token. The global `AddSecurityRequirement` in section 4 applies a token requirement to all endpoints. To mark individual endpoints as anonymous (override), use:

```csharp
[AllowAnonymous]
[HttpPost("Register")]
[ProducesResponseType(typeof(ReadUserDto), StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<IActionResult> Register([FromBody] RegisterUserDto dto) { ... }
```

Alternatively, apply security only per-action using `[SwaggerOperation]` + operation filters for more granular control (advanced; avoid unless needed).

---

## 7. Response Type Conventions per HTTP Method

| HTTP Method | Expected Success Code | Typical Error Codes |
|-------------|----------------------|---------------------|
| GET (by id) | `200 OK` | `404` |
| GET (list)  | `200 OK` | — |
| POST        | `201 Created` | `400`, `401` |
| PUT         | `204 No Content` | `400`, `401`, `404` |
| DELETE      | `204 No Content` | `401`, `404` |
| POST (auth) | `200 OK` (token)  | `400`, `401` |

---

## 8. Anti-Patterns

- **No `[ProducesResponseType]`**: Swagger shows no response schemas — consumers cannot understand the contract.
- **Raw integers** (`[ProducesResponseType(200)]`): Use `StatusCodes.Status200OK` for readability and refactoring safety.
- **`IActionResult` return type only**: Prefer `ActionResult<T>` or add typed `[ProducesResponseType(typeof(T), ...)]` to make the schema explicit.
- **Swagger in production**: `UseSwagger()` outside a development guard exposes internal API structure.
- **Missing XML documentation after enabling `GenerateDocumentationFile`**: Compiler warning 1591 on every undocumented member. Suppress globally via `<NoWarn>` in the `.csproj` as shown in section 2a.
