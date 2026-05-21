---
name: "JWT Authentication Instructions Generator"
description: "Generate instruction file for JWT authentication, token handling, and security best practices in this ASP.NET Core project"
author: "Development Team"
tags: ["jwt", "authentication", "security", "bearer-token", "authorization"]
created: "2026-05-21"
---

# Prompt: Create JWT Authentication & Security Instruction File

## Context

This PostHubAPI project implements JWT-based authentication with:
- **Token Signing**: HS256 (HMAC SHA-256) signature algorithm
- **Token Storage**: User secrets for development, environment variables for production
- **Bearer Scheme**: Standard HTTP Authorization: Bearer header
- **Validation Rules**: Issuer, audience, expiration, and signature verification
- **Password Security**: BCrypt.Net for hashing with automatic salt
- **ASP.NET Core Identity**: User management integration

The project demonstrates:
- Custom JWT settings resolution via `JwtSettingsResolver`
- Token generation with configurable expiration
- Bearer authentication middleware configuration
- Authorization attribute enforcement on controllers
- Ownership-based authorization checks in services

## Instructions

Create a comprehensive `.github/instructions/jwt-authentication.instructions.md` file that covers:

### 1. Token Generation
- JWT structure (header, payload, signature)
- HS256 signing with symmetric key
- Including standard claims (sub, iss, aud, exp, iat)
- Custom claims and their security implications
- Token expiration strategies

### 2. Configuration Management
- Storing JWT secret securely (user-secrets, environment variables)
- ValidIssuer and ValidAudience configuration
- Environment-specific settings (development vs production)
- Accessing configuration in services
- JwtSettingsResolver pattern and benefits

### 3. Bearer Token Handling
- Authorization header format: `Authorization: Bearer {token}`
- Token extraction from requests
- Token validation workflow
- Error responses (401 Unauthorized, 403 Forbidden)
- Retry logic for expired tokens

### 4. Password Security
- BCrypt.Net for password hashing
- Salt generation and verification
- Never storing plain-text passwords
- Password strength validation
- Rehashing considerations for upgraded algorithms

### 5. Authentication Middleware
- JWT Bearer authentication scheme configuration
- Token validation parameters
- HTTPS metadata requirements (environment-specific)
- SaveToken behavior and implications
- Challenge and forbid responses

### 6. Authorization Enforcement
- [Authorize] attribute usage
- [AllowAnonymous] for public endpoints
- Role-based authorization ([Authorize(Roles = "Admin")])
- Ownership-based checks in services
- Custom authorization policies (future enhancement)

### 7. Token Lifecycle
- Token creation at login
- Token transmission to client
- Token storage on client (localStorage, sessionStorage, etc.)
- Token refresh strategies
- Logout and token invalidation (future enhancement)

### 8. Security Best Practices
- HTTPS enforcement for token transmission
- Never logging tokens or sensitive data
- Protecting JWT secret from exposure
- Token expiration and re-authentication
- CORS considerations with tokens
- CSRF protection with stateful sessions (future)

### 9. Testing Authentication
- Creating test tokens for integration tests
- Mocking authentication in unit tests
- Testing authorization logic
- Testing expired token scenarios
- Testing invalid signature scenarios

### 10. Common Vulnerabilities & Mitigation
- Token leakage prevention
- Signature validation importance
- Audience and issuer validation benefits
- Expiration enforcement
- Algorithm confusion attacks

### 11. Future Enhancements
- Refresh tokens for improved UX
- Token revocation/blacklisting
- Multi-factor authentication (MFA)
- Role-based access control (RBAC)
- Permission-based authorization
- OAuth 2.0 / OpenID Connect integration

### 12. Troubleshooting
- "Invalid token" errors and diagnosis
- Issuer/audience mismatch issues
- Signature validation failures
- HTTPS metadata enforcement problems
- Clock skew issues

## Apply To

- `Program.cs` - Authentication configuration
- `Services/Implementations/UserService.cs` - Token generation
- `Configuration/JwtSettingsResolver.cs` - Settings resolution
- `Controllers/*.cs` - Authorization attributes and responses
- `PostHubAPI.Tests/Controllers/UserControllerTests.cs` - Authentication tests

## Version

1.0.0

## Maintainer

Development Team

## Related References

- [Official JWT Documentation](https://tools.ietf.org/html/rfc7519)
- [ASP.NET Core Authentication](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/)
- [PostHubAPI Architecture](docs/architecture.md)
- [PostHubAPI Requirements](docs/PROJECT-REQUIREMENTS.md)
- JWT Configuration: `Program.cs` (lines 46-60)
- JWT Settings Resolver: `Configuration/JwtSettingsResolver.cs`
- User Service: `Services/Implementations/UserService.cs`
