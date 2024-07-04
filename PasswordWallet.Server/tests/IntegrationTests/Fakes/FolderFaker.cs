using Bogus;
using Core.Entities;

namespace IntegrationTests.Fakes;

public sealed class FolderFaker : Faker<Folder>
{
    public FolderFaker(long userId, long position)
    {
        RuleFor(x => x.UserId, userId);
        RuleFor(x => x.Name, f => f.Random.Word());
        RuleFor(x => x.Position, position);
    }
}