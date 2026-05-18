using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using DevBoard.Application.DTOs.Auth;
using DevBoard.Application.DTOs.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace DevBoard.Tests.Integration.Tasks;

public class TasksControllerTests : IClassFixture<DevBoardWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly DevBoardWebApplicationFactory _factory;

    public TasksControllerTests(DevBoardWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<String> GetTokenAsync()
    {
        var register = new RegisterRequestDto(
            $"{Guid.NewGuid()}@test.com",
            "Password123!",
            "Test User");

        var response = await _client.PostAsJsonAsync("/api/auth/register", register);
        var auth = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        return auth!.AccessToken;
    }

    [Fact]
    public async Task CreateTask_ValidRequest_Returns201()
    {
        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // get a board id from seed
        var boardId = await GetSeededBoardIdAsync();

        var request = new CreateTaskRequest(
            "Integration test task",
            "Created in integration test",
            boardId,
            null,
            3);

        // act
        var response = await _client.PostAsJsonAsync("api/tasks", request);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateTask_Unauthenticated_Returns401()
    {
        var request = new CreateTaskRequest(
            "Unauthorized task",
            null,
            Guid.NewGuid(),
            null,
            1);

        var response = await _client.PostAsJsonAsync("/api/tasks", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task<Guid> GetSeededBoardIdAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider
            .GetRequiredService<DevBoard.Infrastructure.Persistence.AppDbContext>();
        var board = db.Boards.First();
        return board.Id;
    }
}