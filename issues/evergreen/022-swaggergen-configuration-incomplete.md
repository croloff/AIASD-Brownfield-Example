# Issue: SwaggerGen Configuration Incomplete in Program.cs

**Priority**: 🔴 **CRITICAL** (P1)

**Category**: API Documentation / Swagger Configuration

## Severity

Critical

## Description

The Swagger/OpenAPI configuration in `Program.cs` (lines 21-23) is incomplete and violates the swagger-openapi.instructions.md requirements. Missing:

1. **OpenApiInfo metadata** - No Title, Version, or Description
2. **JWT Bearer security definition** - `AddSecurityDefinition("Bearer", ...)` missing
3. **Global security requirement** - `AddSecurityRequirement(...)` missing
4. **XML comments integration** - `IncludeXmlComments()` missing

**Current Code**:
```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
```

**Result**: Swagger UI lacks API metadata, JWT authorization button is missing, and method documentation is not populated.

## Violated Rules

**Source**: [swagger-openapi.instructions.md](swagger-openapi.instructions.md)

- **Section 4** (Swagger Middleware in Program.cs): Missing `OpenApiInfo` setup
- **Section 4**: Missing `AddSecurityDefinition` for JWT Bearer scheme
- **Section 4**: Missing `AddSecurityRequirement` to require tokens globally
- **Section 2b**: Missing `IncludeXmlComments()` call
- **Section 5**: Incomplete middleware configuration

**Impact**: 
- API consumers cannot see what the API does
- Authorize button missing from Swagger UI
- Cannot test protected endpoints from Swagger UI
- API documentation is uninformative

## Suggested Remediation

Replace the current Swagger configuration (lines 21-23) with complete implementation per section 4 of swagger-openapi.instructions.md:

```csharp
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "PostHub API",
        Version = "v1",
        Description = "A social posting platform API supporting users, posts, and comments."
    });

    // Wire up XML documentation comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // JWT Bearer security definition
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT token. The 'Bearer ' prefix is added automatically."
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
```

## Acceptance Criteria

- [ ] `OpenApiInfo` includes Title, Version, and Description
- [ ] `IncludeXmlComments()` is called with the XML documentation file path
- [ ] `AddSecurityDefinition("Bearer", ...)` defines JWT Bearer scheme
- [ ] `AddSecurityRequirement(...)` requires Bearer token globally
- [ ] Project builds: `dotnet build`
- [ ] Swagger UI displays API title, version, and description
- [ ] Swagger UI shows **Authorize** button in top right
- [ ] JWT authorization can be tested from Swagger UI
- [ ] Method documentation displays in operation descriptions

## Related Issues

- [Issue #021](021-xml-documentation-not-enabled.md): XML documentation not enabled
- [Issue #011](011-swagger-documentation-incomplete.md): Swagger documentation incomplete
- [Issue #012](012-missing-producesresponsetype-attributes.md): Missing ProducesResponseType attributes

## Provenance

- **Instruction Source**: [.github/instructions/swagger-openapi.instructions.md](.github/instructions/swagger-openapi.instructions.md)
- **Instruction Sections**: 2b (XML comments), 4 (Swagger Middleware), 5 (SwaggerUI middleware)
- **Identified by**: Program.cs Conformance Analysis
- **Date Identified**: 2026-05-21
