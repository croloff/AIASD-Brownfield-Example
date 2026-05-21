# Issue 015: Missing [Produces] and [Consumes] Attributes on Controllers

**Priority**: 🟠 **Medium**  
**Category**: API Design / OpenAPI  
**Status**: Open  
**Created**: 2026-05-21

## Summary

Controller classes lack `[Produces]` and `[Consumes]` class-level attributes. This prevents Swagger from explicitly documenting supported content types and leaves ambiguity about API content negotiation.

## Impact

- **Severity**: Medium
- **Affected Components**: 
  - Controllers/PostController.cs (missing class-level attributes)
  - Controllers/CommentController.cs (missing class-level attributes)
  - Controllers/UserController.cs (missing class-level attributes)
- **Scope**: Swagger documentation clarity

## Current Behavior

```csharp
// PostController - NO [Produces] or [Consumes]
[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    // [HttpGet], [HttpPost], etc.
    // Swagger doesn't explicitly show which content types are supported
}
```

## Expected Behavior (Per Instructions)

Per **aspnet-core-api-design.instructions.md**:

All controllers must declare content types at class level:

```csharp
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]      // ✓ Responses are JSON
[Consumes("application/json")]      // ✓ Requests expect JSON
public class PostController : ControllerBase
{
    // ...
}
```

## What These Attributes Do

| Attribute | Purpose |
|---|---|
| `[Produces("application/json")]` | Declares that endpoints return JSON (Content-Type: application/json) |
| `[Consumes("application/json")]` | Declares that endpoints accept JSON request bodies (Content-Type: application/json) |

## Swagger Impact

**Before (without attributes)**:
```
POST /api/Post
Responses: [Shows response schema but no explicit content type]
```

**After (with attributes)**:
```
POST /api/Post
Consumes: application/json
Produces: application/json
Responses: [Shows response schema with explicit content type]
```

## Required Changes

### Controllers/PostController.cs

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
// ... other usings

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly IMapper _mapper;
    
    public PostController(IPostService postService, IMapper mapper)
    {
        _postService = postService;
        _mapper = mapper;
    }
    
    // ... existing methods
}
```

### Controllers/CommentController.cs

```csharp
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class CommentController : ControllerBase
{
    // ... existing methods
}
```

### Controllers/UserController.cs

```csharp
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class UserController : ControllerBase
{
    // ... existing methods
}
```

## Why This Matters

1. **Clarity**: API consumers see exactly what content types are supported
2. **Validation**: ASP.NET Core can validate request content types automatically
3. **Documentation**: Swagger/OpenAPI explicitly shows content negotiation
4. **Consistency**: Signals that all endpoints use the same content type

## Notes

- All endpoints in this API use JSON exclusively
- No XML, CSV, or other content types are supported
- These attributes improve documentation clarity without changing behavior

## Related Instructions

- **aspnet-core-api-design.instructions.md**: Content negotiation and controller attributes
- **swagger-openapi.instructions.md**: OpenAPI documentation attributes

## Definition of Done

- [ ] PostController has [Produces("application/json")] and [Consumes("application/json")]
- [ ] CommentController has [Produces("application/json")] and [Consumes("application/json")]
- [ ] UserController has [Produces("application/json")] and [Consumes("application/json")]
- [ ] Swagger shows Produces/Consumes for all endpoints
- [ ] All tests pass
