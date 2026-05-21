# Issue 018: Exception Messages Exposed to Clients

**Priority**: 🟠 **Medium**  
**Category**: API Design / Security  
**Status**: Open  
**Created**: 2026-05-21

## Summary

Controllers return raw exception messages to clients (e.g., `NotFound(exception.Message)`). This exposes internal implementation details and stack traces, violating API design best practices.

## Impact

- **Severity**: Medium
- **Affected Components**: 
  - Controllers/PostController.cs (GetPostById, EditPost, DeletePost)
  - Controllers/CommentController.cs (GetComment, EditComment, DeleteComment, GetCommentsByPost)
- **Scope**: Security and API contract clarity

## Current Behavior

```csharp
// PostController - EXPOSES exception details
[HttpGet("{id}")]
public async Task<IActionResult> GetPostById(int id)
{
    try
    {
        var post = await _postService.GetPostByIdAsync(id);
        return Ok(post);
    }
    catch (NotFoundException ex)
    {
        return NotFound(ex.Message);  // ❌ Returns raw exception message
    }
}

// Actual response to client:
// Status: 404 Not Found
// Body: "Post with ID 5 not found in database"
//       ^ Internal implementation detail exposed
```

## Expected Behavior (Per Instructions)

Per **aspnet-core-api-design.instructions.md**:
> Error responses should use generic, user-friendly messages. Never expose internal exception details or stack traces to clients.

```csharp
// PostController - GENERIC error message
[HttpGet("{id}")]
public async Task<IActionResult> GetPostById(int id)
{
    try
    {
        var post = await _postService.GetPostByIdAsync(id);
        return Ok(post);
    }
    catch (NotFoundException)
    {
        // ✓ Returns generic client-friendly message
        return NotFound(new { message = "Post not found" });
    }
}

// Response to client:
// Status: 404 Not Found
// Body: {"message": "Post not found"}
//       ^ Generic, no internal details
```

## Why This Matters

### Security Concerns
- **Information Disclosure**: Exception messages may reveal DB schema, table names, etc.
- **Attack Surface**: Detailed errors help attackers understand system architecture
- **Privacy**: Error details could contain sensitive data (user emails, IDs, etc.)

### API Contract
- **Consistency**: All 404 errors should have similar messages
- **Stability**: Internal error messages change with refactoring; API should not depend on them
- **Professionalism**: User-facing APIs should have polished error messages

## Required Changes

### Error Response Pattern

Create a standard error response object:

```csharp
public class ErrorResponse
{
    public string Message { get; set; }
    public string? ErrorCode { get; set; }
    public Dictionary<string, string[]>? ValidationErrors { get; set; }
}
```

### Controllers/PostController.cs

```csharp
[HttpGet("{id}")]
[ProducesResponseType(typeof(ReadPostDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult<ReadPostDto>> GetPostById(int id)
{
    try
    {
        var post = await _postService.GetPostByIdAsync(id);
        return Ok(post);
    }
    catch (NotFoundException)
    {
        // ✓ Generic client-friendly message
        return NotFound(new { message = "Post not found" });
    }
}

[HttpPut("{id}")]
[Authorize]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> EditPost(int id, EditPostDto editPostDto)
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    
    if (userId == null)
        return Unauthorized(new { message = "Authentication required" });
    
    try
    {
        var post = await _postService.GetPostByIdAsync(id);
        
        // Ownership check (requires Post to have UserId property - Issue #009)
        if (post.UserId != userId)
            return Forbid();
        
        await _postService.EditPostAsync(id, editPostDto, userId);
        return NoContent();
    }
    catch (NotFoundException)
    {
        // ✓ Generic message
        return NotFound(new { message = "Post not found" });
    }
}

[HttpDelete("{id}")]
[Authorize]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> DeletePost(int id)
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    
    if (userId == null)
        return Unauthorized(new { message = "Authentication required" });
    
    try
    {
        var post = await _postService.GetPostByIdAsync(id);
        
        // Ownership check (requires Post to have UserId property - Issue #009)
        if (post.UserId != userId)
            return Forbid();
        
        await _postService.DeletePostAsync(id);
        return NoContent();
    }
    catch (NotFoundException)
    {
        // ✓ Generic message
        return NotFound(new { message = "Post not found" });
    }
}
```

### Controllers/CommentController.cs

Apply the same pattern to all CommentController actions:

```csharp
[HttpGet("{id}")]
[ProducesResponseType(typeof(ReadCommentDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult<ReadCommentDto>> GetComment(int id)
{
    try
    {
        var comment = await _commentService.GetCommentAsync(id);
        return Ok(comment);
    }
    catch (NotFoundException)
    {
        return NotFound(new { message = "Comment not found" });
    }
}

[HttpGet("post/{postId}")]
[ProducesResponseType(typeof(IEnumerable<ReadCommentDto>), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult<IEnumerable<ReadCommentDto>>> GetCommentsByPost(int postId)
{
    try
    {
        var comments = await _commentService.GetCommentsByPostAsync(postId);
        return Ok(comments);
    }
    catch (NotFoundException)
    {
        return NotFound(new { message = "Post not found" });
    }
}

// Similar updates for CreateNewComment, EditComment, DeleteComment
```

## Error Message Guidelines

| Scenario | Response | Message |
|---|---|---|
| Resource not found | 404 | "Post not found" or "Comment not found" |
| Invalid request | 400 | "Invalid request" or field-specific validation messages |
| Unauthorized | 401 | "Authentication required" |
| Forbidden | 403 | "Access denied" |
| Server error | 500 | "An error occurred" |

## Logging Internal Details

While hiding error details from clients, log them for debugging:

```csharp
[HttpGet("{id}")]
public async Task<ActionResult<ReadPostDto>> GetPostById(int id)
{
    try
    {
        var post = await _postService.GetPostByIdAsync(id);
        return Ok(post);
    }
    catch (NotFoundException ex)
    {
        // ✓ Log internal details
        _logger.LogWarning($"Post not found. ID: {id}, Error: {ex.Message}");
        
        // ✓ Return generic message to client
        return NotFound(new { message = "Post not found" });
    }
    catch (Exception ex)
    {
        // ✓ Log unexpected errors
        _logger.LogError($"Unexpected error retrieving post {id}: {ex}");
        
        // ✓ Return generic message to client
        return StatusCode(500, new { message = "An error occurred" });
    }
}
```

## Related Instructions

- **aspnet-core-api-design.instructions.md**: Error handling and response design
- **evergreen-software-development.instructions.md**: § Design for Security and Privacy

## Definition of Done

- [ ] No NotFoundException messages passed directly to clients
- [ ] No exception.Message included in API responses
- [ ] All error responses use generic, user-friendly messages
- [ ] ErrorResponse structure defined for consistency
- [ ] Detailed errors logged server-side with _logger
- [ ] All affected endpoints updated (Post, Comment)
- [ ] Tests verify generic error messages
- [ ] Swagger documentation shows error response schemas
