---
name: "AutoMapper Instructions Generator"
description: "Generate instruction file for AutoMapper DTO and entity mapping patterns in this ASP.NET Core project"
author: "Development Team"
tags: ["automapper", "dto-mapping", "object-mapping", "dtos"]
created: "2026-05-21"
---

# Prompt: Create AutoMapper Instruction File

## Context

This PostHubAPI project uses AutoMapper 12.0.1 for mapping between:
- **Entities** (models stored in database)
- **DTOs** (data transfer objects for API contracts)
- **Request objects** (input from clients)
- **Response objects** (output to clients)

The project demonstrates:
- Profile-based mapping configuration in `Profiles/` folder
- Mapping registration in DI container via `AddAutoMapper()`
- Profile inheritance patterns
- Configuration validation
- Handling complex relationships (navigation properties)
- One-way and two-way mapping strategies

## Instructions

Create a comprehensive `.github/instructions/automapper.instructions.md` file that covers:

### 1. AutoMapper Fundamentals
- Purpose: Entity-to-DTO conversion, request-to-entity binding
- Configuration approach: Profiles vs inline configuration
- Profile class structure and inheritance
- CreateMap<TSource, TDestination>() method
- Convention-based mapping

### 2. Profile Design & Organization
- One Profile per entity (PostProfile, CommentProfile, UserProfile)
- Profile naming conventions
- Profile location and folder structure
- Sharing common mapping logic across profiles
- Profile registration in DI container

### 3. Basic Mapping Patterns
- Property-to-property mapping (automatic)
- Field name matching and case sensitivity
- Ignoring unmapped properties (.ForMember(..., opt => opt.Ignore()))
- Custom property mapping (.ForMember(..., opt => opt.MapFrom(...)))
- Format strings and value transformations

### 4. Complex Mappings
- Navigation properties and nested objects
- Collections mapping (IList<T> to List<T>)
- Flattening nested properties
- Unflattening properties (denormalization)
- Projection queries with ProjectTo<>()

### 5. Request/Response DTOs
- CreatePostDto (POST request model)
- EditPostDto (PUT request model)
- ReadPostDto (GET response model)
- Separate input and output models
- API contracts vs internal models

### 6. One-Way vs Two-Way Mappings
- CreateMap<Entity, Dto>() for read operations
- CreateMap<CreateDto, Entity>() for write operations
- ReverseMap() for bidirectional mapping
- When to use one-way vs two-way
- Performance implications

### 7. Advanced Features
- Custom value resolvers for complex logic
- Custom type converters for entire types
- Null substitution strategies
- Before/After mapping hooks
- Configuration validation (AssertConfigurationIsValid())

### 8. Entity Relationships
- Mapping one-to-many relationships (Post → Comments)
- Mapping many-to-one relationships (Comment → Post)
- Including related entities in DTOs
- Avoiding circular references
- Lazy loading vs eager loading considerations

### 9. Configuration Validation
- AssertConfigurationIsValid() for testing
- Detecting unmapped properties
- Finding mapping errors early
- Unit tests for profile validation
- Production startup validation

### 10. Performance Optimization
- ProjectTo<>() for LINQ queries (IQueryable)
- Avoiding multiple round-trips to database
- Selective property inclusion
- Compiled mapping for high-volume scenarios
- Caching compiled maps

### 11. Testing Mapping Logic
- Unit tests for individual profiles
- Integration tests with actual entities
- Testing custom mappings
- Testing nested object mapping
- Validating DTO structure in API tests

### 12. Common Pitfalls
- Forgetting to configure property mappings
- Circular reference issues
- Over-mapping (exposing internal structure)
- Under-mapping (missing required fields)
- Forgetting null handling
- Configuration not validated

### 13. Mapping in Service Layer
- Injecting IMapper into services
- Mapping before returning to controller
- Mapping for response DTOs vs internal use
- When to map vs when to keep entities
- Performance of multiple map calls

### 14. Mapping in Controller Actions
- Injecting IMapper into controllers
- When controllers should map vs services
- Best practices for separation of concerns
- Mapping request DTOs to entities
- Mapping entities to response DTOs

### 15. Scenario-Based Mapping
- **Create operation**: CreatePostDto → Post entity
- **Read operation**: Post entity → ReadPostDto
- **Update operation**: EditPostDto → Post entity
- **Delete operation**: No mapping needed
- **List operations**: Post[] → ReadPostDto[]

### 16. Configuration Best Practices
- Centralizing common mapping logic
- Documenting non-obvious mappings
- Using profiles for organizational clarity
- Keeping profiles focused and single-responsibility
- Version mapping for API compatibility (future)

## Apply To

- `Profiles/*.cs` - AutoMapper profile files
- `Services/Implementations/*.cs` - Service-layer mapping calls
- `Controllers/*.cs` - API controller mapping
- `Dtos/**/*.cs` - DTO definitions referenced in mapping
- `PostHubAPI.Tests/**/*.cs` - Mapping validation tests

## Version

1.0.0

## Maintainer

Development Team

## Related References

- [Official AutoMapper Documentation](https://docs.automapper.org/)
- [AutoMapper GitHub Repository](https://github.com/AutoMapper/AutoMapper)
- [PostHubAPI Architecture](docs/architecture.md)
- [PostHubAPI Requirements](docs/PROJECT-REQUIREMENTS.md)
- AutoMapper Profiles: `Profiles/*.cs`
- DTO Definitions: `Dtos/**/*.cs`
- Entity Models: `Models/*.cs`
- DI Registration: `Program.cs` (line around `AddAutoMapper()`)
