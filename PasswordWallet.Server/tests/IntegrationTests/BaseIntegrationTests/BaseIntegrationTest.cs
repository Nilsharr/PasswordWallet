using System.Net.Http.Headers;
using IntegrationTests.AppFixtures;

namespace IntegrationTests.BaseIntegrationTests;

public class BaseIntegrationTest(PasswordWalletAppFixture appFixture) : IAsyncLifetime
{
    protected HttpClient AnonymousClient => appFixture.Client;

    protected HttpClient CreateAuthenticatedClient(string token) => appFixture.CreateClient(c =>
        c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token));

    public virtual Task InitializeAsync() => Task.CompletedTask;

    public virtual Task DisposeAsync() => appFixture.ResetDatabase();
}