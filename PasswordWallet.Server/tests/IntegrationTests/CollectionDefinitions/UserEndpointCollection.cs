using IntegrationTests.AppFixtures;

namespace IntegrationTests.CollectionDefinitions;

[CollectionDefinition(CollectionName)]
public class UserEndpointCollection : ICollectionFixture<UserEndpointAppFixture>
{
    public const string CollectionName = nameof(UserEndpointCollection);
}

