using Api.Endpoints.v1.Credentials;
using Api.Endpoints.v1.Credentials.AddCredential;
using Api.Endpoints.v1.Folders;
using Api.Endpoints.v1.Folders.AddFolder;
using Api.Endpoints.v1.User.Register;
using Core.Models;
using FastEndpoints;
using IntegrationTests.Fakes;

namespace IntegrationTests.Extensions;

public static class HttpExtensions
{
    public static async Task<AuthenticationResponse> RegisterUser(this HttpClient client,
        RegisterRequest? registerRequest = null)
    {
       registerRequest ??= new RegisterRequestFaker().Generate();
        return (await client.POSTAsync<RegisterEndpoint, RegisterRequest, AuthenticationResponse>(registerRequest)).Result;
    }

    public static async Task<FolderResponse> AddFolder(this HttpClient authenticatedClient,
        AddFolderRequest? addFolderRequest = null)
    {
        addFolderRequest ??= new AddFolderRequestFaker().Generate();
        return (await authenticatedClient.POSTAsync<AddFolderEndpoint, AddFolderRequest, FolderResponse>(
            addFolderRequest)).Result;
    }

    public static async Task<CredentialResponse> AddCredential(this HttpClient authenticatedClient, Guid folderId)
    {
        var addCredentialRequest = new AddCredentialRequestFaker(folderId).Generate();
        return (await authenticatedClient.POSTAsync<AddCredentialEndpoint, AddCredentialRequest, CredentialResponse>(
            addCredentialRequest)).Result;
    }

    public static async Task<CredentialResponse> AddCredential(this HttpClient authenticatedClient,
        AddCredentialRequest addCredentialRequest)
    {
        return (await authenticatedClient.POSTAsync<AddCredentialEndpoint, AddCredentialRequest, CredentialResponse>(
            addCredentialRequest)).Result;
    }
}