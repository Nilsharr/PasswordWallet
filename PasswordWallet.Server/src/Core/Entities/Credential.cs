namespace Core.Entities;

public class Credential
{
    public long Id { get; init; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? WebAddress { get; set; }
    public string? Description { get; set; }
    public long Position { get; set; }
    public Guid FolderId { get; init; }
    public Folder Folder { get; init; } = null!;
}