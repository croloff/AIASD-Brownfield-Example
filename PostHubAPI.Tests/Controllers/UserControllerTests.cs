using Microsoft.AspNetCore.Mvc;
using PostHubAPI.Controllers;
using PostHubAPI.Dtos.User;
using PostHubAPI.Services.Interfaces;

namespace PostHubAPI.Tests.Controllers;

public class UserControllerTests
{
    [Fact]
    public async Task Register_ReturnsOkWithJwtToken_WhenServiceExecutesSuccessfully()
    {
        // Arrange
        const string expectedToken = "jwt-token";
        var controller = new UserController(new StubUserService
        {
            RegisterHandler = _ => Task.FromResult(expectedToken)
        });

        // Act
        var result = await controller.Register(CreateRegisterDto());

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        var payload = Assert.IsType<string>(okResult.Value);
        Assert.Equal(expectedToken, payload);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        var controller = new UserController(new StubUserService());
        controller.ModelState.AddModelError("Email", "Email is required");

        var result = await controller.Register(CreateRegisterDto());

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }

    [Fact]
    public async Task Register_ReturnsBadRequestMessage_WhenServiceThrowsArgumentException()
    {
        const string expectedMessage = "duplicate user";
        var controller = new UserController(new StubUserService
        {
            RegisterHandler = _ => Task.FromException<string>(new ArgumentException(expectedMessage))
        });

        var result = await controller.Register(CreateRegisterDto());

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
        Assert.Equal(expectedMessage, badRequest.Value);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenInputIsNull()
    {
        // Arrange
        var controller = new UserController(new StubUserService());

        // Act
        var result = await controller.Register(null);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }

    [Fact]
    public async Task Login_ReturnsOkWithStringPayload_WhenServiceSucceeds()
    {
        const string expectedToken = "jwt-token";
        var controller = new UserController(new StubUserService
        {
            LoginHandler = _ => Task.FromResult(expectedToken)
        });

        var result = await controller.Login(CreateLoginDto());

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        var payload = Assert.IsType<string>(okResult.Value);
        Assert.Equal(expectedToken, payload);
    }

    [Fact]
    public async Task Login_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        var controller = new UserController(new StubUserService());
        controller.ModelState.AddModelError("Password", "Password is required");

        var result = await controller.Login(CreateLoginDto());

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }

    [Fact]
    public async Task Login_ReturnsBadRequestMessage_WhenServiceThrowsArgumentException()
    {
        const string expectedMessage = "invalid credentials";
        var controller = new UserController(new StubUserService
        {
            LoginHandler = _ => Task.FromException<string>(new ArgumentException(expectedMessage))
        });

        var result = await controller.Login(CreateLoginDto());

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
        Assert.Equal(expectedMessage, badRequest.Value);
    }

    private static RegisterUserDto CreateRegisterDto() =>
        new()
        {
            Email = "user@example.com",
            Username = "testuser",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

    private static LoginUserDto CreateLoginDto() =>
        new()
        {
            Username = "testuser",
            Password = "Password123!"
        };

    private sealed class StubUserService : IUserService
    {
        public Func<RegisterUserDto, Task<string>> RegisterHandler { get; init; } = _ => Task.FromResult("register-token");
        public Func<LoginUserDto, Task<string>> LoginHandler { get; init; } = _ => Task.FromResult("login-token");

        public Task<string> Register(RegisterUserDto dto) => RegisterHandler(dto);

        public Task<string> Login(LoginUserDto dto) => LoginHandler(dto);
    }
}
