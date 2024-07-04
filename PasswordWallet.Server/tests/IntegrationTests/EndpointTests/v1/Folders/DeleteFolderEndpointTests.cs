using System.Net;
using Api.Endpoints.v1.Folders.DeleteFolder;
using FastEndpoints;
using FluentAssertions;
using IntegrationTests.AppFixtures;
using IntegrationTests.BaseIntegrationTests;
using IntegrationTests.CollectionDefinitions;
using IntegrationTests.Extensions;

namespace IntegrationTests.EndpointTests.v1.Folders;

[Collection(FolderEndpointCollection.CollectionName)]
public class DeleteFolderEndpointTests(FolderEndpointAppFixture appFixture) : BaseIntegrationTest(appFixture)
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
        var deleteFolderRequest = new DeleteFolderRequest(_addedFolderId);

        var response =
            await AnonymousClient.DELETEAsync<DeleteFolderEndpoint, DeleteFolderRequest>(deleteFolderRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ExistingFolder_ShouldDeleteFolder()
    {
        var deleteFolderRequest = new DeleteFolderRequest(_addedFolderId);

        var response =
            await _authenticatedClient.DELETEAsync<DeleteFolderEndpoint, DeleteFolderRequest>(deleteFolderRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task NotExistingFolder_ShouldReturnNotFound()
    {
        var notExistingFolderId = Guid.NewGuid();
        var deleteFolderRequest = new DeleteFolderRequest(notExistingFolderId);

        var response =
            await _authenticatedClient.DELETEAsync<DeleteFolderEndpoint, DeleteFolderRequest>(deleteFolderRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UserDoesntOwnFolder_ShouldReturnForbidden()
    {
        var otherUser = await AnonymousClient.RegisterUser();
        using var otherUserClient = CreateAuthenticatedClient(otherUser.AccessToken);
        var deleteFolderRequest = new DeleteFolderRequest(_addedFolderId);

        var response =
            await otherUserClient.DELETEAsync<DeleteFolderEndpoint, DeleteFolderRequest>(deleteFolderRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}