using System.Net;
using FluentAssertions;
using Moq;
using PasswordWallet.Server.Entities;
using PasswordWallet.Server.Repositories;
using PasswordWallet.Server.Services;
using PasswordWallet.Server.Utils;
using PasswordWallet.UnitTests.ServicesTests.LoginAuditServiceTest.DataGenerators;

namespace PasswordWallet.UnitTests.ServicesTests.LoginAuditServiceTest;

public class LoginAuditServiceTests
{
    // TODO add tests with datetime https://www.youtube.com/watch?v=5DrGdyxnO5A
    [Theory]
    [ClassData(typeof(RegisterLoginAttemptNotExistingAccountExistingLoginIpAddressDataGenerator))]
    public async Task
        RegisterLoginAttempt_NotExistingAccountExistingLoginIpAddress_ShouldLogAnotherLoginAttemptToNonExistentAccountAndSetProperLockout(
            int subsequentBadLogins, bool expectedPermanentLockout, DateTime? expectedTemporaryLockout)
    {
        // Arrange
        var ipAddress = IPAddress.Parse("194.182.41.209");
        const string login = "rharteley4";
        const bool loginSuccessful = false;
        var expectedBadLoginsAmount = subsequentBadLogins * 2;
        const int expectedGoodLoginsAmount = 2;
        var loginIpAddress = new LoginIpAddress
        {
            Id = 3, IpAddress = ipAddress, AmountOfGoodLogins = expectedGoodLoginsAmount,
            AmountOfBadLogins = expectedBadLoginsAmount, SubsequentBadLogins = subsequentBadLogins
        };

        var dateTimeProvider = new Mock<IDateTimeProvider>();
        dateTimeProvider.Setup(x => x.UtcNow).Returns(new DateTime(2022, 10, 5, 14, 30, 40));

        var accountRepository = new Mock<IAccountRepository>();
        accountRepository.Setup(x => x.Get(login, default)).ReturnsAsync((Account?) null);

        var loginIpAddressRepository = new Mock<ILoginIpAddressRepository>();
        loginIpAddressRepository.Setup(x => x.Get(ipAddress, default)).ReturnsAsync(loginIpAddress);
        loginIpAddressRepository.Setup(x => x.SaveChanges(default));

        var loginAuditService = new LoginAuditService(dateTimeProvider.Object, accountRepository.Object,
            loginIpAddressRepository.Object);

        // Act
        var actual = await loginAuditService.RegisterLoginAttempt(ipAddress, login, loginSuccessful);

        // Assert
        loginIpAddressRepository.Verify(x => x.Add(It.IsAny<LoginIpAddress>()), Times.Never);

        actual.account.Should().Be(null);

        actual.loginIpAddress.IpAddress.Should().Be(ipAddress);
        actual.loginIpAddress.AmountOfGoodLogins.Should().Be(expectedGoodLoginsAmount);
        actual.loginIpAddress.AmountOfBadLogins.Should().Be(expectedBadLoginsAmount + 1);
        actual.loginIpAddress.SubsequentBadLogins.Should().Be(subsequentBadLogins + 1);
        actual.loginIpAddress.PermanentLock.Should().Be(expectedPermanentLockout);
        actual.loginIpAddress.TemporaryLock.Should().Be(expectedTemporaryLockout);
    }

    [Fact]
    public async Task
        RegisterLoginAttempt_NotExistingAccountAndNotExistingLoginIpAddress_ShouldLogFirstLoginAttemptToNonExistentAccount()
    {
        // Arrange
        var ipAddress = IPAddress.Parse("155.59.230.44");
        const string login = "gcran1";
        const bool loginSuccessful = false;
        LoginIpAddress? loginIpAddress = null;

        var dateTimeProvider = new Mock<IDateTimeProvider>();

        var accountRepository = new Mock<IAccountRepository>();
        accountRepository.Setup(x => x.Get(login, default)).ReturnsAsync((Account?) null);

        var loginIpAddressRepository = new Mock<ILoginIpAddressRepository>();
        loginIpAddressRepository.Setup(x => x.Get(ipAddress, default)).ReturnsAsync(loginIpAddress);
        loginIpAddressRepository.Setup(x => x.Add(It.IsAny<LoginIpAddress>())).Verifiable();
        loginIpAddressRepository.Setup(x => x.SaveChanges(default));

        var loginAuditService =
            new LoginAuditService(dateTimeProvider.Object, accountRepository.Object, loginIpAddressRepository.Object);

        // Act
        var actual = await loginAuditService.RegisterLoginAttempt(ipAddress, login, loginSuccessful);

        // Assert
        loginIpAddressRepository.Verify(x => x.Add(It.IsAny<LoginIpAddress>()), Times.Once);

        actual.account.Should().Be(null);

        actual.loginIpAddress.IpAddress.Should().Be(ipAddress);
        actual.loginIpAddress.AmountOfGoodLogins.Should().Be(0);
        actual.loginIpAddress.AmountOfBadLogins.Should().Be(1);
        actual.loginIpAddress.SubsequentBadLogins.Should().Be(1);
        actual.loginIpAddress.PermanentLock.Should().Be(false);
        actual.loginIpAddress.TemporaryLock.Should().Be(null);
    }

    [Fact]
    public async Task GetIpAddressBlockadeStatus_NotExistingLoginIpAddress_ShouldReturnNoBlockades()
    {
        // Arrange
        var ipAddress = IPAddress.Parse("135.241.121.131");
        const bool expectedPermanentLock = false;
        DateTime? expectedTemporaryLock = null;

        var dateTimeProvider = new Mock<IDateTimeProvider>();
        var accountRepository = new Mock<IAccountRepository>();

        var loginIpAddressRepository = new Mock<ILoginIpAddressRepository>();
        loginIpAddressRepository.Setup(x => x.Get(ipAddress, default)).ReturnsAsync((LoginIpAddress?) null);

        var loginAuditService = new LoginAuditService(dateTimeProvider.Object, accountRepository.Object,
            loginIpAddressRepository.Object);

        // Act
        var (permanentlyLocked, temporaryLock) = await loginAuditService.GetIpAddressBlockadeStatus(ipAddress);

        // Assert
        permanentlyLocked.Should().Be(expectedPermanentLock);
        temporaryLock.Should().Be(expectedTemporaryLock);
    }

    [Fact]
    public async Task GetIpAddressBlockadeStatus_ExistingLoginIpAddress_ShouldReturnBlockades()
    {
        // Arrange
        var ipAddress = IPAddress.Parse("153.58.168.112");
        const bool expectedPermanentLock = false;
        DateTime? expectedTemporaryLock = DateTime.Now.AddMinutes(2);
        var loginIpAddress = new LoginIpAddress
        {
            Id = 5, IpAddress = ipAddress, PermanentLock = expectedPermanentLock, TemporaryLock = expectedTemporaryLock
        };

        var dateTimeProvider = new Mock<IDateTimeProvider>();
        var accountRepository = new Mock<IAccountRepository>();

        var loginIpAddressRepository = new Mock<ILoginIpAddressRepository>();
        loginIpAddressRepository.Setup(x => x.Get(ipAddress, default)).ReturnsAsync(loginIpAddress);

        var loginAuditService = new LoginAuditService(dateTimeProvider.Object, accountRepository.Object,
            loginIpAddressRepository.Object);

        // Act
        var (permanentlyLocked, temporaryLock) = await loginAuditService.GetIpAddressBlockadeStatus(ipAddress);

        // Assert
        permanentlyLocked.Should().Be(expectedPermanentLock);
        temporaryLock.Should().Be(expectedTemporaryLock);
    }
}