using System.Net;
using Api.Endpoints.v1.User.RefreshToken;
using FastEndpoints;
using FluentAssertions;
using IntegrationTests.AppFixtures;
using IntegrationTests.BaseIntegrationTests;
using IntegrationTests.CollectionDefinitions;
using IntegrationTests.Extensions;

namespace IntegrationTests.EndpointTests.v1.User;

[Collection(UserEndpointCollection.CollectionName)]
public class RefreshTokenEndpointTests(UserEndpointAppFixture appFixture)
    : BaseIntegrationTest(appFixture)
{
    private string _accessToken = default!;
    private string _refreshToken = default!;
    private HttpClient _authenticatedClient = default!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        var registerResponse = await AnonymousClient.RegisterUser();
        _accessToken = registerResponse.AccessToken;
        _refreshToken = registerResponse.RefreshToken;
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
        var request = new RefreshTokenRequest("ewfsdf", "wefsdf");

        var response = await AnonymousClient.POSTAsync<RefreshTokenEndpoint, RefreshTokenRequest>(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ValidRequest_ShouldReturnNewAccessToken()
    {
        var request = new RefreshTokenRequest(_accessToken, _refreshToken);

        var (response, result) =
            await _authenticatedClient
                .POSTAsync<RefreshTokenEndpoint, RefreshTokenRequest, RefreshTokenResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.AccessToken.Should().NotBeNullOrWhiteSpace();
        result.AccessTokenExpiry.Should().BeAfter(DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task EmptyRequest_ShouldReturnBadRequest()
    {
        var request = new RefreshTokenRequest("", "");

        var response = await _authenticatedClient.POSTAsync<RefreshTokenEndpoint, RefreshTokenRequest>(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvalidAccessToken_ShouldReturnUnauthorized()
    {
        var request = new RefreshTokenRequest("asadwdw", _refreshToken);

        var response = await _authenticatedClient.POSTAsync<RefreshTokenEndpoint, RefreshTokenRequest>(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task InvalidRefreshToken_ShouldReturnUnauthorized()
    {
        var request = new RefreshTokenRequest(_accessToken, "sdfewfsf");

        var response = await _authenticatedClient.POSTAsync<RefreshTokenEndpoint, RefreshTokenRequest>(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}