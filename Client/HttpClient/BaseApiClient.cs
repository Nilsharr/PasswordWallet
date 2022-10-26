namespace PasswordWallet.Client.HttpClient;

public class BaseApiClient
{
    //TODO better way to add auth token to request 
    // yeah idk
    public string? BearerToken { get; set; }

    protected Task<HttpRequestMessage> CreateHttpRequestMessageAsync(CancellationToken cancellationToken)
    {
        var msg = new HttpRequestMessage();
        if (BearerToken is not null)
        {
            msg.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", BearerToken);
        }

        return Task.FromResult(msg);
    }
}