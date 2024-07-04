namespace Infrastructure.Options;

public class CorsOptions
{
    public const string SectionName = "CorsOptions";

    public string PolicyName { get; init; } = null!;
    public string[] Origins { get; init; } = [];
}