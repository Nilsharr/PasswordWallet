using System.Collections;
using PasswordWallet.Server.Enums;

namespace PasswordWallet.UnitTests.ServicesTests.LoginAuditServiceTest.DataGenerators;

public class RegisterLoginAttemptNotExistingAccountExistingLoginIpAddressDataGenerator : IEnumerable<object?[]>
{
    public IEnumerator<object?[]> GetEnumerator()
    {
        yield return new object?[] {0, false, null};
        yield return new object?[]
            {(int) IncorrectLoginsThreshold.Low - 1, false, new DateTime(2022, 10, 5, 14, 30, 40).AddSeconds(5)};
        yield return new object?[]
            {(int) IncorrectLoginsThreshold.Low, false, new DateTime(2022, 10, 5, 14, 30, 40).AddSeconds(10)};
        yield return new object?[] {(int) IncorrectLoginsThreshold.Medium, true, null};
        yield return new object?[] {(int) IncorrectLoginsThreshold.High, true, null};
        yield return new object?[] {(int) IncorrectLoginsThreshold.High + 2, true, null};
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}