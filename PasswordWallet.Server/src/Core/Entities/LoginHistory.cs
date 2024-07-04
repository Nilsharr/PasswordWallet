namespace Core.Entities;

public class LoginHistory
{
    public long Id { get; init; }
    public DateTimeOffset Date { get; init; } = DateTimeOffset.UtcNow;
    public bool Correct { get; init; }
    public string? IpAddress { get; init; }
    public long UserId { get; init; }
    public User User { get; init; } = null!;
}