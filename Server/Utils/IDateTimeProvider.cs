namespace PasswordWallet.Server.Utils;

public interface IDateTimeProvider
{
    public DateTime Now { get; }
    public DateTime UtcNow { get; }
}

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
}