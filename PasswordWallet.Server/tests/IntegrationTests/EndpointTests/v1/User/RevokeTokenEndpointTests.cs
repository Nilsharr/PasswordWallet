using System.Net;
using Api.Endpoints.v1.User.RevokeToken;
using FastEndpoints;
using FluentAssertions;
using IntegrationTests.AppFixtures;
using IntegrationTests.BaseIntegrationTests;
using IntegrationTests.CollectionDefinitions;
using IntegrationTests.Extensions;

namespace IntegrationTests.EndpointTests.v1.User;

[Collection(UserEndpointCollection.CollectionName)]
public class RevokeTokenEndpointTests(UserEndpointAppFixture appFixture)
    : BaseIntegrationTest(appFixture)
{
    [Fact]
    public async Task UnauthenticatedUser_ShouldReturnUnauthorized()
    {
        var (response, _) = await AnonymousClient.DELETEAsync<RevokeTokenEndpoint, ErrorResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AuthenticatedUser_ShouldReturnNoContent()
    {
        var registerResponse = await AnonymousClient.RegisterUser();
        using var authenticatedClient = CreateAuthenticatedClient(registerResponse.AccessToken);

        var (response, _) = await authenticatedClient.DELETEAsync<RevokeTokenEndpoint, EmptyRequest>();

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}