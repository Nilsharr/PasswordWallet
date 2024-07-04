using System.Net;
using Api.Endpoints.v1.Folders;
using Api.Endpoints.v1.Folders.AddFolder;
using FastEndpoints;
using FluentAssertions;
using IntegrationTests.AppFixtures;
using IntegrationTests.BaseIntegrationTests;
using IntegrationTests.CollectionDefinitions;
using IntegrationTests.Extensions;
using IntegrationTests.Fakes;

namespace IntegrationTests.EndpointTests.v1.Folders;

[Collection(FolderEndpointCollection.CollectionName)]
public class AddFolderEndpointTests(FolderEndpointAppFixture appFixture) : BaseIntegrationTest(appFixture)
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
        var addFolderRequest = new AddFolderRequestFaker().Generate();

        var response = await AnonymousClient.POSTAsync<AddFolderEndpoint, AddFolderRequest>(addFolderRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ValidRequest_ShouldAddFolder()
    {
        var addFolderRequest = new AddFolderRequestFaker().Generate();

        var (response, result) =
            await _authenticatedClient.POSTAsync<AddFolderEndpoint, AddFolderRequest, FolderResponse>(addFolderRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be(addFolderRequest.Name);
        result.Position.Should().BePositive();
    }

    [Fact]
    public async Task EmptyRequest_ShouldReturnBadRequest()
    {
        var addFolderRequest = new AddFolderRequest("");

        var response = await _authenticatedClient.POSTAsync<AddFolderEndpoint, AddFolderRequest>(addFolderRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}