---
ai_generated: true
model: "anthropic/claude-sonnet-4-5@2026-05-21"
operator: "johnmillerATcodemag-com"
chat_id: "d4512947-565f-41e7-af98-8c001b78d4d6"
prompt: |
  Follow instructions in #prompt:SKILL.md with these arguments: create-automapper-instructions.prompt.md
started: "2026-05-21T00:00:00Z"
ended: "2026-05-21T00:05:00Z"
task_durations:
  - task: "codebase exploration"
    duration: "00:02:00"
  - task: "instruction authoring"
    duration: "00:03:00"
total_duration: "00:05:00"
ai_log: "ai-logs/2026/05/21/d4512947-565f-41e7-af98-8c001b78d4d6/conversation.md"
source: ".github/prompts/create-automapper-instructions.prompt.md"
name: automapper
description: "Use when creating or modifying AutoMapper profiles, DTOs, entity mappings, or configuring object mapping in this ASP.NET Core project. Covers Profile design, CreateMap patterns, IMapper injection, DTO conventions, and navigation property mapping."
applyTo: ["Profiles/**/*.cs", "Dtos/**/*.cs"]
version: "1.0.0"
author: "johnmillerATcodemag-com"
tags: ["automapper", "dto", "mapping", "profiles", "aspnet-core"]
owner: "Development Team"
reviewedDate: "2026-05-21"
nextReview: "2026-08-21"
---

# AutoMapper Instructions

## Overview

Apply these rules when creating or modifying AutoMapper `Profile` classes, DTO classes, or any code that configures or uses `IMapper` in this project.

## Core Conventions

### 1. Profile Structure

- One `Profile` subclass per entity, placed in `Profiles/` and named `<Entity>Profile.cs`.
- Declare all `CreateMap<>()` calls in the constructor only.
- Always define three mappings per entity: create-request → entity, edit-request → entity, entity → read-response.

```csharp
public class PostProfile : Profile
{
    public PostProfile()
    {
        CreateMap<CreatePostDto, Post>();
        CreateMap<EditPostDto, Post>();
        CreateMap<Post, ReadPostDto>();
    }
}
```

### 2. DTO Naming and Location

- Place DTOs under `Dtos/<Entity>/` and name them `Create<Entity>Dto`, `Edit<Entity>Dto`, `Read<Entity>Dto`.
- `Create<Entity>Dto` — input model for POST; include `[Required]` and `[StringLength]` validation attributes.
- `Edit<Entity>Dto` — input model for PUT; properties are optional (no `[Required]`), allowing partial updates.
- `Read<Entity>Dto` — output model for GET; includes `Id`, timestamps, computed fields, and nested read DTOs for navigation properties.

### 3. Naming Conventions Drive Automatic Mapping

AutoMapper maps properties by matching names (case-insensitive). Keep DTO property names identical to entity property names to avoid needing `.ForMember()` overrides. Only use `.ForMember()` when a name or shape difference is intentional.

### 4. Navigation Properties

- In `Read<Entity>Dto`, represent navigation collections as `IList<Read<Related>Dto>?` matching the entity's navigation type.
- AutoMapper resolves nested collections automatically when the nested type also has a registered `CreateMap<>()`.
- Always `Include()` navigation properties before mapping in the service layer—AutoMapper does not trigger lazy loading.

```csharp
// Correct: eagerly load before mapping
List<Post> posts = await context.Posts.Include(p => p.Comments).ToListAsync();
return mapper.Map<IEnumerable<ReadPostDto>>(posts);
```

### 5. IMapper Injection

- Inject `IMapper` via the primary constructor only; never use a static mapper.
- Declare `IMapper` as the last constructor parameter, after data context parameters.

```csharp
public class PostService(ApplicationDbContext context, IMapper mapper) : IPostService
```

### 6. Mapping Invocation

- Use `mapper.Map<TDestination>(source)` for single objects.
- Use `mapper.Map<IEnumerable<TDestination>>(sourceCollection)` for collections—never loop manually.
- Do not call `mapper.Map<TDestination>(source, destination)` (two-argument overload) for create operations; only use it to apply partial edits onto an existing entity.

### 7. Registration

- Register all profiles with `builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())` in `Program.cs`.
- Do not register profiles individually; assembly scanning keeps registration automatic.

### 8. Validation

- Add `mapper.ConfigurationProvider.AssertConfigurationIsValid()` in unit tests to catch unmapped property errors early.
- Do not suppress unmapped member warnings with `.ForAllMembers(opt => opt.Ignore())`—address missing mappings explicitly.

## What to Avoid

- Do not use `ReverseMap()` unless a true bidirectional mapping is required; prefer explicit one-way maps.
- Do not put business logic inside `.ForMember()` resolvers; move logic to the service layer before mapping.
- Do not map directly to entities in controllers; mapping belongs in the service layer.
- Do not create a `UserProfile`—`User` extends `IdentityUser` and is handled by ASP.NET Core Identity, not AutoMapper.
