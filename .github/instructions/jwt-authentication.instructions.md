---
ai_generated: true
model: "anthropic/claude-3.5-sonnet@2024-10-22"
operator: "github-copilot"
chat_id: "jwt-authentication-20260521"
prompt: |
  Create a comprehensive JWT authentication instruction file for the PostHubAPI project covering:
  - JWT fundamentals and token structure
  - Token generation patterns using JwtSecurityToken
  - Claims design and payload configuration
  - Token validation and middleware setup
  - Role-based access control with claims
  - Secret management and secure configuration
  - Token expiration and lifecycle
  - Security best practices and common pitfalls
  - Testing patterns for authentication flows
started: "2026-05-21T14:30:00Z"
ended: "2026-05-21T15:00:00Z"
task_durations:
  - task: "requirements analysis"
    duration: "00:10:00"
  - task: "instruction design"
    duration: "00:12:00"
  - task: "pattern documentation"
    duration: "00:06:00"
  - task: "security review"
    duration: "00:02:00"
total_duration: "00:30:00"
ai_log: "ai-logs/2026/05/21/jwt-authentication-20260521/conversation.md"
source: "create-jwt-authentication-instructions.prompt.md"
applyTo: "Services/Implementations/*.cs,Controllers/**/*.cs,Configuration/*.cs,Program.cs"
---

# JWT Authentication Instructions

## Overview

This instruction file defines patterns and best practices for implementing JWT (JSON Web Token) authentication in the PostHubAPI project. Follow these guidelines when generating tokens, validating credentials, managing claims, and handling secrets to maintain security, consistency, and maintainability across authentication flows.

## Table of Contents

1. [JWT Fundamentals](#jwt-fundamentals)
2. [Token Generation](#token-generation)
3. [Claims Design](#claims-design)
4. [Role-Based Access Control](#role-based-access-control)
5. [Token Configuration](#token-configuration)
6. [Secret Management](#secret-management)
7. [Token Validation and Middleware](#token-validation-and-middleware)
8. [Token Lifecycle](#token-lifecycle)
9. [Security Best Practices](#security-best-practices)
10. [Testing Authentication Flows](#testing-authentication-flows)
11. [Troubleshooting](#troubleshooting)

## JWT Fundamentals

### What is JWT?

A JWT consists of three Base64-encoded parts separated by dots:

```
Header.Payload.Signature
```

- **Header**: Metadata about the token (algorithm, token type)
- **Payload**: Claims (user data, roles, permissions)
- **Signature**: Cryptographic signature verifying the token's integrity

### Token Characteristics

- **Stateless**: No server-side session storage required
- **Self-Contained**: All claims are embedded in the token
- **Tamper-Evident**: Signature prevents unauthorized modification
- **Expirable**: Tokens have a defined lifetime

## Token Generation

### Standard Pattern (UserService)

Follow the `UserService.GetToken()` pattern when generating JWT tokens:

```csharp
private JwtSecurityToken GetToken(IEnumerable<Claim> claims)
{
    JwtSettings jwtSettings = JwtSettingsResolver.Resolve(configuration);
    SymmetricSecurityKey authSigningKey = 
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));

    JwtSecurityToken token = new JwtSecurityToken
    (
        issuer: jwtSettings.ValidIssuer,
        audience: jwtSettings.ValidAudience,
        expires: DateTime.UtcNow.AddHours(3),
        claims: claims,
        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
    );

    return token;
}
```

### Key Requirements

1. **Always use `DateTime.UtcNow`** (not `DateTime.Now`) for consistent UTC time across environments.
2. **Use `SymmetricSecurityKey` with HmacSha256** for signing.
3. **Resolve JWT settings once per token generation** using `JwtSettingsResolver.Resolve()`.
4. **Convert token to string** using `JwtSecurityTokenHandler().WriteToken(token)`.

### Example: Token Generation in Services

```csharp
public async Task<string> Login(LoginUserDto dto)
{
    User? user = await userManager.FindByNameAsync(dto.Username);
    if (user == null || !await userManager.CheckPasswordAsync(user, dto.Password))
    {
        throw new ArgumentException($"Unable to authenticate user {dto.Username}!");
    }

    List<Claim> claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName ?? throw new InvalidOperationException()),
        new Claim(ClaimTypes.Email, user.Email ?? throw new InvalidOperationException()),
        new Claim(ClaimTypes.NameIdentifier, user.Id)
    };

    JwtSecurityToken token = GetToken(claims);
    return new JwtSecurityTokenHandler().WriteToken(token);
}
```

## Claims Design

### Standard Claims

Use standard `ClaimTypes` from `System.Security.Claims` for interoperability:

| Claim | Type | Purpose | Example |
|-------|------|---------|---------|
| `ClaimTypes.NameIdentifier` | `string` | Unique user ID | `"12345"` |
| `ClaimTypes.Name` | `string` | Username | `"john.doe"` |
| `ClaimTypes.Email` | `string` | Email address | `"john@example.com"` |
| `ClaimTypes.Role` | `string` | User role (can be multiple) | `"Admin"`, `"User"` |

### Custom Claims

Define custom claims in a dedicated constant class if needed:

```csharp
public static class CustomClaims
{
    public const string Subscription = "subscription_level";
    public const string Department = "department";
}
```

Then add to token:

```csharp
claims.Add(new Claim(CustomClaims.Subscription, user.SubscriptionLevel));
```

### Claim Best Practices

- ✅ **Include only necessary information** in claims to minimize token size.
- ✅ **Use standard claim types** for common attributes.
- ✅ **Add claims only after user is validated**.
- ❌ **Do not include sensitive data** (passwords, credit cards) in claims.
- ❌ **Do not rely on client-provided claims** during validation.
- ❌ **Do not store large objects** in claims (use minimal, serializable data).

## Role-Based Access Control

### Adding Role Claims

When a user has roles (via ASP.NET Core Identity), include them in the token:

```csharp
private async Task<List<Claim>> BuildClaims(User user)
{
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
        new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
    };

    // Add user roles to claims
    var roles = await userManager.GetRolesAsync(user);
    foreach (var role in roles)
    {
        claims.Add(new Claim(ClaimTypes.Role, role));
    }

    return claims;
}
```

### Using [Authorize] Attribute

Restrict controller actions by role:

```csharp
[HttpDelete("{id}")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> DeletePost(int id)
{
    // Only Admin users can delete posts
    return Ok(await postService.DeletePost(id));
}
```

### Checking Claims in Code

Access current user claims in services or controllers:

```csharp
var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
var userName = User.FindFirst(ClaimTypes.Name)?.Value;
var isAdmin = User.IsInRole("Admin");
```

## Token Configuration

### JWT Settings Schema

Store the following in configuration (`appsettings.json` or environment variables):

```json
{
  "JWT": {
    "ValidIssuer": "http://localhost:5001",
    "ValidAudience": "http://localhost:4200",
    "Secret": "<64+ character secret>"
  }
}
```

### Accessing JWT Settings

Use `JwtSettingsResolver.Resolve()` to safely retrieve settings:

```csharp
var jwtSettings = JwtSettingsResolver.Resolve(configuration);
// Returns: JwtSettings(ValidIssuer, ValidAudience, Secret)
```

### Configuration Requirements

- **ValidIssuer**: The authority issuing the token (usually your API URL).
- **ValidAudience**: The intended recipient of the token (usually your client/SPA URL).
- **Secret**: Cryptographic key for signing (minimum 64 characters recommended).

### Environment-Specific Configuration

Different issuer/audience values for environments:

**appsettings.Development.json:**
```json
{
  "JWT": {
    "ValidIssuer": "http://localhost:5001",
    "ValidAudience": "http://localhost:4200"
  }
}
```

**appsettings.Production.json:**
```json
{
  "JWT": {
    "ValidIssuer": "https://api.example.com",
    "ValidAudience": "https://app.example.com"
  }
}
```

## Secret Management

### 🔒 Critical: Secrets Must Never Be in Source Control

The JWT signing secret MUST NOT be stored in tracked configuration files. Use one of the following approaches:

### 1. User Secrets (Local Development)

Securely store secrets per project during development:

```bash
# Initialize user secrets for the project
dotnet user-secrets init

# Set the JWT secret
dotnet user-secrets set "JWT:Secret" "your-development-secret-here"

# Verify it's set
dotnet user-secrets list
```

Stored location:
- **Windows**: `%APPDATA%\Microsoft\UserSecrets\<project-guid>\secrets.json`
- **macOS/Linux**: `~/.microsoft/usersecrets/<project-guid>/secrets.json`

### 2. Environment Variables (Any Environment)

Set environment variables before running the application:

```bash
# Windows PowerShell
$env:JWT__Secret = "your-secret-here"

# Windows Command Prompt
set JWT__Secret=your-secret-here

# Linux/macOS
export JWT__Secret="your-secret-here"
```

ASP.NET Core automatically reads `JWT__Secret` and maps it to `JWT:Secret` in configuration.

### 3. Azure Key Vault (Production)

For production deployments, use Azure Key Vault:

```csharp
var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsDevelopment())
{
    var keyVaultUrl = new Uri(builder.Configuration["KeyVault:VaultUri"]);
    builder.Configuration.AddAzureKeyVault(
        keyVaultUrl,
        new DefaultAzureCredential());
}
```

### Generating Strong Secrets

Create a cryptographically secure secret (minimum 64 characters):

```csharp
using System.Security.Cryptography;
using System.Text;

var secret = Convert.ToBase64String(
    RandomNumberGenerator.GetBytes(64));
Console.WriteLine(secret);
```

Or use online tools: [ASP.NET Core Identity Secret Generator](https://www.allkeysgenerator.com/Random/Security-Encryption-Key-Generator.html)

### Secret Rotation

When rotating secrets:

1. **Generate new secret** and store in secure source.
2. **Restart application** to pick up new secret.
3. **Test authentication** with both old and new tokens (if supporting multiple secrets).
4. **Remove old secret** after grace period or once clients have re-authenticated.

## Token Validation and Middleware

### JWT Bearer Middleware Setup

Configure in `Program.cs`:

```csharp
builder.Services.AddAuthentication(opts =>
{
    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opts.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opts =>
{
    opts.SaveToken = true;
    opts.RequireHttpsMetadata = !isDevelopment;
    opts.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = jwtSettings.ValidAudience,
        ValidIssuer = jwtSettings.ValidIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.Secret))
    };
});
```

### Validation Parameters

- **ValidateIssuer**: Verify the token was issued by expected authority.
- **ValidateAudience**: Verify the token is intended for this service.
- **IssuerSigningKey**: The cryptographic key used to validate the signature.
- **RequireHttpsMetadata**: In production, always require HTTPS for metadata (set to `true` in Production).

### Adding Middleware to Pipeline

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

**Order matters**: `UseAuthentication()` MUST come before `UseAuthorization()`.

## Token Lifecycle

### Recommended Expiration Times

- **Access Tokens**: 1-3 hours (short-lived)
- **Refresh Tokens**: 7-30 days (if implemented)

Current implementation uses **3-hour expiration**:

```csharp
expires: DateTime.UtcNow.AddHours(3)
```

### Token Expiration Handling

When a token expires, the API returns `401 Unauthorized`:

```json
{
  "error": "unauthorized",
  "message": "Token expired"
}
```

Client responsibility:
- Detect `401` response.
- Redirect user to login (or refresh if refresh token implemented).
- Clear local token storage.

### Modifying Expiration

To change token lifetime, update the `GetToken()` method:

```csharp
// 1 hour
expires: DateTime.UtcNow.AddHours(1)

// 2 hours
expires: DateTime.UtcNow.AddHours(2)

// 30 minutes
expires: DateTime.UtcNow.AddMinutes(30)
```

Document any changes in PR description with rationale.

## Security Best Practices

### 1. Always Use HTTPS in Production

Set `RequireHttpsMetadata = true` in non-development environments:

```csharp
opts.RequireHttpsMetadata = !isDevelopment;
```

Tokens over unencrypted HTTP can be intercepted.

### 2. Use Strong Secrets

- Minimum 64 characters (random, not dictionary words).
- Regenerate secrets if leaked or exposed.
- Store in secure vaults, never in source code.

### 3. Validate on Every Protected Request

The middleware automatically validates tokens on `[Authorize]` endpoints. No manual validation needed.

### 4. Limit Token Scope

Only include necessary claims. Minimize token size:

```csharp
// ✅ Good: Only essential claims
var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, user.Id),
    new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
};

// ❌ Bad: Includes sensitive or unnecessary data
var claims = new List<Claim>
{
    new Claim("Password", user.PasswordHash),
    new Claim("Department", department.Description),
    new Claim("Manager", manager.FullName)
};
```

### 5. Handle Token Errors Gracefully

Catch and log validation failures:

```csharp
try
{
    var token = GetToken(claims);
    return new JwtSecurityTokenHandler().WriteToken(token);
}
catch (SecurityTokenException ex)
{
    _logger.LogError($"Token generation failed: {ex.Message}");
    throw new InvalidOperationException("Failed to generate authentication token.");
}
```

### 6. Protect Sensitive Endpoints

Always use `[Authorize]` on endpoints returning sensitive data:

```csharp
[HttpGet("{id}")]
[Authorize]
public async Task<IActionResult> GetPost(int id)
{
    return Ok(await postService.GetPostById(id));
}
```

Use role restrictions for admin operations:

```csharp
[HttpDelete("{id}")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> DeletePost(int id)
{
    return Ok(await postService.DeletePost(id));
}
```

## Testing Authentication Flows

### Unit Testing Token Generation

```csharp
[Fact]
public void GetToken_WithValidClaims_ReturnsJwtSecurityToken()
{
    // Arrange
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, "user-123"),
        new Claim(ClaimTypes.Name, "test-user")
    };

    var userService = new UserService(configuration, userManager);

    // Act
    var token = userService.GetToken(claims); // Make method public or use reflection

    // Assert
    Assert.NotNull(token);
    Assert.True(token.ValidFrom < DateTime.UtcNow);
    Assert.True(token.ValidTo > DateTime.UtcNow);
}
```

### Integration Testing Login Flow

```csharp
[Fact]
public async Task Login_WithValidCredentials_ReturnsJwtToken()
{
    // Arrange
    var user = new User { Id = "1", UserName = "testuser", Email = "test@example.com" };
    await userManager.CreateAsync(user, "ValidPassword123!");
    var loginDto = new LoginUserDto { Username = "testuser", Password = "ValidPassword123!" };

    // Act
    var token = await userService.Login(loginDto);

    // Assert
    Assert.NotNull(token);
    Assert.NotEmpty(token);
}
```

### Testing Claim Extraction

```csharp
[Fact]
public void Token_ContainsExpectedClaims()
{
    // Arrange
    var handler = new JwtSecurityTokenHandler();
    var token = GenerateTestToken(); // Helper to generate token

    // Act
    var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

    // Assert
    Assert.NotNull(jwtToken);
    Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.NameIdentifier);
    Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Name);
}
```

### Testing Expired Tokens

```csharp
[Fact]
public async Task AuthorizedEndpoint_WithExpiredToken_Returns401()
{
    // Arrange
    var expiredToken = GenerateExpiredToken();

    // Act
    var response = await client.GetAsync(
        "/api/posts",
        new { Authorization = $"Bearer {expiredToken}" });

    // Assert
    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
}
```

## Troubleshooting

### Issue: "Token validation failed"

**Cause**: Issuer, audience, or secret mismatch between token generation and validation.

**Solution**:
1. Verify `ValidIssuer`, `ValidAudience`, and `Secret` match in configuration.
2. Ensure issuer/audience are set consistently across environments.
3. Check that secret hasn't been rotated without updating configuration.

### Issue: "401 Unauthorized on protected endpoints"

**Possible Causes**:
- Token is missing or malformed in `Authorization` header.
- Token has expired.
- Token signature is invalid.

**Debug**:
```csharp
var claims = User.Claims;
var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
```

### Issue: "Invalid token format"

**Cause**: Token not in `Bearer <token>` format.

**Solution**: Ensure Authorization header follows standard:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Issue: Secret not found or application fails at startup

**Cause**: JWT secret not configured in any source (appsettings, environment variables, or user secrets).

**Solution**: Set secret using one of these methods:
```bash
# User secrets (development)
dotnet user-secrets set "JWT:Secret" "your-secret"

# Environment variable
$env:JWT__Secret = "your-secret"
```

### Issue: Token generation works but authentication fails in controllers

**Cause**: Middleware not properly registered or authentication not enabled on endpoint.

**Solution**:
1. Verify `[Authorize]` attribute on controller or action.
2. Check middleware order in `Program.cs`:
   ```csharp
   app.UseAuthentication();
   app.UseAuthorization();
   app.MapControllers();
   ```

## Summary Checklist

When implementing or modifying JWT authentication:

- [ ] Token generation uses `JwtSecurityToken` with `SymmetricSecurityKey`
- [ ] Claims include only necessary, serializable data
- [ ] Role claims added when roles exist on user
- [ ] `[Authorize]` attribute protects sensitive endpoints
- [ ] `[Authorize(Roles = "...")]` restricts admin operations
- [ ] JWT secrets stored in secure source (user secrets, environment, or vault)
- [ ] Secrets NOT in version-controlled configuration files
- [ ] Token expiration set appropriately (recommend 1-3 hours)
- [ ] Middleware configured in correct order (Authentication → Authorization)
- [ ] `RequireHttpsMetadata = false` only in Development
- [ ] Token validation tests include expired token cases
- [ ] Error messages in catch blocks are generic (no token details leaked)
- [ ] Configuration key names consistent: `JWT:ValidIssuer`, `JWT:ValidAudience`, `JWT:Secret`

---

**Document Version**: 1.0.0
**Last Updated**: 2026-05-21
**Maintainer**: Development Team
**Related Instructions**: 
- [aspnet-core-api-design.instructions.md](aspnet-core-api-design.instructions.md)
- [ai-assisted-output.instructions.md](ai-assisted-output.instructions.md)
