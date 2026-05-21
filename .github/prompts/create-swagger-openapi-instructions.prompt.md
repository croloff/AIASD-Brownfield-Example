---
name: "Swagger/OpenAPI Instructions Generator"
description: "Generate instruction file for Swagger/OpenAPI documentation and discovery in this ASP.NET Core Web API"
author: "Development Team"
tags: ["swagger", "openapi", "api-documentation", "api-discovery", "swashbuckle"]
created: "2026-05-21"
---

# Prompt: Create Swagger/OpenAPI Instruction File

## Context

This PostHubAPI project uses Swashbuckle.AspNetCore 6.5.0 for API documentation with:
- **OpenAPI Specification**: Auto-generated from ASP.NET Core API controllers
- **Swagger UI**: Interactive API documentation and testing
- **Environment-Specific**: Enabled in development, disabled in production
- **Auto-Discovery**: Generates from controller actions and DTOs
- **Interactive Testing**: Client can test endpoints directly from UI

The project demonstrates:
- Swagger registration in DI container
- Swagger middleware configuration
- Environment-based Swagger availability
- Auto-generated schema from controllers and DTOs
- Endpoint discoverability

## Instructions

Create a comprehensive `.github/instructions/swagger-openapi.instructions.md` file that covers:

### 1. Swagger & OpenAPI Fundamentals
- OpenAPI specification format and versions (3.0, 3.1)
- Swagger as OpenAPI implementation
- Swagger UI for interactive exploration
- Machine-readable API contracts
- API versioning and compatibility
- API documentation benefits

### 2. Swashbuckle Integration
- Swashbuckle.AspNetCore NuGet package
- Auto-generation from C# code
- Swagger JSON endpoint (/swagger/v1/swagger.json)
- Swagger UI endpoint (/swagger/index.html, /swagger/)
- Configuration options and customization
- Middleware integration

### 3. Configuration & Setup
- AddSwaggerGen() in DI container
- AddSwaggerUI() middleware registration
- Environment-specific Swagger availability
- Swagger endpoint customization
- Title, version, and metadata
- License and contact information

### 4. XML Documentation Comments
- Using /// documentation syntax
- Documenting controllers and actions
- Documenting parameters and return values
- Documenting possible response codes
- Including usage examples
- Schema documentation from comments

### 5. Controller Documentation
- [ApiController] attribute benefits
- [Route()] for URL patterns
- [HttpGet()], [HttpPost()], [HttpPut()], [HttpDelete()]
- [Authorize] attribute visibility
- [AllowAnonymous] for public endpoints
- [Consumes()] and [Produces()]

### 6. Action Method Documentation
- Summary and description of endpoints
- Parameter documentation
- Return type documentation
- Response code documentation
- Example requests and responses
- Usage notes and warnings

### 7. DTO & Model Documentation
- Documenting DTO classes
- Documenting DTO properties
- Data annotations affecting schema
- Required vs optional properties
- String constraints (length, pattern)
- Numeric constraints (range, precision)

### 8. Request Model Validation
- DataAnnotations in DTOs
- [Required] for mandatory fields
- [StringLength()] for string constraints
- [Range()] for numeric constraints
- [RegularExpression()] for pattern matching
- Custom validation attributes
- Client-side validation generation

### 9. Response Documentation
- Documenting response types
- Documenting response status codes
- [ProducesResponseType()] for multiple responses
- Error response documentation
- Successful response examples
- Exception responses

### 10. Authentication Documentation
- Documenting [Authorize] requirement
- Bearer token authentication scheme
- Security scheme definition
- Securing endpoints in Swagger UI
- Token input in interactive testing
- Authorization flow documentation

### 11. Data Type & Schema Mapping
- Primitive types (int, string, bool, etc.)
- Complex types (classes, DTOs)
- Collections and arrays
- Nullable types
- Enums
- DateTime handling

### 12. Status Code Documentation
- 200 OK documentation
- 201 Created documentation
- 204 No Content documentation
- 400 Bad Request documentation
- 401 Unauthorized documentation
- 403 Forbidden documentation
- 404 Not Found documentation
- 500 Internal Server Error documentation

### 13. Example Requests & Responses
- Providing example JSON payloads
- [Example] attribute usage
- Multiple example scenarios
- Example error responses
- Making examples realistic
- Keeping examples maintainable

### 14. API Versioning Documentation
- Single version API documentation
- Multiple version documentation (future)
- Version in URL vs header
- Deprecation notices
- Migration guidance
- Version-specific documentation

### 15. Swagger UI Customization
- Custom CSS and themes
- UI configuration options
- Logo and branding
- Default model expansion depth
- Query string parameters
- URL customization

### 16. Swagger JSON Generation
- Generated OpenAPI specification file
- Location and accessibility
- Automating downloads for CI/CD
- Version control of generated files
- Schema caching
- Performance optimization

### 17. Integration Testing with Swagger
- Using Swagger schema in tests
- Validating response against schema
- Schema compliance testing
- Contract testing with external systems
- Breaking change detection

### 18. Client Generation
- Using Swagger/OpenAPI for code generation
- Client SDK generation from Swagger
- Language-specific code generation tools
- Swagger Codegen usage
- NSwag integration (future)
- TypeScript/JavaScript client generation

### 19. Troubleshooting
- Missing documentation on endpoints
- Incorrect data types in schema
- Authorization not appearing in Swagger UI
- Response type mismatches
- Example JSON parsing errors
- Swagger UI not loading

### 20. Best Practices
- Documenting all public endpoints
- Clear, concise descriptions
- Consistent formatting
- Providing realistic examples
- Keeping documentation in sync with code
- Using consistent naming
- Explaining business rules

### 21. Security Considerations
- Not exposing sensitive documentation in production
- Controlling Swagger UI access
- Security scheme documentation
- API key handling in documentation
- Rate limiting documentation
- Quota documentation

### 22. Performance & Optimization
- Swagger generation performance
- Schema caching
- Lazy loading of documentation
- Reducing Swagger payload size
- CDN distribution (advanced)
- Swagger UI performance tuning

### 23. Advanced Features
- Custom operation filters
- Custom schema filters
- Custom document filters
- Conditional documentation
- Dynamic example generation
- Swagger Codegen plugins

### 24. API Documentation Lifecycle
- Initial API documentation
- Keeping docs in sync with code
- Versioning documentation
- Deprecation communication
- Archive old documentation
- Breaking change notification

### 25. Tools & Utilities
- Swagger Editor for viewing specs
- Postman Swagger import
- ReDoc for alternative documentation
- Swagger UI alternatives
- API testing tools integration
- Automated API testing

## Apply To

- `Controllers/*.cs` - API controller documentation
- `Dtos/**/*.cs` - DTO model documentation
- `Program.cs` - Swagger configuration
- `Models/*.cs` - Entity model documentation (if exposed)
- Integration tests and API testing

## Version

1.0.0

## Maintainer

Development Team

## Related References

- [Official Swagger Documentation](https://swagger.io/)
- [OpenAPI Specification](https://spec.openapis.org/)
- [Swashbuckle GitHub](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- [ASP.NET Core Swagger Integration](https://docs.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger)
- [PostHubAPI Architecture](docs/architecture.md)
- [PostHubAPI Requirements](docs/PROJECT-REQUIREMENTS.md)
- Controllers: `Controllers/*.cs`
- DTOs: `Dtos/**/*.cs`
- Swagger Setup: `Program.cs` (lines 19-20, 67-70)
- Local Swagger URL: `https://localhost:5001/swagger` (development only)
