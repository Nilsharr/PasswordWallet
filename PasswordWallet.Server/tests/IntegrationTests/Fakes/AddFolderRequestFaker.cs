using Api.Endpoints.v1.Folders.AddFolder;
using Bogus;

namespace IntegrationTests.Fakes;

public sealed class AddFolderRequestFaker : Faker<AddFolderRequest>
{
    public AddFolderRequestFaker()
    {
        CustomInstantiator(f => new AddFolderRequest(f.Random.Word()));
    }
}