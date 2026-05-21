---
name: "ASP.NET Core API Design Instructions Generator"
description: "Generate instruction file for RESTful API design, controller patterns, and HTTP semantics in this ASP.NET Core Web API"
author: "Development Team"
tags: ["aspnet-core", "api-design", "rest", "http", "controllers"]
created: "2026-05-21"
---

# Prompt: Create ASP.NET Core API Design Instruction File

## Context

This PostHubAPI project implements a RESTful HTTP API with:
- **Three resource controllers**: UserController, PostController, CommentController
- **Standard HTTP methods**: POST (create), GET (read), PUT (update), DELETE (delete)
- **REST conventions**: URL structure, HTTP status codes, content negotiation
- **Request/Response DTOs**: Separate models for input and output
- **Status codes**: 200 OK, 201 Created, 204 No Content, 400 Bad Request, 401 Unauthorized, 403 Forbidden, 404 Not Found
- **Error handling**: Exception mapping to appropriate HTTP responses
- **Model validation**: DataAnnotations and ModelState checks

The project demonstrates:
- Controller action patterns for CRUD operations
- DTO usage for API contracts
- Dependency injection in controllers
- Status code selection and semantics
- Error response formatting

## Instructions

Create a comprehensive `.github/instructions/aspnet-core-api-design.instructions.md` file that covers:

### 1. RESTful API Principles
- REST conventions and URL design
- Resource-oriented endpoints
- HTTP methods semantics (GET, POST, PUT, DELETE)
- Idempotency and safe operations
- Stateless interaction principles

### 2. URL Design & Routing
- RESTful URL patterns (/api/resource, /api/resource/{id})
- Nested resources (/api/post/{id}/comments)
- Query parameters for filtering and pagination
- Versioning strategies (future enhancement)
- Trailing slashes and consistency

### 3. HTTP Status Codes
- **Success codes**:
  - 200 OK - Successful GET/PUT with response body
  - 201 Created - Resource successfully created
  - 204 No Content - Successful DELETE with no response body
- **Client error codes**:
  - 400 Bad Request - Invalid request format or validation failed
  - 401 Unauthorized - Missing or invalid authentication
  - 403 Forbidden - Authenticated but not authorized (ownership check)
  - 404 Not Found - Resource does not exist
- **Server error codes**:
  - 500 Internal Server Error - Unhandled exceptions
  - 503 Service Unavailable - Maintenance or degradation

### 4. Request/Response Models (DTOs)
- Separation of concerns: Request vs Response DTOs
- Input validation with DataAnnotations
- Nullable reference types for clarity
- AutoMapper for entity-to-DTO conversion
- Avoiding over-posting and under-posting
- Selective field inclusion in responses

### 5. Controller Patterns
- Inheriting from ControllerBase (vs Controller)
- [ApiController] attribute and its benefits
- Action method naming conventions
- Return types: IActionResult vs ActionResult<T>
- Async/await patterns
- Dependency injection in constructors

### 6. Request Validation
- Model state checking (ModelState.IsValid)
- DataAnnotations attributes (Required, StringLength, etc.)
- Custom validation logic in services
- Client error responses (400 Bad Request)
- Validation error message formatting

### 7. Authorization & Authentication
- [Authorize] attributes on controllers and actions
- [AllowAnonymous] for public endpoints
- User identity extraction from HttpContext.User
- Claims-based authorization
- Ownership verification in business logic

### 8. Error Handling & Responses
- Exception handling strategy
- Domain exceptions vs system exceptions
- NotFoundException mapping to 404
- Bad request mapping to 400
- Error response format consistency
- Meaningful error messages for clients

### 9. Response Formats
- JSON as primary format
- Content-Type header negotiation
- Camel case property naming in JSON
- Consistent response envelope (or no envelope)
- Metadata in responses (count, page, etc.)

### 10. Content Negotiation
- Accept header handling
- Default response format (application/json)
- Supporting multiple formats (future)
- Media type versioning

### 11. CORS & Security
- Cross-Origin Resource Sharing (CORS) configuration
- Origin whitelisting
- Credential handling with CORS
- Preflight requests
- Security headers

### 12. API Documentation
- Swagger/OpenAPI integration
- XML documentation comments on actions
- Describing endpoints, parameters, responses
- Swagger UI for interactive documentation
- OpenAPI schema generation

### 13. Async Programming
- Async action methods (async Task<IActionResult>)
- Async service calls and database operations
- Cancellation token propagation
- Deadlock prevention
- Performance implications

### 14. Testing API Endpoints
- Integration tests with WebApplicationFactory
- Testing endpoint behavior
- Testing authorization
- Testing error responses
- Test data seeding

### 15. Best Practices
- Consistent naming conventions
- URL lowercase convention
- Resource-focused design
- Hypermedia links (future)
- API versioning strategy (future)
- Rate limiting (future)
- Deprecation strategies

### 16. Common Pitfalls
- Chatty APIs (multiple round-trips)
- Exposing internal data structures
- Inconsistent status codes
- Poor error messages
- Missing validation
- Leaking exceptions to clients

## Apply To

- `Controllers/*.cs` - All controller files
- `Dtos/**/*.cs` - All DTO definitions
- `Services/Implementations/*.cs` - Service method design
- `Program.cs` - API configuration and middleware

## Version

1.0.0

## Maintainer

Development Team

## Related References

- [Official ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [REST API Best Practices](https://restfulapi.net/)
- [HTTP Status Codes](https://httpwg.org/specs/rfc7231.html#status.codes)
- [PostHubAPI Architecture](docs/architecture.md)
- [PostHubAPI Requirements](docs/PROJECT-REQUIREMENTS.md)
- Controllers: `Controllers/*.cs`
- DTOs: `Dtos/**/*.cs`
- Swagger configuration: `Program.cs` (lines 19-20)
