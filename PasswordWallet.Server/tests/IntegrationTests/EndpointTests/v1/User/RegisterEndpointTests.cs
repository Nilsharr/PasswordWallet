using System.Net;
using System.Text.Json;
using Api.Endpoints.v1.User.Register;
using Core.Models;
using FastEndpoints;
using FluentAssertions;
using IntegrationTests.AppFixtures;
using IntegrationTests.BaseIntegrationTests;
using IntegrationTests.CollectionDefinitions;
using IntegrationTests.Fakes;

namespace IntegrationTests.EndpointTests.v1.User;

[Collection(UserEndpointCollection.CollectionName)]
public class RegisterEndpointTests(UserEndpointAppFixture appFixture)
    : BaseIntegrationTest(appFixture)
{
    [Fact]
    public async Task ValidRequest_ShouldRegisterUser()
    {
        var request = new RegisterRequestFaker().Generate();

        var (response, result) =
            await AnonymousClient.POSTAsync<RegisterEndpoint, RegisterRequest, AuthenticationResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.AccessToken.Should().NotBeNull();
        result.LastSuccessfulLogin.Should().BeNull();
        result.LastUnsuccessfulLogin.Should().BeNull();
    }

    [Fact]
    public async Task EmptyRequest_ShouldReturnBadRequest()
    {
        var request = new RegisterRequest("", "", "");

        var (response, result) =
            await AnonymousClient.POSTAsync<RegisterEndpoint, RegisterRequest, ErrorResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Errors.Should().HaveCount(3);
    }

    [Fact]
    public async Task DuplicatedUsername_ShouldReturnBadRequest()
    {
        var request = new RegisterRequestFaker().Generate();
        await AnonymousClient.POSTAsync<RegisterEndpoint, RegisterRequest, ErrorResponse>(request);
        var duplicatedRequest = new RegisterRequest(request.Username, "R3#xtwRq5!g", "R3#xtwRq5!g");

        var (response, result) =
            await AnonymousClient.POSTAsync<RegisterEndpoint, RegisterRequest, ErrorResponse>(duplicatedRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Errors.Should().HaveCount(1);
        result.Errors.Keys.Should().Equal(JsonNamingPolicy.CamelCase.ConvertName(nameof(request.Username)));
    }

    [Fact]
    public async Task WeakPassword_ShouldReturnBadRequest()
    {
        const string weakPassword = "123456";
        var request = new RegisterRequestFaker(weakPassword).Generate();

        var (response, result) =
            await AnonymousClient.POSTAsync<RegisterEndpoint, RegisterRequest, ErrorResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Errors.Should().HaveCount(1);
        result.Errors.Keys.Should().Equal(JsonNamingPolicy.CamelCase.ConvertName(nameof(request.Password)));
    }

    [Fact]
    public async Task NotMatchingPasswords_ShouldReturnBadRequest()
    {
        const string password = "hRa@vzV4g^4";
        const string notMatchingPassword = "qwerty123";
        var request = new RegisterRequest("Test321", password, notMatchingPassword);

        var (response, result) =
            await AnonymousClient.POSTAsync<RegisterEndpoint, RegisterRequest, ErrorResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Errors.Should().HaveCount(1);
        result.Errors.Keys.Should().Equal(JsonNamingPolicy.CamelCase.ConvertName(nameof(request.ConfirmPassword)));
    }
}