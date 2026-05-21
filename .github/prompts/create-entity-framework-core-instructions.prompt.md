---
name: "Entity Framework Core Instructions Generator"
description: "Generate instruction file for Entity Framework Core data access patterns and best practices in this .NET 8 project"
author: "Development Team"
tags: ["entity-framework-core", "orm", "data-access", "entity-framework"]
created: "2026-05-21"
---

# Prompt: Create Entity Framework Core Instruction File

## Context

This PostHubAPI project uses Entity Framework Core 8.0.1 with environment-specific providers:
- **Development**: InMemory provider for fast, non-persistent testing
- **Production**: SQLite for portable, file-based persistence

The project demonstrates:
- DbContext configuration for multiple providers
- Migration patterns (implicit in development, explicit in production)
- Entity relationships (User → Post → Comment hierarchy)
- Cascade delete behavior
- Lazy loading and eager loading strategies

## Instructions

Create a comprehensive `.github/instructions/entity-framework-core.instructions.md` file that covers:

### 1. DbContext Configuration
- Best practices for registering DbContext in DI container
- Environment-specific provider setup (InMemory vs SQLite)
- Connection string configuration and secrets management
- Initialization and seeding patterns

### 2. Entity Design
- Entity class structure and validation
- Navigation properties and foreign key relationships
- Shadow properties and data annotations
- Timestamp patterns (CreatedAt, UpdatedAt)
- Default values and computed properties

### 3. Queries and LINQ
- LINQ to Entities query patterns
- Eager loading with Include() to prevent N+1 queries
- Explicit loading for conditional data
- Query performance considerations
- AsNoTracking() for read-only queries

### 4. Mutations and Save Operations
- Adding, updating, and deleting entities
- Cascade delete configuration and behavior
- Optimistic concurrency control
- Transaction usage patterns
- SaveChanges() best practices

### 5. Relationships Management
- One-to-Many relationships (User ↔ Post, Post ↔ Comment)
- Foreign key configuration
- Inverse relationships and navigation properties
- Cascade behavior on delete
- Relationship loading strategies

### 6. Testing Patterns
- InMemory provider for unit tests
- Seeding test data
- Asserting on database state
- Isolation between tests
- Mock DbContext considerations

### 7. Common Pitfalls
- Forgetting to include related entities
- N+1 query problems
- Change tracking issues
- Async/await patterns with async queries
- Tracking vs non-tracking query implications

### 8. Migration & Schema Evolution (for future use)
- Creating migrations
- Applying migrations
- Rollback strategies
- Production deployment considerations

## Apply To

- `**/*.cs` files in Services, Data, and Models folders
- Focus on files that use DbContext or entity operations

## Version

1.0.0

## Maintainer

Development Team

## Related References

- [Official EF Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [PostHubAPI Architecture](docs/architecture.md)
- [PostHubAPI Requirements](docs/PROJECT-REQUIREMENTS.md)
- Entity models: `Models/*.cs`
- DbContext: `Data/ApplicationDbContext.cs`
- Services using EF: `Services/Implementations/*.cs`
