using FastEndpoints;

namespace Api.Config.Groups;

public sealed class FolderGroup : Group
{
    public FolderGroup()
    {
        Configure("folders", ep => ep.Description(x => x.WithTags("Folders")));
    }
}