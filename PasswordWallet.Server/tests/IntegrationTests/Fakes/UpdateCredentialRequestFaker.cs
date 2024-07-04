using Api.Endpoints.v1.Credentials.UpdateCredential;
using Bogus;

namespace IntegrationTests.Fakes;

public sealed class UpdateCredentialRequestFaker : Faker<UpdateCredentialRequest>
{
    public UpdateCredentialRequestFaker(long credentialId)
    {
        CustomInstantiator(f => new UpdateCredentialRequest(credentialId, f.Internet.UserName(), f.Internet.Password(),
            f.Internet.Url(), f.Lorem.Paragraph(2)));
    }
}