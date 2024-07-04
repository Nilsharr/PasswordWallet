using System.Net;
using Api.Endpoints.v1.User.GetLoginHistory;
using Api.Endpoints.v1.User.Login;
using Api.Endpoints.v1.User.Register;
using Core.Models;
using FastEndpoints;
using FluentAssertions;
using IntegrationTests.AppFixtures;
using IntegrationTests.BaseIntegrationTests;
using IntegrationTests.CollectionDefinitions;
using IntegrationTests.Extensions;
using IntegrationTests.Fakes;

namespace IntegrationTests.EndpointTests.v1.User;

[Collection(UserEndpointCollection.CollectionName)]
public class GetLoginHistoryEndpointTests(UserEndpointAppFixture appFixture) : BaseIntegrationTest(appFixture)
{
    private RegisterRequest _registerResponse = default!;
    private HttpClient _authenticatedClient = default!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _registerResponse = new RegisterRequestFaker().Generate();
        var registerResponse = await AnonymousClient.RegisterUser(_registerResponse);
        _authenticatedClient = CreateAuthenticatedClient(registerResponse.AccessToken);
    }

    public override async Task DisposeAsync()
    {
        await base.DisposeAsync();
        _authenticatedClient.Dispose();
    }

    [Fact]
    public async Task UnauthenticatedUser_ShouldReturnUnauthorized()
    {
        var getLoginHistoryRequest = new GetLoginHistoryRequest(1, 30);

        var response =
            await AnonymousClient.GETAsync<GetLoginHistoryEndpoint, GetLoginHistoryRequest>(getLoginHistoryRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task HasLoginHistory_ShouldReturnPaginatedList()
    {
        const int amount = 5;
        var getLoginHistoryRequest = new GetLoginHistoryRequest(1, 30);
        for (var i = 0; i < amount; i++)
        {
            var loginRequest = new LoginRequest(_registerResponse.Username, _registerResponse.Password);
            await AnonymousClient.POSTAsync<LoginEndpoint, LoginRequest, AuthenticationResponse>(loginRequest);
        }

        var (response, result) =
            await _authenticatedClient
                .GETAsync<GetLoginHistoryEndpoint, GetLoginHistoryRequest, PaginatedList<GetLoginHistoryResponse>>(
                    getLoginHistoryRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Items.Should().HaveCount(amount);
    }

    [Fact]
    public async Task EmptyHistory_ShouldReturnEmptyPaginatedList()
    {
        var getLoginHistoryRequest = new GetLoginHistoryRequest(1, 30);

        var (response, result) =
            await _authenticatedClient
                .GETAsync<GetLoginHistoryEndpoint, GetLoginHistoryRequest, PaginatedList<GetLoginHistoryResponse>>(
                    getLoginHistoryRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task SuccessfulLogin_ShouldAddCorrectLoginAttempt()
    {
        var getLoginHistoryRequest = new GetLoginHistoryRequest(1, 30);
        var loginRequest = new LoginRequest(_registerResponse.Username, _registerResponse.Password);
        await AnonymousClient.POSTAsync<LoginEndpoint, LoginRequest, AuthenticationResponse>(loginRequest);

        var (_, result) =
            await _authenticatedClient
                .GETAsync<GetLoginHistoryEndpoint, GetLoginHistoryRequest, PaginatedList<GetLoginHistoryResponse>>(
                    getLoginHistoryRequest);

        result.TotalCount.Should().Be(1);
        result.Items.First().Correct.Should().BeTrue();
    }

    [Fact]
    public async Task UnsuccessfulLogin_ShouldAddIncorrectLoginAttempt()
    {
        var getLoginHistoryRequest = new GetLoginHistoryRequest(1, 30);
        var loginRequest = new LoginRequest(_registerResponse.Username, "qwerty123");
        await AnonymousClient.POSTAsync<LoginEndpoint, LoginRequest, AuthenticationResponse>(loginRequest);

        var (_, result) =
            await _authenticatedClient
                .GETAsync<GetLoginHistoryEndpoint, GetLoginHistoryRequest, PaginatedList<GetLoginHistoryResponse>>(
                    getLoginHistoryRequest);

        result.TotalCount.Should().Be(1);
        result.Items.First().Correct.Should().BeFalse();
    }
}