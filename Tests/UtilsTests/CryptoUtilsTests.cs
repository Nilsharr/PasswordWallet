using System.Security.Cryptography;
using FluentAssertions;
using PasswordWallet.Server.Utils;

namespace PasswordWallet.UnitTests.UtilsTests;

public class CryptoUtilsTests
{
    private const string EncryptionPassword = "ffc121a2210958bf74e5a874668f3d978d24b6a8241496ccff3c0ea245e4f126";

    [Theory]
    [InlineData("i5@VM")]
    [InlineData("W^jrgF@9")]
    [InlineData("s74XTrMng8AsFr2R9f")]
    public void AesEncryptToHexString_ValidString_ShouldEncryptToHexString(string value)
    {
        // Act
        var actual = CryptoUtils.AesEncryptToHexString(value, EncryptionPassword);

        // Assert
        actual.Should().NotBeNullOrWhiteSpace().And.NotBe(value).And.BeHexString();
    }

    [Theory]
    [InlineData("690EF855B3F462D44E3279844A1234BCABB347E09BB42B42B24F64828A5D6B49", "test")]
    [InlineData("597DF82D85EDF6240920D630F0A2BDB021A47F2602694A4E9F8643EC4C2325F4", "UsVf8@yZ")]
    [InlineData("CB474EB58ED5E1F7195BE1E6FD9CBEF9260946F39902AEA0D8F2C90FA4CAA775", "Zr$KWs3g")]
    public void AesDecryptToString_ValidEncryptedHexString_ShouldReturnValidDecryptedString(string encrypted,
        string expectedResult)
    {
        // Act
        var actual = CryptoUtils.AesDecryptToString(encrypted, EncryptionPassword);

        // Assert
        actual.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("8F5A45D5DC476F9FB043CD8090136483E6D1B3816EF750B7807C9DB97D65D6D1")]
    [InlineData("29406B9711F6247B2E559C9E015F8FE5DF3BB5EE740EF7D946CFE89659EE0EFC")]
    [InlineData("BB170EE97F196BC367C655861F1A8A902048CF330DCB2D435B284817F511D86B")]
    public void AesDecryptToString_InvalidEncryptedHexString_ShouldThrowCryptographicException(string encrypted)
    {
        // Act
        Action action = () => CryptoUtils.AesDecryptToString(encrypted, EncryptionPassword);

        // Assert
        action.Should().Throw<CryptographicException>();
    }

    [Theory]
    [InlineData("T39N55RM5TZXV45SNG7QVC2YKQYC7FVEDSDBPBHPSN26KR7J47JEVCZ5UU36EA52")]
    [InlineData("HHTYMMLVJNG3D9G6LP9U2CKNGCAT8B5Z6KEKYJSYD5X4GSZVP9SNHLQ3KFURWPFBSR6WTKZSKDJ")]
    [InlineData("QBDAXC7XVQ67WDBYDYHSZ5232MRZPAULYYNYWQ4FFEQUMK8JC8P7FXUV9G")]
    public void AesDecryptToString_EncryptedStringWithInvalidFormat_ShouldThrowFormatException(string encrypted)
    {
        // Act
        Action action = () => CryptoUtils.AesDecryptToString(encrypted, EncryptionPassword);

        // Assert
        action.Should().Throw<FormatException>();
    }

    [Theory]
    [InlineData("$8$3Zu")]
    [InlineData("R)jeXoe4")]
    [InlineData("n(mRW7uHThn14K")]
    public void AesDecryptToString_ValidString_ShouldReverseEncryption(string value)
    {
        // Act
        var actual = CryptoUtils.AesDecryptToString(CryptoUtils.AesEncryptToHexString(value, EncryptionPassword),
            EncryptionPassword);

        // Assert
        actual.Should().Be(value);
    }

    [Theory]
    [InlineData(8)]
    [InlineData(16)]
    [InlineData(32)]
    [InlineData(64)]
    public void GenerateSalt_ValidLength_ShouldReturnValidSalt(int length)
    {
        // Act
        var actual = CryptoUtils.GenerateSalt(length);

        // Assert
        actual.Should().BeHexString().And.HaveLength(length);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(-32)]
    public void GenerateSalt_InvalidLength_ShouldThrowArgumentException(int length)
    {
        // Act
        Action action = () => CryptoUtils.GenerateSalt(length);

        // Assert
        action.Should().Throw<ArgumentException>().WithMessage("*Argument must be a positive number*");
    }

    [Theory]
    [InlineData("1b1a296f47e1829a", new byte[] {0x1b, 0x1a, 0x29, 0x6f, 0x47, 0xe1, 0x82, 0x9a})]
    [InlineData("3e74e8f8a78e5ef635b07103735316c6",
        new byte[] {0x3e, 0x74, 0xe8, 0xf8, 0xa7, 0x8e, 0x5e, 0xf6, 0x35, 0xb0, 0x71, 0x03, 0x73, 0x53, 0x16, 0xc6})]
    [InlineData("0103ef4bf2c0ac0ca4dd2b5a74d9969625eee49515968608968e55e139d4e08c",
        new byte[]
        {
            0x01, 0x03, 0xef, 0x4b, 0xf2, 0xc0, 0xac, 0x0c, 0xa4, 0xdd, 0x2b, 0x5a, 0x74, 0xd9, 0x96, 0x96, 0x25, 0xee,
            0xe4, 0x95, 0x15, 0x96, 0x86, 0x08, 0x96, 0x8e, 0x55, 0xe1, 0x39, 0xd4, 0xe0, 0x8c
        })]
    public void HexStringToBytes_ValidHexString_ShouldReturnByteArray(string value, byte[] expectedValue)
    {
        // Act
        var actual = CryptoUtils.HexStringToBytes(value);

        // Assert
        actual.Should().Equal(expectedValue);
    }

    [Fact]
    public void GenerateSalt_NullValue_ShouldThrowArgumentException()
    {
        // Act
        Action action = () => CryptoUtils.HexStringToBytes(null!);

        // Assert
        action.Should().Throw<ArgumentException>().WithMessage("*Argument cannot be null*");
    }
}