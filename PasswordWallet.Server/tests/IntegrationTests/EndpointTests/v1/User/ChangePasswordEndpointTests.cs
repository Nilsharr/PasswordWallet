using System.Net;
using Api.Endpoints.v1.User.ChangePassword;
using Api.Endpoints.v1.User.Login;
using Api.Endpoints.v1.User.Register;
using FastEndpoints;
using FluentAssertions;
using IntegrationTests.AppFixtures;
using IntegrationTests.BaseIntegrationTests;
using IntegrationTests.CollectionDefinitions;
using IntegrationTests.Extensions;
using IntegrationTests.Fakes;

namespace IntegrationTests.EndpointTests.v1.User;

[Collection(UserEndpointCollection.CollectionName)]
public class ChangePasswordEndpointTests(UserEndpointAppFixture appFixture) : BaseIntegrationTest(appFixture)
{
    private RegisterRequest _registerRequest = default!;
    private HttpClient _authenticatedClient = default!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _registerRequest = new RegisterRequestFaker().Generate();
        var registerResponse = await AnonymousClient.RegisterUser(_registerRequest);
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
        var request = new ChangePasswordRequest("", "", "");

        var response = await AnonymousClient.PATCHAsync<ChangePasswordEndpoint, ChangePasswordRequest>(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ValidRequest_ShouldChangePassword()
    {
        const string newPassword = "mF%3!E67fd8";
        var changePasswordRequest = new ChangePasswordRequest(_registerRequest.Password, newPassword, newPassword);
        var loginRequest = new LoginRequest(_registerRequest.Username, newPassword);

        var changePasswordResponse =
            await _authenticatedClient.PATCHAsync<ChangePasswordEndpoint, ChangePasswordRequest>(
                changePasswordRequest);
        var loginResponse = await AnonymousClient.POSTAsync<LoginEndpoint, LoginRequest>(loginRequest);

        changePasswordResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task EmptyRequest_ShouldReturnBadRequest()
    {
        var request = new ChangePasswordRequest("", "", "");

        var response = await _authenticatedClient.PATCHAsync<ChangePasswordEndpoint, ChangePasswordRequest>(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WrongCurrentPassword_ShouldReturnUnauthorized()
    {
        const string wrongPassword = "123";
        const string newPassword = "mF%3!E67fd8";
        var request = new ChangePasswordRequest(wrongPassword, newPassword, newPassword);

        var response = await _authenticatedClient.PATCHAsync<ChangePasswordEndpoint, ChangePasswordRequest>(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task WeakPassword_ShouldReturnBadRequest()
    {
        const string weakPassword = "652";
        var request = new ChangePasswordRequest(_registerRequest.Password, weakPassword, weakPassword);

        var response = await _authenticatedClient.PATCHAsync<ChangePasswordEndpoint, ChangePasswordRequest>(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task NotMatchingPasswords_ShouldReturnBadRequest()
    {
        const string password = "J3gn4$sB17u";
        const string notMatchingPassword = "Vahdx9#4a9$";

        var request = new ChangePasswordRequest(_registerRequest.Password, password, notMatchingPassword);

        var response = await _authenticatedClient.PATCHAsync<ChangePasswordEndpoint, ChangePasswordRequest>(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}