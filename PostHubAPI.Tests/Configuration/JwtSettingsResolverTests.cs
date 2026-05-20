using Microsoft.Extensions.Configuration;
using PostHubAPI.Configuration;

namespace PostHubAPI.Tests.Configuration;

public class JwtSettingsResolverTests
{
    [Fact]
    public void Resolve_ThrowsInvalidOperationException_WhenSecretIsMissing()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JWT:ValidIssuer"] = "https://localhost:5001",
                ["JWT:ValidAudience"] = "https://localhost:4200"
            })
            .Build();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => JwtSettingsResolver.Resolve(configuration));

        Assert.Contains("JWT:Secret", exception.Message, StringComparison.Ordinal);
        Assert.Contains("JWT__Secret", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Resolve_ReturnsJwtSettings_WhenConfigurationIsValid()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JWT:ValidIssuer"] = "https://localhost:5001",
                ["JWT:ValidAudience"] = "https://localhost:4200",
                ["JWT:Secret"] = "super-secret-key"
            })
            .Build();

        // Act
        var jwtSettings = JwtSettingsResolver.Resolve(configuration);

        // Assert
        Assert.Equal("https://localhost:5001", jwtSettings.ValidIssuer);
        Assert.Equal("https://localhost:4200", jwtSettings.ValidAudience);
        Assert.Equal("super-secret-key", jwtSettings.Secret);
    }

    // TODO: Add tests for partial configurations.
}
