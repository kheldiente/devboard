dotnet new sln -n DevBoard
dotnet new webapi -n DevBoard.Api
dotnet new classlib -n DevBoard.Domain
dotnet new classlib -n DevBoard.Application
dotnet new classlib -n DevBoard.Infrastructure
dotnet new xunit -n DevBoard.Tests

dotnet sln add **/*.csproj

Api → Application → Domain
Infrastructure → Application
Api → Infrastructure (for DI registration only)
Tests → Application, Infrastructure

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
