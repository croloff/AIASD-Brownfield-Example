# Issue 013: EditPost Returns 200 OK Instead of 204 No Content

**Priority**: 🟡 **High**  
**Category**: API Design / REST Semantics  
**Status**: Open  
**Created**: 2026-05-21

## Summary

The PostController.EditPost endpoint returns `200 OK` with response body instead of `204 No Content` with no body. This violates REST semantics and is inconsistent with the DELETE endpoint which correctly returns `204`.

## Impact

- **Severity**: High
- **Affected Components**: 
  - Controllers/PostController.cs::EditPost()
- **Scope**: Violates REST conventions; inconsistent with DELETE behavior

## Current Behavior

```csharp
// PostController.EditPost - INCORRECT
[HttpPut("{id}")]
[Authorize]
public async Task<IActionResult> EditPost(int id, EditPostDto editPostDto)
{
    // ...
    var editedPost = await _postService.EditPostAsync(id, editPostDto);
    return Ok(editedPost);  // ❌ Returns 200 with response body
}
```

## Expected Behavior (Per Instructions)

Per **aspnet-core-api-design.instructions.md**:
- `PUT` operations (full resource replacement) should return `204 No Content`
- No response body is sent with 204
- Client knows update succeeded by status code alone

Per **PROJECT-REQUIREMENTS.md** § API Capabilities:
```
Edit Post Response:
200 OK - Post updated successfully
```

**Note**: The requirements document shows 200 OK, but this contradicts REST conventions and the instruction file. Per instruction precedence, the instruction file takes priority.

## Why This Matters

### REST Convention
- **200 OK**: Successful request with response body containing the modified resource
- **204 No Content**: Successful request with no response body (fire-and-forget)

For PUT operations that don't return the updated resource to the client, **204** is more semantically correct.

### API Consistency
- DELETE /api/Post/{id} returns **204 No Content** ✓
- PUT /api/Post/{id} returns **200 OK** with body ✗ (inconsistent)
- PUT /api/Comment/{id} returns **200 OK** with body ✗ (inconsistent)

## Required Changes

### Controllers/PostController.cs

```csharp
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
        return Unauthorized();
    
    await _postService.EditPostAsync(id, editPostDto, userId);
    
    return NoContent();  // ✓ Returns 204 with no body
}
```

### Controllers/CommentController.cs

Apply the same fix to EditComment:

```csharp
[HttpPut("{id}")]
[Authorize]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> EditComment(int id, EditCommentDto editCommentDto)
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    
    if (userId == null)
        return Unauthorized();
    
    await _commentService.EditCommentAsync(id, editCommentDto, userId);
    
    return NoContent();  // ✓ Returns 204 with no body
}
```

## Breaking Changes

**Potential Client Impact**: Clients expecting response body will need updating:

**Before (200 OK with body)**:
```javascript
const response = await fetch('/api/post/1', {
    method: 'PUT',
    body: JSON.stringify(updatedPost)
});
const updatedPost = await response.json();  // ✓ Works with 200
```

**After (204 No Content)**:
```javascript
const response = await fetch('/api/post/1', {
    method: 'PUT',
    body: JSON.stringify(updatedPost)
});
// No response body to parse
if (response.ok) {
    console.log('Post updated successfully');
}
```

## Related Instructions

- **aspnet-core-api-design.instructions.md**: Status codes and REST semantics
- **PROJECT-REQUIREMENTS.md**: § Core Workflows (Edit Post)

## Definition of Done

- [ ] EditPost returns 204 No Content
- [ ] EditComment returns 204 No Content
- [ ] No response body sent with 204 status
- [ ] Swagger shows 204 as response status
- [ ] All tests updated to expect 204 instead of 200
- [ ] Consistency: DELETE and PUT both return 204
- [ ] Client applications updated if necessary
