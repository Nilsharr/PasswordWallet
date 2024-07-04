using System.Net;
using Api.Endpoints.v1.Folders;
using Api.Endpoints.v1.Folders.GetFolder;
using FastEndpoints;
using FluentAssertions;
using IntegrationTests.AppFixtures;
using IntegrationTests.BaseIntegrationTests;
using IntegrationTests.CollectionDefinitions;
using IntegrationTests.Extensions;

namespace IntegrationTests.EndpointTests.v1.Folders;

[Collection(FolderEndpointCollection.CollectionName)]
public class GetFolderEndpointTests(FolderEndpointAppFixture appFixture) : BaseIntegrationTest(appFixture)
{
    private HttpClient _authenticatedClient = default!;
    private FolderResponse _addedFolderResponse = default!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        var registerResponse = await AnonymousClient.RegisterUser();
        _authenticatedClient = CreateAuthenticatedClient(registerResponse.AccessToken);
        _addedFolderResponse = await _authenticatedClient.AddFolder();
    }

    public override async Task DisposeAsync()
    {
        await base.DisposeAsync();
        _authenticatedClient.Dispose();
    }

    [Fact]
    public async Task UnauthenticatedUser_ShouldReturnUnauthorized()
    {
        var getFolderRequest = new GetFolderRequest(_addedFolderResponse.Id);

        var response = await AnonymousClient.GETAsync<GetFolderEndpoint, GetFolderRequest>(getFolderRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ExistingFolder_ShouldReturnFolder()
    {
        var getFolderRequest = new GetFolderRequest(_addedFolderResponse.Id);

        var (response, result) =
            await _authenticatedClient.GETAsync<GetFolderEndpoint, GetFolderRequest, FolderResponse>(
                getFolderRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Id.Should().Be(_addedFolderResponse.Id);
        result.Name.Should().Be(_addedFolderResponse.Name);
        result.Position.Should().BePositive();
    }

    [Fact]
    public async Task NotExistingFolder_ShouldReturnNotFound()
    {
        var notExistingFolderId = Guid.NewGuid();
        var getFolderRequest = new GetFolderRequest(notExistingFolderId);

        var response = await _authenticatedClient.GETAsync<GetFolderEndpoint, GetFolderRequest>(getFolderRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UserDoesntOwnFolder_ShouldReturnForbidden()
    {
        var otherUser = await AnonymousClient.RegisterUser();
        using var otherUserClient = CreateAuthenticatedClient(otherUser.AccessToken);
        var getFolderRequest = new GetFolderRequest(_addedFolderResponse.Id);

        var response = await otherUserClient.GETAsync<GetFolderEndpoint, GetFolderRequest>(getFolderRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}