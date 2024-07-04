using System.Net;
using Api.Endpoints.v1.Credentials;
using Api.Endpoints.v1.Credentials.AddCredential;
using FastEndpoints;
using FluentAssertions;
using IntegrationTests.AppFixtures;
using IntegrationTests.BaseIntegrationTests;
using IntegrationTests.CollectionDefinitions;
using IntegrationTests.Extensions;
using IntegrationTests.Fakes;

namespace IntegrationTests.EndpointTests.v1.Credentials;

[Collection(CredentialEndpointCollection.CollectionName)]
public class AddCredentialEndpointTests(CredentialEndpointAppFixture appFixture) : BaseIntegrationTest(appFixture)
{
    private HttpClient _authenticatedClient = default!;
    private Guid _folderId;

    public override async Task InitializeAsync()
    {
        var registerResponse = await AnonymousClient.RegisterUser();
        _authenticatedClient = CreateAuthenticatedClient(registerResponse.AccessToken);
        _folderId = (await _authenticatedClient.AddFolder()).Id;
    }

    public override async Task DisposeAsync()
    {
        await base.DisposeAsync();
        _authenticatedClient.Dispose();
    }

    [Fact]
    public async Task UnauthenticatedUser_ShouldReturnUnauthorized()
    {
        var addCredentialRequest = new AddCredentialRequestFaker(_folderId).Generate();

        var response =
            await AnonymousClient.POSTAsync<AddCredentialEndpoint, AddCredentialRequest>(addCredentialRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ValidRequest_ShouldAddCredential()
    {
        var addCredentialRequest = new AddCredentialRequestFaker(_folderId).Generate();

        var (response, result) =
            await _authenticatedClient.POSTAsync<AddCredentialEndpoint, AddCredentialRequest, CredentialResponse>(
                addCredentialRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Id.Should().BePositive();
        result.Username.Should().Be(addCredentialRequest.Username);
        result.WebAddress.Should().Be(addCredentialRequest.WebAddress);
        result.Description.Should().Be(addCredentialRequest.Description);
        result.Position.Should().BePositive();
    }

    [Fact]
    public async Task EmptyRequest_ShouldAddCredential()
    {
        var addCredentialRequest = new AddCredentialRequest(_folderId);

        var (response, result) =
            await _authenticatedClient.POSTAsync<AddCredentialEndpoint, AddCredentialRequest, CredentialResponse>(
                addCredentialRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Id.Should().BePositive();
        result.Username.Should().Be(addCredentialRequest.Username);
        result.WebAddress.Should().Be(addCredentialRequest.WebAddress);
        result.Description.Should().Be(addCredentialRequest.Description);
        result.Position.Should().BePositive();
    }

    [Fact]
    public async Task NotExistingFolderForCredential_ShouldReturnNotFound()
    {
        var notExistingFolderId = Guid.NewGuid();
        var addCredentialRequest = new AddCredentialRequest(notExistingFolderId);

        var response =
            await _authenticatedClient.POSTAsync<AddCredentialEndpoint, AddCredentialRequest>(addCredentialRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task NotOwnedFolderForCredential_ShouldReturnForbidden()
    {
        var otherUser = await AnonymousClient.RegisterUser();
        using var otherUserClient = CreateAuthenticatedClient(otherUser.AccessToken);
        var addCredentialRequest = new AddCredentialRequest(_folderId);

        var response =
            await otherUserClient.POSTAsync<AddCredentialEndpoint, AddCredentialRequest>(addCredentialRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}