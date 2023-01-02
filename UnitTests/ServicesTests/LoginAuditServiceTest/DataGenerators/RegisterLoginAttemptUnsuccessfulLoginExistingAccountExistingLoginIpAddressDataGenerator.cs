using System.Collections;
using PasswordWallet.Server.Enums;

namespace PasswordWallet.UnitTests.ServicesTests.LoginAuditServiceTest.DataGenerators;

public class
    RegisterLoginAttemptUnsuccessfulLoginExistingAccountExistingLoginIpAddressDataGenerator : IEnumerable<object?[]>
{
    public IEnumerator<object?[]> GetEnumerator()
    {
        yield return new object?[] {0, null, 0, false, null};
        yield return new object?[]
        {
            (int) IncorrectAccountLoginsThreshold.Low - 1,
            LoginAuditServiceTests.TestUtcDate.AddSeconds(IncorrectAccountLoginsThreshold.Low.GetLockoutTime()), 0,
            false, null
        };
        yield return new object?[]
        {
            0, null, (int) IncorrectIpAddressLoginsThreshold.Low - 1, false,
            LoginAuditServiceTests.TestUtcDate.AddSeconds(IncorrectIpAddressLoginsThreshold.Low.GetLockoutTime())
        };
        yield return new object?[]
        {
            (int) IncorrectAccountLoginsThreshold.Low - 1,
            LoginAuditServiceTests.TestUtcDate.AddSeconds(IncorrectAccountLoginsThreshold.Low.GetLockoutTime()),
            (int) IncorrectIpAddressLoginsThreshold.Low - 1, false,
            LoginAuditServiceTests.TestUtcDate.AddSeconds(IncorrectIpAddressLoginsThreshold.Low.GetLockoutTime())
        };
        yield return new object?[]
        {
            (int) IncorrectAccountLoginsThreshold.Medium - 1,
            LoginAuditServiceTests.TestUtcDate.AddSeconds(IncorrectAccountLoginsThreshold.Medium.GetLockoutTime()), 0,
            false, null
        };
        yield return new object?[]
        {
            0, null, (int) IncorrectIpAddressLoginsThreshold.Medium - 1, false,
            LoginAuditServiceTests.TestUtcDate.AddSeconds(IncorrectIpAddressLoginsThreshold.Medium.GetLockoutTime())
        };
        yield return new object?[]
        {
            (int) IncorrectAccountLoginsThreshold.Medium - 1,
            LoginAuditServiceTests.TestUtcDate.AddSeconds(IncorrectAccountLoginsThreshold.Medium.GetLockoutTime()),
            (int) IncorrectIpAddressLoginsThreshold.Medium - 1, false,
            LoginAuditServiceTests.TestUtcDate.AddSeconds(IncorrectIpAddressLoginsThreshold.Medium.GetLockoutTime())
        };
        yield return new object?[]
        {
            (int) IncorrectAccountLoginsThreshold.High - 1,
            LoginAuditServiceTests.TestUtcDate.AddSeconds(IncorrectAccountLoginsThreshold.High.GetLockoutTime()), 0,
            false, null
        };
        yield return new object?[]
        {
            0, null, (int) IncorrectIpAddressLoginsThreshold.High - 1, true, null
        };
        yield return new object?[]
        {
            (int) IncorrectAccountLoginsThreshold.High + 2,
            LoginAuditServiceTests.TestUtcDate.AddSeconds(IncorrectAccountLoginsThreshold.High.GetLockoutTime()),
            (int) IncorrectIpAddressLoginsThreshold.High + 2, true, null
        };
        yield return new object?[]
        {
            (int) IncorrectAccountLoginsThreshold.High + 2,
            LoginAuditServiceTests.TestUtcDate.AddSeconds(IncorrectAccountLoginsThreshold.High.GetLockoutTime()), 0,
            false, null
        };
        yield return new object?[]
        {
            0, null, (int) IncorrectIpAddressLoginsThreshold.High + 2, true, null
        };
        yield return new object?[]
        {
            (int) IncorrectAccountLoginsThreshold.High + 2,
            LoginAuditServiceTests.TestUtcDate.AddSeconds(IncorrectAccountLoginsThreshold.High.GetLockoutTime()),
            (int) IncorrectIpAddressLoginsThreshold.High + 2, true, null
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}