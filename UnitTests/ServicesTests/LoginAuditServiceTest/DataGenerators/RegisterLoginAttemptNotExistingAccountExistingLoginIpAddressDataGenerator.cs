using System.Collections;
using PasswordWallet.Server.Enums;

namespace PasswordWallet.UnitTests.ServicesTests.LoginAuditServiceTest.DataGenerators;

public class RegisterLoginAttemptNotExistingAccountExistingLoginIpAddressDataGenerator : IEnumerable<object?[]>
{
    public IEnumerator<object?[]> GetEnumerator()
    {
        yield return new object?[] {0, false, null};
        yield return new object?[]
        {
            (int) IncorrectIpAddressLoginsThreshold.Low - 1, false,
            LoginAuditServiceTests.TestUtcDate.AddSeconds(IncorrectIpAddressLoginsThreshold.Low.GetLockoutTime())
        };
        yield return new object?[]
        {
            (int) IncorrectIpAddressLoginsThreshold.Medium - 1, false,
            LoginAuditServiceTests.TestUtcDate.AddSeconds(IncorrectIpAddressLoginsThreshold.Medium.GetLockoutTime())
        };
        yield return new object?[] {(int) IncorrectIpAddressLoginsThreshold.High - 1, true, null};
        yield return new object?[] {(int) IncorrectIpAddressLoginsThreshold.High, true, null};
        yield return new object?[] {(int) IncorrectIpAddressLoginsThreshold.High + 2, true, null};
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}