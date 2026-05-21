# Issue 014: DateTime.Now Instead of DateTime.UtcNow in JWT Token

**Priority**: 🟡 **High**  
**Category**: Correctness / Time Zone Handling  
**Status**: Open  
**Created**: 2026-05-21

## Summary

JWT token expiration is calculated using `DateTime.Now` instead of `DateTime.UtcNow`. This causes inconsistent token timestamps across different time zones and can lead to token expiration timing issues in distributed systems.

## Impact

- **Severity**: High
- **Affected Components**: 
  - Services/Implementations/UserService.cs::GetToken()
- **Scope**: Token timestamps; potential clock skew in distributed environments

## Current Behavior

```csharp
// UserService.GetToken() - INCORRECT
private string GetToken(User user, JwtSettings jwtSettings)
{
    // ...
    var token = new JwtSecurityToken(
        issuer: jwtSettings.ValidIssuer,
        audience: jwtSettings.ValidAudience,
        claims: claims,
        expires: DateTime.Now.AddHours(3),  // ❌ Uses local time
        signingCredentials: new SigningCredentials(key, SecurityAlgorithm.HmacSha256)
    );
    // ...
}
```

## Expected Behavior (Per Instructions)

Per **jwt-authentication.instructions.md**:
> All timestamps must use UTC (Coordinated Universal Time) for consistency across time zones and distributed servers.

Per **C# best practices**:
- `DateTime.Now`: Returns current local time (affected by server's time zone)
- `DateTime.UtcNow`: Returns current UTC time (timezone-independent)

## The Problem

### Scenario: Server in Multiple Time Zones

**Server A (UTC-5)**:
```
Local time: 14:00 EST
DateTime.Now.AddHours(3) = 17:00 EST = 22:00 UTC
JWT expires: 22:00 UTC
```

**Server B (UTC+1)**:
```
Local time: 20:00 CET
DateTime.Now.AddHours(3) = 23:00 CET = 22:00 UTC
JWT expires: 22:00 UTC
```

While this works in this case, if servers are at different offsets, the token expiration time becomes ambiguous and inconsistent.

### Correct Approach (UTC)

**All Servers**:
```
DateTime.UtcNow.AddHours(3) = consistent UTC time across all locations
JWT expires: [consistent UTC timestamp]
```

## Related JWT Timestamps

All JWT timestamps should use UTC:

```csharp
private string GetToken(User user, JwtSettings jwtSettings)
{
    var utcNow = DateTime.UtcNow;  // Get current UTC time once
    
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email)
    };
    
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));
    var creds = new SigningCredentials(key, SecurityAlgorithm.HmacSha256);
    
    var token = new JwtSecurityToken(
        issuer: jwtSettings.ValidIssuer,
        audience: jwtSettings.ValidAudience,
        claims: claims,
        notBefore: utcNow,          // ✓ UTC
        issuedAt: utcNow,           // ✓ UTC (implicit but good practice)
        expires: utcNow.AddHours(3), // ✓ UTC
        signingCredentials: creds
    );
    
    var tokenHandler = new JwtSecurityTokenHandler();
    return tokenHandler.WriteToken(token);
}
```

## Required Changes

### Services/Implementations/UserService.cs

Replace `DateTime.Now` with `DateTime.UtcNow`:

```csharp
// BEFORE:
expires: DateTime.Now.AddHours(3)

// AFTER:
expires: DateTime.UtcNow.AddHours(3)
```

## Best Practice Pattern

```csharp
// Store UTC now in variable for consistency
var utcNow = DateTime.UtcNow;

var token = new JwtSecurityToken(
    issuer: jwtSettings.ValidIssuer,
    audience: jwtSettings.ValidAudience,
    claims: claims,
    notBefore: utcNow,         // Token valid from now
    issuedAt: utcNow,          // Optional but recommended
    expires: utcNow.AddHours(3), // Token expires in 3 hours
    signingCredentials: creds
);
```

## Related Timestamps

Check for other uses of DateTime.Now in codebase:

- ✓ Post.CreationTime: Uses `DateTime.Now` (intentional for display, could be UTC)
- ✓ Comment.CreationTime: Uses `DateTime.Now` (intentional for display, could be UTC)
- ❌ JWT Token expiration: Should use `DateTime.UtcNow` (critical for correctness)

**Note**: For data model timestamps (CreationTime), using local time is less critical but UTC is still recommended for consistency.

## Related Instructions

- **jwt-authentication.instructions.md**: UTC time requirement for tokens
- **evergreen-software-development.instructions.md**: § Design for Change, correctness and consistency

## Definition of Done

- [ ] DateTime.Now replaced with DateTime.UtcNow in GetToken()
- [ ] Token expiration calculated consistently across all environments
- [ ] All JWT timestamp fields use UTC
- [ ] Tests pass (no breaking changes to behavior)
- [ ] Consider updating CreationTime fields to UtcNow for consistency
