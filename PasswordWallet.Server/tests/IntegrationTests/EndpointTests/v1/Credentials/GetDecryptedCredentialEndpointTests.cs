using System.Net;
using Api.Endpoints.v1.Credentials.AddCredential;
using Api.Endpoints.v1.Credentials.GetDecryptedCredential;
using FastEndpoints;
using FluentAssertions;
using IntegrationTests.AppFixtures;
using IntegrationTests.BaseIntegrationTests;
using IntegrationTests.CollectionDefinitions;
using IntegrationTests.Extensions;
using IntegrationTests.Fakes;

namespace IntegrationTests.EndpointTests.v1.Credentials;

[Collection(CredentialEndpointCollection.CollectionName)]
public class GetDecryptedCredentialEndpointTests(CredentialEndpointAppFixture appFixture)
    : BaseIntegrationTest(appFixture)
{
    private HttpClient _authenticatedClient = default!;
    private Guid _folderId;
    private long _addedCredentialId;
    private string _addedCredentialPassword = default!;

    public override async Task InitializeAsync()
    {
        var registerResponse = await AnonymousClient.RegisterUser();
        _authenticatedClient = CreateAuthenticatedClient(registerResponse.AccessToken);
        _folderId = (await _authenticatedClient.AddFolder()).Id;
        var addCredentialRequest = new AddCredentialRequestFaker(_folderId).Generate();
        _addedCredentialPassword = addCredentialRequest.Password!;
        _addedCredentialId = (await _authenticatedClient.AddCredential(addCredentialRequest)).Id;
    }

    public override async Task DisposeAsync()
    {
        await base.DisposeAsync();
        _authenticatedClient.Dispose();
    }

    [Fact]
    public async Task UnauthenticatedUser_ShouldReturnUnauthorized()
    {
        var getDecryptedCredentialRequest = new GetDecryptedCredentialRequest(_addedCredentialId);

        var response =
            await AnonymousClient.GETAsync<GetDecryptedCredentialEndpoint, GetDecryptedCredentialRequest>(
                getDecryptedCredentialRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CredentialWithNotEmptyPassword_ShouldReturnDecryptedPassword()
    {
        var getDecryptedCredentialRequest = new GetDecryptedCredentialRequest(_addedCredentialId);

        var (response, result) =
            await _authenticatedClient
                .GETAsync<GetDecryptedCredentialEndpoint, GetDecryptedCredentialRequest,
                    GetDecryptedCredentialResponse>(getDecryptedCredentialRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Password.Should().Be(_addedCredentialPassword);
    }

    [Fact]
    public async Task CredentialWithEmptyPassword_ShouldReturnEmptyPassword()
    {
        var emptyCredential = await _authenticatedClient.AddCredential(new AddCredentialRequest(_folderId));
        var getDecryptedCredentialRequest = new GetDecryptedCredentialRequest(emptyCredential.Id);

        var (response, result) =
            await _authenticatedClient
                .GETAsync<GetDecryptedCredentialEndpoint, GetDecryptedCredentialRequest,
                    GetDecryptedCredentialResponse>(getDecryptedCredentialRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Password.Should().BeNull();
    }

    [Fact]
    public async Task NotExistingCredential_ShouldReturnNotFound()
    {
        const long notExistingCredentialId = 12;
        var getDecryptedCredentialRequest = new GetDecryptedCredentialRequest(notExistingCredentialId);

        var response =
            await _authenticatedClient.GETAsync<GetDecryptedCredentialEndpoint, GetDecryptedCredentialRequest>(
                getDecryptedCredentialRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UserDoesntOwnCredential_ShouldReturnForbidden()
    {
        var otherUser = await AnonymousClient.RegisterUser();
        using var otherUserClient = CreateAuthenticatedClient(otherUser.AccessToken);
        var getDecryptedCredentialRequest = new GetDecryptedCredentialRequest(_addedCredentialId);

        var response =
            await otherUserClient.GETAsync<GetDecryptedCredentialEndpoint, GetDecryptedCredentialRequest>(
                getDecryptedCredentialRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}