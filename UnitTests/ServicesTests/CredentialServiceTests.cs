using FluentAssertions;
using Moq;
using PasswordWallet.Server.Entities;
using PasswordWallet.Server.Repositories;
using PasswordWallet.Server.Services;

namespace PasswordWallet.UnitTests.ServicesTests;

public class CredentialServiceTests
{
    [Fact]
    public async Task EncryptAndSaveCredential_ValidCredential_ShouldSaveAndEncryptPassword()
    {
        // Arrange
        const long accountId = 2;
        const string accountPassword =
            "898c56dd5d608e08044b324d730f3cc96a7065ed7ed61830562cf749dfcd74b04f62ffea9ef8bd143f1a894efbc90fd0d3a2f50961832f85d2f032a919e78890";
        const string credentialPassword = "pass123";
        const string encryptedCredentialPassword = "bce17f4d6a82fa94757141ff63d63c768902e58175ac3e195a99b50a2ad1a7b2";

        var account = new Account
        {
            Id = accountId,
            Login = "mebmeb678",
            PasswordHash = accountPassword
        };

        var credential = new Credential
        {
            Login = "login123",
            Password = credentialPassword,
            AccountId = accountId
        };

        var expectedCredential = new Credential
        {
            Id = 1,
            Login = "login123",
            Password = encryptedCredentialPassword,
            AccountId = accountId
        };

        var accountRepository = new Mock<IAccountRepository>();
        accountRepository.Setup(x => x.Get(accountId, default)).ReturnsAsync(account);

        var credentialRepository = new Mock<ICredentialRepository>();
        credentialRepository.Setup(x => x.Add(credential)).Callback(() => credential.Id = 1);
        credentialRepository.Setup(x => x.SaveChanges(default));

        var cryptoService = new Mock<ICryptoService>();
        cryptoService.Setup(x => x.AesEncryptToHexString(credentialPassword, accountPassword))
            .Returns(encryptedCredentialPassword);

        var credentialService =
            new CredentialService(accountRepository.Object, credentialRepository.Object, cryptoService.Object);

        // Act
        var returnedCredential = await credentialService.EncryptAndSaveCredential(accountId, credential);

        // Assert
        cryptoService.Verify(x => x.AesEncryptToHexString(credentialPassword, accountPassword), Times.Once);
        credentialRepository.Verify(x => x.Add(credential), Times.Once);
        credentialRepository.Verify(x => x.SaveChanges(default), Times.Once);

        returnedCredential.Should().BeEquivalentTo(expectedCredential);
    }

    [Fact]
    public async Task EncryptAndUpdateCredential_PasswordChanged_ShouldUpdateAndEncryptPassword()
    {
        // Arrange
        const long accountId = 3;
        const string accountPassword =
            "ded64b24f9b64ade7980a19d9fff8e42f1c3264fc7fa197ae98a6c573ed558b9222b59deab98de6d7b901b0f578871253b68161caa3229e9edd3902d4c60519e";
        const string currentCredentialPassword = "cdf4c41cc5e85ae648fff39f85408c9177e26efc4832946bddeb7ae5e15f4411";
        const string newCredentialPassword = "963147826";
        const string encryptedNewCredentialPassword =
            "e48083b2d0ecfae9b9bd7e3d188a71984843619fc65741e6f66ba1bd41aaaf0c";

        var account = new Account
        {
            Id = accountId,
            Login = "tybnem357",
            PasswordHash = accountPassword
        };

        var credential = new Credential
        {
            Id = 6,
            Login = "atyr343",
            Password = newCredentialPassword,
            AccountId = accountId
        };

        var expectedCredential = new Credential
        {
            Id = 6,
            Login = "atyr343",
            Password = encryptedNewCredentialPassword,
            AccountId = accountId
        };

        var accountRepository = new Mock<IAccountRepository>();
        accountRepository.Setup(x => x.Get(accountId, default)).ReturnsAsync(account);

        var credentialRepository = new Mock<ICredentialRepository>();
        credentialRepository.Setup(x => x.GetPassword(credential.Id, default)).ReturnsAsync(currentCredentialPassword);
        credentialRepository.Setup(x => x.Update(credential)).Verifiable();
        credentialRepository.Setup(x => x.SaveChanges(default));

        var cryptoService = new Mock<ICryptoService>();
        cryptoService.Setup(x => x.AesEncryptToHexString(newCredentialPassword, accountPassword))
            .Returns(encryptedNewCredentialPassword);

        var credentialService =
            new CredentialService(accountRepository.Object, credentialRepository.Object, cryptoService.Object);

        // Act
        var returnedCredential = await credentialService.EncryptAndUpdateCredential(accountId, credential);

        // Assert
        cryptoService.Verify(x => x.AesEncryptToHexString(newCredentialPassword, accountPassword), Times.Once);
        credentialRepository.Verify(x => x.Update(credential), Times.Once);
        credentialRepository.Verify(x => x.SaveChanges(default), Times.Once);

        returnedCredential.Should().BeEquivalentTo(expectedCredential);
    }

    [Fact]
    public async Task EncryptAndUpdateCredential_PasswordNotChanged_ShouldUpdate()
    {
        // Arrange
        const long accountId = 4;
        const string currentCredentialPassword = "bc1fb06c80bd76d9e7d9b39ba600e355b5d974b0ebef3d0ec814388de10cfc2e";

        var credential = new Credential
        {
            Id = 8,
            Login = "bygom333",
            Password = currentCredentialPassword,
            AccountId = accountId
        };

        var accountRepository = new Mock<IAccountRepository>();
        var cryptoService = new Mock<ICryptoService>();

        var credentialRepository = new Mock<ICredentialRepository>();
        credentialRepository.Setup(x => x.GetPassword(credential.Id, default)).ReturnsAsync(currentCredentialPassword);
        credentialRepository.Setup(x => x.Update(credential)).Verifiable();
        credentialRepository.Setup(x => x.SaveChanges(default));

        var credentialService =
            new CredentialService(accountRepository.Object, credentialRepository.Object, cryptoService.Object);

        // Act
        var returnedCredential = await credentialService.EncryptAndUpdateCredential(accountId, credential);

        // Assert
        cryptoService.Verify(x => x.AesEncryptToHexString(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        credentialRepository.Verify(x => x.Update(credential), Times.Once);
        credentialRepository.Verify(x => x.SaveChanges(default), Times.Once);

        returnedCredential.Should().BeEquivalentTo(credential);
    }
}