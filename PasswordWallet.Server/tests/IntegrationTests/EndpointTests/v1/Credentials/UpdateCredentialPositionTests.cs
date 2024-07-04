using System.Net;
using Api.Endpoints.v1.Credentials;
using Api.Endpoints.v1.Credentials.GetCredentials;
using Api.Endpoints.v1.Credentials.UpdateCredentialPosition;
using Core.Models;
using FastEndpoints;
using FluentAssertions;
using IntegrationTests.AppFixtures;
using IntegrationTests.BaseIntegrationTests;
using IntegrationTests.CollectionDefinitions;
using IntegrationTests.Extensions;

namespace IntegrationTests.EndpointTests.v1.Credentials;

[Collection(CredentialEndpointCollection.CollectionName)]
public class UpdateCredentialPositionTests(CredentialEndpointAppFixture appFixture) : BaseIntegrationTest(appFixture)
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
        const int newPosition = 2;
        var updateCredentialPositionRequest = new UpdateCredentialPositionRequest(_addedCredentialId, newPosition);

        var response =
            await AnonymousClient.PATCHAsync<UpdateCredentialPositionEndpoint, UpdateCredentialPositionRequest>(
                updateCredentialPositionRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ValidRequest_ShouldUpdateCredentialPosition()
    {
        const int newPosition = 4;
        const int amount = 5;
        for (var i = 0; i < amount; i++)
        {
            await _authenticatedClient.AddCredential(_folderId);
        }

        var getCredentialsRequest = new GetCredentialsRequest(_folderId);
        var updateCredentialPositionRequest = new UpdateCredentialPositionRequest(_addedCredentialId, newPosition);

        var response =
            await _authenticatedClient.PATCHAsync<UpdateCredentialPositionEndpoint, UpdateCredentialPositionRequest>(
                updateCredentialPositionRequest);
        var (_, result) =
            await _authenticatedClient
                .GETAsync<GetCredentialsEndpoint, GetCredentialsRequest, PaginatedList<CredentialResponse>>(
                    getCredentialsRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        result.Items.Single(x => x.Id == _addedCredentialId).Position.Should().Be(newPosition);
    }

    [Fact]
    public async Task NotExistingCredential_ShouldReturnNotFound()
    {
        const int newPosition = 3;
        const long notExistingCredentialId = 8;
        var updateCredentialPositionRequest = new UpdateCredentialPositionRequest(notExistingCredentialId, newPosition);

        var response =
            await _authenticatedClient.PATCHAsync<UpdateCredentialPositionEndpoint, UpdateCredentialPositionRequest>(
                updateCredentialPositionRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UserDoesntOwnCredential_ShouldReturnForbidden()
    {
        const int newPosition = 5;
        var otherUser = await AnonymousClient.RegisterUser();
        using var otherUserClient = CreateAuthenticatedClient(otherUser.AccessToken);
        var updateCredentialPositionRequest = new UpdateCredentialPositionRequest(_addedCredentialId, newPosition);

        var response =
            await otherUserClient.PATCHAsync<UpdateCredentialPositionEndpoint, UpdateCredentialPositionRequest>(
                updateCredentialPositionRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}