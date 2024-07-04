using Bogus;
using Core.Entities;

namespace IntegrationTests.Fakes;

public sealed class CredentialFaker : Faker<Credential>
{
    public CredentialFaker(Guid folderId, long position)
    {
        RuleFor(x => x.FolderId, folderId);
        RuleFor(x => x.Username, f => f.Internet.UserName());
        RuleFor(x => x.Password, f => f.Internet.Password());
        RuleFor(x => x.WebAddress, f => f.Internet.Url());
        RuleFor(x => x.Description, f => f.Lorem.Paragraph(2));
        RuleFor(x => x.Position, position);
    }
}