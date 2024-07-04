namespace Core.Entities;

public class Folder
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public long Position { get; set; }
    public long UserId { get; init; }
    public User User { get; init; } = null!;

    public IList<Credential> Credentials { get; private set; } = new List<Credential>();
}