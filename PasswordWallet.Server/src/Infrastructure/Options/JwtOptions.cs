namespace Infrastructure.Options;

public class JwtOptions
{
    public const string SectionName = "JwtOptions";

    public string SigningKey { get; init; } = null!;
    public string? Issuer { get; init; }
    public string? Audience { get; init; }
    public TimeSpan AccessTokenLifeTime { get; init; }
    public TimeSpan RefreshTokenLifeTime { get; init; }
}