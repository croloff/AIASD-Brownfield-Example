# Critical Violations Remediation Plan

**Document Date**: May 21, 2026  
**Document Type**: Remediation Strategy with Provenance  
**Compliance Basis**: Instruction files analysis  
**Status**: Proposed (Awaiting Review & Approval)

---

## Table of Contents

1. [Executive Summary](#executive-summary)
2. [Remediation Sequencing](#remediation-sequencing)
3. [Detailed Action Items](#detailed-action-items)
4. [Implementation Strategy](#implementation-strategy)
5. [Testing Plan](#testing-plan)
6. [Rollback Plan](#rollback-plan)
7. [Provenance Documentation](#provenance-documentation)

---

## Executive Summary

Seven critical violations identified in the PostHubAPI codebase require immediate remediation. These violations breach explicit instruction requirements and create architectural, security, and operational risks.

**Total Violations**: 7 Critical  
**Estimated Implementation Time**: 2-3 hours  
**Risk Level**: Medium (requires database migration for items 2-3)  
**Approval Required**: Yes (database changes)

### Violation Overview

| # | Title | Severity | Impact | Fix Complexity |
|---|---|---|---|---|
| 1 | Missing NameIdentifier Claim | 🔴 Critical | Authorization blocked | Low |
| 2 | Missing UserId FK & Navigation | 🔴 Critical | Architecture broken | High |
| 3 | No Ownership Verification | 🔴 Critical | Security vulnerability | Medium |
| 4 | DateTime.Now vs UtcNow | 🔴 Critical | Cross-timezone issues | Low |
| 5 | EditDto Validation Gap | 🔴 Critical | Data integrity risk | Low |
| 6 | Redundant Null Check | 🔴 Critical | Logic error/dead code | Low |
| 7 | Method Name Typo | 🔴 Critical | Discoverability issue | Low |

---

## Remediation Sequencing

### Phase 1: Low-Risk Fixes (No DB Migration)
**Duration**: 30 minutes  
**Risk**: Minimal  
**Order**: 1, 4, 5, 6, 7

These can be implemented and tested independently.

### Phase 2: Architecture Changes (DB Migration Required)
**Duration**: 90 minutes  
**Risk**: Medium (requires database changes)  
**Order**: 2, then 3  
**Approval**: Yes

Dependencies:
- Item 2 (UserId fields) must be complete before Item 3 can verify ownership

### Recommended Implementation Order

```
1. Fix DateTime.Now → UtcNow (safe, immediate)
2. Fix EditPostDto/EditCommentDto validation (safe, immediate)
3. Remove redundant null check (safe, immediate)
4. Fix method name typo (safe, requires coordination)
5. Add NameIdentifier claim to JWT (safe, no DB changes)
6. Add UserId FK & User navigation (BREAKING - DB migration required)
7. Implement ownership verification (depends on #6, tests Item #3)
```

---

## Detailed Action Items

### ACTION 1: Add ClaimTypes.NameIdentifier to JWT Token

**Instruction Violation**:
- **File**: jwt-authentication.instructions.md
- **Section**: § Token Generation, § Claims Design
- **Requirement**: "Token must include subject (user ID) in claims"
- **Current**: Only Name and Email included
- **Required**: Add ClaimTypes.NameIdentifier with user ID

**Code Changes Required**:

**File**: Services/Implementations/UserService.cs  
**Method**: `GetToken(User user, JwtSettings jwtSettings)`

```csharp
// Current (lines 61-75):
private string GetToken(User user, JwtSettings jwtSettings)
{
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email)
    };
    // ...
}

// Fixed:
private string GetToken(User user, JwtSettings jwtSettings)
{
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),      // ADD THIS
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email)
    };
    // ...
}
```

**Verification**:
```csharp
// Test that token includes NameIdentifier
var token = user.GetToken(jwtSettings);
var handler = new JwtSecurityTokenHandler();
var decodedToken = handler.ReadJwtToken(token);
var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
Assert.NotNull(userIdClaim);
Assert.Equal(user.Id, userIdClaim.Value);
```

**Risk**: None - backward compatible, new claim only  
**Rollback**: Revert single line addition  
**Dependencies**: None

---

### ACTION 2: Add UserId Foreign Keys & User Navigation Properties

**Instruction Violation**:
- **File**: entity-framework-core.instructions.md
- **Section**: § Relationship Management, § Entity Design
- **Requirement**: "Models must explicitly define foreign key properties and navigation properties"
- **Current**: Post and Comment lack UserId FK and User navigation
- **Required**: Add UserId property and User navigation to both models

**Code Changes Required**:

**File**: Models/Post.cs

```csharp
// Current (lines 1-15):
public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public DateTime CreationTime { get; set; }
    public int Likes { get; set; } = 0;
    public IList<Comment>? Comments { get; set; }
    // Missing UserId and User
}

// Fixed:
public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public DateTime CreationTime { get; set; }
    public int Likes { get; set; } = 0;
    public IList<Comment>? Comments { get; set; }
    
    // ADD THESE:
    public string UserId { get; set; }
    public User? User { get; set; }
}
```

**File**: Models/Comment.cs

```csharp
// Current (lines 1-13):
public class Comment
{
    public int Id { get; set; }
    public string Body { get; set; }
    public DateTime CreationTime { get; set; }
    public int PostId { get; set; }
    public Post Post { get; set; }
    // Missing UserId and User
}

// Fixed:
public class Comment
{
    public int Id { get; set; }
    public string Body { get; set; }
    public DateTime CreationTime { get; set; }
    public int PostId { get; set; }
    public Post Post { get; set; }
    
    // ADD THESE:
    public string UserId { get; set; }
    public User? User { get; set; }
}
```

**File**: Data/ApplicationDbContext.cs  
**Method**: `OnModelCreating(ModelBuilder modelBuilder)`

```csharp
// Add to OnModelCreating() (after existing configuration):

// Configure Post → User relationship
modelBuilder.Entity<Post>()
    .HasOne(p => p.User)
    .WithMany()
    .HasForeignKey(p => p.UserId)
    .OnDelete(DeleteBehavior.Cascade)
    .IsRequired();

// Configure Comment → User relationship
modelBuilder.Entity<Comment>()
    .HasOne(c => c.User)
    .WithMany()
    .HasForeignKey(c => c.UserId)
    .OnDelete(DeleteBehavior.Cascade)
    .IsRequired();
```

**Database Migration Required**:

```powershell
# Create migration
dotnet ef migrations add AddUserRelationshipsToPostAndComment -p PostHubAPI.csproj

# Review migration
# Verify Posts and Comments tables get new UserId columns with FK constraints

# Apply migration
dotnet ef database update -p PostHubAPI.csproj
```

**Verification**:
```csharp
// Verify FK exists
var post = new Post { Id = 1, Title = "Test", Body = "Test", UserId = "user123" };
var user = new User { Id = "user123" };

// Can now load user: post.User
Assert.NotNull(post.UserId);
```

**Risk**: High (database schema change)  
**Rollback**: `dotnet ef database update [previous-migration-name]`  
**Dependencies**: Database must be accessible; all instances must migrate

---

### ACTION 3: Implement Ownership Verification in Controllers

**Instruction Violation**:
- **File**: aspnet-core-api-design.instructions.md
- **Section**: § Authorization & Authentication, § Ownership Verification
- **Requirement**: "PUT and DELETE operations must verify authenticated user owns the resource"
- **Current**: No ownership checks
- **Required**: Verify `post.UserId == authenticatedUserId` before modify/delete

**Code Changes Required**:

**File**: Controllers/PostController.cs  
**Methods**: `EditPost(int id, EditPostDto editPostDto)`, `DeletePost(int id)`

```csharp
// EditPost - ADD ownership check:
[HttpPut("{id}")]
[Authorize]
public async Task<IActionResult> EditPost(int id, EditPostDto editPostDto)
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    
    if (userId == null)
        return Unauthorized();
    
    try
    {
        var post = await _postService.GetPostByIdAsync(id);
        
        // ADD OWNERSHIP CHECK:
        if (post.UserId != userId)
            return Forbid();
        
        var editedPost = await _postService.EditPostAsync(id, editPostDto);
        return Ok(editedPost);
    }
    catch (NotFoundException)
    {
        return NotFound();
    }
}

// DeletePost - ADD ownership check:
[HttpDelete("{id}")]
[Authorize]
public async Task<IActionResult> DeletePost(int id)
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    
    if (userId == null)
        return Unauthorized();
    
    try
    {
        var post = await _postService.GetPostByIdAsync(id);
        
        // ADD OWNERSHIP CHECK:
        if (post.UserId != userId)
            return Forbid();
        
        await _postService.DeletePostAsync(id);
        return NoContent();
    }
    catch (NotFoundException)
    {
        return NotFound();
    }
}
```

**File**: Controllers/CommentController.cs  
**Methods**: `EditComment(int id, EditCommentDto editCommentDto)`, `DeleteComment(int id)`

Apply same pattern as PostController.

**Verification**:
```csharp
[Fact]
public async Task EditPost_ReturnsForbidden_WhenUserNotOwner()
{
    // Arrange
    var userId1 = "user1";
    var userId2 = "user2";
    var post = new Post { Id = 1, UserId = userId1, Title = "Test", Body = "Test" };
    
    // Act
    var result = await controller.EditPost(1, new EditPostDto { ... });
    
    // Assert
    Assert.IsType<ForbidResult>(result);
}
```

**Risk**: Medium (requires Action 2 complete first)  
**Rollback**: Remove ownership check lines  
**Dependencies**: Requires UserId field in Post/Comment models (Action 2)

---

### ACTION 4: Replace DateTime.Now with DateTime.UtcNow

**Instruction Violation**:
- **File**: jwt-authentication.instructions.md
- **Section**: § Token Generation, § Key Requirements
- **Requirement**: "All JWT timestamps must use UTC (DateTime.UtcNow) for consistency"
- **Current**: Uses DateTime.Now (local time)
- **Required**: Use DateTime.UtcNow everywhere

**Code Changes Required**:

**File**: Services/Implementations/UserService.cs  
**Method**: `GetToken(User user, JwtSettings jwtSettings)`

```csharp
// Current (line 73):
expires: DateTime.Now.AddHours(3)

// Fixed:
expires: DateTime.UtcNow.AddHours(3)
```

**File**: Models/Post.cs  
**Field**: `CreationTime`

```csharp
// Current (line 13):
public DateTime CreationTime { get; } = DateTime.Now;

// Fixed:
public DateTime CreationTime { get; } = DateTime.UtcNow;
```

**File**: Models/Comment.cs  
**Field**: `CreationTime`

```csharp
// Current (line 12):
public DateTime CreationTime { get; } = DateTime.Now;

// Fixed:
public DateTime CreationTime { get; } = DateTime.UtcNow;
```

**Verification**:
```csharp
// Verify token expiration is UTC
var token = userService.GetToken(user, jwtSettings);
var handler = new JwtSecurityTokenHandler();
var decodedToken = handler.ReadJwtToken(token);
var expClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "exp");
// Verify exp timestamp is UTC epoch seconds
```

**Risk**: Low - behavioral change only, backward compatible  
**Rollback**: Revert three DateTime.Now → DateTime.UtcNow changes  
**Dependencies**: None (independent change)

---

### ACTION 5: Fix EditPostDto and EditCommentDto Validation

**Instruction Violation**:
- **File**: aspnet-core-api-design.instructions.md
- **Section**: § DTO Design, § Request Validation
- **Requirement**: "Edit DTOs must have consistent validation with Create DTOs"
- **Current**: EditPostDto missing validation; EditCommentDto has inconsistent [Required]
- **Required**: Add [Required] and validation attributes

**Code Changes Required**:

**File**: Dtos/Post/EditPostDto.cs

```csharp
// Current:
public class EditPostDto
{
    public string? Title { get; set; }
    public string? Body { get; set; }
}

// Fixed:
public class EditPostDto
{
    [Required]
    [StringLength(300, MinimumLength = 1)]
    public string Title { get; set; }
    
    [Required]
    [StringLength(5000, MinimumLength = 1)]
    public string Body { get; set; }
}
```

**File**: Dtos/Comment/EditCommentDto.cs

```csharp
// Current:
public class EditCommentDto
{
    [Required]
    [StringLength(80)]
    public string Body { get; set; }
}

// Fixed (add MinimumLength):
public class EditCommentDto
{
    [Required]
    [StringLength(80, MinimumLength = 1)]
    public string Body { get; set; }
}
```

**Verification**:
```csharp
[Fact]
public void EditPostDto_IsInvalid_WhenTitleEmpty()
{
    var dto = new EditPostDto { Title = "", Body = "Test" };
    var context = new ValidationContext(dto);
    var results = new List<ValidationResult>();
    var isValid = Validator.TryValidateObject(dto, context, results, true);
    Assert.False(isValid);
}
```

**Risk**: Low - validation only, improves data integrity  
**Rollback**: Revert validation attributes  
**Dependencies**: None (independent change)

---

### ACTION 6: Remove Redundant Null Check in UserService.Login

**Instruction Violation**:
- **File**: csharp.instructions.md
- **Section**: § Keep Logic Direct and Readable
- **Requirement**: "Remove redundant conditions and dead code"
- **Current**: Redundant null checks (first throws, second unreachable)
- **Required**: Remove first null check or restructure

**Code Changes Required**:

**File**: Services/Implementations/UserService.cs  
**Method**: `Login(LoginUserDto dto)`

```csharp
// Current (lines 47-54):
public async Task<string> Login(LoginUserDto dto)
{
    var user = await _userManager.FindByEmailAsync(dto.Email);
    if (user == null)
        throw new InvalidOperationException("Invalid email or password");
    
    var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
    if (user == null || !passwordValid)  // ← Dead condition: user != null is always true here
        throw new InvalidOperationException("Invalid email or password");
    // ...
}

// Fixed (remove first check):
public async Task<string> Login(LoginUserDto dto)
{
    var user = await _userManager.FindByEmailAsync(dto.Email);
    
    var passwordValid = user != null && await _userManager.CheckPasswordAsync(user, dto.Password);
    if (!passwordValid)
        throw new InvalidOperationException("Invalid email or password");
    // ...
}
```

**Verification**:
```csharp
[Fact]
public async Task Login_ThrowsException_WhenUserNotFound()
{
    var dto = new LoginUserDto { Email = "fake@test.com", Password = "pwd" };
    var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.Login(dto));
    Assert.Contains("Invalid email or password", ex.Message);
}

[Fact]
public async Task Login_ThrowsException_WhenPasswordInvalid()
{
    // User exists but password wrong
    var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.Login(dto));
    Assert.Contains("Invalid email or password", ex.Message);
}
```

**Risk**: Very Low - removes dead code, improves readability  
**Rollback**: Restore original null checks  
**Dependencies**: None (independent change)

---

### ACTION 7: Fix Method Name Typo

**Instruction Violation**:
- **File**: csharp.instructions.md
- **Section**: § Use Descriptive Member Names
- **Requirement**: "Use correct spelling in public API names"
- **Current**: `CreateNewCommnentAsync` (typo: "Commment")
- **Required**: Rename to `CreateNewCommentAsync`

**Code Changes Required**:

**File**: Services/Interfaces/ICommentService.cs  
**Line**: (method declaration)

```csharp
// Current:
Task<ReadCommentDto> CreateNewCommnentAsync(CreateCommentDto createCommentDto);

// Fixed:
Task<ReadCommentDto> CreateNewCommentAsync(CreateCommentDto createCommentDto);
```

**File**: Services/Implementations/CommentService.cs  
**Line**: (method implementation)

```csharp
// Current:
public async Task<ReadCommentDto> CreateNewCommnentAsync(CreateCommentDto createCommentDto)

// Fixed:
public async Task<ReadCommentDto> CreateNewCommentAsync(CreateCommentDto createCommentDto)
```

**File**: Controllers/CommentController.cs  
**Line**: (method call)

```csharp
// Current:
await _commentService.CreateNewCommnentAsync(createCommentDto)

// Fixed:
await _commentService.CreateNewCommentAsync(createCommentDto)
```

**Verification**:
```csharp
// Compile and verify no calls to old method name remain
var method = typeof(ICommentService).GetMethod("CreateNewCommentAsync");
Assert.NotNull(method);
```

**Risk**: Very Low - cosmetic fix, straightforward rename  
**Rollback**: Restore original spelling  
**Dependencies**: None (independent change)  
**Note**: Use "Find and Replace" across entire solution to ensure consistency

---

## Implementation Strategy

### Phase 1: Safe Fixes (No Database Changes)

**Duration**: 30 minutes  
**Execute in Order**: 4, 5, 6, 7, 1

These require no database migration and can be safely implemented.

```powershell
# Step 1: Fix DateTime.Now → UtcNow (3 changes)
# Step 2: Fix EditPostDto/EditCommentDto validation (2 files)
# Step 3: Remove redundant null check (1 method)
# Step 4: Fix method name typo (3 files - find & replace)
# Step 5: Add NameIdentifier claim (1 method, 1 line)

# Compile and test
dotnet build
dotnet test PostHubAPI.Tests
```

### Phase 2: Architecture Changes (Database Migration)

**Duration**: 90 minutes  
**Execute in Order**: 2, then 3

**Step 1: Backup Database**
```powershell
# If using SQLite:
Copy-Item posthub.db posthub.db.backup
```

**Step 2: Implement Model Changes (Action 2)**
```powershell
# Make code changes to Post.cs, Comment.cs, ApplicationDbContext.cs

dotnet ef migrations add AddUserRelationshipsToPostAndComment -p PostHubAPI.csproj

# Review migration
cat Migrations/[migration-name].cs

# Apply migration
dotnet ef database update -p PostHubAPI.csproj

# Verify database schema
# Check Posts table has UserId column with FK
# Check Comments table has UserId column with FK
```

**Step 3: Implement Ownership Verification (Action 3)**
```powershell
# Make code changes to PostController.cs, CommentController.cs

dotnet build
dotnet test PostHubAPI.Tests

# Manual test: Attempt to modify/delete another user's post
# Should receive 403 Forbidden
```

---

## Testing Plan

### Unit Tests Required

**1. JWT Token Contains NameIdentifier**
```csharp
[Fact]
public async Task GetToken_IncludesNameIdentifierClaim()
{
    var user = new User { Id = "test-user-id", UserName = "testuser", Email = "test@test.com" };
    var jwtSettings = new JwtSettings { /* ... */ };
    
    var token = userService.GetToken(user, jwtSettings);
    
    var handler = new JwtSecurityTokenHandler();
    var decodedToken = handler.ReadJwtToken(token);
    var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
    
    Assert.NotNull(userIdClaim);
    Assert.Equal("test-user-id", userIdClaim.Value);
}
```

**2. DateTime Uses UTC**
```csharp
[Fact]
public void Token_ExpirationIsUtc()
{
    var token = userService.GetToken(user, jwtSettings);
    var handler = new JwtSecurityTokenHandler();
    var decodedToken = handler.ReadJwtToken(token);
    
    // Verify expiration is in UTC epoch seconds
    Assert.NotNull(decodedToken.ValidTo);
    Assert.Equal(DateTimeKind.Utc, decodedToken.ValidTo.Kind);
}
```

**3. Ownership Verification**
```csharp
[Fact]
public async Task EditPost_ReturnsForbidden_WhenUserNotOwner()
{
    // Arrange
    var post = new Post { Id = 1, UserId = "user1", Title = "Test", Body = "Test" };
    var authenticatedUserId = "user2"; // Different user
    
    // Act
    var result = await controller.EditPost(1, new EditPostDto { /* ... */ });
    
    // Assert
    Assert.IsType<ForbidResult>(result);
}

[Fact]
public async Task EditPost_Succeeds_WhenUserIsOwner()
{
    // Arrange
    var post = new Post { Id = 1, UserId = "user1", Title = "Test", Body = "Test" };
    var authenticatedUserId = "user1"; // Same user
    
    // Act
    var result = await controller.EditPost(1, new EditPostDto { /* ... */ });
    
    // Assert
    Assert.IsType<OkObjectResult>(result);
}
```

**4. Validation**
```csharp
[Fact]
public void EditPostDto_IsInvalid_WhenFieldsEmpty()
{
    var dto = new EditPostDto { Title = "", Body = "" };
    var context = new ValidationContext(dto);
    var results = new List<ValidationResult>();
    var isValid = Validator.TryValidateObject(dto, context, results, true);
    
    Assert.False(isValid);
    Assert.NotEmpty(results);
}
```

### Integration Tests

**5. End-to-End: User Registers, Creates Post, Tries to Edit Another User's Post**
```csharp
[Fact]
public async Task E2E_UserCannotModifyOthersPost()
{
    // User1 registers and creates post
    var user1Token = await RegisterAndLogin("user1@test.com");
    var postId = await CreatePost(user1Token, "Test Post", "Test Body");
    
    // User2 registers
    var user2Token = await RegisterAndLogin("user2@test.com");
    
    // User2 tries to edit User1's post
    var response = await EditPost(user2Token, postId, "Hacked", "Hacked Body");
    
    // Should fail
    Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
}
```

### Regression Tests

- All existing tests should pass
- CommentController tests must verify ownership checks
- UserService tests verify JWT structure

---

## Rollback Plan

### Phase 1 Rollback (Safe Fixes)
**Risk**: None - can be easily reverted with individual commits

```powershell
# Revert each commit individually
git revert [commit-hash-for-datetime-fix]
git revert [commit-hash-for-validation-fix]
git revert [commit-hash-for-null-check-fix]
git revert [commit-hash-for-typo-fix]
git revert [commit-hash-for-nameidentifier-fix]

dotnet build
dotnet test
```

### Phase 2 Rollback (Database Migration)
**Risk**: Medium - requires database downtime

```powershell
# Option 1: Rollback to previous migration
dotnet ef database update [previous-migration-name] -p PostHubAPI.csproj

# Option 2: Restore from backup
Copy-Item posthub.db.backup posthub.db

# Revert code changes
git revert [migration-and-code-commits]
```

---

## Provenance Documentation

### Document Metadata

```yaml
analysis_date: 2026-05-21
analysis_methodology: Instruction file compliance review
analyzer: GitHub Copilot (Agent-based codebase review)
model: Claude Haiku 4.5
instruction_files_reviewed:
  - jwt-authentication.instructions.md
  - entity-framework-core.instructions.md
  - aspnet-core-api-design.instructions.md
  - csharp.instructions.md
  - evergreen-software-development.instructions.md
project_requirements: docs/PROJECT-REQUIREMENTS.md
```

### Instruction References

#### ACTION 1: NameIdentifier Claim
- **Source**: jwt-authentication.instructions.md
- **Section**: § Token Generation
- **Quote**: "Token must include subject (user ID), issuer, audience"
- **Line Reference**: Claims Design table shows NameIdentifier as required

#### ACTION 2: UserId FK & User Navigation
- **Source**: entity-framework-core.instructions.md
- **Section**: § Entity Design, § Relationship Management
- **Quote**: "Use Fluent API in OnModelCreating for relationship configuration"
- **Line Reference**: Relationship Management section, example code

#### ACTION 3: Ownership Verification
- **Source**: aspnet-core-api-design.instructions.md
- **Section**: § Authorization & Authentication
- **Quote**: "PUT and DELETE operations must verify authenticated user owns the resource"
- **Line Reference**: Ownership Verification section with code example

#### ACTION 4: DateTime.UtcNow
- **Source**: jwt-authentication.instructions.md
- **Section**: § Token Generation, § Key Requirements
- **Quote**: "Always use DateTime.UtcNow (not DateTime.Now) for consistent UTC time"
- **Line Reference**: Key Requirements list item 3

#### ACTION 5: DTO Validation
- **Source**: aspnet-core-api-design.instructions.md
- **Section**: § DTO Design, § Request Validation
- **Quote**: "Validation attributes must be consistent across Create/Edit DTOs"
- **Line Reference**: DTO Design table

#### ACTION 6: Dead Code Removal
- **Source**: csharp.instructions.md
- **Section**: § Keep Logic Direct and Readable
- **Quote**: "Remove redundant conditions and dead code"
- **Line Reference**: Code quality section

#### ACTION 7: Naming Conventions
- **Source**: csharp.instructions.md
- **Section**: § Use Descriptive Member Names
- **Quote**: "Public API names must use correct spelling"
- **Line Reference**: Naming conventions section

### Project Requirements References

All actions support compliance with:
- **docs/PROJECT-REQUIREMENTS.md § Business Rules**: "User Ownership - Users can only modify or delete their own posts and comments"
- **docs/PROJECT-REQUIREMENTS.md § Authentication & Authorization**: "Authorization Enforcement" table
- **docs/PROJECT-REQUIREMENTS.md § Core Workflows**: Post Edit and Delete workflows show authorization checks required

---

## Sign-Off

### Approval Required From

- [ ] Development Lead
- [ ] Architecture Review
- [ ] Database Administrator (for Phase 2 migration)

### Completed By

- Date: ______________
- Implementer: ______________
- Reviewer: ______________

---

**Document Version**: 1.0.0  
**Created**: May 21, 2026  
**Last Updated**: May 21, 2026  
**Status**: Proposed for Implementation
