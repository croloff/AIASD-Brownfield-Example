---
ai_generated: true
model: "openai/gpt-5.3-codex@unknown"
operator: "lawcarl"
chat_id: "create-sqlite-database-instructions-20260521"
prompt: |
  Follow instructions in #prompt:SKILL.md with these arguments:
  create-sqlite-databse-instructions.prompt.md
started: "2026-05-21T20:05:00Z"
ended: "2026-05-21T20:17:00Z"
task_durations:
  - task: "requirements alignment"
    duration: "00:03:00"
  - task: "instruction authoring"
    duration: "00:07:00"
  - task: "traceability updates"
    duration: "00:02:00"
total_duration: "00:12:00"
ai_log: "ai-logs/2026/05/21/create-sqlite-database-instructions-20260521/conversation.md"
source: ".github/prompts/create-sqlite-database-instructions.prompt.md"
name: sqlite-database
description: Use when configuring SQLite provider behavior, connection strings, schema changes, migrations, and runtime operations for EF Core persistence in PostHubAPI
applyTo: Program.cs,Data/*.cs,Models/*.cs,appsettings.json,appsettings.Development.json,PostHubAPI.Tests/**/*.cs
version: "1.0.0"
author: "lawcarl"
tags: ["sqlite", "entity-framework-core", "database", "persistence"]
owner: "Development Team"
reviewedDate: "2026-05-21"
nextReview: "2026-08-21"
---

# SQLite Database Instructions

## Overview

Use these instructions when implementing or modifying SQLite persistence behavior in PostHubAPI. Keep development and production behavior explicit, secure, and testable.

## 1. Provider Strategy

- Keep provider selection centralized in `Program.cs`.
- Use `UseInMemoryDatabase(...)` only for development and test scenarios that intentionally avoid file persistence.
- Use `UseSqlite(...)` for non-development runtime persistence.
- Do not mix providers inside the same request pipeline.

## 2. Connection Strings and Secret Handling

- Resolve `ConnectionStrings:DefaultConnection` from configuration for SQLite.
- Keep secrets and environment-specific values out of source-controlled JSON when possible.
- Prefer user-secrets or environment variables for local overrides.
- Validate that resolved SQLite paths are readable and writable before serving traffic.

## 3. DbContext Configuration

- Register `ApplicationDbContext` once in DI and avoid duplicate registrations.
- Keep provider branching simple and deterministic (`IsDevelopment` vs non-development).
- Configure provider options in startup, not inside services.
- Avoid runtime mutation of connection strings after app startup.

## 4. Schema Modeling for SQLite

- Define keys and relationships explicitly in EF Core model configuration.
- Use clear nullable vs required semantics to avoid implicit schema drift.
- Remember SQLite type affinity rules:
  - `INTEGER` for numeric IDs and booleans (`0`/`1`)
  - `TEXT` for strings and typical DateTime serialization
  - `REAL` for floating point
  - `BLOB` for binary payloads
- Use value converters when domain types need explicit persistence behavior.

## 5. Migrations and Schema Evolution

- Prefer EF Core migrations over ad hoc schema scripts.
- Review migration diffs before applying to shared environments.
- Do not edit old migration files after they have shipped.
- Apply migrations as part of deployment/startup policy for non-development environments.
- Plan destructive changes with backward-compatible rollout steps.

## 6. Transactions and Concurrency

- Wrap multi-entity write operations in explicit transactions when atomicity is required.
- Handle `database is locked` scenarios with retry/backoff strategy at service boundaries when appropriate.
- Keep write transactions short to reduce lock contention.
- Use savepoints only when nested recovery behavior is necessary.

## 7. Performance and Indexing

- Add indexes for high-selectivity filters and common sort columns.
- Add indexes for frequently traversed foreign keys.
- Keep heavy projections server-side and project only required fields.
- Use `AsNoTracking()` for read-only queries.
- Evaluate query plans during regressions (`EXPLAIN QUERY PLAN`).

## 8. Maintenance and Operations

- Monitor database file size growth and retention strategy.
- Schedule periodic integrity checks (`PRAGMA integrity_check`) in operational workflows.
- Use `VACUUM` intentionally during maintenance windows when compaction is needed.
- Validate backup and restore procedures regularly, not only during incidents.

## 9. Testing Strategy

- Use EF Core InMemory provider for fast unit tests that do not depend on relational behavior.
- Use SQLite-based integration tests when relational behavior matters.
- Isolate test databases per test scope to prevent cross-test leakage.
- Seed only minimal deterministic data required for assertions.

## 10. Troubleshooting Guidelines

- If startup fails, first validate connection string resolution and file path permissions.
- If locking errors occur, inspect long-lived transactions and concurrent writers.
- If migration failures occur, compare model snapshot, migration order, and target file path.
- If query performance degrades, inspect indexing and materialization points before optimizing code flow.

## 11. Deployment and Security

- Ensure deployment target grants least-privilege file access to the SQLite file location.
- Do not store sensitive SQLite files in publicly readable directories.
- Protect backup artifacts with equivalent or stronger access controls.
- Keep a documented restore/runbook path for operational recovery.

## 12. Upgrade and Exit Strategy

- Reassess SQLite fit when sustained concurrent write load increases.
- Define migration path to server databases (SQL Server or PostgreSQL) before scaling pressure becomes urgent.
- Keep data access patterns provider-agnostic where practical to reduce migration risk.

## Review Checklist

- [ ] Provider selection is explicit and environment-specific.
- [ ] SQLite connection string and file path handling are secure.
- [ ] Migrations are reviewed and deployment-safe.
- [ ] Indexing and read/write patterns are performance-aware.
- [ ] Tests cover both fast unit and relational integration scenarios.
- [ ] Backup, restore, and integrity checks are documented.

## Related References

- `.github/instructions/entity-framework-core.instructions.md`
- `.github/instructions/aspnet-core-api-design.instructions.md`
- `Program.cs`
- `Data/ApplicationDbContext.cs`
- `Models/Post.cs`
- `Models/Comment.cs`
- `Models/User.cs`
- `appsettings.json`
- `docs/architecture.md`
