using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace UnitTests.ServiceTests;

public class CredentialServiceTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IEncryptionService _encryptionService = Substitute.For<IEncryptionService>();

    [Theory]
    [InlineData("*v3&FWqk")]
    [InlineData("@ts@Wj2C")]
    [InlineData("H5!sRJLC")]
    public async Task EncryptAndSaveCredential_NotEmptyPassword_ShouldEncrypt(string password)
    {
        const long userId = 1;
        const string encryptedPassword = "15117b282328146ac6afebaa8acd80e7";
        const long position = 1;
        var credential = new Credential { Password = password };
        var credentialService = CreateCredentialService();
        _encryptionService.Encrypt(Arg.Any<string>(), Arg.Any<string>()).Returns(encryptedPassword);
        _unitOfWork.CredentialRepository.GetNextAvailablePosition(Arg.Any<Guid>()).Returns(position);

        var result = await credentialService.EncryptAndSaveCredential(userId, credential);

        result.Should().NotBeNull();
        result.Password.Should().Be(encryptedPassword);
        result.Position.Should().Be(position);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("      ")]
    public async Task EncryptAndSaveCredential_EmptyPassword_ShouldNotEncrypt(string? password)
    {
        const long userId = 1;
        const long position = 5;
        var credential = new Credential { Password = password };
        var credentialService = CreateCredentialService();
        _unitOfWork.CredentialRepository.GetNextAvailablePosition(Arg.Any<Guid>()).Returns(position);

        var result = await credentialService.EncryptAndSaveCredential(userId, credential);

        result.Should().NotBeNull();
        result.Password.Should().Be(password);
        result.Position.Should().Be(position);
    }

    [Theory]
    [InlineData("5bbE2c$a")]
    [InlineData("CmWgKW2!")]
    [InlineData("s#r97UKp")]
    public async Task EncryptAndUpdateCredential_NotEmptyPassword_ShouldEncrypt(string password)
    {
        const long userId = 1;
        const string encryptedPassword = "f942c2e463d4f8c854b0183ee3c80dcd";
        var oldCredential = new Credential { Password = "xyz123" };
        var updatedCredential = new Credential { Password = password };
        var credentialService = CreateCredentialService();
        _unitOfWork.CredentialRepository.Get(Arg.Any<long>()).Returns(oldCredential);
        _encryptionService.Encrypt(Arg.Any<string>(), Arg.Any<string>()).Returns(encryptedPassword);

        var result = await credentialService.EncryptAndUpdateCredential(userId, updatedCredential);

        result.Should().NotBeNull();
        result.Password.Should().Be(encryptedPassword);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("      ")]
    public async Task EncryptAndUpdateCredential_EmptyPassword_ShouldNotEncrypt(string? password)
    {
        const long userId = 1;
        var oldCredential = new Credential { Password = "xyz123" };
        var updatedCredential = new Credential { Password = password };
        var credentialService = CreateCredentialService();
        _unitOfWork.CredentialRepository.Get(Arg.Any<long>()).Returns(oldCredential);

        var result = await credentialService.EncryptAndUpdateCredential(userId, updatedCredential);

        result.Should().NotBeNull();
        result.Password.Should().Be(password);
    }

    [Fact]
    public async Task EncryptAndUpdateCredential_Always_ShouldUpdateUsername()
    {
        const long userId = 1;
        const string updatedUsername = "TestUser";
        var oldCredential = new Credential { Username = "User123" };
        var updatedCredential = new Credential { Username = updatedUsername };
        var credentialService = CreateCredentialService();
        _unitOfWork.CredentialRepository.Get(Arg.Any<long>()).Returns(oldCredential);

        var result = await credentialService.EncryptAndUpdateCredential(userId, updatedCredential);

        result.Should().NotBeNull();
        result.Username.Should().Be(updatedUsername);
    }

    [Fact]
    public async Task EncryptAndUpdateCredential_Always_ShouldUpdateWebAddress()
    {
        const long userId = 1;
        const string updatedWebAddress = "https://google.com";
        var oldCredential = new Credential { WebAddress = "https://duckduckgo.com" };
        var updatedCredential = new Credential { WebAddress = updatedWebAddress };
        var credentialService = CreateCredentialService();
        _unitOfWork.CredentialRepository.Get(Arg.Any<long>()).Returns(oldCredential);

        var result = await credentialService.EncryptAndUpdateCredential(userId, updatedCredential);

        result.Should().NotBeNull();
        result.WebAddress.Should().Be(updatedWebAddress);
    }

    [Fact]
    public async Task EncryptAndUpdateCredential_Always_ShouldUpdateDescription()
    {
        const long userId = 1;
        const string updatedDescription = "Lorem ipsum";
        var oldCredential = new Credential { Description = "Ipsum lorem", };
        var updatedCredential = new Credential { Description = updatedDescription };
        var credentialService = CreateCredentialService();
        _unitOfWork.CredentialRepository.Get(Arg.Any<long>()).Returns(oldCredential);

        var result = await credentialService.EncryptAndUpdateCredential(userId, updatedCredential);

        result.Should().NotBeNull();
        result.Description.Should().Be(updatedDescription);
    }

    [Fact]
    public async Task EncryptAndUpdateCredential_Always_ShouldNotUpdateId()
    {
        const long userId = 1;
        const long oldCredentialId = 5;
        const long newCredentialId = 8;
        var oldFolderId = Guid.NewGuid();
        var oldCredential = new Credential { Id = oldCredentialId, FolderId = oldFolderId };
        var updatedCredential = new Credential { Id = newCredentialId, FolderId = Guid.NewGuid() };

        var credentialService = CreateCredentialService();
        _unitOfWork.CredentialRepository.Get(Arg.Any<long>()).Returns(oldCredential);

        var result = await credentialService.EncryptAndUpdateCredential(userId, updatedCredential);

        result.Should().NotBeNull();
        result.Id.Should().Be(oldCredentialId);
        result.FolderId.Should().Be(oldFolderId);
    }

    [Fact]
    public async Task DecryptCredentialPassword_NotEmptyPassword_ShouldReturnDecryptedPassword()
    {
        const long userId = 1;
        const long credentialId = 2;
        const string encryptedPassword = "0800fc577294c34e0b28ad2839435945";
        const string decryptedPassword = "LNJY(77w";
        var credentialService = CreateCredentialService();
        _unitOfWork.CredentialRepository.GetPassword(Arg.Any<long>()).Returns(encryptedPassword);
        _encryptionService.Decrypt(Arg.Any<string>(), Arg.Any<string>()).Returns(decryptedPassword);

        var result = await credentialService.DecryptCredentialPassword(credentialId, userId);

        result.Should().Be(decryptedPassword);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("      ")]
    public async Task DecryptCredentialPassword_EmptyPassword_ShouldReturnEmptyPassword(string? password)
    {
        const long userId = 1;
        const long credentialId = 2;
        _unitOfWork.CredentialRepository.GetPassword(Arg.Any<long>()).Returns(password);

        var credentialService = CreateCredentialService();

        var result = await credentialService.DecryptCredentialPassword(credentialId, userId);

        result.Should().Be(password);
    }

    [Fact]
    public async Task UpdateCredentialsEncryption_NotEmptyPassword_ShouldUpdateEncryption()
    {
        const long userId = 1;
        const string oldUserPassword = "0800fc577294c34e0b28ad2839435945";
        const string newUserPassword = "5141dc8f82b51d5bbd493380ddf391c7";
        const string newEncryptedCredentialPassword = "19e4760d105b66856bdc585f4b2637e2";
        List<Credential> credentials = [new Credential { Password = "978e001e7d12fcaf99c3a2b561f25d39" }];
        var credentialService = CreateCredentialService();
        _unitOfWork.CredentialRepository.GetUserCredentials(Arg.Any<long>()).Returns(credentials);
        _encryptionService.Encrypt(Arg.Any<string>(), Arg.Any<string>()).Returns(newEncryptedCredentialPassword);

        await credentialService.UpdateCredentialsEncryption(userId, oldUserPassword, newUserPassword);

        credentials[0].Password.Should().Be(newEncryptedCredentialPassword);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("      ")]
    public async Task UpdateCredentialsEncryption_EmptyPasswords_ShouldNotEncrypt(string? password)
    {
        const long userId = 1;
        const string oldUserPassword = "0800fc577294c34e0b28ad2839435945";
        const string newUserPassword = "5141dc8f82b51d5bbd493380ddf391c7";
        List<Credential> credentials = [new Credential { Password = password }, new Credential { Password = password }];
        var credentialService = CreateCredentialService();
        _unitOfWork.CredentialRepository.GetUserCredentials(Arg.Any<long>()).Returns(credentials);

        await credentialService.UpdateCredentialsEncryption(userId, oldUserPassword, newUserPassword);

        foreach (var credential in credentials)
        {
            credential.Password.Should().Be(password);
        }
    }

    private CredentialService CreateCredentialService() =>
        new(_unitOfWork, _encryptionService, Substitute.For<ILogger<CredentialService>>());
}