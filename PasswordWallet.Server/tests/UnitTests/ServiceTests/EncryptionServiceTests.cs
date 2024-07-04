using System.Security.Cryptography;
using FluentAssertions;
using Infrastructure.Cryptography;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace UnitTests.ServiceTests;

public class EncryptionServiceTests
{
    private readonly ILogger<EncryptionService> _logger = Substitute.For<ILogger<EncryptionService>>();

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("r6G*79QF18")]
    [InlineData("Y3YV&n4y6n")]
    [InlineData("J()Mk4zc1^sd")]
    public void Encrypt_Always_ShouldReturnEncryptedText(string plainText)
    {
        const string key = "978e001e7d12fcaf99c3a2b561f25d39";
        var encryptionService = new EncryptionService(_logger);

        var result = encryptionService.Encrypt(plainText, key);

        result.Should().NotBeNullOrWhiteSpace().And.NotBe(plainText);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("r6G*79QF18")]
    [InlineData("Y3YV&n4y6n")]
    [InlineData("J()Mk4zc1^sd")]
    public void Encrypt_DifferentKeys_ShouldProduceDifferentEncryption(string plainText)
    {
        const string key = "b893a5562c5fb922a2e3f9eddcef8633";
        const string differentKey = "3e71865aeab731aa502a70eaf8763987";
        var encryptionService = new EncryptionService(_logger);

        var result = encryptionService.Encrypt(plainText, key);
        var differentResult = encryptionService.Encrypt(plainText, differentKey);

        result.Should().NotBe(differentResult);
    }

    [Fact]
    public void Decrypt_CorrectKey_ShouldReturnPlainText()
    {
        const string plainText = "XEhv9@iQ4lg*";
        const string key = "b226433e6bd0ae6646048581bf535343";
        var encryptionService = new EncryptionService(_logger);
        var encrypted = encryptionService.Encrypt(plainText, key);

        var result = encryptionService.Decrypt(encrypted, key);

        result.Should().Be(plainText);
    }

    [Fact]
    public void Decrypt_IncorrectKey_ShouldThrowCryptographicException()
    {
        const string plainText = "XEhv9@iQ4lg*";
        const string correctKey = "4bebc883967fc0613dc5d19b1c932971";
        const string incorrectKey = "9ff87973b0969a1f109f7d9135b35416";
        var encryptionService = new EncryptionService(_logger);
        var encrypted = encryptionService.Encrypt(plainText, correctKey);

        Action action = () => encryptionService.Decrypt(encrypted, incorrectKey);

        action.Should().Throw<CryptographicException>();
    }
}