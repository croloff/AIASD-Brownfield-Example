using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using PostHubAPI.Dtos.Post;

namespace PostHubAPI.Tests.Controllers;

public class PostControllerAuthorizationTests
{
    private const string ValidIssuer = "https://localhost:5001";
    private const string ValidAudience = "https://localhost:4200";
    private const string Secret = "integration-test-secret-value-1234567890";

    [Fact]
    public async Task CreatePost_ReturnsUnauthorized_WhenUserIsAnonymous()
    {
        // Arrange
        using var factory = CreateFactory();
        using var client = CreateClient(factory);

        // Act
        var response = await client.PostAsJsonAsync("/api/Post", CreatePostDto());

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task EditPost_ReturnsUnauthorized_WhenRequestIsAnonymous()
    {
        // Arrange
        using var factory = CreateFactory();
        using var client = CreateClient(factory);
        var postId = await CreateAuthorizedPostAsync(client);

        // Act
        var response = await client.PutAsJsonAsync($"/api/Post/{postId}", CreateEditPostDto());

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeletePost_ReturnsUnauthorized_WhenRequestIsAnonymous()
    {
        // Arrange
        using var factory = CreateFactory();
        using var client = CreateClient(factory);
        var postId = await CreateAuthorizedPostAsync(client);

        // Act
        var response = await client.DeleteAsync($"/api/Post/{postId}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreatePost_ReturnsCreated_WhenRequestHasValidJwt()
    {
        // Arrange
        using var factory = CreateFactory();
        using var client = CreateClient(factory);

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/Post")
        {
            Content = JsonContent.Create(CreatePostDto())
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", CreateJwt());

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var newPostId = await response.Content.ReadFromJsonAsync<int>();
        Assert.True(newPostId > 0);
    }

    [Fact]
    public async Task CreatePost_ReturnsCreated_WhenUserIsAuthorized()
    {
        // Arrange
        using var factory = CreateFactory();
        using var client = CreateClient(factory);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GenerateValidJwtToken());

        // Act
        var response = await client.PostAsJsonAsync("/api/Post", CreatePostDto());

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private static WebApplicationFactory<Program> CreateFactory()
    {
        Environment.SetEnvironmentVariable("JWT__ValidIssuer", ValidIssuer);
        Environment.SetEnvironmentVariable("JWT__ValidAudience", ValidAudience);
        Environment.SetEnvironmentVariable("JWT__Secret", Secret);

        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment(Environments.Development);
                builder.ConfigureAppConfiguration((_, configBuilder) =>
                {
                    configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["JWT:ValidIssuer"] = ValidIssuer,
                        ["JWT:ValidAudience"] = ValidAudience,
                        ["JWT:Secret"] = Secret
                    });
                });
            });
    }

    private static HttpClient CreateClient(WebApplicationFactory<Program> factory)
    {
        return factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });
    }

    private static async Task<int> CreateAuthorizedPostAsync(HttpClient client)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/Post")
        {
            Content = JsonContent.Create(CreatePostDto())
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", CreateJwt());

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var postId = await response.Content.ReadFromJsonAsync<int>();
        Assert.True(postId > 0);
        return postId;
    }

    private static string CreateJwt()
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
        var token = new JwtSecurityToken(
            issuer: ValidIssuer,
            audience: ValidAudience,
            claims:
            [
                new Claim(ClaimTypes.Name, "integration-user"),
                new Claim(ClaimTypes.Email, "integration@example.com")
            ],
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static CreatePostDto CreatePostDto() =>
        new()
        {
            Title = "Authorized title",
            Body = "Authorized body"
        };

    private static EditPostDto CreateEditPostDto() =>
        new()
        {
            Title = "Updated title",
            Body = "Updated body"
        };

    private static string GenerateValidJwtToken()
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
        var token = new JwtSecurityToken(
            issuer: ValidIssuer,
            audience: ValidAudience,
            claims:
            [
                new Claim(ClaimTypes.Name, "integration-user"),
                new Claim(ClaimTypes.Email, "integration@example.com")
            ],
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
