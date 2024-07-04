using System.Net;
using Api.Endpoints.v1.Credentials;
using Api.Endpoints.v1.Credentials.GetCredentials;
using Api.Endpoints.v1.Credentials.MoveCredential;
using Core.Models;
using FastEndpoints;
using FluentAssertions;
using IntegrationTests.AppFixtures;
using IntegrationTests.BaseIntegrationTests;
using IntegrationTests.CollectionDefinitions;
using IntegrationTests.Extensions;

namespace IntegrationTests.EndpointTests.v1.Credentials;

[Collection(CredentialEndpointCollection.CollectionName)]
public class MoveCredentialEndpointTests(CredentialEndpointAppFixture appFixture) : BaseIntegrationTest(appFixture)
{
    private HttpClient _authenticatedClient = default!;
    private Guid _folderId;
    private Guid _otherFolderId;
    private long _addedCredentialId;

    public override async Task InitializeAsync()
    {
        var registerResponse = await AnonymousClient.RegisterUser();
        _authenticatedClient = CreateAuthenticatedClient(registerResponse.AccessToken);
        _folderId = (await _authenticatedClient.AddFolder()).Id;
        _addedCredentialId = (await _authenticatedClient.AddCredential(_folderId)).Id;
        _otherFolderId = (await _authenticatedClient.AddFolder()).Id;
    }

    public override async Task DisposeAsync()
    {
        await base.DisposeAsync();
        _authenticatedClient.Dispose();
    }

    [Fact]
    public async Task UnauthenticatedUser_ShouldReturnUnauthorized()
    {
        var moveCredentialRequest = new MoveCredentialRequest(_addedCredentialId, _otherFolderId);

        var response =
            await AnonymousClient.PATCHAsync<MoveCredentialEndpoint, MoveCredentialRequest>(moveCredentialRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ValidRequest_ShouldMoveCredentialToAnotherFolder()
    {
        var moveCredentialRequest = new MoveCredentialRequest(_addedCredentialId, _otherFolderId);
        var getCredentialsRequest = new GetCredentialsRequest(_otherFolderId);

        var response =
            await _authenticatedClient.PATCHAsync<MoveCredentialEndpoint, MoveCredentialRequest>(moveCredentialRequest);
        var (_, result) =
            await _authenticatedClient
                .GETAsync<GetCredentialsEndpoint, GetCredentialsRequest, PaginatedList<CredentialResponse>>(
                    getCredentialsRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        result.Items.Should().Contain(x => x.Id == _addedCredentialId);
        result.Items.Max(x => x.Position).Should().Be(result.Items.Single(x => x.Id == _addedCredentialId).Position);
    }

    [Fact]
    public async Task NotExistingCredential_ShouldReturnNotFound()
    {
        const long notExistingCredentialId = 12;
        var moveCredentialRequest = new MoveCredentialRequest(notExistingCredentialId, _otherFolderId);

        var response =
            await _authenticatedClient.PATCHAsync<MoveCredentialEndpoint, MoveCredentialRequest>(moveCredentialRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UserDoesntOwnCredential_ShouldReturnForbidden()
    {
        var otherUser = await AnonymousClient.RegisterUser();
        using var otherUserClient = CreateAuthenticatedClient(otherUser.AccessToken);
        var moveCredentialRequest = new MoveCredentialRequest(_addedCredentialId, _otherFolderId);

        var response =
            await otherUserClient.PATCHAsync<MoveCredentialEndpoint, MoveCredentialRequest>(moveCredentialRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task NotExistingFolderToMove_ShouldReturnNotFound()
    {
        var notExistingFolderId = Guid.NewGuid();
        var moveCredentialRequest = new MoveCredentialRequest(_addedCredentialId, notExistingFolderId);

        var response =
            await _authenticatedClient.PATCHAsync<MoveCredentialEndpoint, MoveCredentialRequest>(moveCredentialRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task NotOwnedFolderToMove_ShouldReturnForbidden()
    {
        var otherUser = await AnonymousClient.RegisterUser();
        using var otherUserClient = CreateAuthenticatedClient(otherUser.AccessToken);
        var otherUserFolder = await otherUserClient.AddFolder();
        var moveCredentialRequest = new MoveCredentialRequest(_addedCredentialId, otherUserFolder.Id);

        var response =
            await _authenticatedClient.PATCHAsync<MoveCredentialEndpoint, MoveCredentialRequest>(moveCredentialRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}