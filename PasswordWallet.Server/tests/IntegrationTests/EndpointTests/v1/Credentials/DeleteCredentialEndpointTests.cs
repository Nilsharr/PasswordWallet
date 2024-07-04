using System.Net;
using Api.Endpoints.v1.Credentials.DeleteCredential;
using FastEndpoints;
using FluentAssertions;
using IntegrationTests.AppFixtures;
using IntegrationTests.BaseIntegrationTests;
using IntegrationTests.CollectionDefinitions;
using IntegrationTests.Extensions;

namespace IntegrationTests.EndpointTests.v1.Credentials;

[Collection(CredentialEndpointCollection.CollectionName)]
public class DeleteCredentialEndpointTests(CredentialEndpointAppFixture appFixture) : BaseIntegrationTest(appFixture)
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
        var deleteCredentialRequest = new DeleteCredentialRequest(_addedCredentialId);

        var response =
            await AnonymousClient.DELETEAsync<DeleteCredentialEndpoint, DeleteCredentialRequest>(
                deleteCredentialRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ExistingCredential_ShouldDeleteFolder()
    {
        var deleteCredentialRequest = new DeleteCredentialRequest(_addedCredentialId);

        var response =
            await _authenticatedClient.DELETEAsync<DeleteCredentialEndpoint, DeleteCredentialRequest>(
                deleteCredentialRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task NotExistingCredential_ShouldReturnNotFound()
    {
        const long notExistingCredentialId = 10;
        var deleteCredentialRequest = new DeleteCredentialRequest(notExistingCredentialId);

        var response =
            await _authenticatedClient.DELETEAsync<DeleteCredentialEndpoint, DeleteCredentialRequest>(
                deleteCredentialRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UserDoesntOwnCredential_ShouldReturnForbidden()
    {
        var otherUser = await AnonymousClient.RegisterUser();
        using var otherUserClient = CreateAuthenticatedClient(otherUser.AccessToken);
        var deleteCredentialRequest = new DeleteCredentialRequest(_addedCredentialId);

        var response =
            await otherUserClient.DELETEAsync<DeleteCredentialEndpoint, DeleteCredentialRequest>(
                deleteCredentialRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}