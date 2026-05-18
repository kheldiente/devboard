using System.Net;
using System.Net.Http.Json;
using DevBoard.Application.DTOs.Auth;
using FluentAssertions;

namespace DevBoard.Tests.Integration.Auth;

public class AuthControllerTests : IClassFixture<DevBoardWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(DevBoardWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ValidRequest_Returns200WithTokens()
    {
        // arrange
        var request = new RegisterRequestDto(
            "integration@test.com",
            "Password123!",
            "Integration User");

        // act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);
        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_InvalidCredentials_Returns401()
    {
        // arrange
        var request = new LoginRequestDto("wrong@test.com", "WrongPassword!");

        // act
         var response = await _client.PostAsJsonAsync("/api/auth/login", request);

         // assert
         response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
