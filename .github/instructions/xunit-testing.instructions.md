---
ai_generated: true
model: "openai/gpt-5.3-codex@unknown"
operator: "lawcarl"
chat_id: "create-xunit-testing-instructions-20260521"
prompt: |
  Follow instructions in #prompt:SKILL.md with these arguments: create-xunit-testing-instructions.prompt.md
started: "2026-05-21T16:40:30Z"
ended: "2026-05-21T16:45:30Z"
task_durations:
  - task: "instruction discovery and alignment"
    duration: "00:02:00"
  - task: "xunit instruction authoring"
    duration: "00:02:00"
  - task: "traceability updates"
    duration: "00:01:00"
total_duration: "00:05:00"
ai_log: "ai-logs/2026/05/21/create-xunit-testing-instructions-20260521/conversation.md"
source: ".github/prompts/create-xunit-testing-instructions.prompt.md"
name: xunit-testing
description: Use when writing or updating xUnit unit/integration tests for PostHubAPI, including controller authorization behavior, configuration tests, and test isolation patterns.
applyTo: "PostHubAPI.Tests/**/*.cs"
version: "1.0.0"
author: "Development Team"
tags: ["xunit", "testing", "aspnet-core", "integration-testing", "mocking"]
owner: "Development Team"
reviewedDate: "2026-05-21"
nextReview: "2026-08-21"
---

# xUnit Testing Instructions

## Overview

Use these guidelines when creating or modifying tests in `PostHubAPI.Tests`. Prefer reliable, behavior-focused tests that validate outcomes rather than implementation details.

## Scope

- Unit tests for isolated service and utility behavior
- Integration tests for API endpoints and authorization
- Configuration tests for startup and JWT settings resolution

## Test Structure

- Name test classes with `*Tests` suffix.
- Use `Fact` for single-case behavior and `Theory` for data-driven behavior.
- Use Arrange-Act-Assert sections with clear separation.
- Keep one behavior assertion focus per test when practical.

## Naming Conventions

- Test method format: `MethodName_WhenCondition_ShouldExpectedResult`.
- Keep names explicit about the condition and expected behavior.
- Use domain language from the API (post, comment, user, owner, authorization).

## Unit Testing Guidelines

- Isolate the class under test from external dependencies.
- Mock only boundaries (repositories, external services, clock/context abstractions).
- Assert observable outputs:
  - returned DTO/entity values
  - thrown exceptions
  - state transitions
- Avoid asserting private implementation details.

## Integration Testing Guidelines

- Use `WebApplicationFactory<Program>` for endpoint-level tests.
- Issue real HTTP requests through `HttpClient`.
- Validate full HTTP contract:
  - status code
  - response content type
  - response body shape and key fields
- Prefer realistic request payloads matching DTO contracts.

## Authorization Testing

- Validate anonymous access to protected endpoints returns `401`.
- Validate authenticated but unauthorized access returns `403` when applicable.
- Validate owner-authorized access succeeds for edit/delete paths.
- Cover both positive and negative ownership scenarios.

## Async and Reliability

- Keep test methods async when calling async APIs.
- Avoid `Task.Result` and `.Wait()` in tests.
- Await all asynchronous operations and assertions.
- Avoid time-based flakiness; do not rely on sleeps or execution order.

## Test Data and Isolation

- Build deterministic test data with explicit values.
- Avoid shared mutable state between tests.
- Reset test state per test case or fixture lifecycle.
- Use minimal builders/factories when setup duplication grows.

## Exception and Validation Testing

- Assert exception type first, then relevant message/content.
- Cover validation failures for invalid input DTOs.
- Verify bad input maps to expected API response behavior in integration tests.

## EF Core Testing Considerations

- Prefer provider behavior that matches test intent.
- Use seeded data for query and relationship scenarios.
- Verify save/update/delete effects through re-query, not tracked object assumptions.
- Ensure each test has isolated database context/state.

## What to Avoid

- Over-mocking framework internals.
- Asserting exact framework-generated error text unless contract requires it.
- Tests coupled to method call counts when behavior assertions are sufficient.
- Large all-in-one tests that validate unrelated concerns.

## Minimal Checklist

- [ ] Covers happy path and at least one failure path
- [ ] Uses deterministic, isolated setup
- [ ] Validates behavior/contract, not internal implementation
- [ ] Uses async patterns correctly
- [ ] Names test clearly with condition and expected outcome

## Suggested Patterns in This Repository

- Keep controller authorization tests near `PostControllerAuthorizationTests` style.
- Keep configuration resolution tests near `JwtSettingsResolverTests` style.
- Group tests by target layer under `PostHubAPI.Tests/Controllers`, `PostHubAPI.Tests/Configuration`, and future `PostHubAPI.Tests/Services`.

## Related Instructions

- `.github/instructions/aspnet-core-api-design.instructions.md`
- `.github/instructions/entity-framework-core.instructions.md`
- `.github/instructions/csharp.instructions.md`
