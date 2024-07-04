using System.Net;
using Api.Endpoints.v1.Credentials;
using Api.Endpoints.v1.Credentials.UpdateCredential;
using FastEndpoints;
using FluentAssertions;
using IntegrationTests.AppFixtures;
using IntegrationTests.BaseIntegrationTests;
using IntegrationTests.CollectionDefinitions;
using IntegrationTests.Extensions;
using IntegrationTests.Fakes;

namespace IntegrationTests.EndpointTests.v1.Credentials;

[Collection(CredentialEndpointCollection.CollectionName)]
public class UpdateCredentialEndpointTests(CredentialEndpointAppFixture appFixture) : BaseIntegrationTest(appFixture)
{
    private HttpClient _authenticatedClient = default!;
    private Guid _folderId;
    private long _addedCredentialId;

    public override async Task InitializeAsync()
    {
        var registerResponse = await AnonymousClient.RegisterUser();
        _authenticatedClient = CreateAuthenticatedClient(registerResponse.AccessToken);
        _folderId = (await _authenticatedClient.AddFolder()).Id;
        _addedCredentialId = (await _authenticatedClient.AddCredential(_folderId)).Id;
    }

    public override async Task DisposeAsync()
    {
        await base.DisposeAsync();
        _authenticatedClient.Dispose();
    }

    [Fact]
    public async Task UnauthenticatedUser_ShouldReturnUnauthorized()
    {
        var updateCredentialRequest = new UpdateCredentialRequestFaker(_addedCredentialId).Generate();

        var response =
            await AnonymousClient.PUTAsync<UpdateCredentialEndpoint, UpdateCredentialRequest>(updateCredentialRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ValidRequest_ShouldUpdateCredential()
    {
        var updateCredentialRequest = new UpdateCredentialRequestFaker(_addedCredentialId).Generate();

        var (response, result) =
            await _authenticatedClient.PUTAsync<UpdateCredentialEndpoint, UpdateCredentialRequest, CredentialResponse>(
                updateCredentialRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Id.Should().BePositive();
        result.Username.Should().Be(updateCredentialRequest.Username);
        result.WebAddress.Should().Be(updateCredentialRequest.WebAddress);
        result.Description.Should().Be(updateCredentialRequest.Description);
    }

    [Fact]
    public async Task EmptyRequest_ShouldUpdateCredential()
    {
        var updateCredentialRequest = new UpdateCredentialRequest(_addedCredentialId);

        var (response, result) =
            await _authenticatedClient.PUTAsync<UpdateCredentialEndpoint, UpdateCredentialRequest, CredentialResponse>(
                updateCredentialRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Id.Should().BePositive();
        result.Username.Should().Be(updateCredentialRequest.Username);
        result.WebAddress.Should().Be(updateCredentialRequest.WebAddress);
        result.Description.Should().Be(updateCredentialRequest.Description);
    }

    [Fact]
    public async Task NotExistingCredential_ShouldReturnNotFound()
    {
        const long notExistingId = 8;
        var updateCredentialRequest = new UpdateCredentialRequestFaker(notExistingId).Generate();

        var response =
            await _authenticatedClient.PUTAsync<UpdateCredentialEndpoint, UpdateCredentialRequest>(
                updateCredentialRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UserDoesntOwnCredential_ShouldReturnForbidden()
    {
        var otherUser = await AnonymousClient.RegisterUser();
        using var otherUserClient = CreateAuthenticatedClient(otherUser.AccessToken);
        var updateCredentialRequest = new UpdateCredentialRequestFaker(_addedCredentialId).Generate();

        var response =
            await otherUserClient.PUTAsync<UpdateCredentialEndpoint, UpdateCredentialRequest>(updateCredentialRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}