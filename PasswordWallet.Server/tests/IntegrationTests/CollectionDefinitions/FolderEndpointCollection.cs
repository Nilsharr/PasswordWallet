using IntegrationTests.AppFixtures;

namespace IntegrationTests.CollectionDefinitions;

[CollectionDefinition(CollectionName)]
public class FolderEndpointCollection : ICollectionFixture<FolderEndpointAppFixture>
{
    public const string CollectionName = nameof(FolderEndpointCollection);
}