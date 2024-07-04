using System.Net;
using Api.Endpoints.v1.User.Login;
using Api.Endpoints.v1.User.Register;
using Core.Models;
using FastEndpoints;
using FluentAssertions;
using Infrastructure.Options;
using IntegrationTests.AppFixtures;
using IntegrationTests.BaseIntegrationTests;
using IntegrationTests.CollectionDefinitions;
using IntegrationTests.Extensions;
using IntegrationTests.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IntegrationTests.EndpointTests.v1.User;

[Collection(UserEndpointCollection.CollectionName)]
public class LoginEndpointTests : BaseIntegrationTest
{
    private readonly LoginSecurityOptions _loginSecurityOptions;
    private RegisterRequest _registerRequest = default!;

    public LoginEndpointTests(UserEndpointAppFixture appFixture) : base(appFixture)
    {
        var scope = appFixture.Services.CreateScope();
        _loginSecurityOptions = scope.ServiceProvider.GetRequiredService<IOptions<LoginSecurityOptions>>().Value;
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _registerRequest = new RegisterRequestFaker().Generate();
        await AnonymousClient.RegisterUser(_registerRequest);
    }

    [Fact]
    public async Task ValidLoginData_ShouldLoginUser()
    {
        var loginRequest = new LoginRequest(_registerRequest.Username, _registerRequest.Password);

        var (response, result) =
            await AnonymousClient.POSTAsync<LoginEndpoint, LoginRequest, AuthenticationResponse>(loginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.AccessToken.Should().NotBeNull();
    }

    [Fact]
    public async Task EmptyLoginData_ShouldReturnUnauthorized()
    {
        var request = new LoginRequest("", "");

        var (response, result) =
            await AnonymousClient.POSTAsync<LoginEndpoint, LoginRequest, ErrorResponse>(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Errors.Should().HaveCount(2);
    }

    [Fact]
    public async Task InvalidLogin_ShouldReturnUnauthorized()
    {
        var loginRequest = new LoginRequest("Azxcfug", _registerRequest.Password);

        var response = await AnonymousClient.POSTAsync<LoginEndpoint, LoginRequest>(loginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task InvalidPassword_ShouldReturnUnauthorized()
    {
        var loginRequest = new LoginRequest(_registerRequest.Username, "qwerty123");

        var response = await AnonymousClient.POSTAsync<LoginEndpoint, LoginRequest>(loginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task MultipleUnsuccessfulLogins_ShouldBlockadeUser()
    {
        var loginRequest = new LoginRequest(_registerRequest.Username, "qwerty123");
        for (var i = 0; i < _loginSecurityOptions.MaxFailedAccessAttempts; i++)
        {
            await AnonymousClient.POSTAsync<LoginEndpoint, LoginRequest, ErrorResponse>(loginRequest);
        }

        var response = await AnonymousClient.POSTAsync<LoginEndpoint, LoginRequest>(loginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}