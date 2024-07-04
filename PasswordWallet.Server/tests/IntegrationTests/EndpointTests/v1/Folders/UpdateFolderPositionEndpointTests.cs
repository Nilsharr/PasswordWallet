using System.Net;
using Api.Endpoints.v1.Folders;
using Api.Endpoints.v1.Folders.GetFolders;
using Api.Endpoints.v1.Folders.UpdateFolderPosition;
using FastEndpoints;
using FluentAssertions;
using IntegrationTests.AppFixtures;
using IntegrationTests.BaseIntegrationTests;
using IntegrationTests.CollectionDefinitions;
using IntegrationTests.Extensions;

namespace IntegrationTests.EndpointTests.v1.Folders;

[Collection(FolderEndpointCollection.CollectionName)]
public class UpdateFolderPositionEndpointTests(FolderEndpointAppFixture appFixture) : BaseIntegrationTest(appFixture)
{
    private HttpClient _authenticatedClient = default!;
    private Guid _addedFolderId;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        var registerResponse = await AnonymousClient.RegisterUser();
        _authenticatedClient = CreateAuthenticatedClient(registerResponse.AccessToken);

        _addedFolderId = (await _authenticatedClient.AddFolder()).Id;
    }

    public override async Task DisposeAsync()
    {
        await base.DisposeAsync();
        _authenticatedClient.Dispose();
    }

    [Fact]
    public async Task UnauthenticatedUser_ShouldReturnUnauthorized()
    {
        const int newPosition = 5;
        var updateFolderPositionRequest = new UpdateFolderPositionRequest(_addedFolderId, newPosition);

        var response =
            await AnonymousClient
                .PATCHAsync<UpdateFolderPositionEndpoint, UpdateFolderPositionRequest>(updateFolderPositionRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ValidRequest_ShouldUpdateFolderPosition()
    {
        const int newPosition = 6;
        const int amount = 8;
        for (var i = 0; i < amount; i++)
        {
            await _authenticatedClient.AddFolder();
        }

        var updateFolderPositionRequest = new UpdateFolderPositionRequest(_addedFolderId, newPosition);

        var response =
            await _authenticatedClient.PATCHAsync<UpdateFolderPositionEndpoint, UpdateFolderPositionRequest>(
                updateFolderPositionRequest);
        var (_, result) = await _authenticatedClient.GETAsync<GetFoldersEndpoint, IList<FolderResponse>>();

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        result.Single(x => x.Id == _addedFolderId).Position.Should().Be(newPosition);
    }

    [Fact]
    public async Task NotExistingFolder_ShouldReturnNotFound()
    {
        const int newPosition = 3;
        var notExistingFolderId = Guid.NewGuid();
        var updateFolderPositionRequest = new UpdateFolderPositionRequest(notExistingFolderId, newPosition);

        var response =
            await _authenticatedClient.PATCHAsync<UpdateFolderPositionEndpoint, UpdateFolderPositionRequest>(
                updateFolderPositionRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UserDoesntOwnFolder_ShouldReturnForbidden()
    {
        const int newPosition = 7;
        var otherUser = await AnonymousClient.RegisterUser();
        using var otherUserClient = CreateAuthenticatedClient(otherUser.AccessToken);
        var updateFolderPositionRequest = new UpdateFolderPositionRequest(_addedFolderId, newPosition);

        var response =
            await otherUserClient
                .PATCHAsync<UpdateFolderPositionEndpoint, UpdateFolderPositionRequest>(updateFolderPositionRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}