---
name: "xUnit Testing Instructions Generator"
description: "Generate instruction file for unit and integration testing patterns using xUnit in this ASP.NET Core project"
author: "Development Team"
tags: ["xunit", "testing", "unit-tests", "integration-tests", "mocking"]
created: "2026-05-21"
---

# Prompt: Create xUnit Testing Instruction File

## Context

This PostHubAPI project uses xUnit for testing with:
- **Test Project**: PostHubAPI.Tests
- **Test Framework**: xUnit with ASP.NET Core Test Host
- **Test Types**: Unit tests, integration tests, authorization tests
- **Test Organization**: Organized by layer (Controllers, Services, Configuration)
- **Existing Tests**: UserControllerTests, PostControllerAuthorizationTests, JwtSettingsResolverTests

The project demonstrates:
- Controller integration testing
- Authorization behavior verification
- Configuration resolution testing
- Service layer testing
- Test isolation and data setup

## Instructions

Create a comprehensive `.github/instructions/xunit-testing.instructions.md` file that covers:

### 1. xUnit Framework Basics
- xUnit project structure and conventions
- Test class naming (TestName, *Tests suffix)
- Test method naming ([Fact], [Theory] attributes)
- Assertion methods (Assert.Equal, Assert.Null, Assert.Throws, etc.)
- Test fixtures and setup/teardown (IAsyncLifetime)

### 2. Test Organization
- Organizing tests by layer (Controllers, Services, Configuration)
- One test class per class under test
- Test method organization within classes
- Test data setup and teardown
- Shared fixtures across test classes

### 3. Unit Test Patterns
- Testing individual methods in isolation
- Mocking dependencies
- Arranging test data (Arrange-Act-Assert)
- Testing happy paths and error cases
- Testing edge cases and boundary conditions

### 4. Integration Testing
- Testing multiple components together
- Using ASP.NET Core Test Host (WebApplicationFactory)
- Creating test servers and HTTP clients
- Making actual HTTP requests in tests
- Asserting on HTTP responses (status code, content)

### 5. Testing Controllers
- WebApplicationFactory<T> setup
- Creating test HttpClient
- Making HTTP requests (GET, POST, PUT, DELETE)
- Asserting response status codes
- Parsing and asserting response content
- Testing error scenarios

### 6. Testing Authorization
- Testing [Authorize] attributes
- Testing authenticated vs anonymous access
- Testing authorization failures (401, 403)
- Testing resource ownership checks
- Simulating authenticated user requests

### 7. Mocking Dependencies
- Mocking repositories and services
- Mocking external APIs
- Using Moq library (if included)
- Stubbing return values
- Verifying mock method calls

### 8. Testing Configuration
- Testing configuration resolution (JwtSettingsResolver)
- Testing environment-specific settings
- Testing configuration loading from appsettings
- Testing user secrets resolution
- Testing environment variable overrides

### 9. Testing Validation
- Testing ModelState validation
- Testing DataAnnotations enforcement
- Testing custom validation logic
- Testing validation error messages
- Testing invalid input rejection

### 10. Testing Database Access (EF Core)
- Using InMemory provider for tests
- Seeding test data
- Testing query behavior
- Testing save operations
- Testing relationship loading
- Test data isolation

### 11. Test Data Builders & Fixtures
- Creating reusable test data builders
- Fixture patterns for shared setup
- Minimal builders for specific scenarios
- Avoiding test data duplication
- Factory patterns for entity creation

### 12. Async Testing
- Using async/await in test methods
- Testing async operations
- Avoiding Task.Result in tests
- Awaiting async assertions
- Testing cancellation tokens

### 13. Parameterized Tests
- [Theory] attribute for multiple test cases
- [InlineData] for simple parameters
- [MemberData] for complex test data
- [ClassData] for test data classes
- Reducing test code duplication

### 14. Assertion Patterns
- Assert.Equal() for equality
- Assert.Null() / Assert.NotNull() for nullability
- Assert.True() / Assert.False() for booleans
- Assert.Throws<T>() for exceptions
- Assert.IsType<T>() for type checking
- Custom assertions and extension methods

### 15. Exception Testing
- Testing that methods throw expected exceptions
- Testing exception types
- Testing exception messages
- Testing no exception is thrown
- Recording exceptions for assertions

### 16. Test Quality & Best Practices
- Naming tests clearly (What, When, Then)
- One assertion focus per test (when possible)
- Avoiding test interdependencies
- Cleaning up resources (IDisposable)
- Using constants for magic values
- Avoiding complex test logic

### 17. Testing Services
- Isolating service logic with mocked dependencies
- Testing business rule enforcement
- Testing data transformation
- Testing error handling
- Testing validation in services

### 18. Testing Ownership & Authorization Logic
- Testing user ownership verification
- Testing authorization service calls
- Testing 403 Forbidden responses
- Testing cross-user access prevention
- Testing admin override logic (future)

### 19. Performance Testing
- Load testing endpoints
- Measuring response times
- Testing N+1 query problems
- Benchmarking critical paths
- Load generation tools

### 20. Test Coverage
- Code coverage metrics
- Identifying untested code paths
- Coverage thresholds and targets
- Avoiding false coverage (bad tests)
- Critical path coverage priority

### 21. Continuous Integration Testing
- Running tests in CI/CD pipeline
- Failing builds on test failure
- Collecting coverage reports
- Trending test metrics
- Preventing regressions

### 22. Common Pitfalls
- Tests dependent on execution order
- Shared state between tests
- Tests too tightly coupled to implementation
- Not cleaning up resources
- Ignoring flaky tests
- Over-testing implementation details

### 23. Debugging Tests
- Running individual tests
- Running tests with breakpoints
- Outputting debug information
- Test output helpers
- Verbose test logging

### 24. Future Testing Enhancements
- Snapshot testing for complex objects
- Property-based testing with FsCheck
- Contract testing with Pact
- Performance benchmarking with BenchmarkDotNet
- Chaos engineering and failure injection

## Apply To

- `PostHubAPI.Tests/**/*.cs` - All test files
- `PostHubAPI.Tests/Controllers/*.cs` - Controller tests
- `PostHubAPI.Tests/Services/*.cs` - Service tests (future)
- `PostHubAPI.Tests/Configuration/*.cs` - Configuration tests
- `PostHubAPI.Tests/Data/*.cs` - Data access tests (future)

## Version

1.0.0

## Maintainer

Development Team

## Related References

- [Official xUnit Documentation](https://xunit.net/docs/getting-started)
- [xUnit GitHub Repository](https://github.com/xunit/xunit)
- [ASP.NET Core Testing](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests)
- [PostHubAPI Architecture](docs/architecture.md)
- [PostHubAPI Requirements](docs/PROJECT-REQUIREMENTS.md)
- Test Project: `PostHubAPI.Tests/`
- Existing Tests: `PostHubAPI.Tests/Controllers/*.cs`
- Configuration Tests: `PostHubAPI.Tests/Configuration/JwtSettingsResolverTests.cs`
