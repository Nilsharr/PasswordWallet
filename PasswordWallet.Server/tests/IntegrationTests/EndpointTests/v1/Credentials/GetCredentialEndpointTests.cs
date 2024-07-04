using System.Net;
using Api.Endpoints.v1.Credentials;
using Api.Endpoints.v1.Credentials.GetCredential;
using FastEndpoints;
using FluentAssertions;
using IntegrationTests.AppFixtures;
using IntegrationTests.BaseIntegrationTests;
using IntegrationTests.CollectionDefinitions;
using IntegrationTests.Extensions;

namespace IntegrationTests.EndpointTests.v1.Credentials;

[Collection(CredentialEndpointCollection.CollectionName)]
public class GetCredentialEndpointTests(CredentialEndpointAppFixture appFixture) : BaseIntegrationTest(appFixture)
{
    private HttpClient _authenticatedClient = default!;
    private Guid _folderId;
    private CredentialResponse _addedCredentialResponse = default!;

    public override async Task InitializeAsync()
    {
        var registerResponse = await AnonymousClient.RegisterUser();
        _authenticatedClient = CreateAuthenticatedClient(registerResponse.AccessToken);
        _folderId = (await _authenticatedClient.AddFolder()).Id;
        _addedCredentialResponse = await _authenticatedClient.AddCredential(_folderId);
    }

    public override async Task DisposeAsync()
    {
        await base.DisposeAsync();
        _authenticatedClient.Dispose();
    }

    [Fact]
    public async Task UnauthenticatedUser_ShouldReturnUnauthorized()
    {
        var getCredentialRequest = new GetCredentialRequest(_addedCredentialResponse.Id);

        var response =
            await AnonymousClient.GETAsync<GetCredentialEndpoint, GetCredentialRequest>(getCredentialRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ExistingCredential_ShouldReturnCredential()
    {
        var getCredentialRequest = new GetCredentialRequest(_addedCredentialResponse.Id);

        var (response, result) =
            await _authenticatedClient.GETAsync<GetCredentialEndpoint, GetCredentialRequest, CredentialResponse>(
                getCredentialRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Id.Should().Be(_addedCredentialResponse.Id);
        result.Username.Should().Be(_addedCredentialResponse.Username);
        result.WebAddress.Should().Be(_addedCredentialResponse.WebAddress);
        result.Description.Should().Be(_addedCredentialResponse.Description);
        result.Position.Should().BePositive();
    }

    [Fact]
    public async Task NotExistingCredential_ShouldReturnNotFound()
    {
        const long notExistingCredentialId = 12;
        var getCredentialRequest = new GetCredentialRequest(notExistingCredentialId);

        var response =
            await _authenticatedClient.GETAsync<GetCredentialEndpoint, GetCredentialRequest>(getCredentialRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UserDoesntOwnCredential_ShouldReturnForbidden()
    {
        var otherUser = await AnonymousClient.RegisterUser();
        using var otherUserClient = CreateAuthenticatedClient(otherUser.AccessToken);
        var getCredentialRequest = new GetCredentialRequest(_addedCredentialResponse.Id);

        var response =
            await otherUserClient.GETAsync<GetCredentialEndpoint, GetCredentialRequest>(getCredentialRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}