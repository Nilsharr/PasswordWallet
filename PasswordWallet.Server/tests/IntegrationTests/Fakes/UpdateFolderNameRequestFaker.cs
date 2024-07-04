using Api.Endpoints.v1.Folders.UpdateFolderName;
using Bogus;

namespace IntegrationTests.Fakes;

public sealed class UpdateFolderNameRequestFaker : Faker<UpdateFolderNameRequest>
{
    public UpdateFolderNameRequestFaker(Guid folderId)
    {
        CustomInstantiator(f => new UpdateFolderNameRequest(folderId, f.Random.Word()));
    }
}