using System.Net;
using Api.Endpoints.v1.Credentials;
using Api.Endpoints.v1.Credentials.GetCredentials;
using Core.Models;
using FastEndpoints;
using FluentAssertions;
using IntegrationTests.AppFixtures;
using IntegrationTests.BaseIntegrationTests;
using IntegrationTests.CollectionDefinitions;
using IntegrationTests.Extensions;

namespace IntegrationTests.EndpointTests.v1.Credentials;

[Collection(CredentialEndpointCollection.CollectionName)]
public class GetCredentialsEndpointTests(CredentialEndpointAppFixture appFixture) : BaseIntegrationTest(appFixture)
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
        var getCredentialsRequest = new GetCredentialsRequest(_folderId, 1, 30);

        var response =
            await AnonymousClient.GETAsync<GetCredentialsEndpoint, GetCredentialsRequest>(getCredentialsRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task HasCredentials_ShouldReturnPaginatedList()
    {
        const int amount = 5;
        var getCredentialsRequest = new GetCredentialsRequest(_folderId, 1, 30);
        for (var i = 0; i < amount; i++)
        {
            await _authenticatedClient.AddCredential(_folderId);
        }

        var (response, result) =
            await _authenticatedClient
                .GETAsync<GetCredentialsEndpoint, GetCredentialsRequest, PaginatedList<CredentialResponse>>(
                    getCredentialsRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Items.Should().HaveCount(amount);
    }

    [Fact]
    public async Task EmptyCredentials_ShouldReturnEmptyPaginatedList()
    {
        var getCredentialsRequest = new GetCredentialsRequest(_folderId, 1, 30);

        var (response, result) =
            await _authenticatedClient
                .GETAsync<GetCredentialsEndpoint, GetCredentialsRequest, PaginatedList<CredentialResponse>>(
                    getCredentialsRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task NotExistingFolderWithCredentials_ShouldReturnNotFound()
    {
        var notExistingFolderId = Guid.NewGuid();
        var getCredentialsRequest = new GetCredentialsRequest(notExistingFolderId, 1, 30);

        var response =
            await _authenticatedClient.GETAsync<GetCredentialsEndpoint, GetCredentialsRequest>(getCredentialsRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task NotOwnedFolderWithCredentials_ShouldReturnForbidden()
    {
        var otherUser = await AnonymousClient.RegisterUser();
        using var otherUserClient = CreateAuthenticatedClient(otherUser.AccessToken);
        var getCredentialsRequest = new GetCredentialsRequest(_folderId, 1, 30);

        var response =
            await otherUserClient.GETAsync<GetCredentialsEndpoint, GetCredentialsRequest>(getCredentialsRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}