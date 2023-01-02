namespace PasswordWallet.Server.Enums;

public enum IncorrectAccountLoginsThreshold
{
    Low = 3,
    Medium = 5,
    High = 10
}

public static class IncorrectAccountLoginsThresholdMethods
{
    /// <summary>
    /// Returns lockout time in seconds
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static long GetLockoutTime(this IncorrectAccountLoginsThreshold s1)
    {
        return s1 switch
        {
            IncorrectAccountLoginsThreshold.Low => 15,
            IncorrectAccountLoginsThreshold.Medium => 30,
            IncorrectAccountLoginsThreshold.High => 60,
            _ => throw new ArgumentOutOfRangeException(nameof(s1))
        };
    }
}