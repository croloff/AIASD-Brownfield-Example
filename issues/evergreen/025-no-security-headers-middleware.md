# Issue: Security Headers Middleware Missing from Program.cs

**Priority**: 🔴 **CRITICAL** (P1)

**Category**: Security / HTTP Headers

## Severity

Critical

## Description

Security headers are not configured in the HTTP middleware pipeline, violating ASP.NET Core API Design instructions (section 11). Missing headers include:

- `X-Content-Type-Options: nosniff` - Prevents MIME-type sniffing attacks
- `X-Frame-Options: DENY` - Prevents clickjacking attacks
- `X-XSS-Protection: 1; mode=block` - Enables browser XSS protections
- `Strict-Transport-Security: max-age=31536000; includeSubDomains` - Forces HTTPS (HSTS)

**Current State**: No security headers middleware registered.

**Security Impact**:
- Vulnerable to clickjacking (framing attacks)
- Vulnerable to MIME-type sniffing
- HTTPS enforcement not declared to browsers
- Browser XSS protections not enabled

## Violated Rules

**Source**: [aspnet-core-api-design.instructions.md](aspnet-core-api-design.instructions.md) - Section 11: CORS & Security

**Requirements**:
- Add security headers middleware to HTTP pipeline
- Return `X-Content-Type-Options: nosniff` on all responses
- Return `X-Frame-Options: DENY` on all responses
- Return `X-XSS-Protection: 1; mode=block` on all responses
- Return `Strict-Transport-Security` in production
- HTTPS enforced in production via `UseHttpsRedirection()` and `UseHsts()`

## Suggested Remediation

Add security headers middleware to `Program.cs` in the middleware configuration section (after `var app = builder.Build();`):

```csharp
var app = builder.Build();

// Security headers middleware
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    
    if (!app.Environment.IsDevelopment())
    {
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    }
    
    await next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// ... rest of middleware
```

### Alternative: Middleware Order

Ensure middleware order is correct:

1. Exception handling
2. Security headers ← Add here
3. Swagger (dev only)
4. HTTPS redirection
5. CORS
6. Authentication
7. Authorization
8. Controllers

**Complete Middleware Order in Program.cs**:
```csharp
var app = builder.Build();

// 1. Exception handling (first)
app.UseExceptionHandler(...);

// 2. Security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    if (!app.Environment.IsDevelopment())
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    await next();
});

// 3. Swagger (dev only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 4. HTTPS redirection
app.UseHttpsRedirection();

// 5. CORS
app.UseCors("AllowLocalhost");

// 6. Authentication
app.UseAuthentication();

// 7. Authorization
app.UseAuthorization();

// 8. Controllers
app.MapControllers();

app.Run();
```

## Acceptance Criteria

- [ ] Security headers middleware is added to `Program.cs`
- [ ] Middleware is placed in correct order (after exception handler, before CORS)
- [ ] `X-Content-Type-Options: nosniff` is returned on all responses
- [ ] `X-Frame-Options: DENY` is returned on all responses
- [ ] `X-XSS-Protection: 1; mode=block` is returned on all responses
- [ ] `Strict-Transport-Security` is returned on production responses (not dev)
- [ ] Project builds: `dotnet build`
- [ ] API starts: `dotnet run`
- [ ] Verify headers with: `curl -i http://localhost:5000/api/posts`
- [ ] Response includes all four security headers
- [ ] HSTS header included when running in production mode

## Verification Commands

```bash
# Test locally
curl -i http://localhost:5000/api/posts

# Should include headers:
# X-Content-Type-Options: nosniff
# X-Frame-Options: DENY
# X-XSS-Protection: 1; mode=block
# Strict-Transport-Security: max-age=31536000; includeSubDomains (in prod)
```

## Security Headers Reference

| Header | Purpose | Value |
|--------|---------|-------|
| `X-Content-Type-Options` | Prevent MIME-type sniffing | `nosniff` |
| `X-Frame-Options` | Prevent clickjacking | `DENY` or `SAMEORIGIN` |
| `X-XSS-Protection` | Enable browser XSS filters | `1; mode=block` |
| `Strict-Transport-Security` | Force HTTPS | `max-age=31536000; includeSubDomains` |

## Related Issues

- [Issue #023](023-no-cors-configuration.md): CORS configuration missing
- [Issue #024](024-missing-global-error-handling.md): Missing global error handling

## Provenance

- **Instruction Source**: [.github/instructions/aspnet-core-api-design.instructions.md](.github/instructions/aspnet-core-api-design.instructions.md)
- **Instruction Section**: 11. CORS & Security, Security Headers subsection
- **Identified by**: Program.cs Conformance Analysis
- **Date Identified**: 2026-05-21
