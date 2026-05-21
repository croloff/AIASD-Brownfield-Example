---
ai_generated: true
model: "anthropic/claude-3.5-sonnet@2024-10-22"
operator: "github-copilot"
chat_id: "aspnet-core-api-design-20260521"
prompt: |
  Create a comprehensive ASP.NET Core API Design instruction file for PostHubAPI covering:
  - RESTful API principles and URL design
  - HTTP methods semantics and status codes
  - Request/response DTO patterns
  - Controller patterns and action methods
  - Request validation and authorization
  - Error handling and content negotiation
  - CORS, security, API documentation
  - Async programming and testing
  - Best practices and common pitfalls
started: "2026-05-21T10:00:00Z"
ended: "2026-05-21T10:30:00Z"
task_durations:
  - task: "instruction design"
    duration: "00:15:00"
  - task: "content authoring"
    duration: "00:12:00"
  - task: "validation and polish"
    duration: "00:03:00"
total_duration: "00:30:00"
ai_log: "ai-logs/2026/05/21/aspnet-core-api-design-20260521/conversation.md"
source: "create-aspnet-core-api-design-instructions.prompt.md"
applyTo: "Controllers/*.cs,Dtos/**/*.cs,Services/Implementations/*.cs,Program.cs"
---

# ASP.NET Core API Design Instructions

## Overview

This instruction file defines design principles and implementation patterns for building RESTful HTTP APIs in this ASP.NET Core Web API project. Follow these guidelines when designing controllers, routes, request/response models, and error handling to maintain consistency, usability, and security across the API.

## 1. RESTful API Principles

### REST Fundamentals

- **Resource-Oriented**: Design endpoints around resources (Users, Posts, Comments) rather than actions.
- **Uniform Interface**: Use standard HTTP methods consistently for similar operations.
- **Stateless**: Each request contains all information needed; server does not store client context.
- **Client-Server Separation**: Independent evolution of client and server components.
- **Layered System**: Clients cannot assume direct connection to end system.

### Key Principles

- Endpoint names should be **nouns** (resources), not verbs (actions).
  - ✅ `/api/posts`, `/api/users/{id}`, `/api/posts/{id}/comments`
  - ❌ `/api/getPost`, `/api/createUser`, `/api/updateComments`
- Use **HTTP verbs** to represent actions on resources.
- Expose **fewer endpoints** with clear semantics rather than many specialized endpoints.
- Maintain **backwards compatibility** when evolving the API.
- Design for **discoverability** so clients can learn the API structure.

## 2. URL Design & Routing

### Resource URL Patterns

#### Single Resource

```
GET    /api/users/{id}              - Retrieve a specific user
POST   /api/users                   - Create a new user
PUT    /api/users/{id}              - Update a specific user
DELETE /api/users/{id}              - Delete a specific user
```

#### Resource Collections

```
GET    /api/users                   - List all users (with optional pagination/filtering)
POST   /api/users                   - Create a new user
```

#### Nested Resources

```
GET    /api/posts/{postId}/comments          - Get all comments for a post
POST   /api/posts/{postId}/comments          - Create a comment on a post
GET    /api/posts/{postId}/comments/{id}     - Get specific comment
PUT    /api/posts/{postId}/comments/{id}     - Update specific comment
DELETE /api/posts/{postId}/comments/{id}     - Delete specific comment
```

### Naming Conventions

- Use **lowercase** for URL paths: `/api/posts` not `/api/Posts`
- Use **hyphens** (kebab-case) for multi-word resources: `/api/post-comments` not `/api/postcomments`
- Use **curly braces** for path parameters: `/api/posts/{id}`, `/api/posts/{postId}/comments/{commentId}`
- Use **query strings** for filtering, pagination, sorting:
  ```
  GET /api/posts?skip=0&take=10&sortBy=createdDate&order=desc
  GET /api/posts?userId=5&status=published
  ```

### Trailing Slashes

- **Avoid** trailing slashes: `/api/posts` not `/api/posts/`
- Configure routing to ignore trailing slashes for consistency
- Document expected behavior in API documentation

## 3. HTTP Status Codes

### Success Codes (2xx)

#### 200 OK

- Used for **successful GET, PUT, or DELETE** operations that return a response body.
- Example: `GET /api/users/1` returns user details with status 200.
- Include the complete resource in the response body.

```csharp
[HttpGet("{id}")]
public async Task<ActionResult<ReadUserDto>> GetUser(int id)
{
    var user = await _userService.GetUserByIdAsync(id);
    if (user == null)
        return NotFound();
    
    return Ok(user); // 200 OK
}
```

#### 201 Created

- Used for **successful POST** operations that create a new resource.
- Include the created resource in the response body.
- Include the `Location` header with the URL of the newly created resource.

```csharp
[HttpPost]
public async Task<ActionResult<ReadUserDto>> CreateUser(CreateUserDto dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);
    
    var user = await _userService.CreateUserAsync(dto);
    
    // Return 201 Created with Location header
    return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
}
```

#### 204 No Content

- Used for **successful DELETE or PUT operations** that do not return a response body.
- Indicates the operation succeeded but there is no content to return.
- Client should not expect a response body.

```csharp
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteUser(int id)
{
    var user = await _userService.GetUserByIdAsync(id);
    if (user == null)
        return NotFound();
    
    await _userService.DeleteUserAsync(id);
    
    return NoContent(); // 204 No Content
}
```

### Client Error Codes (4xx)

#### 400 Bad Request

- Used when the **request format is invalid** or **validation fails**.
- Include error details in the response body explaining what went wrong.
- Return `ModelState` errors for validation failures.

```csharp
[HttpPost]
public async Task<ActionResult<ReadPostDto>> CreatePost(CreatePostDto dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState); // 400 Bad Request
    
    try
    {
        var post = await _postService.CreatePostAsync(dto);
        return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
    }
    catch (ArgumentException ex)
    {
        return BadRequest(new { message = ex.Message }); // 400 Bad Request
    }
}
```

#### 401 Unauthorized

- Used when the **request lacks valid authentication** or authentication is required.
- Includes missing or expired JWT tokens.
- Do not return 403 for missing authentication; use 401.

```csharp
[Authorize] // Returns 401 if not authenticated
[HttpGet("profile")]
public async Task<ActionResult<ReadUserDto>> GetProfile()
{
    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
    var user = await _userService.GetUserByIdAsync(userId);
    return Ok(user); // 200 OK if authenticated
}
```

#### 403 Forbidden

- Used when the **request is authenticated but not authorized** for the resource.
- Typically indicates an **ownership or permission check failed**.
- Client is authenticated but lacks permission to access the resource.

```csharp
[Authorize]
[HttpPut("{id}")]
public async Task<IActionResult> UpdatePost(int id, EditPostDto dto)
{
    var post = await _postService.GetPostByIdAsync(id);
    if (post == null)
        return NotFound(); // 404 if post doesn't exist
    
    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
    if (post.UserId != userId)
        return Forbid(); // 403 Forbidden - not the owner
    
    await _postService.UpdatePostAsync(id, dto);
    return NoContent(); // 204 No Content
}
```

#### 404 Not Found

- Used when the **requested resource does not exist**.
- Include a meaningful error message identifying which resource was not found.

```csharp
[HttpGet("{id}")]
public async Task<ActionResult<ReadUserDto>> GetUser(int id)
{
    var user = await _userService.GetUserByIdAsync(id);
    
    if (user == null)
        return NotFound(new { message = $"User with ID {id} not found." }); // 404
    
    return Ok(user); // 200 OK
}
```

### Server Error Codes (5xx)

#### 500 Internal Server Error

- Used for **unhandled exceptions** and **unexpected server errors**.
- Log the full exception details for debugging.
- Return a generic error message to clients (do not expose sensitive details).
- Example: Database connection failure, null reference exception.

```csharp
catch (Exception ex)
{
    _logger.LogError(ex, "Unexpected error processing request");
    return StatusCode(500, new { message = "An internal server error occurred." }); // 500
}
```

#### 503 Service Unavailable

- Used when the **service is temporarily unavailable** (maintenance, degradation).
- Include a `Retry-After` header when possible.

## 4. Request/Response Models (DTOs)

### DTO Purpose

**Data Transfer Objects (DTOs)** separate the API contract from internal domain models:

- **Input DTOs**: Validate and shape client requests (e.g., `CreateUserDto`)
- **Output DTOs**: Serialize and shape server responses (e.g., `ReadUserDto`)
- **Prevent over-posting**: Control which fields clients can modify
- **Prevent under-posting**: Require essential fields
- **Hide internal details**: Expose only necessary properties to clients
- **Enable transformation**: Apply formatting or computed properties

### DTO Design

#### Input DTO (Request)

```csharp
// Dtos/User/CreateUserDto.cs
public class CreateUserDto
{
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(50, MinimumLength = 3, 
        ErrorMessage = "Username must be between 3 and 50 characters.")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email format is invalid.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 6,
        ErrorMessage = "Password must be at least 6 characters.")]
    public string Password { get; set; }
}
```

#### Output DTO (Response)

```csharp
// Dtos/User/ReadUserDto.cs
public class ReadUserDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

#### Edit DTO (Update Request)

```csharp
// Dtos/User/EditUserDto.cs
public class EditUserDto
{
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; }

    [EmailAddress]
    public string Email { get; set; }
    
    // Password update typically in a separate endpoint
}
```

### AutoMapper for DTO Conversion

Use AutoMapper to map between entities and DTOs:

```csharp
// Profiles/UserProfile.cs
public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, ReadUserDto>();
        CreateMap<CreateUserDto, User>();
        CreateMap<EditUserDto, User>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
```

Then in controllers:

```csharp
[HttpPost]
public async Task<ActionResult<ReadUserDto>> CreateUser(CreateUserDto dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);
    
    var user = _mapper.Map<User>(dto); // DTO to entity
    await _userService.CreateUserAsync(user);
    
    var readDto = _mapper.Map<ReadUserDto>(user); // Entity to DTO
    return CreatedAtAction(nameof(GetUser), new { id = user.Id }, readDto);
}
```

### Avoid Over-posting and Under-posting

- **Over-posting**: Client sends more fields than the API accepts. Use specific input DTOs.
  - ❌ Accept entire User entity; client could modify `Id` or `CreatedAt`
  - ✅ Accept `CreateUserDto` with only `Username`, `Email`, `Password`

- **Under-posting**: Client omits required fields. Use `[Required]` annotations.
  - ❌ `public string Email { get; set; }` (nullable, not validated)
  - ✅ `[Required] public string Email { get; set; }`

## 5. Controller Patterns

### Controller Base Class

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    // ControllerBase: No View support (API only)
    // ControllerBase: Automatic ModelState validation via [ApiController]
}
```

- Inherit from **`ControllerBase`** (not `Controller`) for API controllers.
- Use **`[ApiController]`** attribute for automatic validation and error handling.
- Use **`[Route("api/[controller]")]`** for consistent base path.

### Action Method Naming

Follow these naming conventions for clarity:

```csharp
// GET /api/users/{id}
[HttpGet("{id}")]
public async Task<ActionResult<ReadUserDto>> GetUser(int id) { }

// GET /api/users
[HttpGet]
public async Task<ActionResult<IEnumerable<ReadUserDto>>> GetUsers() { }

// POST /api/users
[HttpPost]
public async Task<ActionResult<ReadUserDto>> CreateUser(CreateUserDto dto) { }

// PUT /api/users/{id}
[HttpPut("{id}")]
public async Task<IActionResult> UpdateUser(int id, EditUserDto dto) { }

// DELETE /api/users/{id}
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteUser(int id) { }
```

### Return Types

#### `ActionResult<T>`

Preferred for most endpoints. Provides type safety and automatic OpenAPI documentation.

```csharp
[HttpGet("{id}")]
public async Task<ActionResult<ReadUserDto>> GetUser(int id)
{
    var user = await _userService.GetUserByIdAsync(id);
    if (user == null)
        return NotFound();
    
    return Ok(user);
}
```

Benefits:
- Type-safe return values
- Automatic API documentation
- Intellisense support
- Implicit conversion from T to ActionResult<T>

#### `IActionResult`

Use when returning different types or for non-standard responses.

```csharp
[HttpPost("login")]
public async Task<IActionResult> Login(LoginUserDto dto)
{
    // Can return different types: Ok, BadRequest, Unauthorized
    var result = await _userService.AuthenticateAsync(dto);
    if (result == null)
        return Unauthorized();
    
    return Ok(new { token = result.Token });
}
```

### Dependency Injection in Controllers

```csharp
[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly IMapper _mapper;
    private readonly ILogger<PostsController> _logger;

    public PostsController(
        IPostService postService,
        IMapper mapper,
        ILogger<PostsController> logger)
    {
        _postService = postService;
        _mapper = mapper;
        _logger = logger;
    }

    // Actions use injected dependencies
}
```

### Async/Await Patterns

Always use async methods for I/O operations:

```csharp
[HttpGet("{id}")]
public async Task<ActionResult<ReadPostDto>> GetPost(int id)
{
    // Async database call
    var post = await _postService.GetPostByIdAsync(id);
    
    if (post == null)
        return NotFound();
    
    return Ok(post);
}

[HttpPost]
public async Task<ActionResult<ReadPostDto>> CreatePost(CreatePostDto dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);
    
    try
    {
        // Async service call
        var post = await _postService.CreatePostAsync(dto);
        return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creating post");
        return StatusCode(500, new { message = "Internal server error" });
    }
}
```

Benefits:
- Non-blocking operations
- Better scalability
- Proper resource utilization
- Prevention of thread pool starvation

## 6. Request Validation

### Model State Validation

Use `ModelState.IsValid` to check DataAnnotations:

```csharp
[HttpPost]
public async Task<ActionResult<ReadUserDto>> CreateUser(CreateUserDto dto)
{
    // [ApiController] automatically returns 400 with ModelState if invalid
    if (!ModelState.IsValid)
        return BadRequest(ModelState);
    
    var user = await _userService.CreateUserAsync(dto);
    return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
}
```

### DataAnnotations Attributes

```csharp
public class CreatePostDto
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, MinimumLength = 5,
        ErrorMessage = "Title must be between 5 and 200 characters.")]
    public string Title { get; set; }

    [Required]
    [StringLength(5000, MinimumLength = 10,
        ErrorMessage = "Content must be at least 10 characters.")]
    public string Content { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "User ID must be valid.")]
    public int UserId { get; set; }
}
```

Common attributes:
- `[Required]` - Field is mandatory
- `[StringLength(max)]` or `[StringLength(max, MinimumLength = min)]`
- `[Range(min, max)]` - Numeric range
- `[EmailAddress]` - Valid email format
- `[Url]` - Valid URL format
- `[RegularExpression(pattern)]` - Regex validation
- `[Compare(otherProperty)]` - Compare two properties (e.g., Password confirmation)

### Custom Validation Logic

For business logic that cannot be expressed with DataAnnotations:

```csharp
[HttpPost]
public async Task<ActionResult<ReadPostDto>> CreatePost(CreatePostDto dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);
    
    try
    {
        var post = await _postService.CreatePostAsync(dto);
        return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
    }
    catch (ArgumentException ex)
    {
        // Business logic validation
        return BadRequest(new { message = ex.Message });
    }
    catch (NotFoundException ex)
    {
        // Referenced entity not found
        return NotFound(new { message = ex.Message });
    }
}
```

### Error Response Format

Standardize error responses:

```csharp
// Validation error response (400)
{
    "errors": {
        "Title": ["Title is required."],
        "Content": ["Content must be at least 10 characters."]
    }
}

// Business logic error response (400)
{
    "message": "User with that email already exists."
}

// Not found response (404)
{
    "message": "Post with ID 123 not found."
}

// Unauthorized response (401)
{
    "message": "Invalid credentials."
}

// Forbidden response (403)
{
    "message": "You do not have permission to modify this post."
}
```

## 7. Authorization & Authentication

### Authorization Attributes

Use `[Authorize]` to require authentication and `[AllowAnonymous]` for public endpoints:

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize] // All actions require authentication by default
public class PostsController : ControllerBase
{
    [AllowAnonymous] // Override: public endpoint
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReadPostDto>>> GetPosts()
    {
        // Public endpoint
        return Ok(await _postService.GetAllPostsAsync());
    }

    [Authorize] // Explicit, but inherited from class
    [HttpGet("{id}")]
    public async Task<ActionResult<ReadPostDto>> GetPost(int id)
    {
        // Authenticated users can read posts
        var post = await _postService.GetPostByIdAsync(id);
        if (post == null)
            return NotFound();
        
        return Ok(post);
    }
}
```

### User Identity Extraction

Extract the authenticated user from `User` principal:

```csharp
[Authorize]
[HttpGet("profile")]
public async Task<ActionResult<ReadUserDto>> GetProfile()
{
    // Extract user ID from JWT claim
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim == null)
        return Unauthorized();
    
    var userId = int.Parse(userIdClaim.Value);
    var user = await _userService.GetUserByIdAsync(userId);
    
    return Ok(user);
}
```

### Ownership Verification

Check ownership before allowing modifications:

```csharp
[Authorize]
[HttpPut("{id}")]
public async Task<IActionResult> UpdatePost(int id, EditPostDto dto)
{
    var post = await _postService.GetPostByIdAsync(id);
    if (post == null)
        return NotFound();
    
    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
    if (post.UserId != userId)
        return Forbid(); // 403 Forbidden - not the owner
    
    await _postService.UpdatePostAsync(id, dto);
    return NoContent();
}

[Authorize]
[HttpDelete("{id}")]
public async Task<IActionResult> DeletePost(int id)
{
    var post = await _postService.GetPostByIdAsync(id);
    if (post == null)
        return NotFound();
    
    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
    if (post.UserId != userId)
        return Forbid(); // 403 Forbidden
    
    await _postService.DeletePostAsync(id);
    return NoContent();
}
```

### Claims-Based Authorization

Use claims for role-based or policy-based authorization:

```csharp
// In Program.cs
services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Admin"));
});

// In controller
[Authorize(Policy = "AdminOnly")]
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteUser(int id)
{
    // Only admin users can delete users
    await _userService.DeleteUserAsync(id);
    return NoContent();
}
```

## 8. Error Handling & Responses

### Exception Handling Strategy

Create custom exceptions for domain-specific errors:

```csharp
// Exceptions/NotFoundException.cs
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

// Exceptions/ValidationException.cs
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}
```

Map exceptions to HTTP responses in controllers:

```csharp
[HttpPost]
public async Task<ActionResult<ReadPostDto>> CreatePost(CreatePostDto dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState); // 400 for validation errors
    
    try
    {
        var post = await _postService.CreatePostAsync(dto);
        return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
    }
    catch (ValidationException ex)
    {
        return BadRequest(new { message = ex.Message }); // 400
    }
    catch (NotFoundException ex)
    {
        return NotFound(new { message = ex.Message }); // 404
    }
    catch (UnauthorizedAccessException ex)
    {
        return Forbid(); // 403
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unexpected error creating post");
        return StatusCode(500, new { message = "Internal server error" }); // 500
    }
}
```

### Middleware for Global Exception Handling

Create middleware to handle exceptions globally:

```csharp
// Middleware/ExceptionHandlingMiddleware.cs
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new { message = "An internal server error occurred." };

        switch (exception)
        {
            case NotFoundException:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                response = new { message = exception.Message };
                break;
            case ValidationException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response = new { message = exception.Message };
                break;
            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                break;
        }

        return context.Response.WriteAsJsonAsync(response);
    }
}

// In Program.cs
app.UseMiddleware<ExceptionHandlingMiddleware>();
```

### Meaningful Error Messages

Provide actionable error messages to clients:

```csharp
// ❌ Avoid vague messages
return BadRequest(new { message = "Error" });
return NotFound();
return StatusCode(500);

// ✅ Provide context and actionable information
return BadRequest(new { message = "Email address is already registered. Use a different email or reset your password." });
return NotFound(new { message = $"Post with ID {id} not found. Check the ID and try again." });
return StatusCode(500, new { message = "An internal server error occurred. Please contact support with reference ID: {referenceId}" });
```

## 9. Response Formats

### JSON as Primary Format

All responses should use JSON format:

```csharp
[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<ReadPostDto>> GetPost(int id)
    {
        var post = await _postService.GetPostByIdAsync(id);
        if (post == null)
            return NotFound();
        
        return Ok(post); // Automatically serialized to JSON
    }
}
```

### Content-Type Header

Ensure responses include correct `Content-Type`:

```
Content-Type: application/json; charset=utf-8
```

ASP.NET Core automatically sets this header.

### Camel Case Property Naming

Configure JSON serialization to use camel case (JavaScript convention):

```csharp
// In Program.cs
services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
```

Result:
```csharp
public class ReadUserDto
{
    public int Id { get; set; }           // Serialized as "id"
    public string Username { get; set; } // Serialized as "username"
    public DateTime CreatedAt { get; set; } // Serialized as "createdAt"
}
```

### Consistent Response Structure

Maintain consistent response envelopes:

```csharp
// Simple response (single resource)
{
    "id": 1,
    "username": "john",
    "email": "john@example.com",
    "createdAt": "2026-01-15T10:30:00Z"
}

// Array response (collection)
[
    { "id": 1, "username": "john", "email": "john@example.com" },
    { "id": 2, "username": "jane", "email": "jane@example.com" }
]

// Error response
{
    "message": "User not found",
    "statusCode": 404
}

// Paginated response
{
    "data": [ /* items */ ],
    "totalCount": 50,
    "pageNumber": 1,
    "pageSize": 10
}
```

### Metadata in Responses

Include relevant metadata for pagination:

```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<ReadPostDto>>> GetPosts([FromQuery] int skip = 0, [FromQuery] int take = 10)
{
    var (posts, total) = await _postService.GetPostsAsync(skip, take);
    
    Response.Headers.Add("X-Total-Count", total.ToString());
    Response.Headers.Add("X-Skip", skip.ToString());
    Response.Headers.Add("X-Take", take.ToString());
    
    return Ok(posts);
}
```

## 10. Content Negotiation

### Accept Header Handling

Respect the `Accept` header to determine response format:

```csharp
// Client sends Accept: application/json
// Server returns JSON response
```

ASP.NET Core handles content negotiation automatically when multiple formatters are registered.

### Default Response Format

JSON is the default format for ASP.NET Core APIs:

```csharp
// In Program.cs
services.AddControllers();
    // .AddXmlDataContractSerializerFormatters(); // Add XML support if needed
```

### Supporting Multiple Formats (Future)

If supporting multiple formats:

```csharp
[HttpGet("{id}")]
[Produces("application/json", "application/xml")]
public async Task<ActionResult<ReadPostDto>> GetPost(int id)
{
    var post = await _postService.GetPostByIdAsync(id);
    if (post == null)
        return NotFound();
    
    return Ok(post); // Returns JSON or XML based on Accept header
}
```

## 11. CORS & Security

### CORS Configuration

Configure Cross-Origin Resource Sharing in `Program.cs`:

```csharp
// In Program.cs
services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", builder =>
    {
        builder
            .WithOrigins("http://localhost:3000", "http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Apply CORS policy
app.UseCors("AllowLocalhost");
```

### Origin Whitelisting

Always use explicit origin whitelisting instead of `AllowAnyOrigin`:

```csharp
// ❌ Avoid: Allows any origin
.WithOrigins("*")
.AllowCredentials(); // INVALID: Cannot use * with AllowCredentials

// ✅ Use explicit whitelist
.WithOrigins("https://example.com", "https://app.example.com")
.AllowCredentials();
```

### Credential Handling

Configure CORS credentials carefully:

```csharp
// Allow credentials (for cookies, JWT in Authorization header)
.AllowCredentials()

// Ensure client sends credentials
// In JavaScript:
fetch('/api/posts', {
    method: 'GET',
    credentials: 'include' // Include cookies and auth headers
});
```

### Preflight Requests

CORS preflight requests (OPTIONS) are handled automatically:

```
OPTIONS /api/posts
Access-Control-Allow-Origin: http://localhost:3000
Access-Control-Allow-Methods: GET, POST, PUT, DELETE
Access-Control-Allow-Headers: Content-Type, Authorization
```

### Security Headers

Add security headers in middleware:

```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    
    await next();
});
```

### HTTPS Enforcement

Require HTTPS in production:

```csharp
// In Program.cs
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseHsts(); // HTTP Strict-Transport-Security
}
```

## 12. API Documentation

### Swagger/OpenAPI Integration

Configure Swagger in `Program.cs`:

```csharp
// In Program.cs
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PostHub API",
        Version = "v1",
        Description = "A simple social media API for posts and comments",
        Contact = new OpenApiContact
        {
            Name = "PostHub Team",
            Email = "support@posthub.com"
        }
    });

    // Include XML documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Add JWT security definition
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Description = "JWT Authorization header using the Bearer scheme"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

// Add Swagger UI middleware
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PostHub API v1");
});
```

### XML Documentation Comments

Document endpoints with XML comments:

```csharp
/// <summary>
/// Retrieves a specific post by ID.
/// </summary>
/// <param name="id">The post ID</param>
/// <returns>The requested post if found</returns>
/// <response code="200">Post found</response>
/// <response code="404">Post not found</response>
[HttpGet("{id}")]
[ProduceResponseType(typeof(ReadPostDto), StatusCodes.Status200OK)]
[ProduceResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult<ReadPostDto>> GetPost(int id)
{
    var post = await _postService.GetPostByIdAsync(id);
    if (post == null)
        return NotFound();
    
    return Ok(post);
}
```

### ProduceResponseType Attribute

Explicitly document response types:

```csharp
[HttpPost]
[ProduceResponseType(typeof(ReadPostDto), StatusCodes.Status201Created)]
[ProduceResponseType(StatusCodes.Status400BadRequest)]
[ProduceResponseType(StatusCodes.Status401Unauthorized)]
public async Task<ActionResult<ReadPostDto>> CreatePost(CreatePostDto dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);
    
    var post = await _postService.CreatePostAsync(dto);
    return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
}
```

### Swagger UI

Access interactive documentation at `https://localhost:5001/swagger/ui`

## 13. Async Programming

### Async Action Methods

All I/O operations must be async:

```csharp
// ✅ Correct: Async database call
[HttpGet("{id}")]
public async Task<ActionResult<ReadUserDto>> GetUser(int id)
{
    var user = await _userService.GetUserByIdAsync(id);
    if (user == null)
        return NotFound();
    
    return Ok(user);
}

// ❌ Avoid: Blocking call
[HttpGet("{id}")]
public ActionResult<ReadUserDto> GetUser(int id)
{
    var user = _userService.GetUserById(id); // Blocks thread
    if (user == null)
        return NotFound();
    
    return Ok(user);
}
```

### Async Service Calls

Services should expose async methods:

```csharp
public interface IPostService
{
    Task<PostDto> GetPostByIdAsync(int id);
    Task<PostDto> CreatePostAsync(CreatePostDto dto);
    Task UpdatePostAsync(int id, EditPostDto dto);
    Task DeletePostAsync(int id);
}

public class PostService : IPostService
{
    public async Task<PostDto> GetPostByIdAsync(int id)
    {
        var post = await _context.Posts.FindAsync(id);
        return _mapper.Map<PostDto>(post);
    }
}
```

### Cancellation Token Propagation

Pass cancellation tokens for graceful shutdown:

```csharp
[HttpGet("{id}")]
public async Task<ActionResult<ReadPostDto>> GetPost(int id, CancellationToken cancellationToken)
{
    var post = await _postService.GetPostByIdAsync(id, cancellationToken);
    if (post == null)
        return NotFound();
    
    return Ok(post);
}

public async Task<PostDto> GetPostByIdAsync(int id, CancellationToken cancellationToken)
{
    var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    return _mapper.Map<PostDto>(post);
}
```

### Deadlock Prevention

Avoid blocking on async code:

```csharp
// ❌ Can cause deadlock
var result = _userService.GetUserAsync(1).Result; // NEVER use .Result or .Wait()

// ✅ Use await
var result = await _userService.GetUserAsync(1);
```

## 14. Testing API Endpoints

### Integration Tests with WebApplicationFactory

Create integration tests using `WebApplicationFactory`:

```csharp
public class PostControllerTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;

    public async Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Fact]
    public async Task GetPost_WithValidId_ReturnsOkWithPost()
    {
        // Arrange
        var postId = 1;

        // Act
        var response = await _client.GetAsync($"/api/posts/{postId}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var post = await response.Content.ReadAsAsync<ReadPostDto>();
        post.Id.Should().Be(postId);
    }

    [Fact]
    public async Task GetPost_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/posts/99999");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreatePost_WithValidData_ReturnsCreated()
    {
        // Arrange
        var dto = new CreatePostDto
        {
            Title = "Test Post",
            Content = "This is a test post.",
            UserId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/posts", dto);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        var createdPost = await response.Content.ReadAsAsync<ReadPostDto>();
        createdPost.Title.Should().Be("Test Post");
    }

    [Fact]
    public async Task UpdatePost_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var dto = new EditPostDto { Title = "" }; // Invalid: empty title

        // Act
        var response = await _client.PutAsJsonAsync("/api/posts/1", dto);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
}
```

### Testing Authorization

Test authorization and ownership checks:

```csharp
[Fact]
public async Task UpdatePost_WithoutAuthentication_ReturnsUnauthorized()
{
    // Arrange
    var dto = new EditPostDto { Title = "Updated" };

    // Act
    var response = await _client.PutAsJsonAsync("/api/posts/1", dto);

    // Assert
    response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
}

[Fact]
public async Task UpdatePost_NotOwner_ReturnsForbidden()
{
    // Arrange
    _client.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", "token-for-user-2");
    
    var dto = new EditPostDto { Title = "Updated" };
    var postId = 1; // Created by user 1

    // Act
    var response = await _client.PutAsJsonAsync($"/api/posts/{postId}", dto);

    // Assert
    response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
}
```

### Test Data Seeding

Seed test data in test fixtures:

```csharp
public class PostControllerTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;

    public async Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Replace database with in-memory for testing
                    var descriptor = services.FirstOrDefault(d =>
                        d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase("TestDatabase"));
                });
            });

        _client = _factory.CreateClient();
        
        // Seed test data
        await SeedTestDataAsync();
    }

    private async Task SeedTestDataAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        context.Users.Add(new User { Id = 1, Username = "testuser", Email = "test@example.com" });
        context.Posts.Add(new Post { Id = 1, Title = "Test Post", UserId = 1 });
        
        await context.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
```

## 15. Best Practices

### Consistent Naming Conventions

- **Controllers**: Plural resource names + "Controller"
  - `UsersController`, `PostsController`, `CommentsController`
- **Actions**: Verb + Resource Name (optional) or HTTP method convention
  - `GetUser`, `CreatePost`, `UpdateComment`, `DeleteUser`
- **Routes**: Lowercase, plural resources
  - `/api/users`, `/api/posts/{id}`, `/api/posts/{postId}/comments`
- **DTOs**: Resource Name + "Dto" (Input: Create/Edit, Output: Read)
  - `CreateUserDto`, `EditPostDto`, `ReadCommentDto`
- **Services**: Resource Name + "Service"
  - `UserService`, `PostService`, `CommentService`

### Lowercase URLs

Always use lowercase in routes:

```csharp
// ✅ Correct
[Route("api/[controller]")] // Becomes /api/users (lowercase)

// ❌ Avoid
[Route("api/Users")] // Becomes /api/Users (mixed case)
```

### Resource-Focused Design

Design around resources, not actions:

```csharp
// ✅ Resource-focused
GET    /api/posts
POST   /api/posts
GET    /api/posts/{id}
PUT    /api/posts/{id}
DELETE /api/posts/{id}

// ❌ Action-focused (avoid)
GET    /api/getPosts
POST   /api/createPost
GET    /api/getPost
PUT    /api/updatePost
DELETE /api/deletePost
```

### Hypermedia Links (Future)

Consider hypermedia links for API discoverability:

```json
{
    "id": 1,
    "title": "My Post",
    "_links": {
        "self": { "href": "/api/posts/1" },
        "edit": { "href": "/api/posts/1", "method": "PUT" },
        "delete": { "href": "/api/posts/1", "method": "DELETE" },
        "comments": { "href": "/api/posts/1/comments" }
    }
}
```

### API Versioning Strategy (Future)

Plan for versioning when the API evolves:

```csharp
// URL-based versioning
GET /api/v1/users
GET /api/v2/users

// Header-based versioning
GET /api/users
Header: API-Version: 1

// Query parameter versioning
GET /api/users?version=1
```

### Rate Limiting (Future)

Implement rate limiting to prevent abuse:

```csharp
services.AddRateLimiter(options =>
{
    options.AddSlidingWindowLimiter("default", configure => 
    {
        configure.PermitLimit = 100;
        configure.Window = TimeSpan.FromMinutes(1);
    });
});
```

### Deprecation Strategies

Mark endpoints as deprecated with warnings:

```csharp
[Obsolete("Use /api/v2/users instead. This endpoint will be removed on 2026-12-31.")]
[HttpGet("legacy")]
public async Task<IActionResult> GetUsersLegacy()
{
    Response.Headers.Add("Deprecation", "true");
    Response.Headers.Add("Sunset", "Sun, 31 Dec 2026 23:59:59 GMT");
    
    return Ok(/* ... */);
}
```

## 16. Common Pitfalls

### ❌ Chatty APIs

Multiple round-trips for related data:

```csharp
// Chatty: 5 API calls
var user = await GetUserAsync(1);        // 1st call
var posts = await GetUserPostsAsync(1);  // 2nd call
foreach (var post in posts)
{
    var comments = await GetPostCommentsAsync(post.Id); // 3 more calls
}
```

**Better**: Return nested data in single call:

```csharp
// Efficient: 1 API call
GET /api/users/1?include=posts,comments
```

### ❌ Exposing Internal Data Structures

Exposing internal entities in API:

```csharp
// ❌ Never expose entities directly
[HttpGet("{id}")]
public async Task<ActionResult<User>> GetUser(int id) // User entity
{
    return await _context.Users.FindAsync(id);
}

// ✅ Use DTOs
[HttpGet("{id}")]
public async Task<ActionResult<ReadUserDto>> GetUser(int id) // DTO
{
    var user = await _userService.GetUserByIdAsync(id);
    return Ok(_mapper.Map<ReadUserDto>(user));
}
```

### ❌ Inconsistent Status Codes

Different status codes for similar scenarios:

```csharp
// ❌ Inconsistent
public IActionResult GetUser(int id)
{
    var user = _service.GetUser(id);
    if (user == null)
        return Ok(null); // Sometimes 200 with null
}

public IActionResult GetPost(int id)
{
    var post = _service.GetPost(id);
    if (post == null)
        return NotFound(); // Sometimes 404
}

// ✅ Consistent
public ActionResult<ReadUserDto> GetUser(int id)
{
    var user = _service.GetUser(id);
    if (user == null)
        return NotFound(); // Always 404 for missing resources
    
    return Ok(_mapper.Map<ReadUserDto>(user));
}

public ActionResult<ReadPostDto> GetPost(int id)
{
    var post = _service.GetPost(id);
    if (post == null)
        return NotFound(); // Always 404 for missing resources
    
    return Ok(_mapper.Map<ReadPostDto>(post));
}
```

### ❌ Poor Error Messages

Vague or technical error messages:

```csharp
// ❌ Unhelpful
return BadRequest("Validation failed");
return BadRequest("Invalid input");
return StatusCode(500, "Error occurred");

// ✅ Actionable
return BadRequest(new { message = "Email must be valid and unique. Check your input or use a different email address." });
return BadRequest(new { message = "Username must be 3-50 characters and alphanumeric." });
return StatusCode(500, new { message = "Unable to process request. Please try again later or contact support at support@example.com." });
```

### ❌ Missing Validation

Allowing invalid data:

```csharp
// ❌ No validation
[HttpPost]
public async Task<ActionResult<ReadUserDto>> CreateUser(CreateUserDto dto)
{
    var user = _mapper.Map<User>(dto); // No checks
    await _context.Users.AddAsync(user);
    await _context.SaveChangesAsync();
    
    return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
}

// ✅ Validation
[HttpPost]
public async Task<ActionResult<ReadUserDto>> CreateUser(CreateUserDto dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);
    
    var existingUser = await _userService.GetUserByEmailAsync(dto.Email);
    if (existingUser != null)
        return BadRequest(new { message = "Email already registered." });
    
    var user = await _userService.CreateUserAsync(dto);
    return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
}
```

### ❌ Leaking Exceptions to Clients

Exposing internal implementation details:

```csharp
// ❌ Exposes full stack trace
return StatusCode(500, ex.ToString());

// ✅ Generic message with unique reference
var referenceId = Guid.NewGuid();
_logger.LogError(ex, $"Error {referenceId}");
return StatusCode(500, new { message = "An error occurred. Reference: " + referenceId });
```

## Delivery Checklist

Before committing API code:

- [ ] All endpoints follow RESTful conventions (nouns for resources, HTTP verbs for actions)
- [ ] Appropriate status codes used (200, 201, 204, 400, 401, 403, 404, 500)
- [ ] Input validation applied (DataAnnotations, ModelState checks)
- [ ] Authorization checks in place for protected endpoints
- [ ] Error responses are consistent and actionable
- [ ] DTOs separate API contract from domain models
- [ ] Async/await patterns used for all I/O operations
- [ ] Endpoints are documented with XML comments
- [ ] CORS configured appropriately for client origins
- [ ] Security headers included in responses
- [ ] Tests cover happy paths, error cases, and authorization
- [ ] No sensitive data exposed in responses or logs

## Summary

ASP.NET Core API design emphasizes:

- **REST principles**: Resource-oriented, standard HTTP verbs, stateless interaction
- **Clear contracts**: DTOs for input/output, explicit validation
- **Proper status codes**: Meaningful codes for client error handling
- **Security**: Authorization, HTTPS, secure headers
- **Documentation**: Swagger, XML comments, clear error messages
- **Testability**: Integration tests, mocked dependencies
- **Consistency**: Uniform naming, response formats, error handling

Follow these principles to build reliable, maintainable, and discoverable APIs.

---

**Document Version**: 1.0.0
**Last Updated**: 2026-05-21
**Maintainer**: Development Team
**Related Files**: [Controllers](../../Controllers), [DTOs](../../Dtos), [Services](../../Services), [Program.cs](../../Program.cs)
