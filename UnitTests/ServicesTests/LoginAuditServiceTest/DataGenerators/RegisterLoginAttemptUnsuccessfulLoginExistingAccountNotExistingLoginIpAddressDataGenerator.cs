using System.Collections;
using PasswordWallet.Server.Enums;

namespace PasswordWallet.UnitTests.ServicesTests.LoginAuditServiceTest.DataGenerators;

public class
    RegisterLoginAttemptUnsuccessfulLoginExistingAccountNotExistingLoginIpAddressDataGenerator : IEnumerable<object?[]>
{
    public IEnumerator<object?[]> GetEnumerator()
    {
        yield return new object?[] {0, null};
        yield return new object?[]
        {
            (int) IncorrectAccountLoginsThreshold.Low - 1,
            LoginAuditServiceTests.TestUtcDate.AddSeconds(IncorrectAccountLoginsThreshold.Low.GetLockoutTime())
        };
        yield return new object?[]
        {
            (int) IncorrectAccountLoginsThreshold.Medium - 1,
            LoginAuditServiceTests.TestUtcDate.AddSeconds(IncorrectAccountLoginsThreshold.Medium.GetLockoutTime())
        };
        yield return new object?[]
        {
            (int) IncorrectAccountLoginsThreshold.High - 1,
            LoginAuditServiceTests.TestUtcDate.AddSeconds(IncorrectAccountLoginsThreshold.High.GetLockoutTime())
        };
        yield return new object?[]
        {
            (int) IncorrectAccountLoginsThreshold.High,
            LoginAuditServiceTests.TestUtcDate.AddSeconds(IncorrectAccountLoginsThreshold.High.GetLockoutTime())
        };
        yield return new object?[]
        {
            (int) IncorrectAccountLoginsThreshold.High + 2,
            LoginAuditServiceTests.TestUtcDate.AddSeconds(IncorrectAccountLoginsThreshold.High.GetLockoutTime())
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}