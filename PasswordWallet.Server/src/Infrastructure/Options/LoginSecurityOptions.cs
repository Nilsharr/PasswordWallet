namespace Infrastructure.Options;

public class LoginSecurityOptions
{
    public const string SectionName = "LoginSecurityOptions";
    public int MaxFailedAccessAttempts { get; init; }
    public TimeSpan LockoutTime { get; init; }
}