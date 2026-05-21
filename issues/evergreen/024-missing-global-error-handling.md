# Issue: Missing Global Error Handling Middleware

**Priority**: 🔴 **CRITICAL** (P1)

**Category**: Security / Error Handling

## Severity

Critical

## Description

`Program.cs` lacks centralized error handling middleware, violating ASP.NET Core API Design instructions (section 9) and Evergreen Software Development principles. Without global error handling:

- Unhandled exceptions expose stack traces and internal details to clients
- No consistent error response format across the API
- Errors are not logged centrally
- Clients receive raw exception messages instead of helpful guidance
- Security vulnerabilities (information disclosure)

**Current State**:
- No exception middleware wrapping request pipeline
- No centralized error response serialization
- Raw exceptions propagated to HTTP response

## Violated Rules

**Source**: 
- [aspnet-core-api-design.instructions.md](aspnet-core-api-design.instructions.md) - Section 9: Error Handling and Content Negotiation
- [evergreen-software-development.instructions.md](evergreen-software-development.instructions.md) - Principle 5: Design for Operability

**Requirements**:
- Return generic error messages to clients (no stack traces)
- Log full exception details for debugging
- Return consistent error response format (JSON with message and optional error code)
- Distinguish between client errors (4xx) and server errors (5xx)
- Never expose sensitive internal details

## Suggested Remediation

### Step 1: Create Global Exception Middleware

Create `Program.cs` error handling middleware (inline or in separate file):

```csharp
// Add this in Program.cs after building the app:
var app = builder.Build();

// Add error handling middleware FIRST (wraps all others)
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        // Log the full exception for debugging
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(exception, "Unhandled exception in request to {Path}", context.Request.Path);

        // Return generic error response (never expose stack trace)
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = new
        {
            message = "An internal server error occurred. Please try again later.",
            traceId = context.TraceIdentifier // For support reference
        };

        await context.Response.WriteAsJsonAsync(response);
    });
});

// Add other middleware after exception handling
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```

### Step 2: Add Logging Configuration

In `Program.cs` services section, ensure logging is configured:

```csharp
// Add after builder = WebApplication.CreateBuilder(args)
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

if (builder.Environment.IsDevelopment())
{
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
}
else
{
    builder.Logging.SetMinimumLevel(LogLevel.Information);
}
```

### Step 3: Handle Specific Exception Types (Optional but Recommended)

For domain-specific errors, add handlers:

```csharp
// In middleware setup:
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (NotFoundException ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Resource not found: {Message}", ex.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        await context.Response.WriteAsJsonAsync(new { message = ex.Message });
    }
    catch (UnauthorizedAccessException ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Access denied: {Message}", ex.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsJsonAsync(new { message = "You do not have permission to access this resource." });
    }
});
```

## Acceptance Criteria

- [ ] Global exception middleware is added to `Program.cs`
- [ ] `UseExceptionHandler()` is called **first** in middleware pipeline (before other middleware)
- [ ] Unhandled exceptions return 500 with generic message (no stack trace exposed)
- [ ] Full exception details are logged internally
- [ ] TraceId is included in error response for support correlation
- [ ] Project builds: `dotnet build`
- [ ] API starts: `dotnet run`
- [ ] Trigger an unhandled error (e.g., divide by zero in controller)
- [ ] Verify client receives: `{"message": "An internal server error occurred...", "traceId": "..."}`
- [ ] Verify server logs contain full stack trace and exception details
- [ ] Verify no sensitive details (stack traces, SQL, file paths) in error response

## Example Error Responses

**Good (before fix)**: Stack trace exposed
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
  "title": "An error occurred while processing your request.",
  "status": 500,
  "detail": "System.DivideByZeroException: Attempted to divide by zero.\n   at PostHubAPI.Controllers.PostController.GetPost(Int32 id) in C:\\repos\\PostHubAPI\\Controllers\\PostController.cs:line 42"
}
```

**Good (after fix)**: Generic message, TraceId for correlation
```json
{
  "message": "An internal server error occurred. Please try again later.",
  "traceId": "0HN1GBQV89ABC:00000001"
}
```

## Related Issues

- [Issue #018](018-exception-messages-exposed-to-clients.md): Exception messages exposed to clients
- [Issue #026](026-no-structured-logging-configuration.md): No structured logging configuration

## Provenance

- **Instruction Source**: 
  - [.github/instructions/aspnet-core-api-design.instructions.md](.github/instructions/aspnet-core-api-design.instructions.md) - Section 9
  - [.github/instructions/evergreen-software-development.instructions.md](.github/instructions/evergreen-software-development.instructions.md) - Principle 5
- **Identified by**: Program.cs Conformance Analysis
- **Date Identified**: 2026-05-21
