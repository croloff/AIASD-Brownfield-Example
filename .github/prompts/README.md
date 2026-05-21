---
title: "Tech Stack Instruction Prompts Index"
description: "Collection of prompts for generating instruction files for PostHubAPI technology stack components"
created: "2026-05-21"
version: "1.0.0"
---

# Tech Stack Instruction Prompts

This directory contains prompt files (`.prompt.md`) that define the scope and requirements for generating comprehensive instruction files for the PostHubAPI tech stack components.

## Overview

Each prompt file defines a specific technology or pattern used in PostHubAPI and provides detailed guidance for generating an instruction file that covers best practices, patterns, and implementation strategies.

---

## Available Prompts

### 1. Entity Framework Core
**File**: [`create-entity-framework-core-instructions.prompt.md`](create-entity-framework-core-instructions.prompt.md)

**Scope**: ORM patterns, DbContext configuration, entity relationships, LINQ queries, migrations, and data access best practices.

**Key Topics**:
- DbContext configuration for multiple providers (InMemory, SQLite)
- Entity design and relationships
- Query patterns and performance optimization
- Mutations and transactions
- Testing with InMemory provider

**Applies To**: `Services/Implementations/*.cs`, `Data/*.cs`, `Models/*.cs`

---

### 2. JWT Authentication & Security
**File**: [`create-jwt-authentication-instructions.prompt.md`](create-jwt-authentication-instructions.prompt.md)

**Scope**: Token generation, bearer authentication, password security, ASP.NET Core Identity integration, and security best practices.

**Key Topics**:
- JWT token structure and HS256 signing
- Configuration management and user secrets
- Bearer token handling and validation
- Password security with BCrypt
- Authentication middleware and authorization enforcement
- Testing authentication scenarios

**Applies To**: `Program.cs`, `Services/Implementations/UserService.cs`, `Configuration/JwtSettingsResolver.cs`, `Controllers/*.cs`

---

### 3. ASP.NET Core API Design
**File**: [`create-aspnet-core-api-design-instructions.prompt.md`](create-aspnet-core-api-design-instructions.prompt.md)

**Scope**: RESTful API principles, HTTP semantics, controller patterns, DTO design, and API best practices.

**Key Topics**:
- RESTful URL design and routing
- HTTP status codes and semantics
- Request/Response DTOs and validation
- Controller patterns and actions
- Authorization and error handling
- Content negotiation and CORS
- API documentation with Swagger

**Applies To**: `Controllers/*.cs`, `Dtos/**/*.cs`, `Services/Implementations/*.cs`, `Program.cs`

---

### 4. AutoMapper
**File**: [`create-automapper-instructions.prompt.md`](create-automapper-instructions.prompt.md)

**Scope**: Object mapping between entities and DTOs, profile configuration, and DTO transformation patterns.

**Key Topics**:
- Profile design and organization
- Basic and complex mapping patterns
- Request/Response DTOs
- Navigation property mapping
- Configuration validation
- Performance optimization
- Testing mapping logic

**Applies To**: `Profiles/*.cs`, `Services/Implementations/*.cs`, `Controllers/*.cs`, `Dtos/**/*.cs`

---

### 5. xUnit Testing
**File**: [`create-xunit-testing-instructions.prompt.md`](create-xunit-testing-instructions.prompt.md)

**Scope**: Unit and integration testing patterns, test organization, controller testing, authorization testing, and test quality.

**Key Topics**:
- xUnit framework basics and conventions
- Unit test patterns and mocking
- Integration testing with WebApplicationFactory
- Controller and authorization testing
- Test data builders and fixtures
- Async testing patterns
- Parameterized and property-based testing
- Test coverage and quality metrics

**Applies To**: `PostHubAPI.Tests/**/*.cs`, test organization and structure

---

### 6. SQLite Database
**File**: [`create-sqlite-database-instructions.prompt.md`](create-sqlite-database-instructions.prompt.md)

**Scope**: SQLite setup, configuration, schema design, performance, backup strategies, and database administration.

**Key Topics**:
- SQLite fundamentals and setup
- Connection strings and EF Core configuration
- Schema design and data types
- Migrations and schema evolution
- Transactions and ACID properties
- Performance optimization and indexes
- Backup and recovery strategies
- Troubleshooting and deployment

**Applies To**: `Program.cs`, `Data/ApplicationDbContext.cs`, `appsettings.json`, `Models/*.cs`

---

### 7. Swagger/OpenAPI Documentation
**File**: [`create-swagger-openapi-instructions.prompt.md`](create-swagger-openapi-instructions.prompt.md)

**Scope**: API documentation with Swagger/OpenAPI, interactive API exploration, schema generation, and documentation best practices.

**Key Topics**:
- Swagger and OpenAPI fundamentals
- Swashbuckle integration
- Configuration and setup
- XML documentation comments
- Controller and action documentation
- DTO model documentation
- Response and status code documentation
- Authentication documentation
- Customization and client generation

**Applies To**: `Controllers/*.cs`, `Dtos/**/*.cs`, `Program.cs`, `Models/*.cs`

---

## Using These Prompts

### To Generate an Instruction File

1. **Open the prompt file** in your editor
2. **Use the content as input** to your AI assistant with the following command structure:

   ```
   Using the following prompt file, generate the corresponding instruction file:
   
   [Paste the complete prompt file content]
   ```

3. **Place the generated instruction file** in `.github/instructions/` with the appropriate name
4. **Add provenance metadata** according to [ai-assisted-output.instructions.md](../.github/instructions/ai-assisted-output.instructions.md)

### Example

To generate the Entity Framework Core instruction file:

```markdown
Using the prompt file located at `.github/prompts/create-entity-framework-core-instructions.prompt.md`, 
generate a comprehensive instruction file for Entity Framework Core best practices and patterns in this project. 
Name it `entity-framework-core.instructions.md` and place it in `.github/instructions/`.
```

---

## Quick Reference: Tech Stack Components

| Component | Package | Version | Instruction Prompt | Status |
|-----------|---------|---------|-------------------|--------|
| Entity Framework Core | `Microsoft.EntityFrameworkCore.Sqlite` | 8.0.1 | [EF Core Prompt](create-entity-framework-core-instructions.prompt.md) | ✅ Available |
| JWT Authentication | `Microsoft.AspNetCore.Authentication.JwtBearer` | 8.0.0 | [JWT Prompt](create-jwt-authentication-instructions.prompt.md) | ✅ Available |
| ASP.NET Core API | `Microsoft.AspNetCore.App` | 8.0 | [API Design Prompt](create-aspnet-core-api-design-instructions.prompt.md) | ✅ Available |
| AutoMapper | `AutoMapper.Extensions.Microsoft.DependencyInjection` | 12.0.1 | [AutoMapper Prompt](create-automapper-instructions.prompt.md) | ✅ Available |
| xUnit Testing | `xunit` | Latest | [xUnit Prompt](create-xunit-testing-instructions.prompt.md) | ✅ Available |
| SQLite Database | `Microsoft.EntityFrameworkCore.Sqlite` | 8.0.1 | [SQLite Prompt](create-sqlite-database-instructions.prompt.md) | ✅ Available |
| Swagger/OpenAPI | `Swashbuckle.AspNetCore` | 6.5.0 | [Swagger Prompt](create-swagger-openapi-instructions.prompt.md) | ✅ Available |
| ASP.NET Core Identity | `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | 8.0.1 | [JWT Auth Prompt](create-jwt-authentication-instructions.prompt.md) | ✅ Available |
| Password Hashing | `BCrypt.Net` | 0.1.0 | [JWT Auth Prompt](create-jwt-authentication-instructions.prompt.md) | ✅ Available |

---

## Prompt File Structure

Each prompt file follows this structure:

```yaml
---
name: "Human-Readable Technology Name"
description: "Brief description of the instruction file to be generated"
author: "Development Team"
tags: ["tag1", "tag2", "tag3"]
created: "YYYY-MM-DD"
---

# Prompt: [Technology Name] Instruction File

## Context
(Background on how the tech is used in this project)

## Instructions
(Numbered sections covering all aspects of the technology)

### 1. [Section]
(Detailed guidance for this section)

## Apply To
(File patterns and locations affected by this instruction)

## Version
(Version of the prompt)

## Maintainer
(Who maintains this prompt)

## Related References
(Links to official docs, related files, etc.)
```

---

## Generation Workflow

### Step 1: Select a Prompt
Choose the technology you want instruction coverage for from the table above.

### Step 2: Prepare Input
Copy the entire prompt file content.

### Step 3: Generate Instruction File
Use an AI assistant or instructions author to generate the corresponding `.instructions.md` file based on the prompt.

### Step 4: Validate & Review
- Ensure all sections from the prompt are covered
- Verify examples are appropriate for PostHubAPI
- Check for consistency with existing instruction files
- Validate YAML front matter with AI-assisted-output requirements

### Step 5: Place & Commit
- Save as `.github/instructions/[technology-name].instructions.md`
- Add AI provenance metadata
- Include chat log reference
- Update this index if creating new prompts

---

## Guidelines for Instruction File Authors

1. **Comprehensiveness**: Cover all sections in the prompt thoroughly
2. **Specificity**: Tailor examples to PostHubAPI where possible
3. **Consistency**: Follow the structure and tone of existing instruction files
4. **Provenance**: Include full AI-assisted-output metadata in YAML front matter
5. **Cross-Reference**: Link to related instruction files
6. **Practical**: Include actionable patterns, not just theory
7. **Updated**: Keep instruction files in sync with project needs

---

## Future Enhancements

Planned prompts for future tech stack components:

- [ ] **ASP.NET Core Identity** - User management and role-based authorization
- [ ] **Configuration Management** - Environment-specific settings and secrets
- [ ] **Logging & Monitoring** - Structured logging and observability
- [ ] **Dependency Injection** - DI container configuration and best practices
- [ ] **SOLID Principles** - Application of design principles in .NET
- [ ] **Error Handling** - Exception handling strategies and patterns
- [ ] **Performance Optimization** - Profiling, caching, and optimization techniques
- [ ] **Security Best Practices** - OWASP, SQL injection prevention, XSS protection
- [ ] **Docker & Containerization** - Building and deploying containers
- [ ] **CI/CD Integration** - GitHub Actions and automated testing/deployment

---

## Related Documentation

- [Project Requirements](../docs/PROJECT-REQUIREMENTS.md) - Overall project scope and architecture
- [Architecture Overview](../docs/architecture.md) - System design and component relationships
- [AI-Assisted Output](../instructions/ai-assisted-output.instructions.md) - Provenance and documentation standards
- [Instruction Files Guide](../instructions/instruction-files.instructions.md) - How to create instruction files
- [Evergreen Software Development](../instructions/evergreen-software-development.instructions.md) - Core development principles

---

**Index Version**: 1.0.0  
**Last Updated**: May 21, 2026  
**Maintainer**: Development Team  
**Total Prompts**: 7  
**Generated Instructions**: 0 of 7
