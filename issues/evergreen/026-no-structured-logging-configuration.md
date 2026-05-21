# Issue: No Structured Logging Configuration in Program.cs

**Priority**: 🟡 **IMPORTANT** (P2)

**Category**: Observability / Logging

## Severity

High

## Description

Structured logging is not explicitly configured in `Program.cs`, limiting observability and operational visibility. While .NET provides basic console logging by default, the application lacks:

- Explicit log level configuration per environment
- Structured logging output for better log aggregation
- Clear logging configuration as code
- Proper documentation of logging behavior

This violates Evergreen Software Development principles (Principle 5: Design for Operability) which requires:
- Structured logs, metrics, and tracing for critical workflows
- Surface actionable errors with enough context to diagnose quickly

## Violated Rules

**Source**: [evergreen-software-development.instructions.md](evergreen-software-development.instructions.md)

**Principle 5: Design for Operability**:
> Include structured logs, metrics, and tracing for critical workflows. Surface actionable errors with enough context to diagnose quickly.

**Current State**:
- No explicit logging configuration in `Program.cs`
- Relying on default ASP.NET Core logging setup
- No environment-aware log level configuration
- No indication of what is being logged or at what levels

## Suggested Remediation

Add explicit logging configuration to `Program.cs` after the builder is created (around line 14):

```csharp
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var isDevelopment = builder.Environment.IsDevelopment();
var jwtSettings = JwtSettingsResolver.Resolve(configuration);

// Configure structured logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Set log levels per environment
if (isDevelopment)
{
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
    builder.Logging.AddDebug();
}
else
{
    builder.Logging.SetMinimumLevel(LogLevel.Information);
}

// Add services to the container.
builder.Services.AddControllers();
// ... rest of configuration
```

### Enhanced Configuration (Recommended)

For better structured logging, consider adding a structured logging provider like Serilog:

**Step 1**: Install Serilog NuGet packages (in future)
```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
```

**Step 2**: Configure in Program.cs
```csharp
var builder = WebApplication.CreateBuilder(args);

// Configure structured logging with Serilog
builder
    .Host
    .UseSerilog((context, loggerConfig) =>
    {
        loggerConfig
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "PostHubAPI")
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}");

        if (!context.HostingEnvironment.IsDevelopment())
        {
            loggerConfig.MinimumLevel.Information();
        }
        else
        {
            loggerConfig.MinimumLevel.Debug();
        }
    });

// ... rest of configuration
```

**Step 3**: Add logging configuration to `appsettings.json`
```json
{
  "Serilog": {
    "MinimumLevel": "Debug",
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      }
    ]
  }
}
```

### Usage in Services and Controllers

Once configured, inject `ILogger<T>` to use logging:

```csharp
public class PostService : IPostService
{
    private readonly ILogger<PostService> _logger;

    public PostService(ApplicationDbContext context, ILogger<PostService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PostDto> GetPostByIdAsync(int id)
    {
        _logger.LogDebug("Retrieving post with ID {PostId}", id);
        
        var post = await _context.Posts.FindAsync(id);
        if (post == null)
        {
            _logger.LogWarning("Post with ID {PostId} not found", id);
            throw new NotFoundException($"Post {id} not found");
        }

        _logger.LogDebug("Successfully retrieved post {PostId}", id);
        return new PostDto { /* ... */ };
    }
}
```

## Acceptance Criteria

- [ ] Explicit logging configuration is present in `Program.cs`
- [ ] `builder.Logging.ClearProviders()` is called
- [ ] `builder.Logging.AddConsole()` is registered
- [ ] Log level is set to `Debug` in development, `Information` in production
- [ ] Project builds: `dotnet build`
- [ ] API starts: `dotnet run`
- [ ] Console logs appear during startup (e.g., "Now listening on...")
- [ ] Log output includes timestamp, level, and message
- [ ] Services can inject `ILogger<T>` without errors
- [ ] Log levels are applied correctly per environment
- [ ] Error and warning messages are logged with context

## Expected Log Output

**Development**:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
dbug: PostHubAPI.Services.PostService[0]
      Retrieving post with ID 1
```

**Production**:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://0.0.0.0:80
```

## Related Issues

- [Issue #024](024-missing-global-error-handling.md): Missing global error handling

## Future Enhancements

- [ ] Add Serilog for structured logging with JSON output
- [ ] Add Application Insights integration for cloud observability
- [ ] Add log correlation IDs for tracing requests
- [ ] Add metrics collection (e.g., Prometheus)
- [ ] Add distributed tracing (e.g., OpenTelemetry)

## Provenance

- **Instruction Source**: [.github/instructions/evergreen-software-development.instructions.md](.github/instructions/evergreen-software-development.instructions.md)
- **Instruction Section**: Principle 5: Design for Operability
- **Identified by**: Program.cs Conformance Analysis
- **Date Identified**: 2026-05-21
