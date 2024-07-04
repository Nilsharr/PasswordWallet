using System.Net;
using Api.Endpoints.v1.Folders.UpdateFolderName;
using FastEndpoints;
using FluentAssertions;
using IntegrationTests.AppFixtures;
using IntegrationTests.BaseIntegrationTests;
using IntegrationTests.CollectionDefinitions;
using IntegrationTests.Extensions;
using IntegrationTests.Fakes;

namespace IntegrationTests.EndpointTests.v1.Folders;

[Collection(FolderEndpointCollection.CollectionName)]
public class UpdateFolderNameEndpointTests(FolderEndpointAppFixture appFixture) : BaseIntegrationTest(appFixture)
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
        var updateFolderNameRequest = new UpdateFolderNameRequestFaker(_addedFolderId).Generate();

        var response =
            await AnonymousClient
                .PATCHAsync<UpdateFolderNameEndpoint, UpdateFolderNameRequest>(updateFolderNameRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ValidRequest_ShouldUpdateFolderName()
    {
        var updateFolderNameRequest = new UpdateFolderNameRequestFaker(_addedFolderId).Generate();

        var response =
            await _authenticatedClient.PATCHAsync<UpdateFolderNameEndpoint, UpdateFolderNameRequest>(
                updateFolderNameRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task EmptyRequest_ShouldReturnBadRequest()
    {
        var updateFolderNameRequest = new UpdateFolderNameRequest(_addedFolderId, "");

        var response =
            await _authenticatedClient.PATCHAsync<UpdateFolderNameEndpoint, UpdateFolderNameRequest>(
                updateFolderNameRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task NotExistingFolder_ShouldReturnNotFound()
    {
        var notExistingFolderId = Guid.NewGuid();
        var updateFolderNameRequest = new UpdateFolderNameRequestFaker(notExistingFolderId).Generate();

        var response =
            await _authenticatedClient.PATCHAsync<UpdateFolderNameEndpoint, UpdateFolderNameRequest>(
                updateFolderNameRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UserDoesntOwnFolder_ShouldReturnForbidden()
    {
        var otherUser = await AnonymousClient.RegisterUser();
        using var otherUserClient = CreateAuthenticatedClient(otherUser.AccessToken);
        var updateFolderNameRequest = new UpdateFolderNameRequestFaker(_addedFolderId).Generate();

        var response =
            await otherUserClient
                .PATCHAsync<UpdateFolderNameEndpoint, UpdateFolderNameRequest>(updateFolderNameRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}