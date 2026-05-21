# Implementation Checklist

**Date Started**: _____________  
**Team Lead**: _____________  
**Status**: In Progress

---

## Phase 1: Safe Fixes (No Database Migration)

**Estimated Duration**: 30 minutes  
**Risk Level**: Low  
**Testing Required**: Unit tests for each action

### ACTION 4: Replace DateTime.Now with DateTime.UtcNow

- [ ] **Code Change 1**: Services/Implementations/UserService.cs line 73
  - [ ] Replace `DateTime.Now.AddHours(3)` with `DateTime.UtcNow.AddHours(3)`
  - [ ] Git commit with message: "fix: use UtcNow instead of Now for JWT token expiration"

- [ ] **Code Change 2**: Models/Post.cs line 13
  - [ ] Replace `DateTime.Now` with `DateTime.UtcNow`
  - [ ] Git commit with message: "fix: use UtcNow for Post CreationTime"

- [ ] **Code Change 3**: Models/Comment.cs line 12
  - [ ] Replace `DateTime.Now` with `DateTime.UtcNow`
  - [ ] Git commit with message: "fix: use UtcNow for Comment CreationTime"

- [ ] **Verification**:
  - [ ] Run: `dotnet build`
  - [ ] Run: `dotnet test PostHubAPI.Tests`
  - [ ] Manual verification: Check token expiration is UTC

**Completed By**: _____________ **Date**: _____________

---

### ACTION 5: Fix EditPostDto and EditCommentDto Validation

- [ ] **Code Change 1**: Dtos/Post/EditPostDto.cs
  - [ ] Add `[Required]` to Title property
  - [ ] Add `[StringLength(300, MinimumLength = 1)]` to Title
  - [ ] Add `[Required]` to Body property
  - [ ] Add `[StringLength(5000, MinimumLength = 1)]` to Body
  - [ ] Git commit with message: "fix: add validation to EditPostDto"

- [ ] **Code Change 2**: Dtos/Comment/EditCommentDto.cs
  - [ ] Update Body validation to include MinimumLength: `[StringLength(80, MinimumLength = 1)]`
  - [ ] Git commit with message: "fix: add MinimumLength to EditCommentDto validation"

- [ ] **Verification**:
  - [ ] Run: `dotnet build`
  - [ ] Run: `dotnet test PostHubAPI.Tests`
  - [ ] Manual test: Attempt to update post with empty title → Should fail validation

**Completed By**: _____________ **Date**: _____________

---

### ACTION 6: Remove Redundant Null Check in UserService.Login

- [ ] **Code Change**: Services/Implementations/UserService.cs line 47-54
  - [ ] Remove first null check (lines 47-49)
  - [ ] Restructure logic to single check: `if (!passwordValid)` where passwordValid combines user existence and password check
  - [ ] Git commit with message: "fix: remove redundant null check in UserService.Login"

- [ ] **Verification**:
  - [ ] Run: `dotnet build`
  - [ ] Run: `dotnet test PostHubAPI.Tests`
  - [ ] Unit test: Verify both "user not found" and "password invalid" return same error message

**Completed By**: _____________ **Date**: _____________

---

### ACTION 7: Fix Method Name Typo

- [ ] **Code Change 1**: Services/Interfaces/ICommentService.cs
  - [ ] Rename method: `CreateNewCommnentAsync` → `CreateNewCommentAsync`
  - [ ] Git commit with message: "fix: correct method name typo CreateNewCommnentAsync → CreateNewCommentAsync"

- [ ] **Code Change 2**: Services/Implementations/CommentService.cs
  - [ ] Rename method implementation: `CreateNewCommnentAsync` → `CreateNewCommentAsync`

- [ ] **Code Change 3**: Controllers/CommentController.cs
  - [ ] Update all calls from `CreateNewCommnentAsync` to `CreateNewCommentAsync`

- [ ] **Verification**:
  - [ ] Run: `dotnet build`
  - [ ] Search solution for "CreateNewCommnent" → Should find 0 results
  - [ ] Run: `dotnet test PostHubAPI.Tests`

**Completed By**: _____________ **Date**: _____________

---

### ACTION 1: Add ClaimTypes.NameIdentifier to JWT Token

- [ ] **Code Change**: Services/Implementations/UserService.cs method GetToken() line 61-75
  - [ ] Add line: `new Claim(ClaimTypes.NameIdentifier, user.Id),` as first claim
  - [ ] Ensure using statement: `using System.Security.Claims;`
  - [ ] Git commit with message: "fix: add NameIdentifier claim to JWT token"

- [ ] **Verification**:
  - [ ] Run: `dotnet build`
  - [ ] Run: `dotnet test PostHubAPI.Tests`
  - [ ] Unit test: Decode token and verify NameIdentifier claim present
  - [ ] Unit test: Verify claim value equals user.Id

**Completed By**: _____________ **Date**: _____________

---

### Phase 1 Summary

- [ ] All 5 code changes completed
- [ ] All commits pushed to feature branch
- [ ] All tests passing
- [ ] Code review completed
- [ ] PR merged to main

**Phase 1 Completed By**: _____________ **Date**: _____________

---

## Phase 2: Architecture Changes (Database Migration Required)

**Estimated Duration**: 90 minutes  
**Risk Level**: Medium (database schema change)  
**Approval Required**: Database Administrator

### Pre-Migration Checklist

- [ ] Database backed up: `posthub.db.backup` created
- [ ] Team notified of maintenance window
- [ ] Staging environment migration tested
- [ ] Rollback procedure documented and tested

---

### ACTION 2: Add UserId Foreign Keys & User Navigation Properties

#### Code Changes

- [ ] **File**: Models/Post.cs
  - [ ] Add property: `public string UserId { get; set; }`
  - [ ] Add property: `public User? User { get; set; }`
  - [ ] Git commit with message: "feature: add UserId FK and User navigation to Post model"

- [ ] **File**: Models/Comment.cs
  - [ ] Add property: `public string UserId { get; set; }`
  - [ ] Add property: `public User? User { get; set; }`
  - [ ] Git commit with message: "feature: add UserId FK and User navigation to Comment model"

- [ ] **File**: Data/ApplicationDbContext.cs method OnModelCreating()
  - [ ] Add Post → User relationship configuration
  - [ ] Add Comment → User relationship configuration
  - [ ] Configure cascade delete behavior
  - [ ] Git commit with message: "feature: configure Post/Comment User relationships"

#### Database Migration

- [ ] **Create Migration**:
  - [ ] Run: `dotnet ef migrations add AddUserRelationshipsToPostAndComment -p PostHubAPI.csproj`
  - [ ] Review generated migration file
  - [ ] Verify schema changes:
    - [ ] Posts table has new UserId column (string, not null)
    - [ ] Posts.UserId has FK to AspNetUsers.Id
    - [ ] Comments table has new UserId column (string, not null)
    - [ ] Comments.UserId has FK to AspNetUsers.Id
    - [ ] Cascade delete configured

- [ ] **Local Testing**:
  - [ ] Run: `dotnet ef database update -p PostHubAPI.csproj`
  - [ ] Run: `dotnet build`
  - [ ] Run: `dotnet test PostHubAPI.Tests`
  - [ ] Verify database schema with DbContext

- [ ] **Staging Environment**:
  - [ ] Deploy code to staging
  - [ ] Backup staging database
  - [ ] Apply migration: `dotnet ef database update`
  - [ ] Run full test suite
  - [ ] Smoke test: Verify app starts and endpoints respond

- [ ] **Production Deployment** (if required):
  - [ ] Backup production database: posthub.db.backup
  - [ ] Deploy code
  - [ ] Apply migration: `dotnet ef database update`
  - [ ] Smoke test: Verify app health
  - [ ] Monitor error logs for 30 minutes

**ACTION 2 Completed By**: _____________ **Date**: _____________

---

### ACTION 3: Implement Ownership Verification in Controllers

#### Code Changes

- [ ] **File**: Controllers/PostController.cs method EditPost()
  - [ ] Extract userId from JWT: `User.FindFirst(ClaimTypes.NameIdentifier)?.Value`
  - [ ] Check userId != null → return Unauthorized()
  - [ ] Load post
  - [ ] Check post.UserId == userId → return Forbid() if not owner
  - [ ] Continue with edit logic
  - [ ] Git commit with message: "fix: add ownership verification to PostController.EditPost"

- [ ] **File**: Controllers/PostController.cs method DeletePost()
  - [ ] Extract userId from JWT
  - [ ] Check userId != null → return Unauthorized()
  - [ ] Load post
  - [ ] Check post.UserId == userId → return Forbid() if not owner
  - [ ] Continue with delete logic
  - [ ] Git commit with message: "fix: add ownership verification to PostController.DeletePost"

- [ ] **File**: Controllers/CommentController.cs method EditComment()
  - [ ] Apply same pattern as PostController.EditPost
  - [ ] Git commit with message: "fix: add ownership verification to CommentController.EditComment"

- [ ] **File**: Controllers/CommentController.cs method DeleteComment()
  - [ ] Apply same pattern as PostController.DeletePost
  - [ ] Git commit with message: "fix: add ownership verification to CommentController.DeleteComment"

#### Testing

- [ ] **Unit Tests**:
  - [ ] Run: `dotnet test PostHubAPI.Tests`
  - [ ] Verify: EditPost_ReturnsForbidden_WhenUserNotOwner passes
  - [ ] Verify: DeletePost_ReturnsForbidden_WhenUserNotOwner passes
  - [ ] Verify: EditPost_Succeeds_WhenUserIsOwner passes
  - [ ] Verify: DeletePost_Succeeds_WhenUserIsOwner passes

- [ ] **Integration Tests**:
  - [ ] Manual test: User1 creates post → User2 cannot edit → Returns 403
  - [ ] Manual test: User1 creates post → User1 can edit → Returns 200/204
  - [ ] Manual test: Same for comments

- [ ] **Security Review**:
  - [ ] Verify no way to bypass ownership check
  - [ ] Verify error message doesn't expose security details
  - [ ] Review for OWASP issues

**ACTION 3 Completed By**: _____________ **Date**: _____________

---

### Phase 2 Summary

- [ ] All code changes implemented
- [ ] Database migration created and tested
- [ ] All environments migrated (local → staging → production if applicable)
- [ ] All tests passing
- [ ] Security review completed
- [ ] No regressions detected
- [ ] Documentation updated

**Phase 2 Completed By**: _____________ **Date**: _____________

---

## Final Sign-Off

### Pre-Release Checks

- [ ] All 7 actions completed
- [ ] All tests passing (unit, integration, regression)
- [ ] Code review approved
- [ ] Security review approved
- [ ] Database changes verified on production
- [ ] Error logs monitored (24 hours)
- [ ] User feedback collected (no reported issues)

### Release Notes

**Version**: X.X.X  
**Date**: _____________

**Bug Fixes**:
- Fixed JWT token missing user ID claim (NameIdentifier)
- Fixed redundant null check in UserService.Login
- Fixed method name typo: CreateNewCommnentAsync → CreateNewCommentAsync

**Security Fixes**:
- Added ownership verification for post/comment edit and delete operations
- Fixed DateTime timezone inconsistency in JWT tokens

**Enhancements**:
- Added UserId foreign keys to Post and Comment entities
- Added validation to EditPostDto and EditCommentDto

### Approval Sign-Off

- [ ] Development Lead: _________________ Date: _____________
- [ ] Architecture Review: _________________ Date: _____________
- [ ] Database Administrator: _________________ Date: _____________
- [ ] Product Owner: _________________ Date: _____________

---

**Document Version**: 1.0  
**Created**: May 21, 2026  
**Last Updated**: _____________  
**Next Review**: After all phases complete
