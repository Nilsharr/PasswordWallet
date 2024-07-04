using FastEndpoints;

namespace Api.Config.Groups;

public sealed class CredentialGroup : Group
{
    public CredentialGroup()
    {
        Configure("credentials", ep => ep.Description(x => x.WithTags("Credentials")));
    }
}