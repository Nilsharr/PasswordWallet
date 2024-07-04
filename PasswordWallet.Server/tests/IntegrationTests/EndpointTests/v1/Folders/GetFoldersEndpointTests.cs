using System.Net;
using Api.Endpoints.v1.Folders;
using Api.Endpoints.v1.Folders.DeleteFolder;
using Api.Endpoints.v1.Folders.GetFolders;
using Core.Constants;
using FastEndpoints;
using FluentAssertions;
using IntegrationTests.AppFixtures;
using IntegrationTests.BaseIntegrationTests;
using IntegrationTests.CollectionDefinitions;
using IntegrationTests.Extensions;

namespace IntegrationTests.EndpointTests.v1.Folders;

[Collection(FolderEndpointCollection.CollectionName)]
public class GetFoldersEndpointTests(FolderEndpointAppFixture appFixture) : BaseIntegrationTest(appFixture)
{
    private HttpClient _authenticatedClient = default!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        var registerResponse = await AnonymousClient.RegisterUser();
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
        var (response, _) = await AnonymousClient.GETAsync<GetFoldersEndpoint, EmptyRequest>();

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task NewUser_ShouldHaveDefaultFolder()
    {
        var (response, result) = await _authenticatedClient.GETAsync<GetFoldersEndpoint, IList<FolderResponse>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().HaveCount(1);
        result[0].Name.Should().Be(FolderConstants.DefaultFolderName);
        result[0].Position.Should().Be(1);
    }

    [Fact]
    public async Task HasFolders_ShouldReturnFolderList()
    {
        const int amount = 3;
        await RemoveDefaultFolder();
        for (var i = 0; i < amount; i++)
        {
            await _authenticatedClient.AddFolder();
        }

        var (response, result) = await _authenticatedClient.GETAsync<GetFoldersEndpoint, IList<FolderResponse>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().HaveCount(amount);
    }

    [Fact]
    public async Task EmptyFolders_ShouldReturnEmptyList()
    {
        await RemoveDefaultFolder();

        var (response, result) = await _authenticatedClient.GETAsync<GetFoldersEndpoint, IList<FolderResponse>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().BeEmpty();
    }

    private async Task RemoveDefaultFolder()
    {
        var (_, result) = await _authenticatedClient.GETAsync<GetFoldersEndpoint, IList<FolderResponse>>();

        var deleteFolderRequest = new DeleteFolderRequest(result.Single().Id);
        await _authenticatedClient.DELETEAsync<DeleteFolderEndpoint, DeleteFolderRequest>(deleteFolderRequest);
    }
}