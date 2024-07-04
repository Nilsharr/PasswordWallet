namespace Core.Entities;

public class User
{
    public long Id { get; init; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? RefreshToken { get; set; }
    public DateTimeOffset? RefreshTokenExpiry { get; set; }
    public int SubsequentBadLogins { get; set; }
    public DateTimeOffset? LockoutTime { get; set; }

    public IList<Folder> Folders { get; init; } = new List<Folder>();
    public IList<LoginHistory> LoginHistories { get; private set; } = new List<LoginHistory>();
}