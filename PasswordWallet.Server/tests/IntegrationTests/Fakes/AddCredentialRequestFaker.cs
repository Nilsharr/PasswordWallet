using Api.Endpoints.v1.Credentials.AddCredential;
using Bogus;

namespace IntegrationTests.Fakes;

public sealed class AddCredentialRequestFaker : Faker<AddCredentialRequest>
{
    public AddCredentialRequestFaker(Guid folderId)
    {
        CustomInstantiator(f => new AddCredentialRequest(folderId, f.Internet.UserName(), f.Internet.Password(),
            f.Internet.Url(), f.Lorem.Paragraph(2)));
    }
}