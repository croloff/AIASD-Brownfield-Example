---
ai_generated: true
model: "anthropic/claude-3.5-sonnet@2024-10-22"
operator: "GitHub Copilot"
chat_id: "project-requirements-doc-20260521"
prompt: |
  Create a comprehensive project requirements document that contains business rules, workflows, purpose, tech stack, and architecture
started: "2026-05-21T00:00:00Z"
ended: "2026-05-21T00:15:00Z"
task_durations:
  - task: "codebase analysis"
    duration: "00:05:00"
  - task: "requirements documentation"
    duration: "00:08:00"
  - task: "document structuring"
    duration: "00:02:00"
total_duration: "00:15:00"
ai_log: "ai-logs/2026/05/21/project-requirements-doc-20260521/conversation.md"
source: "github-copilot-chat"
---

# PostHubAPI: Project Requirements Document

**Last Updated**: May 21, 2026  
**Project**: PostHubAPI  
**Framework**: .NET 8 / ASP.NET Core Web API  
**Status**: Active Development  

---

## Table of Contents

1. [Executive Summary](#executive-summary)
2. [Project Purpose](#project-purpose)
3. [Business Rules](#business-rules)
4. [Core Workflows](#core-workflows)
5. [Technology Stack](#technology-stack)
6. [Architecture](#architecture)
7. [Data Models](#data-models)
8. [API Capabilities](#api-capabilities)
9. [Authentication & Authorization](#authentication--authorization)
10. [Environmental Configuration](#environmental-configuration)
11. [Quality & Testing](#quality--testing)
12. [Deployment & Operations](#deployment--operations)
13. [Constraints & Known Issues](#constraints--known-issues)

---

## Executive Summary

PostHubAPI is a lightweight, modular blogging platform API built with ASP.NET Core and Entity Framework Core. The system supports user management with JWT authentication, post creation and management with comment threading, and environment-based persistence strategies for development and production deployment.

**Key Characteristics**:
- RESTful HTTP API
- JWT-based authentication
- Multi-environment support (Development with in-memory DB, Production with SQLite)
- Service-oriented architecture with dependency injection
- Comprehensive test coverage with xUnit
- Swagger/OpenAPI documentation

---

## Project Purpose

PostHubAPI serves as a backend service for a blogging platform with the following primary objectives:

### Primary Goals

1. **User Management**: Enable user registration, authentication, and secure session management
2. **Content Creation**: Provide facilities for users to create, edit, and delete blog posts
3. **Community Engagement**: Support threaded comments on posts for user interaction
4. **Secure Access**: Enforce role-based access control and user-owned content protection
5. **Scalable Foundation**: Establish clean architecture patterns for future feature expansion

### Intended Use Cases

- **Content Authors**: Register, create posts, manage their own content
- **Readers**: Browse posts and comment on content
- **Community**: Foster discussion through structured comment threads
- **Administrators**: Monitor platform health and content moderation (future)

---

## Business Rules

### User Management Rules

| Rule | Description |
|------|-------------|
| **User Uniqueness** | Email addresses must be globally unique within the system; duplicate registration attempts are rejected |
| **Password Security** | Passwords are hashed using BCrypt with automatic salt generation; plain-text passwords are never stored |
| **Authentication Required** | Content creation and modification require a valid JWT token; anonymous users have read-only access |
| **User Ownership** | Users can only modify or delete their own posts and comments; cross-user modification is forbidden |
| **Session Expiry** | JWT tokens have a configurable expiration window; expired tokens require re-authentication |

### Post Management Rules

| Rule | Description |
|------|-------------|
| **Post Creation** | Only authenticated users can create posts; posts require non-empty title and body |
| **Post Ownership** | Only the post author can edit or delete their own posts |
| **Post Visibility** | All posts are publicly readable; access control is only enforced for modification |
| **Timestamp Immutability** | Post creation time is set at creation and cannot be modified; represents post age in listing queries |
| **Likes Tracking** | Posts maintain a cumulative like count; likes are tracked per-post, not per-user |

### Comment Management Rules

| Rule | Description |
|------|-------------|
| **Comment Association** | Comments must reference an existing post; orphaned comments are invalid |
| **Comment Ownership** | Only the comment author can edit or delete their own comments |
| **Comment Ordering** | Comments are ordered by creation timestamp within each post thread |
| **Comment Hierarchy** | Current implementation supports flat comment threads; nested replies are not supported |
| **Authentication Required** | All comment operations (create, read, update, delete) require valid JWT authentication |

### Data Integrity Rules

| Rule | Description |
|------|-------------|
| **Referential Integrity** | Deleting a post cascades to delete all associated comments |
| **Null Safety** | Required fields (Title, Body, Email) cannot be null in the database |
| **Default Values** | New posts default to 0 likes and current timestamp for creation time |

---

## Core Workflows

### User Registration & Authentication Workflow

```
1. User initiates registration
   └─ POST /api/User/Register with email, password
   
2. UserService validates input
   └─ Email uniqueness check
   └─ Password strength validation (via DataAnnotations)
   
3. Password hashing
   └─ BCrypt.Net hashes password with automatic salt
   
4. User persistence
   └─ User entity stored in ApplicationDbContext
   └─ Returns 201 Created on success
   └─ Returns 400 Bad Request if email already exists
```

### User Login & Token Issuance Workflow

```
1. User initiates login
   └─ POST /api/User/Login with email, password
   
2. UserService validates credentials
   └─ ASP.NET Identity validates password hash
   
3. JWT token generation
   └─ JwtSettingsResolver provides token configuration
   └─ Token signed with HS256 (HMAC SHA-256)
   └─ Token includes subject (user ID), issuer, audience
   
4. Return token to client
   └─ Client stores token for subsequent requests
   └─ Token sent in Authorization: Bearer header
```

### Post Creation Workflow

```
1. Authenticated user initiates post creation
   └─ POST /api/Post with CreatePostDto
   └─ Request includes Authorization header with JWT
   
2. PostService validates request
   └─ User ID extracted from JWT claims
   └─ Post content validated (non-empty title/body)
   
3. Post persistence
   └─ New Post entity created with userId reference
   └─ Creation timestamp auto-set to DateTime.Now
   └─ Likes initialized to 0
   
4. Response mapping
   └─ AutoMapper converts Post entity to ReadPostDto
   └─ Returns 201 Created with post data
```

### Post Edit Workflow

```
1. Authenticated user initiates post edit
   └─ PUT /api/Post/{id} with EditPostDto
   
2. Authorization check
   └─ PostService verifies post exists (404 if not)
   └─ PostService verifies user is post author
   └─ Returns 403 Forbidden if unauthorized
   
3. Content update
   └─ Title and body updated with new values
   └─ Creation timestamp preserved (not modified)
   
4. Persistence and response
   └─ Changes saved to database
   └─ Updated post returned to client
```

### Comment Thread Workflow

```
1. Authenticated user initiates comment creation
   └─ POST /api/Comment with CreateCommentDto
   └─ Request includes PostId reference
   
2. Validation
   └─ Post existence verified (404 if not found)
   └─ User ID extracted from JWT
   └─ Comment body validated (non-empty)
   
3. Comment persistence
   └─ New Comment entity linked to Post and User
   └─ Creation timestamp auto-set
   
4. Thread retrieval
   └─ GET /api/Comment/post/{postId} returns all comments
   └─ Comments ordered by creation timestamp
   └─ Each comment mapped to ReadCommentDto
```

### Post Deletion with Cascade Workflow

```
1. Authenticated user initiates post deletion
   └─ DELETE /api/Post/{id}
   
2. Authorization check
   └─ Verify post exists (404 if not)
   └─ Verify user is post author (403 if not)
   
3. Cascade deletion
   └─ All comments associated with post are deleted
   └─ Post entity deleted from database
   
4. Response
   └─ Returns 204 No Content on success
```

---

## Technology Stack

### Runtime & Framework

| Component | Version | Purpose |
|-----------|---------|---------|
| **.NET** | 8.0 | Runtime environment |
| **ASP.NET Core** | 8.0 | Web API framework and HTTP server |
| **C#** | Latest | Primary development language |

### Data Access & Persistence

| Component | Version | Purpose |
|-----------|---------|---------|
| **Entity Framework Core** | 8.0.1 | ORM for database abstraction |
| **EF Core InMemory** | 8.0.1 | Development database provider (in-process) |
| **EF Core SQLite** | 8.0.1 | Production database provider |
| **SQLite** | Latest | Production database engine |

### Authentication & Security

| Component | Version | Purpose |
|-----------|---------|---------|
| **ASP.NET Core Identity** | 8.0.1 | User and role management |
| **JWT Bearer** | 8.0.0 | JWT token validation and enforcement |
| **BCrypt.Net** | 0.1.0 | Password hashing with salt |
| **System.IdentityModel.Tokens.Jwt** | Implicit | JWT creation and signing |

### API Documentation & Discovery

| Component | Version | Purpose |
|-----------|---------|---------|
| **Swashbuckle.AspNetCore** | 6.5.0 | Swagger/OpenAPI generation and UI |

### Object Mapping

| Component | Version | Purpose |
|-----------|---------|---------|
| **AutoMapper** | 12.0.1 | DTO and entity mapping configuration |
| **AutoMapper.Extensions.Microsoft.DependencyInjection** | 12.0.1 | DI integration for AutoMapper |

### Testing Framework

| Component | Version | Purpose |
|-----------|---------|---------|
| **xUnit** | Latest | Unit and integration test framework |
| **ASP.NET Core Test Host** | 8.0 | In-process test server for API testing |

### Build & Project Configuration

| Component | Purpose |
|-----------|---------|
| **SDK-style .csproj** | Modern project file format with implicit defaults |
| **User Secrets** | Local secret management for JWT secret in development |

---

## Architecture

### Layered Architecture Pattern

PostHubAPI follows a **clean, layered architecture** with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────┐
│                    HTTP Layer                            │
│  Controllers (routing, validation, response mapping)    │
├─────────────────────────────────────────────────────────┤
│                  Business Logic Layer                    │
│  Services (orchestration, rules enforcement)            │
├─────────────────────────────────────────────────────────┤
│                  Data Access Layer                       │
│  EF Core DbContext (persistence abstraction)            │
├─────────────────────────────────────────────────────────┤
│                  Storage Layer                           │
│  Database (InMemory/SQLite)                             │
└─────────────────────────────────────────────────────────┘
```

### Project Structure

```
PostHubAPI/
├── Controllers/                          # HTTP endpoint handlers
│   ├── UserController.cs                # Auth endpoints
│   ├── PostController.cs                # Post CRUD endpoints
│   └── CommentController.cs             # Comment endpoints
├── Services/
│   ├── Interfaces/                      # Service contracts
│   │   ├── IUserService.cs
│   │   ├── IPostService.cs
│   │   └── ICommentService.cs
│   └── Implementations/                 # Business logic
│       ├── UserService.cs
│       ├── PostService.cs
│       └── CommentService.cs
├── Data/
│   └── ApplicationDbContext.cs          # EF Core DbContext
├── Models/                               # Entity models (DB schema)
│   ├── User.cs
│   ├── Post.cs
│   └── Comment.cs
├── Dtos/                                 # Data Transfer Objects
│   ├── User/
│   │   ├── RegisterUserDto.cs
│   │   └── LoginUserDto.cs
│   ├── Post/
│   │   ├── CreatePostDto.cs
│   │   ├── EditPostDto.cs
│   │   └── ReadPostDto.cs
│   └── Comment/
│       ├── CreateCommentDto.cs
│       ├── EditCommentDto.cs
│       └── ReadCommentDto.cs
├── Profiles/                             # AutoMapper configurations
│   ├── PostProfile.cs
│   └── CommentProfile.cs
├── Configuration/                        # Startup configuration helpers
│   └── JwtSettingsResolver.cs           # JWT settings resolution
├── Exceptions/                           # Domain-specific exceptions
│   └── NotFoundException.cs
├── Program.cs                            # DI container & middleware setup
└── appsettings*.json                     # Configuration files
```

### Dependency Injection Container

The DI container (configured in `Program.cs`) manages:

```csharp
// Service registration
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IUserService, UserService>();

// DbContext registration (environment-specific)
// - Development: InMemory provider
// - Production: SQLite provider

// Authentication
// - ASP.NET Core Identity
// - JWT Bearer token validation

// AutoMapper
// - Profile-based mapping configuration
```

### Authentication & Authorization Flow

```
┌─────────────────────────────────────────────────────────┐
│  Client Request with JWT Token                          │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────┐
│  Authentication Middleware                              │
│  - Extract token from Authorization header              │
│  - Validate signature with JWT secret                   │
│  - Validate issuer and audience                         │
│  - Check expiration time                                │
└────────────────────┬────────────────────────────────────┘
                     │
         ┌───────────┴──────────┐
         │                      │
         ▼                      ▼
    Valid Token           Invalid/Expired
         │                      │
         ▼                      ▼
  Extract User Claims      401 Unauthorized
  Add ClaimsPrincipal      Response
         │
         ▼
┌─────────────────────────────────────────────────────────┐
│  Authorization Middleware                               │
│  - Check [Authorize] attributes                         │
│  - Verify user claims                                   │
│  - Check role requirements                              │
└────────────────────┬────────────────────────────────────┘
                     │
         ┌───────────┴──────────┐
         │                      │
         ▼                      ▼
   Authorized            Forbidden
  Request passes        403 Forbidden
  to controller          Response
```

### Error Handling Pattern

```
┌─────────────────────────────────────────────────────────┐
│  Service Method Execution                               │
└────────────────────┬────────────────────────────────────┘
                     │
         ┌───────────┴──────────────────┐
         │                              │
         ▼                              ▼
    Success Path              Exception Path
         │                              │
         ▼                    ┌─────────┴──────────┐
  Return DTOs                 │                    │
         │                    ▼                    ▼
         │              NotFoundException    Other Exception
         │                    │                    │
         │                    ▼                    ▼
         │              Caught by Controller   Unhandled/500
         │                    │
         │                    ▼
         │          Return 404 Not Found
         │
         └──────────────────┬─────────────────────┐
                            │                     │
                            ▼                     ▼
                       200 OK          400 Bad Request
                       Response        (Model Validation)
```

---

## Data Models

### User Entity

```csharp
public class User : IdentityUser
{
    // Inherited from IdentityUser:
    // - Id (GUID)
    // - UserName (username)
    // - Email (unique)
    // - PasswordHash (BCrypt)
    // - NormalizedEmail
    // - NormalizedUserName
    // - ConcurrencyStamp (optimistic concurrency)
    // - SecurityStamp
    // - PhoneNumber
    // - LockoutEnd
    // - LockoutEnabled
    // - AccessFailedCount
    // - EmailConfirmed
    // - PhoneNumberConfirmed
    // - TwoFactorEnabled
    
    // Navigation: Posts and Comments (implicit via userId)
}
```

**Relationships**:
- One-to-Many with Post (User creates multiple Posts)
- One-to-Many with Comment (User creates multiple Comments)

### Post Entity

```csharp
public class Post
{
    public int Id { get; set; }                    // Auto-generated primary key
    public string Title { get; set; }              // Post title (required)
    public string Body { get; set; }               // Post content (required)
    public DateTime CreationTime { get; set; }     // Fixed creation timestamp
    public int Likes { get; set; } = 0;            // Like counter
    public IList<Comment>? Comments { get; set; }  // Navigation property
    // Note: UserId (FK) is inherited from IdentityUser context
}
```

**Relationships**:
- Many-to-One with User (each Post has one author)
- One-to-Many with Comment (cascade delete)

### Comment Entity

```csharp
public class Comment
{
    public int Id { get; set; }                    // Auto-generated primary key
    public string Body { get; set; }               // Comment text (required)
    public DateTime CreationTime { get; set; }     // Fixed creation timestamp
    public int PostId { get; set; }                // Foreign key to Post
    public Post Post { get; set; }                 // Navigation property
    // Note: UserId (FK) is implicit via IdentityUser context
}
```

**Relationships**:
- Many-to-One with Post (each Comment references one Post)
- Many-to-One with User (each Comment has one author)

### Database Schema

```sql
-- User table (from ASP.NET Core Identity)
AspNetUsers (
    Id, UserName, Email, PasswordHash, ...
)

-- Post table
Posts (
    Id (PK), Title, Body, CreationTime, Likes, UserId (FK)
)

-- Comment table
Comments (
    Id (PK), Body, CreationTime, PostId (FK), UserId (FK)
)

-- Foreign key constraints:
Posts.UserId → AspNetUsers.Id (cascade delete)
Comments.PostId → Posts.Id (cascade delete)
Comments.UserId → AspNetUsers.Id (cascade delete)
```

---

## API Capabilities

### User Management Endpoints

#### Register User

```http
POST /api/User/Register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePassword123!"
}

Responses:
201 Created - User registration successful
400 Bad Request - Validation failed or email already exists
```

#### Login User

```http
POST /api/User/Login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePassword123!"
}

Responses:
200 OK - Login successful, JWT token returned
401 Unauthorized - Invalid credentials
```

### Post Management Endpoints

#### Create Post

```http
POST /api/Post
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "My First Post",
  "body": "This is the content of my post."
}

Responses:
201 Created - Post created successfully
400 Bad Request - Validation failed
401 Unauthorized - Missing/invalid token
```

#### Get All Posts

```http
GET /api/Post

Responses:
200 OK - Returns list of all posts
```

#### Get Post by ID

```http
GET /api/Post/{id}

Responses:
200 OK - Post found
404 Not Found - Post does not exist
```

#### Edit Post

```http
PUT /api/Post/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "Updated Title",
  "body": "Updated content."
}

Responses:
200 OK - Post updated successfully
400 Bad Request - Validation failed
401 Unauthorized - Missing/invalid token
403 Forbidden - User is not post author
404 Not Found - Post does not exist
```

#### Delete Post

```http
DELETE /api/Post/{id}
Authorization: Bearer {token}

Responses:
204 No Content - Post deleted successfully
401 Unauthorized - Missing/invalid token
403 Forbidden - User is not post author
404 Not Found - Post does not exist
```

### Comment Management Endpoints

#### Create Comment

```http
POST /api/Comment
Authorization: Bearer {token}
Content-Type: application/json

{
  "postId": 1,
  "body": "Great post! I have a question..."
}

Responses:
201 Created - Comment created successfully
400 Bad Request - Validation failed
401 Unauthorized - Missing/invalid token
404 Not Found - Post does not exist
```

#### Get Comments by Post

```http
GET /api/Comment/post/{postId}

Responses:
200 OK - Returns list of comments for post
404 Not Found - Post does not exist
```

#### Get Comment by ID

```http
GET /api/Comment/{id}

Responses:
200 OK - Comment found
404 Not Found - Comment does not exist
```

#### Edit Comment

```http
PUT /api/Comment/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "body": "Updated comment text..."
}

Responses:
200 OK - Comment updated successfully
400 Bad Request - Validation failed
401 Unauthorized - Missing/invalid token
403 Forbidden - User is not comment author
404 Not Found - Comment does not exist
```

#### Delete Comment

```http
DELETE /api/Comment/{id}
Authorization: Bearer {token}

Responses:
204 No Content - Comment deleted successfully
401 Unauthorized - Missing/invalid token
403 Forbidden - User is not comment author
404 Not Found - Comment does not exist
```

---

## Authentication & Authorization

### JWT Token Structure

Tokens are signed using **HS256 (HMAC SHA-256)** and contain:

```
Header:
{
  "alg": "HS256",
  "typ": "JWT"
}

Payload:
{
  "sub": "{userId}",           // Subject (user ID)
  "iss": "PostHubAPI",          // Issuer
  "aud": "PostHubAPI-Client",   // Audience
  "exp": {timestamp},           // Expiration time
  "iat": {timestamp}            // Issued at time
}

Signature:
HMACSHA256(Base64(Header) + "." + Base64(Payload), Secret)
```

### Token Validation Rules

| Validation | Rule | Impact |
|-----------|------|--------|
| **Signature** | Token must be signed with the correct secret | Invalid token rejected |
| **Issuer** | Token issuer must match `JWT:ValidIssuer` | Invalid issuer rejected |
| **Audience** | Token audience must match `JWT:ValidAudience` | Invalid audience rejected |
| **Expiration** | Token must not be expired | Expired token triggers re-auth |
| **HTTPS Metadata** | Token must come via HTTPS (except development) | Non-HTTPS requests rejected in production |

### Authorization Enforcement

| Resource | Permissions |
|----------|-------------|
| **POST /api/Post** | [Authorize] - Any authenticated user |
| **PUT /api/Post/{id}** | [Authorize] + ownership check - Only post author |
| **DELETE /api/Post/{id}** | [Authorize] + ownership check - Only post author |
| **POST /api/Comment** | [Authorize] - Any authenticated user |
| **PUT /api/Comment/{id}** | [Authorize] + ownership check - Only comment author |
| **DELETE /api/Comment/{id}** | [Authorize] + ownership check - Only comment author |
| **GET endpoints** | No authentication required |

---

## Environmental Configuration

### Development Environment

| Setting | Value | Purpose |
|---------|-------|---------|
| **Database** | EF Core InMemory | Fast, non-persistent for testing |
| **Swagger** | Enabled | Interactive API documentation |
| **HTTPS Required** | Disabled for JWT | Allows HTTP in development |
| **Logging** | Verbose | Detailed execution traces |

**Configuration File**: `appsettings.Development.json`

### Production Environment

| Setting | Value | Purpose |
|---------|-------|---------|
| **Database** | SQLite (file-based) | Persistent, portable data store |
| **Swagger** | Disabled | Reduces attack surface |
| **HTTPS Required** | Enabled for JWT | Enforces secure token transmission |
| **Logging** | Info level | Balanced logging for operations |

**Configuration File**: `appsettings.json`

### User Secrets (Development)

```powershell
# Initialize secrets storage
dotnet user-secrets init --project .\PostHubAPI.csproj

# Set JWT secret (required)
dotnet user-secrets set "JWT:Secret" "your-very-long-random-secret-here" --project .\PostHubAPI.csproj

# Set optional SQLite connection string
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=posthub.db" --project .\PostHubAPI.csproj
```

### Configuration Hierarchy

1. **User Secrets** (development only, highest priority)
2. **Environment Variables**
3. **appsettings.Development.json** (development)
4. **appsettings.json** (all environments)

### Required Configuration Keys

```json
{
  "JWT": {
    "Secret": "{{ must be set via user-secrets in development }}",
    "ValidIssuer": "PostHubAPI",
    "ValidAudience": "PostHubAPI-Client"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=posthub.db" // Production only
  }
}
```

---

## Quality & Testing

### Testing Strategy

**Test Framework**: xUnit with ASP.NET Core Test Host

**Test Levels**:

1. **Unit Tests**: Service logic, individual functions
   - JwtSettingsResolverTests
   - Configuration resolution tests

2. **Integration Tests**: Controller behavior, authorization
   - UserControllerTests
   - PostControllerAuthorizationTests
   - End-to-end API workflows

3. **Authorization Tests**: Access control enforcement
   - Token validation
   - Ownership verification
   - Role-based access

### Test Execution

```powershell
# Run all tests
dotnet test .\PostHubAPI.Tests\PostHubAPI.Tests.csproj

# Run specific test class
dotnet test .\PostHubAPI.Tests\PostHubAPI.Tests.csproj --filter ClassName=JwtSettingsResolverTests

# Run with coverage
dotnet test .\PostHubAPI.Tests\PostHubAPI.Tests.csproj /p:CollectCoverage=true
```

### Code Quality Standards

- **Null Safety**: Nullable reference types enabled (`<Nullable>enable</Nullable>`)
- **Implicit Usings**: Modern C# implicit global usings enabled
- **Static Analysis**: Language features enforce null checks and type safety
- **Dependency Injection**: All services registered and injected, no service locator antipattern

---

## Deployment & Operations

### Build Process

```powershell
# Restore dependencies
dotnet restore

# Build project
dotnet build --configuration Release

# Publish self-contained deployment
dotnet publish --configuration Release --output ./publish
```

### Deployment Checklist

- [ ] User secrets configured or environment variables set
- [ ] Database connection string configured (production)
- [ ] HTTPS certificate configured (production)
- [ ] JWT secret set to a strong, random value
- [ ] Valid issuer and audience values configured
- [ ] Swagger disabled in production
- [ ] Logging configured appropriately
- [ ] Application started and health-checked

### Runtime Requirements

- **.NET Runtime**: 8.0 or later
- **Memory**: Minimum 256 MB (recommended 512 MB)
- **Storage**: Database file space (production, typically small)
- **Network**: HTTP/HTTPS port access (default: 5000/5001)

### Monitoring & Observability

**Recommended Metrics**:
- API response time and throughput
- Authentication success/failure rates
- Database query performance
- Error rates by endpoint
- Resource utilization (CPU, memory, disk)

**Recommended Logs**:
- Authorization failures
- Validation errors
- Database exceptions
- Configuration warnings

---

## Constraints & Known Issues

### Current Limitations

1. **Flat Comment Structure**: Comments are not nested; all comments are direct replies to posts, not to other comments
2. **No Comment Threading**: Threading or branching of conversations is not supported
3. **No Post Editing Audit**: Post edit history is not tracked; original content is lost on edit
4. **No Soft Deletes**: All deletes are permanent; no recovery mechanism exists
5. **No Role-Based Access**: No administrator or moderator roles implemented
6. **No Content Moderation**: No flagging, reporting, or content review system
7. **Like Mechanism**: Likes are cumulative only; no user-specific like tracking or un-like capability
8. **No Pagination**: All results returned in full; no limit/offset pagination

### Known Issues

1. **JWT Configuration Key Mismatch**: JWT secret settings may be inconsistently referenced across configuration
2. **Legacy BCrypt Package**: BCrypt.Net version 0.1.0 is outdated; consider upgrading
3. **JWT Secret in Source Control**: Risk of accidental exposure if not properly managed with user-secrets
4. **InMemory Database Default**: Development database does not persist; data is lost on application restart
5. **RequireHttpsMetadata Disabled**: Development environment disables HTTPS requirement for JWT validation
6. **Edit Post Validation Gap**: Edit endpoint may have missing validation for post ownership
7. **Typo in Public API Name**: Public API naming may contain inconsistencies
8. **README Placeholder**: Clone URL in documentation may need updating

### Future Enhancement Opportunities

1. **Nested Comments**: Support reply-to-comment relationships
2. **User Profiles**: Add user profile pages with bio and avatar
3. **Search Functionality**: Full-text search on posts and comments
4. **Like System Enhancement**: Track per-user likes with unlike capability
5. **Pagination**: Implement limit/offset and cursor-based pagination
6. **Caching**: Redis integration for frequently accessed data
7. **Push Notifications**: Real-time notifications for interactions
8. **Admin Dashboard**: Content moderation and user management interface
9. **Rate Limiting**: API throttling to prevent abuse
10. **Versioning**: API versioning strategy for backward compatibility

---

## Conclusion

PostHubAPI is a foundational blogging platform with clean architecture, security best practices, and clear separation of concerns. The system is designed for extensibility and maintainability, supporting future feature additions while maintaining code quality and user security. The documented business rules, workflows, and technical specifications provide a clear roadmap for development, testing, and deployment.

---

**Document Version**: 1.0.0  
**Last Updated**: May 21, 2026  
**Next Review**: August 21, 2026  
**Owner**: Development Team
