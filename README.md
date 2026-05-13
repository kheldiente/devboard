# DevBoard

A production-style task and board management backend built with modern ASP.NET Core practices.  
This project demonstrates Clean Architecture, CQRS with MediatR, JWT authentication, EF Core with PostgreSQL, background jobs using Hangfire, and automated testing with Testcontainers.

---

# Tech Stack

- ASP.NET Core 8 Web API
- PostgreSQL
- Entity Framework Core
- Clean Architecture
- CQRS + MediatR
- JWT Authentication + Refresh Tokens
- FluentValidation
- Hangfire
- xUnit
- Testcontainers
- Docker
- GitHub Actions

---

# Architecture

```text
┌────────────────────┐
│    DevBoard.Api    │
│  Controllers/API   │
└─────────┬──────────┘
          │
          ▼
┌────────────────────┐
│ DevBoard.Application│
│ CQRS + MediatR      │
│ Business Logic      │
└─────────┬──────────┘
          │
          ▼
┌────────────────────┐
│  DevBoard.Domain   │
│   Core Entities    │
│  Business Rules    │
└─────────┬──────────┘
          │
          ▼
┌────────────────────┐
│DevBoard.Infrastructure│
│ EF Core + PostgreSQL │
│ External Services    │
└────────────────────┘
```

---

# Swagger

```
http://localhost:5196/swagger
```

---

# Solution Setup

## Create the Solution

```bash
dotnet new sln -n DevBoard

dotnet new webapi -n DevBoard.Api
dotnet new classlib -n DevBoard.Domain
dotnet new classlib -n DevBoard.Application
dotnet new classlib -n DevBoard.Infrastructure
dotnet new xunit -n DevBoard.Tests

dotnet sln add **/*.csproj
```

---

# Project References

```text
DevBoard.Api
 ├── DevBoard.Application
 └── DevBoard.Infrastructure

DevBoard.Application
 └── DevBoard.Domain

DevBoard.Infrastructure
 └── DevBoard.Application

DevBoard.Tests
 ├── DevBoard.Application
 └── DevBoard.Infrastructure
```

> `DevBoard.Api` references `Infrastructure` only for dependency injection registration.

---

# NuGet Packages

## Infrastructure

```bash
dotnet add DevBoard.Infrastructure package Microsoft.EntityFrameworkCore
dotnet add DevBoard.Infrastructure package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add DevBoard.Infrastructure package Microsoft.AspNetCore.Identity.EntityFrameworkCore
```

## Application

```bash
dotnet add DevBoard.Application package MediatR
dotnet add DevBoard.Application package FluentValidation
dotnet add DevBoard.Application package FluentValidation.DependencyInjectionExtensions
```

## API

```bash
dotnet add DevBoard.Api package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add DevBoard.Api package Hangfire.AspNetCore
dotnet add DevBoard.Api package Hangfire.PostgreSql
```

## Tests

```bash
dotnet add DevBoard.Tests package Testcontainers.PostgreSql
dotnet add DevBoard.Tests package Microsoft.AspNetCore.Mvc.Testing
```

---

# Authentication Flow

This project uses:

- Short-lived JWT access tokens
- Long-lived refresh tokens with rotation

## Authentication Lifecycle

```text
Login
  ├── Validate credentials
  ├── Issue access token (15 minutes)
  ├── Issue refresh token (7 days)
  └── Store refresh token in database
```

## Client Storage Strategy

| Token Type | Storage |
|---|---|
| Access Token | In-memory only |
| Refresh Token | HTTP-only cookie |

## Refresh Flow

```text
401 Unauthorized
  └── Client calls /auth/refresh
        ├── Validate refresh token
        ├── Rotate refresh token
        ├── Invalidate previous token
        └── Return new access token
```

Refresh token rotation helps prevent replay attacks and is commonly discussed during backend interviews.

---

# CQRS with MediatR

## Request Flow

```text
Controller
   ↓
IMediator
   ↓
Command / Query Handler
   ↓
Business Logic
   ↓
Response
```

Controllers remain thin while business logic stays inside handlers.

---

# Command vs Query

| Type | Responsibility |
|---|---|
| Command | Changes application state |
| Query | Reads data only |

## Example

```csharp
// Command
public record CreateTaskCommand(
    string Title,
    Guid BoardId
) : IRequest<Guid>;

// Query
public record GetTasksByBoardQuery(
    Guid BoardId
) : IRequest<List<TaskDto>>;
```

### Why use handlers instead of controllers?

Handlers are:

- Easier to unit test
- Independent of HTTP concerns
- Reusable across transports
- Better aligned with separation of concerns

---

# Pipeline Behaviors

Pipeline behaviors act like middleware for MediatR requests.

## Recommended Order

```csharp
services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(ValidationBehavior<,>)
);

services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(LoggingBehavior<,>)
);

services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(PerformanceBehavior<,>)
);
```

## Behaviors

| Behavior | Purpose |
|---|---|
| ValidationBehavior | Runs FluentValidation before handlers |
| LoggingBehavior | Logs request name and execution time |
| PerformanceBehavior | Warns on slow requests |

---

# Entity Framework Core

## Rich Domain Models

Avoid anemic entities by keeping business rules inside the domain model.

```csharp
public class TaskItem
{
    public TaskStatus Status { get; private set; }

    public void Transition(TaskStatus newStatus)
    {
        // Validate allowed transitions
        Status = newStatus;
    }
}
```

---

# Value Objects with Owned Types

```csharp
modelBuilder.Entity<TaskItem>()
    .OwnsOne(t => t.Priority);
```

---

# Migrations

## Create Migration

```bash
dotnet ef migrations add AddRefreshTokenTable \
  -p DevBoard.Infrastructure \
  -s DevBoard.Api
```

## Apply Migration

```bash
dotnet ef database update \
  -p DevBoard.Infrastructure \
  -s DevBoard.Api
```

```bash
dotnet ef migrations add AddProjectsBoardsTasks \ 
    -p DevBoard.Infrastructure \ 
    -s DevBoard.Api
```

```bash
dotnet ef database update \
    -p DevBoard.Instracture \
    -s DevBoard.Api
```

```
-p DevBoard.Infrastructure - the project that contains AppDbContext and where migrations files are saved \
-s DevBoard.Api - the startup project that has appsettings.json with the connection string and DI setup
```

> Never modify a migration after it has been applied. Create a new migration instead.

---

# Avoiding the N+1 Query Problem

## Bad

```csharp
var tasks = context.Tasks.ToList();

foreach (var task in tasks)
{
    Console.WriteLine(task.Assignee.Name);
}
```

This generates additional queries per task.

---

## Better

```csharp
var tasks = context.Tasks
    .Include(t => t.Assignee)
    .ToList();
```

For read-heavy endpoints, prefer projections with `Select()` to avoid loading entire entities unnecessarily.

---

# Background Jobs with Hangfire

## Fire-and-Forget

```csharp
BackgroundJob.Enqueue(
    () => emailService.SendWelcome(userId)
);
```

## Delayed Jobs

```csharp
BackgroundJob.Schedule(
    () => task.SendReminder(taskId),
    TimeSpan.FromHours(24)
);
```

## Recurring Jobs

```csharp
RecurringJob.AddOrUpdate(
    "overdue-scanner",
    () => scanner.RunAsync(),
    Cron.Daily
);
```

Hangfire persists jobs in PostgreSQL, allowing retries even after application restarts.

Key concepts to understand:

- Retry policies
- Idempotent job handlers
- Persistent background processing

---

# Testing Strategy

## Unit Tests

Focus on business logic and handlers only.

```csharp
public class CreateTaskHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_ReturnsNewTaskId()
    {
        // Arrange
        var repo = Substitute.For<ITaskRepository>();
        var handler = new CreateTaskHandler(repo);

        var command = new CreateTaskCommand(
            "Fix login bug",
            boardId
        );

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None
        );

        // Assert
        result.Should().NotBeEmpty();
    }
}
```

---

## Integration Tests

Run against a real PostgreSQL instance using Testcontainers.

```csharp
public class TasksApiTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _db =
        new PostgreSqlBuilder().Build();

    public async Task InitializeAsync()
    {
        await _db.StartAsync();

        // Configure WebApplicationFactory
        // with container connection string
    }

    [Fact]
    public async Task POST_tasks_creates_and_returns_201()
    {
        // Real HTTP endpoint
        // Real database
        // Real migrations
    }

    public async Task DisposeAsync()
    {
        await _db.DisposeAsync();
    }
}
```

## Testing Philosophy

| Test Type | Validates |
|---|---|
| Unit Tests | Business logic correctness |
| Integration Tests | Application wiring and infrastructure |

Both are necessary for confidence in production systems.

---

# Docker Setup

## docker-compose.yml

```yaml
services:
  api:
    build: ./backend
    ports:
      - "5000:8080"
    environment:
      - ConnectionStrings__Default=Host=db;Database=devboard
      - Jwt__Secret=${JWT_SECRET}
    depends_on:
      - db

  db:
    image: postgres:16
    environment:
      POSTGRES_DB: devboard
      POSTGRES_PASSWORD: ${DB_PASSWORD}
    volumes:
      - pgdata:/var/lib/postgresql/data

volumes:
  pgdata:
```

---

# CI with GitHub Actions

```yaml
name: CI

on:
  - push
  - pull_request

jobs:
  test:
    runs-on: ubuntu-latest

    services:
      postgres:
        image: postgres:16
        env:
          POSTGRES_PASSWORD: test
        options: >-
          --health-cmd pg_isready
          --health-interval 10s

    steps:
      - uses: actions/checkout@v4

      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 8.x

      - run: dotnet restore
      - run: dotnet build --no-restore
      - run: dotnet test --no-build
```

---

# Running the Project

## Run API

```bash
dotnet run --project DevBoard.Api
```

## Run with Docker

```bash
docker compose up --build
```

---

# Key Engineering Decisions

| Decision | Reason |
|---|---|
| Clean Architecture | Clear separation of concerns |
| CQRS + MediatR | Thin controllers and testable business logic |
| JWT + Refresh Tokens | Secure stateless authentication |
| PostgreSQL | Reliable relational database |
| Hangfire | Persistent background job processing |
| Testcontainers | Real integration testing environment |
| Docker | Consistent local and deployment environments |

---

# Future Improvements

- Redis caching
- Distributed tracing
- Role-based authorization
- WebSocket notifications
- OpenTelemetry metrics
- Kubernetes deployment
- Rate limiting
- Multi-tenant support

---

# License

MIT License