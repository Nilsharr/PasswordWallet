using System.Net;
using Api.Endpoints.v1.User.UsernameAvailability;
using FastEndpoints;
using FluentAssertions;
using IntegrationTests.AppFixtures;
using IntegrationTests.BaseIntegrationTests;
using IntegrationTests.CollectionDefinitions;
using IntegrationTests.Extensions;
using IntegrationTests.Fakes;

namespace IntegrationTests.EndpointTests.v1.User;

[Collection(UserEndpointCollection.CollectionName)]
public class UsernameAvailabilityEndpointTests(UserEndpointAppFixture appFixture) : BaseIntegrationTest(appFixture)
{
    [Fact]
    public async Task UsernameDoesntExists_UsernameShouldBeAvailable()
    {
        const string username = "TestUser";
        var usernameAvailabilityRequest = new UsernameAvailabilityRequest(username);

        var (response, result) =
            await AnonymousClient
                .GETAsync<UsernameAvailabilityEndpoint, UsernameAvailabilityRequest, UsernameAvailabilityResponse>(
                    usernameAvailabilityRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.IsAvailable.Should().BeTrue();
    }

    [Fact]
    public async Task UsernameExists_UsernameShouldBeUnavailable()
    {
        var registerRequest = new RegisterRequestFaker().Generate();
        await AnonymousClient.RegisterUser(registerRequest);
        var usernameAvailabilityRequest = new UsernameAvailabilityRequest(registerRequest.Username);

        var (response, result) =
            await AnonymousClient
                .GETAsync<UsernameAvailabilityEndpoint, UsernameAvailabilityRequest, UsernameAvailabilityResponse>(
                    usernameAvailabilityRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.IsAvailable.Should().BeFalse();
    }
}