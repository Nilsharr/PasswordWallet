namespace PasswordWallet.Server.Enums;

public enum IncorrectIpAddressLoginsThreshold
{
    Low = 5,
    Medium = 10,
    High = 15
}

public static class IncorrectIpAddressLoginsThresholdMethods
{
    /// <summary>
    /// Returns lockout time in seconds
    /// </summary>
    /// <param name="s1"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static long GetLockoutTime(this IncorrectIpAddressLoginsThreshold s1)
    {
        return s1 switch
        {
            IncorrectIpAddressLoginsThreshold.Low => 60,
            IncorrectIpAddressLoginsThreshold.Medium => 120,
            IncorrectIpAddressLoginsThreshold.High => throw new ArgumentException(
                "There should be set permanent lockout for high threshold", nameof(s1)),
            _ => throw new ArgumentOutOfRangeException(nameof(s1))
        };
    }
}