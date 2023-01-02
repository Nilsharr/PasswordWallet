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
    public static DateTime TestUtcDate => new(2022, 10, 5, 14, 30, 40);

    private readonly Mock<IDateTimeProvider> _dateTimeProvider;

    public LoginAuditServiceTests()
    {
        _dateTimeProvider = new Mock<IDateTimeProvider>();
        _dateTimeProvider.Setup(x => x.UtcNow).Returns(TestUtcDate);
    }

    [Fact]
    public async Task
        RegisterLoginAttempt_SuccessfulLoginExistingAccountNotExistingLoginIpAddress_ShouldLogAnotherLoginAttemptToExistentAccountAndSetProperLockout()
    {
        // Arrange
        var ipAddress = IPAddress.Parse("177.233.168.100");
        const string login = "rhatfull2";
        const bool loginSuccessful = true;

        var account = new Account {Id = 7, Login = login, SubsequentBadLogins = 4};
        var expectedAccountLogin = new AccountLogin {Correct = loginSuccessful, AccountId = account.Id};
        var expectedLoginIpAddress = new LoginIpAddress
        {
            IpAddress = ipAddress, AmountOfGoodLogins = 1, AmountOfBadLogins = 0, SubsequentBadLogins = 0,
            AccountLogins = new List<AccountLogin> {expectedAccountLogin}
        };

        var accountRepository = new Mock<IAccountRepository>();
        accountRepository.Setup(x => x.Get(login, default)).ReturnsAsync(account);

        var loginIpAddressRepository = new Mock<ILoginIpAddressRepository>();
        loginIpAddressRepository.Setup(x => x.Get(ipAddress, default)).ReturnsAsync((LoginIpAddress?) null);
        loginIpAddressRepository.Setup(x => x.SaveChanges(default));

        var loginAuditService = new LoginAuditService(_dateTimeProvider.Object, accountRepository.Object,
            loginIpAddressRepository.Object);

        // Act
        var actual = await loginAuditService.RegisterLoginAttempt(ipAddress, login, loginSuccessful);

        // Assert
        loginIpAddressRepository.Verify(x => x.Add(It.IsAny<LoginIpAddress>()), Times.Once);

        actual.account.Should().NotBeNull();
        actual.account!.SubsequentBadLogins.Should().Be(0);
        actual.account.LockoutTime.Should().Be(null);
        actual.loginIpAddress.Should().BeEquivalentTo(expectedLoginIpAddress);
    }

    [Theory]
    [ClassData(typeof(RegisterLoginAttemptUnsuccessfulLoginExistingAccountNotExistingLoginIpAddressDataGenerator))]
    public async Task
        RegisterLoginAttempt_UnsuccessfulLoginExistingAccountNotExistingLoginIpAddress_ShouldLogAnotherLoginAttemptToExistentAccountAndSetProperLockout(
            int accountSubsequentBadLogins, DateTime? expectedAccountLockout)
    {
        // Arrange
        var ipAddress = IPAddress.Parse("185.68.125.134");
        const string login = "sIOneTch";
        const bool loginSuccessful = false;

        var account = new Account {Id = 4, Login = login, SubsequentBadLogins = accountSubsequentBadLogins};
        var expectedAccountLogin = new AccountLogin {Correct = loginSuccessful, AccountId = account.Id};
        var expectedLoginIpAddress = new LoginIpAddress
        {
            IpAddress = ipAddress, AmountOfGoodLogins = 0, AmountOfBadLogins = 1, SubsequentBadLogins = 1,
            AccountLogins = new List<AccountLogin> {expectedAccountLogin}
        };

        var accountRepository = new Mock<IAccountRepository>();
        accountRepository.Setup(x => x.Get(login, default)).ReturnsAsync(account);

        var loginIpAddressRepository = new Mock<ILoginIpAddressRepository>();
        loginIpAddressRepository.Setup(x => x.Get(ipAddress, default)).ReturnsAsync((LoginIpAddress?) null);
        loginIpAddressRepository.Setup(x => x.SaveChanges(default));

        var loginAuditService = new LoginAuditService(_dateTimeProvider.Object, accountRepository.Object,
            loginIpAddressRepository.Object);

        // Act
        var actual = await loginAuditService.RegisterLoginAttempt(ipAddress, login, loginSuccessful);

        // Assert
        loginIpAddressRepository.Verify(x => x.Add(It.IsAny<LoginIpAddress>()), Times.Once);

        actual.account.Should().NotBeNull();
        actual.account!.SubsequentBadLogins.Should().Be(accountSubsequentBadLogins + 1);
        actual.account.LockoutTime.Should().Be(expectedAccountLockout);
        actual.loginIpAddress.Should().BeEquivalentTo(expectedLoginIpAddress);
    }

    [Fact]
    public async Task
        RegisterLoginAttempt_SuccessfulLoginExistingAccountExistingLoginIpAddress_ShouldLogAnotherLoginAttemptToExistentAccountAndSetProperLockout()
    {
        // Arrange
        var ipAddress = IPAddress.Parse("178.78.137.238");
        const string login = "ttoulamain9";
        const bool loginSuccessful = true;
        const int ipAddressGoodLogins = 8;

        var account = new Account {Id = 7, Login = login, SubsequentBadLogins = 4};
        var loginIpAddress = new LoginIpAddress
        {
            IpAddress = ipAddress, AmountOfGoodLogins = ipAddressGoodLogins, AmountOfBadLogins = 4,
            SubsequentBadLogins = 2,
        };

        var expectedAccountLogin = new AccountLogin {Correct = loginSuccessful, AccountId = account.Id};
        var expectedLoginIpAddress = new LoginIpAddress
        {
            IpAddress = ipAddress, AmountOfGoodLogins = ipAddressGoodLogins + 1, AmountOfBadLogins = 4,
            SubsequentBadLogins = 0,
            AccountLogins = new List<AccountLogin> {expectedAccountLogin}
        };

        var accountRepository = new Mock<IAccountRepository>();
        accountRepository.Setup(x => x.Get(login, default)).ReturnsAsync(account);

        var loginIpAddressRepository = new Mock<ILoginIpAddressRepository>();
        loginIpAddressRepository.Setup(x => x.Get(ipAddress, default)).ReturnsAsync(loginIpAddress);
        loginIpAddressRepository.Setup(x => x.SaveChanges(default));

        var loginAuditService = new LoginAuditService(_dateTimeProvider.Object, accountRepository.Object,
            loginIpAddressRepository.Object);

        // Act
        var actual = await loginAuditService.RegisterLoginAttempt(ipAddress, login, loginSuccessful);

        // Assert
        loginIpAddressRepository.Verify(x => x.Add(It.IsAny<LoginIpAddress>()), Times.Never);

        actual.account.Should().NotBeNull();
        actual.account!.SubsequentBadLogins.Should().Be(0);
        actual.account.LockoutTime.Should().Be(null);

        actual.loginIpAddress.Should().BeEquivalentTo(expectedLoginIpAddress);
    }

    [Theory]
    [ClassData(typeof(RegisterLoginAttemptUnsuccessfulLoginExistingAccountExistingLoginIpAddressDataGenerator))]
    public async Task
        RegisterLoginAttempt_UnsuccessfulLoginExistingAccountExistingLoginIpAddress_ShouldLogAnotherLoginAttemptToExistentAccountAndSetProperLockout(
            int accountSubsequentBadLogins, DateTime? expectedAccountLockout, int ipAddressSubsequentBadLogins,
            bool ipAddressExpectedPermanentLockout, DateTime? ipAddressExpectedTemporaryLockout)
    {
        // Arrange
        var ipAddress = IPAddress.Parse("232.89.2.94");
        const string login = "waers8";
        var ipAddressExpectedBadLoginsAmount = ipAddressSubsequentBadLogins * 2;
        const bool loginSuccessful = false;
        const int ipAddressGoodLogins = 7;

        var account = new Account {Id = 2, Login = login, SubsequentBadLogins = accountSubsequentBadLogins};
        var loginIpAddress = new LoginIpAddress
        {
            Id = 5, IpAddress = ipAddress, AmountOfGoodLogins = ipAddressGoodLogins,
            AmountOfBadLogins = ipAddressExpectedBadLoginsAmount, SubsequentBadLogins = ipAddressSubsequentBadLogins
        };

        var accountRepository = new Mock<IAccountRepository>();
        accountRepository.Setup(x => x.Get(login, default)).ReturnsAsync(account);

        var loginIpAddressRepository = new Mock<ILoginIpAddressRepository>();
        loginIpAddressRepository.Setup(x => x.Get(ipAddress, default)).ReturnsAsync(loginIpAddress);
        loginIpAddressRepository.Setup(x => x.SaveChanges(default));

        var loginAuditService = new LoginAuditService(_dateTimeProvider.Object, accountRepository.Object,
            loginIpAddressRepository.Object);

        // Act
        var actual = await loginAuditService.RegisterLoginAttempt(ipAddress, login, loginSuccessful);

        // Assert
        loginIpAddressRepository.Verify(x => x.Add(It.IsAny<LoginIpAddress>()), Times.Never);

        actual.account.Should().NotBeNull();
        actual.account!.SubsequentBadLogins.Should().Be(accountSubsequentBadLogins + 1);
        actual.account.LockoutTime.Should().Be(expectedAccountLockout);

        actual.loginIpAddress.IpAddress.Should().Be(ipAddress);
        actual.loginIpAddress.AmountOfGoodLogins.Should().Be(ipAddressGoodLogins);
        actual.loginIpAddress.AmountOfBadLogins.Should().Be(ipAddressExpectedBadLoginsAmount + 1);
        actual.loginIpAddress.SubsequentBadLogins.Should().Be(ipAddressSubsequentBadLogins + 1);
        actual.loginIpAddress.PermanentLock.Should().Be(ipAddressExpectedPermanentLockout);
        actual.loginIpAddress.TemporaryLock.Should().Be(ipAddressExpectedTemporaryLockout);
    }

    [Theory]
    [ClassData(typeof(RegisterLoginAttemptNotExistingAccountExistingLoginIpAddressDataGenerator))]
    public async Task
        RegisterLoginAttempt_NotExistingAccountExistingLoginIpAddress_ShouldLogAnotherLoginAttemptToNonExistentAccountAndSetProperLockout(
            int ipAddressSubsequentBadLogins, bool expectedIpAddressPermanentLockout,
            DateTime? expectedIpAddressTemporaryLockout)
    {
        // Arrange
        var ipAddress = IPAddress.Parse("194.182.41.209");
        const string login = "rharteley4";
        const bool loginSuccessful = false;
        var expectedBadLoginsAmount = ipAddressSubsequentBadLogins * 2;
        const int expectedGoodLoginsAmount = 2;
        var loginIpAddress = new LoginIpAddress
        {
            Id = 3, IpAddress = ipAddress, AmountOfGoodLogins = expectedGoodLoginsAmount,
            AmountOfBadLogins = expectedBadLoginsAmount, SubsequentBadLogins = ipAddressSubsequentBadLogins
        };

        var accountRepository = new Mock<IAccountRepository>();
        accountRepository.Setup(x => x.Get(login, default)).ReturnsAsync((Account?) null);

        var loginIpAddressRepository = new Mock<ILoginIpAddressRepository>();
        loginIpAddressRepository.Setup(x => x.Get(ipAddress, default)).ReturnsAsync(loginIpAddress);
        loginIpAddressRepository.Setup(x => x.SaveChanges(default));

        var loginAuditService = new LoginAuditService(_dateTimeProvider.Object, accountRepository.Object,
            loginIpAddressRepository.Object);

        // Act
        var actual = await loginAuditService.RegisterLoginAttempt(ipAddress, login, loginSuccessful);

        // Assert
        loginIpAddressRepository.Verify(x => x.Add(It.IsAny<LoginIpAddress>()), Times.Never);

        actual.account.Should().Be(null);

        actual.loginIpAddress.IpAddress.Should().Be(ipAddress);
        actual.loginIpAddress.AmountOfGoodLogins.Should().Be(expectedGoodLoginsAmount);
        actual.loginIpAddress.AmountOfBadLogins.Should().Be(expectedBadLoginsAmount + 1);
        actual.loginIpAddress.SubsequentBadLogins.Should().Be(ipAddressSubsequentBadLogins + 1);
        actual.loginIpAddress.PermanentLock.Should().Be(expectedIpAddressPermanentLockout);
        actual.loginIpAddress.TemporaryLock.Should().Be(expectedIpAddressTemporaryLockout);
    }

    [Fact]
    public async Task
        RegisterLoginAttempt_NotExistingAccountAndNotExistingLoginIpAddress_ShouldLogFirstLoginAttemptToNonExistentAccount()
    {
        // Arrange
        var ipAddress = IPAddress.Parse("155.59.230.44");
        const string login = "gcran1";
        const bool loginSuccessful = false;

        var accountRepository = new Mock<IAccountRepository>();
        accountRepository.Setup(x => x.Get(login, default)).ReturnsAsync((Account?) null);

        var loginIpAddressRepository = new Mock<ILoginIpAddressRepository>();
        loginIpAddressRepository.Setup(x => x.Get(ipAddress, default)).ReturnsAsync((LoginIpAddress?) null);
        loginIpAddressRepository.Setup(x => x.Add(It.IsAny<LoginIpAddress>())).Verifiable();
        loginIpAddressRepository.Setup(x => x.SaveChanges(default));

        var loginAuditService =
            new LoginAuditService(_dateTimeProvider.Object, accountRepository.Object, loginIpAddressRepository.Object);

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

        var accountRepository = new Mock<IAccountRepository>();

        var loginIpAddressRepository = new Mock<ILoginIpAddressRepository>();
        loginIpAddressRepository.Setup(x => x.Get(ipAddress, default)).ReturnsAsync((LoginIpAddress?) null);

        var loginAuditService = new LoginAuditService(_dateTimeProvider.Object, accountRepository.Object,
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

        var accountRepository = new Mock<IAccountRepository>();

        var loginIpAddressRepository = new Mock<ILoginIpAddressRepository>();
        loginIpAddressRepository.Setup(x => x.Get(ipAddress, default)).ReturnsAsync(loginIpAddress);

        var loginAuditService = new LoginAuditService(_dateTimeProvider.Object, accountRepository.Object,
            loginIpAddressRepository.Object);

        // Act
        var (permanentlyLocked, temporaryLock) = await loginAuditService.GetIpAddressBlockadeStatus(ipAddress);

        // Assert
        permanentlyLocked.Should().Be(expectedPermanentLock);
        temporaryLock.Should().Be(expectedTemporaryLock);
    }
}