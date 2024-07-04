using IntegrationTests.AppFixtures;

namespace IntegrationTests.CollectionDefinitions;

[CollectionDefinition(CollectionName)]
public class RepositoryCollection : ICollectionFixture<RepositoryAppFixture>
{
    public const string CollectionName = nameof(RepositoryCollection);
}

