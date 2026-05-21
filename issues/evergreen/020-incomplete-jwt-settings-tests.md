# Issue 020: Incomplete JWT Settings Tests

**Priority**: 🟡 **Medium Gap**  
**Category**: Testing / Configuration  
**Status**: Open  
**Created**: 2026-05-21

## Summary

[JwtSettingsResolverTests.cs](JwtSettingsResolverTests.cs) has a TODO comment indicating incomplete test coverage for partial/missing JWT configurations. Tests should verify behavior when individual JWT settings are missing or invalid.

## Impact

- **Severity**: Medium (Test Coverage Gap)
- **Affected Components**: 
  - PostHubAPI.Tests/Configuration/JwtSettingsResolverTests.cs
- **Scope**: Configuration validation and error handling

## Current Test Gap

**Line in JwtSettingsResolverTests.cs**:
```csharp
// TODO: Add tests for partial configurations.
```

This TODO indicates that tests don't cover edge cases where:
- `JWT:Secret` is missing
- `JWT:ValidIssuer` is missing
- `JWT:ValidAudience` is missing
- Multiple settings are missing
- Settings are empty strings
- Settings are null

## Why This Matters

### Startup Reliability
JWT configuration errors should be caught at startup, not during token generation:
```csharp
// Bad: Error at token generation time
if (string.IsNullOrEmpty(jwtSettings.Secret))
    throw new InvalidOperationException("JWT Secret not configured");

// Good: Error at DI registration time (tests this)
JwtSettingsResolver.Validate(config);  // Fail fast
```

### Configuration Validation Pattern
The instruction file (jwt-authentication.instructions.md) recommends validating settings at startup:
```csharp
var jwtSettings = JwtSettingsResolver.Resolve(configuration);
if (!jwtSettings.IsValid())
    throw new InvalidOperationException("Invalid JWT configuration");
```

## Missing Test Cases

### Test 1: MissingSecret

```csharp
[Fact]
public void Resolve_ThrowsException_WhenSecretIsMissing()
{
    // Arrange
    var config = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string>
        {
            { "JWT:ValidIssuer", "TestIssuer" },
            { "JWT:ValidAudience", "TestAudience" }
            // Missing: JWT:Secret
        })
        .Build();
    
    // Act & Assert
    var ex = Assert.Throws<InvalidOperationException>(
        () => JwtSettingsResolver.Resolve(config)
    );
    Assert.Contains("Secret", ex.Message);
}
```

### Test 2: MissingIssuer

```csharp
[Fact]
public void Resolve_ThrowsException_WhenIssuerIsMissing()
{
    // Arrange
    var config = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string>
        {
            { "JWT:Secret", "test-secret-key-min-32-chars-required" },
            { "JWT:ValidAudience", "TestAudience" }
            // Missing: JWT:ValidIssuer
        })
        .Build();
    
    // Act & Assert
    var ex = Assert.Throws<InvalidOperationException>(
        () => JwtSettingsResolver.Resolve(config)
    );
    Assert.Contains("Issuer", ex.Message);
}
```

### Test 3: MissingAudience

```csharp
[Fact]
public void Resolve_ThrowsException_WhenAudienceIsMissing()
{
    // Arrange
    var config = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string>
        {
            { "JWT:Secret", "test-secret-key-min-32-chars-required" },
            { "JWT:ValidIssuer", "TestIssuer" }
            // Missing: JWT:ValidAudience
        })
        .Build();
    
    // Act & Assert
    var ex = Assert.Throws<InvalidOperationException>(
        () => JwtSettingsResolver.Resolve(config)
    );
    Assert.Contains("Audience", ex.Message);
}
```

### Test 4: EmptySecret

```csharp
[Fact]
public void Resolve_ThrowsException_WhenSecretIsEmpty()
{
    // Arrange
    var config = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string>
        {
            { "JWT:Secret", "" },  // Empty string
            { "JWT:ValidIssuer", "TestIssuer" },
            { "JWT:ValidAudience", "TestAudience" }
        })
        .Build();
    
    // Act & Assert
    var ex = Assert.Throws<InvalidOperationException>(
        () => JwtSettingsResolver.Resolve(config)
    );
    Assert.Contains("Secret", ex.Message);
}
```

### Test 5: SecretTooShort

```csharp
[Fact]
public void Resolve_ThrowsException_WhenSecretIsTooShort()
{
    // Arrange
    var config = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string>
        {
            { "JWT:Secret", "short" },  // Less than 32 characters
            { "JWT:ValidIssuer", "TestIssuer" },
            { "JWT:ValidAudience", "TestAudience" }
        })
        .Build();
    
    // Act & Assert
    var ex = Assert.Throws<InvalidOperationException>(
        () => JwtSettingsResolver.Resolve(config)
    );
    Assert.Contains("Secret", ex.Message);
}
```

### Test 6: AllSettingsMissing

```csharp
[Fact]
public void Resolve_ThrowsException_WhenAllSettingsMissing()
{
    // Arrange
    var config = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string>())  // Empty config
        .Build();
    
    // Act & Assert
    var ex = Assert.Throws<InvalidOperationException>(
        () => JwtSettingsResolver.Resolve(config)
    );
    // Should mention at least one missing setting
    Assert.True(
        ex.Message.Contains("Secret") ||
        ex.Message.Contains("Issuer") ||
        ex.Message.Contains("Audience")
    );
}
```

### Test 7: ValidConfiguration (Happy Path)

```csharp
[Fact]
public void Resolve_ReturnsValidSettings_WhenAllSettingsProvided()
{
    // Arrange
    var config = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string>
        {
            { "JWT:Secret", "very-long-secret-key-that-meets-minimum-length-requirement" },
            { "JWT:ValidIssuer", "TestIssuer" },
            { "JWT:ValidAudience", "TestAudience" }
        })
        .Build();
    
    // Act
    var result = JwtSettingsResolver.Resolve(config);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal("very-long-secret-key-that-meets-minimum-length-requirement", result.Secret);
    Assert.Equal("TestIssuer", result.ValidIssuer);
    Assert.Equal("TestAudience", result.ValidAudience);
}
```

## JwtSettingsResolver Implementation Pattern

To support these tests, JwtSettingsResolver should validate configuration:

```csharp
public class JwtSettingsResolver
{
    private const int MinimumSecretLength = 32;
    
    public static JwtSettings Resolve(IConfiguration configuration)
    {
        var settings = new JwtSettings();
        configuration.GetSection("JWT").Bind(settings);
        
        // Validate settings
        if (string.IsNullOrWhiteSpace(settings.Secret))
            throw new InvalidOperationException("JWT:Secret is required and cannot be empty");
        
        if (settings.Secret.Length < MinimumSecretLength)
            throw new InvalidOperationException($"JWT:Secret must be at least {MinimumSecretLength} characters");
        
        if (string.IsNullOrWhiteSpace(settings.ValidIssuer))
            throw new InvalidOperationException("JWT:ValidIssuer is required and cannot be empty");
        
        if (string.IsNullOrWhiteSpace(settings.ValidAudience))
            throw new InvalidOperationException("JWT:ValidAudience is required and cannot be empty");
        
        return settings;
    }
}
```

## Test File Structure

```csharp
namespace PostHubAPI.Tests.Configuration
{
    public class JwtSettingsResolverTests
    {
        // Existing passing tests (keep these)
        [Fact]
        public void Resolve_ReturnsJwtSettings_WhenConfigIsValid()
        {
            // Existing test...
        }
        
        // NEW: Partial configuration tests
        [Fact]
        public void Resolve_ThrowsException_WhenSecretIsMissing()
        {
            // New test
        }
        
        [Fact]
        public void Resolve_ThrowsException_WhenIssuerIsMissing()
        {
            // New test
        }
        
        [Fact]
        public void Resolve_ThrowsException_WhenAudienceIsMissing()
        {
            // New test
        }
        
        [Fact]
        public void Resolve_ThrowsException_WhenSecretIsEmpty()
        {
            // New test
        }
        
        [Fact]
        public void Resolve_ThrowsException_WhenSecretIsTooShort()
        {
            // New test
        }
        
        [Fact]
        public void Resolve_ThrowsException_WhenAllSettingsMissing()
        {
            // New test
        }
    }
}
```

## Related Instructions

- **xunit-testing.instructions.md**: Configuration testing patterns
- **jwt-authentication.instructions.md**: JWT configuration validation
- **evergreen-software-development.instructions.md**: § Build with Defensive Quality

## Definition of Done

- [ ] TODO comment removed from JwtSettingsResolverTests.cs
- [ ] Test added for missing Secret
- [ ] Test added for missing ValidIssuer
- [ ] Test added for missing ValidAudience
- [ ] Test added for empty Secret
- [ ] Test added for Secret too short
- [ ] Test added for all settings missing
- [ ] Test added for valid configuration (happy path)
- [ ] JwtSettingsResolver validates configuration at resolution time
- [ ] All tests pass
- [ ] Configuration errors caught at startup, not runtime
