# Issue: CORS Configuration Missing from Program.cs

**Priority**: 🔴 **CRITICAL** (P1)

**Category**: Security / HTTP Middleware

## Severity

Critical

## Description

Cross-Origin Resource Sharing (CORS) is not configured in `Program.cs`, violating ASP.NET Core API Design instructions section 11. Without CORS:

- Browser-based clients (web apps, SPAs) cannot access the API
- All cross-origin requests are rejected with CORS errors
- API is only usable from same-origin or non-browser clients
- No origin whitelisting control exists

**Current State**:
- No `services.AddCors()` registration
- No `app.UseCors()` middleware
- Clients receive: `Access to XMLHttpRequest at 'http://api' from origin 'http://client' has been blocked by CORS policy`

## Violated Rules

**Source**: [aspnet-core-api-design.instructions.md](aspnet-core-api-design.instructions.md) - Section 11: CORS & Security

**Requirements**:
- Must register CORS service with `AddCors()` before `Build()`
- Must define a policy with explicit origin whitelisting (never `AllowAnyOrigin`)
- Must apply the policy with `app.UseCors()` in the middleware pipeline
- Must include CORS before Authorization middleware
- Must use explicit origin lists, not wildcards with credentials

## Suggested Remediation

### Step 1: Add CORS Service Registration

In `Program.cs`, after line 16 (after `var jwtSettings = ...`), add:

```csharp
// Configure CORS with explicit origin whitelisting
builder.Services.AddCors(options =>
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
```

### Step 2: Apply CORS Middleware

In `Program.cs`, after `var app = builder.Build();` (after line 70), add CORS middleware **before** authentication:

```csharp
var app = builder.Build();

// Apply CORS policy BEFORE authentication middleware
app.UseCors("AllowLocalhost");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```

### Step 3: Environment-Specific Configuration

For production, use configuration instead of hardcoded origins:

```csharp
var allowedOrigins = configuration.GetSection("CorsAllowedOrigins").Get<string[]>() 
    ?? new[] { "https://example.com" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowConfigured", builder =>
    {
        builder
            .WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});
```

### Step 4: Update Configuration

Add to `appsettings.Development.json`:
```json
{
  "CorsAllowedOrigins": ["http://localhost:3000", "http://localhost:4200"]
}
```

Add to `appsettings.json`:
```json
{
  "CorsAllowedOrigins": ["https://app.example.com"]
}
```

## Acceptance Criteria

- [ ] `services.AddCors()` is registered with explicit policy in `Program.cs`
- [ ] Policy is named `"AllowLocalhost"` (or similar) and defines allowed origins explicitly
- [ ] Policy includes `.AllowAnyMethod()`, `.AllowAnyHeader()`, `.AllowCredentials()`
- [ ] `app.UseCors("AllowLocalhost")` is called **before** `UseAuthentication()`
- [ ] CORS middleware appears in the correct order in the pipeline
- [ ] Project builds: `dotnet build`
- [ ] API starts: `dotnet run`
- [ ] Browser-based request from whitelisted origin succeeds (200/204/404, not CORS error)
- [ ] Browser-based request from non-whitelisted origin is rejected with CORS error
- [ ] Configuration drives allowed origins per environment
- [ ] Production configuration defines appropriate origin whitelist

## Security Notes

⚠️ **Do NOT use**:
```csharp
.WithOrigins("*")
.AllowCredentials() // ❌ INVALID: Cannot use * with credentials
```

⚠️ **Always use explicit origins**:
```csharp
.WithOrigins("https://example.com", "https://app.example.com")
.AllowCredentials() // ✅ VALID
```

## Related Issues

- [Issue #025](025-no-security-headers-middleware.md): No security headers middleware

## Provenance

- **Instruction Source**: [.github/instructions/aspnet-core-api-design.instructions.md](.github/instructions/aspnet-core-api-design.instructions.md)
- **Instruction Section**: 11. CORS & Security
- **Identified by**: Program.cs Conformance Analysis
- **Date Identified**: 2026-05-21
