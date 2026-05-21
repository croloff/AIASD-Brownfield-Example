# Issue 016: Edit DTOs Require All Fields (No Partial Updates)

**Priority**: 🟠 **Medium**  
**Category**: API Design / DTO Validation  
**Status**: Open  
**Created**: 2026-05-21

## Summary

Edit DTOs (EditPostDto, EditCommentDto) have `[Required]` attributes that force all fields to be present. This prevents implementing partial update semantics. PATCH-style endpoints (which are not yet implemented) would require making fields optional.

## Impact

- **Severity**: Medium
- **Affected Components**: 
  - Dtos/Post/EditPostDto.cs
  - Dtos/Comment/EditCommentDto.cs
- **Scope**: Edit endpoint flexibility; future PATCH support

## Current Behavior

```csharp
// EditCommentDto - FORCES all fields
public class EditCommentDto
{
    [Required]                    // ❌ Forces Body to be present
    [StringLength(80)]
    public string Body { get; set; }
}

// EditPostDto - Missing [Required] but implies requirement
public class EditPostDto
{
    public string? Title { get; set; }
    public string? Body { get; set; }
}
```

## Expected Behavior (Per Instructions)

Per **automapper.instructions.md**:
> Edit DTOs should support partial updates where applicable. Fields should be optional to allow clients to update only what they need.

### Current API Style: PUT (Full Update)
```csharp
public class EditPostDto
{
    [Required]
    public string Title { get; set; }     // Must provide
    
    [Required]
    public string Body { get; set; }      // Must provide
}
```

### Future API Style: PATCH (Partial Update)
```csharp
public class EditPostDto
{
    public string? Title { get; set; }    // Optional - only update if provided
    public string? Body { get; set; }     // Optional - only update if provided
}
```

## Required Changes

### Dtos/Post/EditPostDto.cs

```csharp
public class EditPostDto
{
    // Make fields nullable and remove [Required]
    [StringLength(300)]
    public string? Title { get; set; }
    
    [StringLength(5000)]
    public string? Body { get; set; }
}
```

### Dtos/Comment/EditCommentDto.cs

```csharp
public class EditCommentDto
{
    // Remove [Required], make field nullable
    [StringLength(80)]
    public string? Body { get; set; }
}
```

## Service Layer Logic

Update PostService and CommentService to only update provided fields:

```csharp
// PostService.EditPostAsync - UPDATED
public async Task<ReadPostDto> EditPostAsync(int id, EditPostDto editPostDto)
{
    var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
    
    if (post == null)
        throw new NotFoundException("Post not found");
    
    // Only update provided fields
    if (!string.IsNullOrWhiteSpace(editPostDto.Title))
        post.Title = editPostDto.Title;
    
    if (!string.IsNullOrWhiteSpace(editPostDto.Body))
        post.Body = editPostDto.Body;
    
    // CreationTime is never modified
    
    _context.Posts.Update(post);
    await _context.SaveChangesAsync();
    
    return _mapper.Map<ReadPostDto>(post);
}
```

## Validation Considerations

Since fields are now optional, add validation to ensure at least one field is provided:

```csharp
// In PostController.EditPost
if (editPostDto.Title == null && editPostDto.Body == null)
{
    return BadRequest(new { message = "At least one field (Title or Body) must be provided" });
}
```

Or add a custom validator:

```csharp
public class EditPostDtoValidator : AbstractValidator<EditPostDto>
{
    public EditPostDtoValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(300)
            .When(x => x.Title != null);
        
        RuleFor(x => x.Body)
            .MaximumLength(5000)
            .When(x => x.Body != null);
        
        // At least one field required
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Title) || !string.IsNullOrWhiteSpace(x.Body))
            .WithMessage("At least one field (Title or Body) must be provided");
    }
}
```

## Current API Semantics

**PUT** (Full replacement) typically requires all fields:
```json
{
  "title": "Updated Title",
  "body": "Updated body content"
}
```

**PATCH** (Partial update) allows selective fields:
```json
{
  "title": "Updated Title only"
}
```

Currently, the API uses **PUT** semantics but forces full replacement. Making fields optional prepares for PATCH support in the future.

## Examples

### Current (Forces both fields):
```bash
curl -X PUT /api/Post/1 \
  -H "Content-Type: application/json" \
  -d '{"title": "New Title"}'
# Error: Body is required
```

### After Fix (Allows partial update):
```bash
curl -X PUT /api/Post/1 \
  -H "Content-Type: application/json" \
  -d '{"title": "New Title"}'
# Success: Updates only title, preserves body
```

## Related Instructions

- **automapper.instructions.md**: DTO design patterns
- **aspnet-core-api-design.instructions.md**: Request validation

## Definition of Done

- [ ] EditPostDto fields are nullable (string?)
- [ ] EditCommentDto fields are nullable (string?)
- [ ] [Required] attributes removed from Edit DTOs
- [ ] Service methods updated to handle partial updates
- [ ] Validation ensures at least one field is provided
- [ ] Tests updated for partial update scenarios
- [ ] API consumers can now update individual fields
