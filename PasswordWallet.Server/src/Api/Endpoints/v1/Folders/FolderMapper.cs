using Api.Endpoints.v1.Folders.AddFolder;
using Core.Entities;
using FastEndpoints;

namespace Api.Endpoints.v1.Folders;

public class FolderMapper : ResponseMapper<FolderResponse, Folder>
{
    public override FolderResponse FromEntity(Folder folder) => new(folder.Id, folder.Name, folder.Position);

    public IEnumerable<FolderResponse> FromEntities(IEnumerable<Folder> folders) =>
        folders.Select(f => new FolderResponse(f.Id, f.Name, f.Position));

    public Folder ToEntity(AddFolderRequest req) => new()
    {
        Name = req.Name,
        UserId = req.UserId
    };
}