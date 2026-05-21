---
name: fix-issue-022-swaggergen-configuration-incomplete
description: Fix Issue 22 by implementing complete SwaggerGen configuration in Program.cs, including OpenApiInfo, JWT Bearer security, global security requirement, and XML comments integration.
temperature: 0.2
tags: ["fix", "issue", "swagger", "openapi", "program-cs"]
ai_generated: true
model: "openai/gpt-5.3-codex@unknown"
operator: "lawcarl"
chat_id: "create-prompt-fix-issue-22-20260521"
prompt: |
  create a prompt to fix issue 22
started: "2026-05-21T00:00:00Z"
ended: "2026-05-21T00:05:00Z"
task_durations:
  - task: "requirements alignment"
    duration: "00:02:00"
  - task: "prompt authoring"
    duration: "00:03:00"
total_duration: "00:05:00"
ai_log: "ai-logs/2026/05/21/create-prompt-fix-issue-22-20260521/conversation.md"
source: "github-copilot-chat"
owner: "Development Team"
version: "1.0.0"
prompt_metadata:
  id: fix-issue-022-swaggergen-configuration-incomplete
  title: Fix Issue 022 SwaggerGen Configuration Incomplete
  owner: lawcarl
  version: 1.0.0
  output_path: .github/prompts/fix-issue-022-swaggergen-configuration-incomplete.prompt.md
  category: implementation
  output_format: markdown
---

# Fix Issue 22: SwaggerGen Configuration Incomplete

## Context

Issue file: `issues/evergreen/022-swaggergen-configuration-incomplete.md`

The current Swagger setup in `Program.cs` is incomplete:

```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
```

The fix must satisfy repository guidance in:

- `.github/instructions/swagger-openapi.instructions.md`
- `.github/instructions/aspnet-core-api-design.instructions.md`
- `.github/instructions/csharp.instructions.md`

## Objective

Implement complete Swagger/OpenAPI configuration in `Program.cs` so Swagger UI includes API metadata, JWT Bearer authorization support, global security requirement, and XML comments.

## Required Changes

1. Replace the minimal `AddSwaggerGen()` call with `AddSwaggerGen(options => { ... })`.
2. Add `SwaggerDoc("v1", new OpenApiInfo { ... })` with:
   - `Title = "PostHub API"`
   - `Version = "v1"`
   - `Description = "A social posting platform API supporting users, posts, and comments."`
3. Add XML comments integration:
   - Build XML file name from executing assembly.
   - Build absolute path with `Path.Combine(AppContext.BaseDirectory, xmlFile)`.
   - Call `options.IncludeXmlComments(xmlPath);`
4. Add JWT Bearer security definition with `AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { ... })`:
   - `Type = SecuritySchemeType.Http`
   - `Scheme = "bearer"`
   - `BearerFormat = "JWT"`
   - `In = ParameterLocation.Header`
   - `Name = "Authorization"`
   - Include clear user-facing description for token entry.
5. Add global security requirement with `AddSecurityRequirement(...)` referencing the `Bearer` scheme.
6. Keep existing environment gating for Swagger middleware (`UseSwagger` / `UseSwaggerUI`) unless a separate issue explicitly requires changing it.

## Implementation Constraints

- Make the minimal targeted change required for Issue 22.
- Do not refactor unrelated startup or authentication code.
- Use explicit OpenAPI model types where clarity is needed.
- Keep code style consistent with existing `Program.cs` formatting.

## Verification Steps

1. Run build: `dotnet build`
2. Confirm build succeeds.
3. Run app and verify Swagger UI:
   - Title/version/description are visible.
   - **Authorize** button appears.
   - Protected endpoints show lock/authorization behavior.
   - Operation docs include XML comments when available.

## Acceptance Criteria

- [ ] `OpenApiInfo` configured with Title, Version, Description
- [ ] XML comments integrated with `IncludeXmlComments(...)`
- [ ] `AddSecurityDefinition("Bearer", ...)` implemented
- [ ] `AddSecurityRequirement(...)` implemented globally
- [ ] Project builds successfully
- [ ] Swagger UI shows metadata and **Authorize** button

## Output Format

Return:

1. A concise summary of what changed
2. The exact file modified
3. Validation results (`dotnet build` + UI checks)
4. Any follow-up needed if XML docs are not yet enabled in `.csproj`
