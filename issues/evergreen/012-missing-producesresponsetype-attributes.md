# Issue 012: Missing [ProducesResponseType] Attributes on Controller Actions

**Priority**: 🟡 **High**  
**Category**: API Design / OpenAPI  
**Status**: Open  
**Created**: 2026-05-21

## Summary

No controller actions declare `[ProducesResponseType]` attributes. This prevents Swagger from showing response status codes and schemas, making the API contract unclear to consumers.

## Impact

- **Severity**: High
- **Affected Components**: 
  - Controllers/PostController.cs (all actions)
  - Controllers/CommentController.cs (all actions)
  - Controllers/UserController.cs (Register, Login)
- **Scope**: Swagger shows no response information

## Current Behavior

```csharp
// PostController - NO ProducesResponseType
[HttpGet]
public async Task<IActionResult> GetAllPosts()
{
    var posts = await _postService.GetAllPostsAsync();
    return Ok(posts);
}

// Swagger sees only: status 200, no schema
```

## Expected Behavior (Per Instructions)

Per **aspnet-core-api-design.instructions.md**:

All actions must declare response types:
```csharp
[HttpGet]
[ProducesResponseType(typeof(IEnumerable<ReadPostDto>), StatusCodes.Status200OK)]
public async Task<ActionResult<IEnumerable<ReadPostDto>>> GetAllPosts()
{
    var posts = await _postService.GetAllPostsAsync();
    return Ok(posts);
}
```

## Required Changes

### PostController Actions

| Action | Required [ProducesResponseType] |
|---|---|
| GetAllPosts | 200 (IEnumerable<ReadPostDto>) |
| GetPostById | 200 (ReadPostDto), 404 |
| CreatePost | 201 (int), 400, 401 |
| EditPost | 204, 400, 401, 403, 404 |
| DeletePost | 204, 401, 403, 404 |

### CommentController Actions

| Action | Required [ProducesResponseType] |
|---|---|
| GetComment | 200 (ReadCommentDto), 404 |
| GetCommentsByPost | 200 (IEnumerable<ReadCommentDto>), 404 |
| CreateNewComment | 201 (int), 400, 401, 404 |
| EditComment | 204, 400, 401, 403, 404 |
| DeleteComment | 204, 401, 403, 404 |

### UserController Actions

| Action | Required [ProducesResponseType] |
|---|---|
| Register | 201 (string), 400 |
| Login | 200 (string), 401 |

## Example Implementation

```csharp
[HttpPost]
[ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public async Task<ActionResult<int>> CreatePost(CreatePostDto createPostDto)
{
    var postId = await _postService.CreatePostAsync(createPostDto, User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    return Created($"/api/post/{postId}", postId);
}
```

## Swagger Impact

**Before (without [ProducesResponseType])**:
```
GET /api/Post
[empty response section]
```

**After (with [ProducesResponseType])**:
```
GET /api/Post
Responses:
  200 OK
    [schema showing IEnumerable<ReadPostDto>]
```

## Related Instructions

- **aspnet-core-api-design.instructions.md**: Response type declarations
- **swagger-openapi.instructions.md**: ProducesResponseType attributes
- **PROJECT-REQUIREMENTS.md**: § API Capabilities (endpoints)

## Definition of Done

- [ ] All 5 PostController actions have [ProducesResponseType]
- [ ] All 5 CommentController actions have [ProducesResponseType]
- [ ] All 2 UserController actions have [ProducesResponseType]
- [ ] Status codes match actual controller responses
- [ ] DTOs specified for 200/201 responses
- [ ] Swagger shows response schemas for all endpoints
- [ ] All tests pass
