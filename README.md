Phase 1 — Foundation

Project setup
bashdotnet new sln -n DevBoard
dotnet new webapi -n DevBoard.Api
dotnet new classlib -n DevBoard.Domain
dotnet new classlib -n DevBoard.Application
dotnet new classlib -n DevBoard.Infrastructure
dotnet new xunit -n DevBoard.Tests

dotnet sln add **/*.csproj

################################
Project references:

Api → Application → Domain
Infrastructure → Application
Api → Infrastructure (for DI registration only)
Tests → Application, Infrastructure

################################
Key NuGet packages

# Infrastructure
dotnet add DevBoard.Infrastructure package Microsoft.EntityFrameworkCore
dotnet add DevBoard.Infrastructure package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add DevBoard.Infrastructure package Microsoft.AspNetCore.Identity.EntityFrameworkCore

# Application
dotnet add DevBoard.Application package MediatR
dotnet add DevBoard.Application package FluentValidation
dotnet add DevBoard.Application package FluentValidation.DependencyInjectionExtensions

# Api
dotnet add DevBoard.Api package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add DevBoard.Api package Hangfire.AspNetCore
dotnet add DevBoard.Api package Hangfire.PostgreSql

# Tests
dotnet add DevBoard.Tests package Testcontainers.PostgreSql
dotnet add DevBoard.Tests package Microsoft.AspNetCore.Mvc.Testing

################################
Auth mental model — JWT + refresh tokens

Login request
  → validate credentials
  → issue short-lived access token (15 min)
  → issue long-lived refresh token (7 days, stored in DB)
  → return both to client

Client stores:
  access token  → memory (never localStorage)
  refresh token → httpOnly cookie

On 401:
  → client hits /auth/refresh with cookie
  → server validates refresh token, rotates it (old one invalidated)
  → returns new access token
The rotation part (invalidating old refresh tokens) is what interviewers want to hear about — it prevents replay attacks.

################################
Phase 2 — CQRS with MediatR

The pattern in plain terms
Controller → sends Command or Query via IMediator
                → MediatR finds the right Handler
                → Handler does the work
                → returns result
Controllers become thin. All business logic lives in handlers. This is the key interview point.

################################
Pipeline behaviors — register these in order

ValidationBehavior     ← runs FluentValidation before handler
LoggingBehavior        ← logs command name + elapsed time
PerformanceBehavior    ← warns if handler takes > 500ms

csharp// Registration order matters
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

################################
Command vs Query — the rule
Command: changes state, returns nothing (or just an ID)
Query:   reads state, never changes anything

// Command
public record CreateTaskCommand(string Title, Guid BoardId) : IRequest<Guid>;

// Query
public record GetTasksByBoardQuery(Guid BoardId) : IRequest<List<TaskDto>>;
Interview trap to avoid
"Why not just put this logic in the controller?"
→ Handlers are unit-testable in isolation — no HTTP context needed. Controllers are just transport.

################################
Phase 3 — EF Core

Code-first tips that come up in interviews
Avoid anemic models — put behavior on your entities:
csharppublic class TaskItem
{
    public TaskStatus Status { get; private set; }

    public void Transition(TaskStatus newStatus)
    {
        // validate allowed transitions here
        // don't let EF or a service do this logic
        Status = newStatus;
    }
}
################################
Owned types for value objects:
csharp// In your DbContext OnModelCreating
modelBuilder.Entity<TaskItem>()
    .OwnsOne(t => t.Priority);

################################
Migrations discipline:
bash# Always name migrations meaningfully
dotnet ef migrations add AddRefreshTokenTable -p DevBoard.Infrastructure -s DevBoard.Api
dotnet ef database update -p DevBoard.Infrastructure -s DevBoard.Api
Never edit a migration after it's been applied. Add a new one.

################################
N+1 — the most common EF interview question
csharp// BAD — hits DB once per task
var tasks = context.Tasks.ToList();
foreach (var task in tasks)
    Console.WriteLine(task.Assignee.Name); // extra query each time

// GOOD
var tasks = context.Tasks
    .Include(t => t.Assignee)
    .ToList();
Know this cold. Then mention Select() projections as even better for read-heavy queries (don't load full entities you don't need).

################################
Phase 4 — Background jobs with Hangfire
Three job types to know

csharp// Fire and forget — runs once, immediately
BackgroundJob.Enqueue(() => emailService.SendWelcome(userId));

// Delayed
BackgroundJob.Schedule(() => task.SendReminder(taskId), TimeSpan.FromHours(24));

// Recurring — cron syntax
RecurringJob.AddOrUpdate("overdue-scanner",
    () => scanner.RunAsync(),
    Cron.Daily);
Interview angle
Hangfire persists jobs to PostgreSQL — so if your API restarts mid-job, it retries. Mention retry policies and idempotency: your job handler should be safe to run twice with the same input.

################################
Phase 5 — Testing strategy
Unit tests — handlers only, no DB
csharppublic class CreateTaskHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_ReturnsNewTaskId()
    {
        // Arrange
        var repo = Substitute.For<ITaskRepository>(); // NSubstitute
        var handler = new CreateTaskHandler(repo);
        var command = new CreateTaskCommand("Fix login bug", boardId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty(); // FluentAssertions
    }
}
Integration tests — real Postgres via Testcontainers
csharppublic class TasksApiTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _db = new PostgreSqlBuilder().Build();

    public async Task InitializeAsync()
    {
        await _db.StartAsync();
        // build WebApplicationFactory with connection string pointing to container
    }

    [Fact]
    public async Task POST_tasks_creates_and_returns_201()
    {
        // hits real HTTP endpoints → real DB → real migrations
    }

    public async Task DisposeAsync() => await _db.DisposeAsync();
}
The interview distinction: unit tests prove your logic is correct, integration tests prove your wiring is correct. You need both.
################################

Phase 6 — Docker + CI
docker-compose.yml shape
yamlservices:
  api:
    build: ./backend
    ports: ["5000:8080"]
    environment:
      - ConnectionStrings__Default=Host=db;Database=devboard;...
      - Jwt__Secret=${JWT_SECRET}
    depends_on: [db]

  db:
    image: postgres:16
    environment:
      POSTGRES_DB: devboard
      POSTGRES_PASSWORD: ${DB_PASSWORD}
    volumes:
      - pgdata:/var/lib/postgresql/data

  hangfire-dashboard:
    # same api image, different entry — or expose via api
GitHub Actions — minimal but correct
yamlname: CI
on: [push, pull_request]

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

The README matters
When you push to GitHub, your README is what a hiring manager sees first. Structure it as:

What the app does (2 sentences)
Architecture diagram (even a simple ASCII one)
How to run locally (docker compose up — one command)
Key technical decisions and why (this is your interview answer in writing)

################################

To run this project, `dotnet run --project DevBoard.Api`
