# Issue 009: Missing User Relationships in Models

**Priority**: 🔴 **Critical**  
**Category**: Architecture / Entity Framework Core  
**Status**: Open  
**Created**: 2026-05-21

## Summary

Post and Comment entities lack explicit `UserId` foreign key properties and User navigation properties. This breaks the ownership-based authorization architecture required by the project requirements and instruction files.

## Impact

- **Severity**: Critical
- **Affected Components**: 
  - Models/Post.cs
  - Models/Comment.cs
  - Data/ApplicationDbContext.cs
  - Services/Implementations/PostService.cs
  - Services/Implementations/CommentService.cs
  - Controllers/PostController.cs
  - Controllers/CommentController.cs
- **Scope**: Cannot verify user ownership for authorization checks

## Current Behavior

```csharp
// Post.cs - MISSING:
public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public DateTime CreationTime { get; set; }
    public int Likes { get; set; } = 0;
    public IList<Comment>? Comments { get; set; }
    
    // Missing:
    // public string UserId { get; set; }  // FK to AspNetUsers
    // public User? User { get; set; }      // Navigation
}

// Comment.cs - MISSING:
public class Comment
{
    public int Id { get; set; }
    public string Body { get; set; }
    public DateTime CreationTime { get; set; }
    public int PostId { get; set; }
    public Post Post { get; set; }
    
    // Missing:
    // public string UserId { get; set; }  // FK to AspNetUsers
    // public User? User { get; set; }      // Navigation
}
```

## Expected Behavior (Per Instructions)

Per **PROJECT-REQUIREMENTS.md** § Data Models:

```csharp
// Post entity relationships:
// - Many-to-One with User (each Post has one author)

// Comment entity relationships:
// - Many-to-One with User (each Comment has one author)
```

Per **entity-framework-core.instructions.md**:
- Models must explicitly define foreign key properties
- Navigation properties must be configured in DbContext
- Ownership checks require FK relationships to be present

## Required Changes

### 1. Models/Post.cs
Add UserId FK and User navigation:
```csharp
public string UserId { get; set; }  // FK to AspNetUsers
public User? User { get; set; }      // Navigation property
```

### 2. Models/Comment.cs
Add UserId FK and User navigation:
```csharp
public string UserId { get; set; }  // FK to AspNetUsers
public User? User { get; set; }      // Navigation property
```

### 3. Data/ApplicationDbContext.cs
Configure relationships in `OnModelCreating()`:
```csharp
// Post → User relationship
modelBuilder.Entity<Post>()
    .HasOne(p => p.User)
    .WithMany()
    .HasForeignKey(p => p.UserId)
    .OnDelete(DeleteBehavior.Cascade);

// Comment → User relationship
modelBuilder.Entity<Comment>()
    .HasOne(c => c.User)
    .WithMany()
    .HasForeignKey(c => c.UserId)
    .OnDelete(DeleteBehavior.Cascade);
```

### 4. Enable Database Migration
```powershell
dotnet ef migrations add AddUserRelationshipsToPostAndComment
dotnet ef database update
```

## Blocking Issues

- Post edit/delete authorization checks cannot verify ownership
- Comment edit/delete authorization checks cannot verify ownership
- No way to load user data alongside posts/comments for audit trails

## Related Instructions

- **PROJECT-REQUIREMENTS.md**: § Data Models, § Core Workflows, § Business Rules (User Ownership)
- **entity-framework-core.instructions.md**: Foreign key relationships, navigation properties
- **aspnet-core-api-design.instructions.md**: Authorization patterns

## Definition of Done

- [ ] UserId and User properties added to Post model
- [ ] UserId and User properties added to Comment model
- [ ] DbContext configured with FK and cascade delete
- [ ] Database migration created and applied
- [ ] PostService can access post.UserId for authorization
- [ ] CommentService can access comment.UserId for authorization
- [ ] All tests pass
