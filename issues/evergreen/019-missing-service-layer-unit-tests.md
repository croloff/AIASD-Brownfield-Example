# Issue 019: Missing Service-Layer Unit Tests

**Priority**: 🟡 **High Gap**  
**Category**: Testing / Quality  
**Status**: Open  
**Created**: 2026-05-21

## Summary

No unit tests exist for PostService and CommentService business logic. Only controller and configuration tests are present. Service-layer tests are essential for validating business rules in isolation.

## Impact

- **Severity**: High (Test Coverage Gap)
- **Affected Components**: 
  - Services/Implementations/PostService.cs (no tests)
  - Services/Implementations/CommentService.cs (no tests)
  - Existing test suites incomplete
- **Scope**: Business logic validation

## Current Test Coverage

| Component | Tests | Status |
|---|---|---|
| JwtSettingsResolver | ✓ JwtSettingsResolverTests.cs | Complete |
| PostController | ✓ PostControllerAuthorizationTests.cs | Partial |
| UserController | ✓ UserControllerTests.cs | Partial |
| PostService | ✗ Missing | 🔴 Gap |
| CommentService | ✗ Missing | 🔴 Gap |

## Why Service Tests Matter

### Unit Testing Pyramid
```
         /\
        /  \  E2E Integration Tests
       /____\
      /      \
     / Service \  ← Missing
    /   Tests   \
   /____________\
  /              \
 /  Unit Tests    \  ← Faster, more focused
/__________________\
```

### Benefits
1. **Isolation**: Test business logic without HTTP layer
2. **Speed**: Unit tests run 100x faster than integration tests
3. **Clarity**: Error messages show exactly which rule failed
4. **Maintainability**: Easy to refactor service without updating many controller tests

## Required Test Classes

### PostHubAPI.Tests/Services/PostServiceTests.cs

```csharp
using Xunit;
using Moq;
using PostHubAPI.Data;
using PostHubAPI.Models;
using PostHubAPI.Dtos.Post;
using PostHubAPI.Services.Implementations;
using PostHubAPI.Exceptions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace PostHubAPI.Tests.Services
{
    public class PostServiceTests
    {
        private readonly IPostService _postService;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        
        public PostServiceTests()
        {
            // Setup in-memory DB
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            
            // Setup AutoMapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new PostProfile());
            });
            _mapper = config.CreateMapper();
            
            _postService = new PostService(_context, _mapper);
        }
        
        // Test methods below
    }
}
```

### Test Methods for PostService

#### GetAllPostsAsync Tests

```csharp
[Fact]
public async Task GetAllPostsAsync_ReturnsAllPosts_WhenPostsExist()
{
    // Arrange
    var posts = new List<Post>
    {
        new Post { Id = 1, Title = "Post 1", Body = "Body 1", Likes = 0, CreationTime = DateTime.Now, UserId = "user1" },
        new Post { Id = 2, Title = "Post 2", Body = "Body 2", Likes = 5, CreationTime = DateTime.Now, UserId = "user2" }
    };
    _context.Posts.AddRange(posts);
    await _context.SaveChangesAsync();
    
    // Act
    var result = await _postService.GetAllPostsAsync();
    
    // Assert
    Assert.NotEmpty(result);
    Assert.Equal(2, result.Count());
}

[Fact]
public async Task GetAllPostsAsync_ReturnsEmpty_WhenNoPosts()
{
    // Act
    var result = await _postService.GetAllPostsAsync();
    
    // Assert
    Assert.Empty(result);
}

[Fact]
public async Task GetAllPostsAsync_IncludesComments_ForEachPost()
{
    // Arrange
    var post = new Post { Id = 1, Title = "Post 1", Body = "Body 1", Likes = 0, CreationTime = DateTime.Now, UserId = "user1" };
    var comment = new Comment { Id = 1, Body = "Comment 1", CreationTime = DateTime.Now, PostId = 1, UserId = "user2" };
    post.Comments = new List<Comment> { comment };
    
    _context.Posts.Add(post);
    await _context.SaveChangesAsync();
    
    // Act
    var result = await _postService.GetAllPostsAsync();
    
    // Assert
    Assert.NotEmpty(result.First().Comments);
}
```

#### GetPostByIdAsync Tests

```csharp
[Fact]
public async Task GetPostByIdAsync_ReturnsPost_WhenPostExists()
{
    // Arrange
    var post = new Post { Id = 1, Title = "Test Post", Body = "Test Body", Likes = 0, CreationTime = DateTime.Now, UserId = "user1" };
    _context.Posts.Add(post);
    await _context.SaveChangesAsync();
    
    // Act
    var result = await _postService.GetPostByIdAsync(1);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal("Test Post", result.Title);
}

[Fact]
public async Task GetPostByIdAsync_ThrowsNotFoundException_WhenPostNotFound()
{
    // Act & Assert
    await Assert.ThrowsAsync<NotFoundException>(
        () => _postService.GetPostByIdAsync(999)
    );
}
```

#### CreatePostAsync Tests

```csharp
[Fact]
public async Task CreatePostAsync_CreatesPost_WithValidInput()
{
    // Arrange
    var createDto = new CreatePostDto { Title = "New Post", Body = "New Body" };
    var userId = "user1";
    
    // Act
    var result = await _postService.CreatePostAsync(createDto, userId);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal("New Post", result.Title);
    Assert.Equal(0, result.Likes);
}

[Fact]
public async Task CreatePostAsync_InitializesLikesToZero_ForNewPost()
{
    // Arrange
    var createDto = new CreatePostDto { Title = "Post", Body = "Body" };
    
    // Act
    var result = await _postService.CreatePostAsync(createDto, "user1");
    
    // Assert
    Assert.Equal(0, result.Likes);
}
```

#### EditPostAsync Tests

```csharp
[Fact]
public async Task EditPostAsync_UpdatesPost_WithValidInput()
{
    // Arrange
    var post = new Post { Id = 1, Title = "Old", Body = "Old", Likes = 0, CreationTime = DateTime.Now, UserId = "user1" };
    _context.Posts.Add(post);
    await _context.SaveChangesAsync();
    
    var editDto = new EditPostDto { Title = "New", Body = "New" };
    
    // Act
    var result = await _postService.EditPostAsync(1, editDto, "user1");
    
    // Assert
    Assert.Equal("New", result.Title);
    Assert.Equal("New", result.Body);
}

[Fact]
public async Task EditPostAsync_PreservesCreationTime_WhenEditing()
{
    // Arrange
    var originalTime = new DateTime(2026, 1, 1, 10, 0, 0);
    var post = new Post { Id = 1, Title = "Old", Body = "Old", Likes = 0, CreationTime = originalTime, UserId = "user1" };
    _context.Posts.Add(post);
    await _context.SaveChangesAsync();
    
    var editDto = new EditPostDto { Title = "New", Body = "New" };
    
    // Act
    var result = await _postService.EditPostAsync(1, editDto, "user1");
    
    // Assert
    Assert.Equal(originalTime, result.CreationTime);
}

[Fact]
public async Task EditPostAsync_ThrowsNotFoundException_WhenPostNotFound()
{
    // Arrange
    var editDto = new EditPostDto { Title = "New", Body = "New" };
    
    // Act & Assert
    await Assert.ThrowsAsync<NotFoundException>(
        () => _postService.EditPostAsync(999, editDto, "user1")
    );
}
```

#### DeletePostAsync Tests

```csharp
[Fact]
public async Task DeletePostAsync_DeletesPost_WhenPostExists()
{
    // Arrange
    var post = new Post { Id = 1, Title = "Post", Body = "Body", Likes = 0, CreationTime = DateTime.Now, UserId = "user1" };
    _context.Posts.Add(post);
    await _context.SaveChangesAsync();
    
    // Act
    await _postService.DeletePostAsync(1);
    
    // Assert
    var deleted = await _context.Posts.FirstOrDefaultAsync(p => p.Id == 1);
    Assert.Null(deleted);
}

[Fact]
public async Task DeletePostAsync_CascadesDelete_ToAssociatedComments()
{
    // Arrange
    var post = new Post { Id = 1, Title = "Post", Body = "Body", Likes = 0, CreationTime = DateTime.Now, UserId = "user1" };
    var comment = new Comment { Id = 1, Body = "Comment", CreationTime = DateTime.Now, PostId = 1, UserId = "user2" };
    post.Comments = new List<Comment> { comment };
    
    _context.Posts.Add(post);
    await _context.SaveChangesAsync();
    
    // Act
    await _postService.DeletePostAsync(1);
    
    // Assert
    var deletedComment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == 1);
    Assert.Null(deletedComment);
}

[Fact]
public async Task DeletePostAsync_ThrowsNotFoundException_WhenPostNotFound()
{
    // Act & Assert
    await Assert.ThrowsAsync<NotFoundException>(
        () => _postService.DeletePostAsync(999)
    );
}
```

### CommentServiceTests.cs

Create similar tests for CommentService:
- GetCommentAsync (success, not found)
- GetCommentsByPostAsync (success, not found)
- CreateCommentAsync (success, post not found)
- EditCommentAsync (success, not found, ownership)
- DeleteCommentAsync (success, not found, ownership)

## Related Instructions

- **xunit-testing.instructions.md**: Unit test patterns and structure
- **entity-framework-core.instructions.md**: Testing with in-memory database
- **evergreen-software-development.instructions.md**: § Build with Defensive Quality

## Definition of Done

- [ ] PostServiceTests.cs created with 15+ test methods
- [ ] CommentServiceTests.cs created with 15+ test methods
- [ ] All CRUD operations tested (Create, Read, Update, Delete)
- [ ] Error cases tested (not found, validation)
- [ ] Business rules validated (ownership, cascade delete)
- [ ] In-memory database used for test isolation
- [ ] AutoMapper configured for tests
- [ ] All tests pass
- [ ] Test coverage increased to >70% for service layer
