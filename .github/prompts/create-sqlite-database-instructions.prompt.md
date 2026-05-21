---
name: "SQLite Database Instructions Generator"
description: "Generate instruction file for SQLite database setup, configuration, and best practices in this Entity Framework Core project"
author: "Development Team"
tags: ["sqlite", "database", "entity-framework-core", "persistence"]
created: "2026-05-21"
---

# Prompt: Create SQLite Database Instruction File

## Context

This PostHubAPI project uses SQLite for production persistence with:
- **Database Provider**: Microsoft.EntityFrameworkCore.Sqlite 8.0.1
- **File-based storage**: Portable database file
- **Configuration**: Connection string via appsettings.json or user secrets
- **Initialization**: EF Core database creation via migrations (future)
- **Development**: InMemory provider used instead
- **Production**: SQLite for portable deployment

The project demonstrates:
- Environment-specific database provider selection
- SQLite connection string configuration
- Database file management
- EF Core schema mapping to SQLite
- Atomic operations and transactions
- Schema evolution strategies

## Instructions

Create a comprehensive `.github/instructions/sqlite-database.instructions.md` file that covers:

### 1. SQLite Fundamentals
- SQLite as embedded database engine
- File-based database (single .db file)
- Zero-configuration operation
- Portability advantages
- Limitations and constraints
- Concurrency and locking behavior

### 2. Setup & Installation
- Microsoft.EntityFrameworkCore.Sqlite package
- Version compatibility with EF Core
- System.Data.SQLite alternative considerations
- Platform-specific considerations (Windows, Linux, macOS)
- Licensing (public domain)

### 3. Connection Strings
- Basic SQLite connection string format
- Absolute and relative file paths
- Connection string in appsettings.json
- Connection string in user secrets
- Environment variable override
- Connection pooling options

### 4. EF Core Configuration
- AddDbContext with UseSqlite()
- DbContext setup in Program.cs
- Development (InMemory) vs Production (SQLite) strategy
- Environment detection for provider selection
- Lazy initialization considerations

### 5. Database Creation & Deletion
- Automatic database file creation
- Database initialization patterns
- Database seeding on first run
- Database reset during development
- Removing database files
- Database location best practices

### 6. Schema Design
- EF Core model-first approach
- Data annotations for schema definition
- Fluent API configuration
- SQLite data type mappings
- Primary keys and constraints
- Foreign key relationships

### 7. Data Types
- SQLite type system (TEXT, INTEGER, REAL, BLOB)
- EF Core to SQLite mapping
- String length constraints
- Decimal precision handling
- DateTime storage as TEXT or INTEGER
- Boolean representation (INTEGER 0/1)

### 8. Migrations
- Creating migrations from models
- Applying migrations to SQLite
- Migration history tracking
- Rollback strategies
- Idempotent migrations
- Production deployment of migrations

### 9. Transactions & ACID Properties
- Transaction support in SQLite
- BeginTransaction() for explicit control
- Atomicity of operations
- Isolation levels
- Savepoints for nested transactions
- Rollback on exception

### 10. Performance Optimization
- Query performance with indexes
- Creating indexes on foreign keys
- Query execution plans
- VACUUM and database optimization
- WAL (Write-Ahead Logging) mode
- Connection pooling configuration

### 11. Indexes & Query Optimization
- Creating indexes for frequent queries
- Composite indexes for multi-column queries
- Index management and maintenance
- Query analyzer and EXPLAIN QUERY PLAN
- Avoiding index bloat
- Performance regression testing

### 12. Backup & Recovery
- SQLite backup strategies
- File-based backup approach
- WAL mode backup considerations
- Point-in-time recovery
- Testing restore procedures
- Production backup automation

### 13. Maintenance & Administration
- Database integrity checking (PRAGMA integrity_check)
- Analyzing and updating statistics (ANALYZE)
- Vacuuming (rebuilding) database
- Handling database corruption
- Monitoring database size
- Cleanup strategies

### 14. Multi-User Scenarios
- SQLite locking and concurrency
- Handling database is locked errors
- Connection timeout settings
- Reader/writer contention
- Scaling limitations with SQLite
- Upgrading to server database (future)

### 15. SQLite-Specific Behaviors
- No native UUID type (TEXT or BLOB)
- Type affinity system
- Pragma statements and configuration
- Foreign key constraint enforcement
- Autoincrement vs ROWID
- PRAGMA journal_mode implications

### 16. Development Workflow
- Using SQLite during development
- Recreating database for clean state
- Seeding test data
- Database introspection tools
- SQLite browsers and viewers
- Quick iteration with local database

### 17. Testing with SQLite
- Using InMemory provider for unit tests
- Using SQLite for integration tests
- Test database isolation
- Seeding test data
- Cleanup between tests
- Performance considerations

### 18. Troubleshooting
- "Database is locked" errors
- Connection string issues
- File permission problems
- Invalid database file
- Migration failures
- Performance issues
- Data corruption detection

### 19. Deployment
- Including SQLite runtime with deployment
- Database file location on deployment server
- File permissions on production
- Backup automation
- Monitoring database health
- Upgrade procedures

### 20. Comparison with Other Databases
- SQLite vs SQL Server
- SQLite vs PostgreSQL
- SQLite vs MySQL
- When to upgrade from SQLite
- Migration strategies to other databases
- Scaling considerations

### 21. Advanced Features
- Full-text search (FTS) modules
- JSON extensions
- Custom functions
- Virtual tables
- Attached databases
- User-defined collations

### 22. Best Practices
- File path handling (relative vs absolute)
- Database naming conventions
- Backup frequency recommendations
- Archive old databases
- Version control considerations
- Documentation of schema

### 23. Security Considerations
- Database file permissions
- User access control
- Encryption at rest (extensions)
- Secure backup storage
- Data sanitization before deletion
- Audit trail implementation (future)

### 24. Monitoring & Observability
- Database size monitoring
- Query performance tracking
- Connection monitoring
- Resource utilization
- Slow query logging
- Error tracking

### 25. Future Enhancements
- Encrypted SQLite database
- Automatic backups to cloud
- Replication to cloud database
- Scaling to dedicated database server
- Database change tracking for sync

## Apply To

- `Program.cs` - DbContext and SQLite configuration
- `Data/ApplicationDbContext.cs` - DbContext definition
- `appsettings.json` - Connection string configuration
- `PostHubAPI.Tests/**/*.cs` - Test database setup
- `Models/*.cs` - Entity model and schema definition

## Version

1.0.0

## Maintainer

Development Team

## Related References

- [Official SQLite Documentation](https://www.sqlite.org/docs.html)
- [SQLite Home Page](https://www.sqlite.org/)
- [EF Core SQLite Provider](https://docs.microsoft.com/en-us/ef/core/providers/sqlite/)
- [PostHubAPI Architecture](docs/architecture.md)
- [PostHubAPI Requirements](docs/PROJECT-REQUIREMENTS.md)
- [Entity Framework Core Instructions](../.github/instructions/entity-framework-core.instructions.md)
- DbContext: `Data/ApplicationDbContext.cs`
- Configuration: `Program.cs` (lines 32-40)
- Connection string: `appsettings.json`
