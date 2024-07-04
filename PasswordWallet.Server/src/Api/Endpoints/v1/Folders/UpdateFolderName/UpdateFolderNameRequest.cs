namespace Api.Endpoints.v1.Folders.UpdateFolderName;

public record UpdateFolderNameRequest(Guid FolderId, string Name);