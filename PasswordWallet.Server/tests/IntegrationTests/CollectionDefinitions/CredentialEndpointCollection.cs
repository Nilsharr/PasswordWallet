using IntegrationTests.AppFixtures;

namespace IntegrationTests.CollectionDefinitions;

[CollectionDefinition(CollectionName)]
public class CredentialEndpointCollection : ICollectionFixture<CredentialEndpointAppFixture>
{
    public const string CollectionName = nameof(CredentialEndpointCollection);
}