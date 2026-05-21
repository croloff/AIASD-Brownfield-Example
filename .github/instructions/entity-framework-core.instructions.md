---
ai_generated: true
model: "openai/gpt-5.3-codex@unknown"
operator: "lawcarl"
chat_id: "create-entity-framework-core-instructions-20260521"
prompt: |
  Create a comprehensive `.github/instructions/entity-framework-core.instructions.md` file
  that covers EF Core DbContext setup, entity design, querying, mutations,
  relationship management, testing patterns, common pitfalls, and migration guidance
  for PostHubAPI.
started: "2026-05-21T19:10:00Z"
ended: "2026-05-21T19:22:00Z"
task_durations:
  - task: "requirements alignment"
    duration: "00:03:00"
  - task: "instruction authoring"
    duration: "00:07:00"
  - task: "traceability updates"
    duration: "00:02:00"
total_duration: "00:12:00"
ai_log: "ai-logs/2026/05/21/create-entity-framework-core-instructions-20260521/conversation.md"
source: ".github/prompts/create-entity-framework-core-instructions.prompt.md"
name: entity-framework-core
description: Use when implementing or modifying Entity Framework Core DbContext configuration, entity relationships, LINQ queries, save operations, and EF Core testing in PostHubAPI
applyTo: Data/*.cs,Models/*.cs,Services/Implementations/*.cs,PostHubAPI.Tests/**/*.cs
version: "1.0.0"
author: "lawcarl"
tags: ["entity-framework-core", "data-access", "orm", "dotnet"]
owner: "Development Team"
reviewedDate: "2026-05-21"
nextReview: "2026-08-21"
---

# Entity Framework Core Instructions

## Overview

Use these instructions when implementing or modifying EF Core behavior in PostHubAPI. Apply this guidance to DbContext configuration, entities, services, and tests so behavior is consistent across environments.

## 1. DbContext Configuration

- Register `ApplicationDbContext` once in DI and centralize provider selection in `Program.cs`.
- Keep provider behavior environment-specific:
  - Development: `UseInMemoryDatabase(...)`
  - Non-development: `UseSqlite(...)` using `ConnectionStrings:DefaultConnection`
- Do not hardcode connection strings or secrets; resolve them from configuration and user-secrets/environment variables.
- Ensure production startup applies migrations automatically when required by deployment strategy.
- Keep seed logic deterministic and idempotent for local/dev scenarios.

## 2. Entity Design

- Keep entities focused on persistence concerns; business rules belong in services.
- Use explicit foreign keys plus navigation properties for relationships.
- Model timestamps consistently (`CreatedAt`, `UpdatedAt`) and set defaults explicitly.
- Favor Fluent API in `OnModelCreating` for relationship and cascade behavior to keep rules centralized.
- Use nullable reference types deliberately; avoid ambiguous optional/required semantics.

## 3. Queries and LINQ

- Compose queries with LINQ-to-Entities only; avoid materializing early unless needed.
- Use `Include` and `ThenInclude` for known related data to prevent N+1 patterns.
- Use `AsNoTracking()` for read-only query paths.
- Project to DTOs in query pipelines where practical to reduce loaded columns.
- Keep filters, ordering, and pagination server-side before `ToListAsync()`.

## 4. Mutations and Save Operations

- Use async mutation APIs (`AddAsync`, `SaveChangesAsync`) for web request flows.
- For updates, load tracked entity, modify explicit fields, then persist.
- For deletes, verify entity existence first and enforce ownership/authorization before removal.
- Group multi-entity writes in transactions when atomicity is required.
- Handle concurrency explicitly when adding versioning in future updates.

## 5. Relationship Management

- Maintain one-to-many patterns used by the domain:
  - User -> Posts
  - Post -> Comments
  - User -> Comments
- Configure delete behavior intentionally; avoid accidental data loss from cascade chains.
- Prefer eager loading when a service requires predictable graph shape.
- Use explicit loading only when conditional relationship loading is truly necessary.

## 6. Testing Patterns

- Use InMemory provider for service-level tests requiring lightweight persistence behavior.
- Create isolated database names per test to prevent cross-test pollution.
- Seed only the minimum data needed for assertions.
- Assert both returned DTOs and resulting database state when validating mutations.
- Use controller tests for authorization and route semantics; keep EF behavior tests in service/data-layer tests.

## 7. Common Pitfalls to Avoid

- Missing `Include` statements for related data needed by response mapping.
- Relying on tracked state unintentionally across service methods.
- Mixing sync and async EF APIs in request paths.
- Calling `SaveChangesAsync` multiple times inside one logical write operation.
- Assuming InMemory behavior matches relational providers for all query semantics.

## 8. Migration and Schema Evolution

- Keep migration commands explicit and review generated SQL-impact changes.
- Apply migrations in non-development environments as part of deployment/startup policy.
- Never modify historical migration files after they ship to shared environments.
- Plan destructive schema changes with backward-compatible rollout steps.

## 9. Review Checklist

- [ ] Query paths avoid N+1 and use `AsNoTracking()` where read-only.
- [ ] Relationship loading strategy is explicit and justified.
- [ ] Mutations are async and persist changes exactly once per unit of work.
- [ ] Provider-specific behavior is considered for development vs production.
- [ ] Tests isolate data and validate persisted outcomes.

## Related References

- `.github/instructions/csharp.instructions.md`
- `.github/instructions/aspnet-core-api-design.instructions.md`
- `Data/ApplicationDbContext.cs`
- `Models/User.cs`
- `Models/Post.cs`
- `Models/Comment.cs`
- `Services/Implementations/PostService.cs`
- `Services/Implementations/CommentService.cs`
- `docs/architecture.md`
