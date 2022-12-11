using System.Security.Cryptography;
using FluentAssertions;
using Microsoft.Extensions.Options;
using PasswordWallet.Server.Services;
using PasswordWallet.Server.Utils;

namespace PasswordWallet.UnitTests.ServicesTests;

public class CryptoServiceTests
{
    private const string EncryptionPassword = "ffc121a2210958bf74e5a874668f3d978d24b6a8241496ccff3c0ea245e4f126";
    private const string PasswordPepper = "827F4D2BCDFF9CB80ECE978F17A0C6A5F609348B6464AB5A5D4CFD60435257B7";
    private readonly CryptoService _cryptoService;

    public CryptoServiceTests()
    {
        // Arrange
        var appSettings = Options.Create(new AppSettings());
        _cryptoService = new CryptoService(appSettings);
    }

    [Theory]
    [InlineData("e@8#sN",
        "AFDC22DA7A59140F47BBDC04B5688215CC863FB7CE119AA9D2AD936BBCB939207A25DA3EC804D2589002FFA1A81B77F05C4881327037302B19934C2E2F326831",
        "F294EEF14AA32AA056144793035CCD6581F686C0B601E0BF5C8B0036A0791D77F3B06265D491CF06C8C68B2023615367BAE219A9EFD7CE0623AFCBEFAFDC203D")]
    [InlineData("Tuj)8iUJ",
        "E624142A5A2B0AAA5974246FB951DAE54E14451405AE8B85CB4618C3907ADD2CD41DE8B912E1F2B1944DA4C1D2D5D7EB6AB7A46694A89236DF887531B73E3E2A",
        "35B5853424AC773331E741B28097E2E8C94A95932DB5606F0F80D49189FBED329D876D8E1D93909158DD1790D4E286160BE21EEDE4AE8E44D1D146D82756446E")]
    [InlineData("cs#D6RL2t6K",
        "DEE8A3F5C054D282705C1B3C9863FAD8EDA9789DDA49A1E3E61AB8F08FA162D5C2FA609ACBFE44E55579546B962D05C46137BC51C540E5C2C3BBBEF3F2314278",
        "F1CC546203F9B4AF12947D4913C44FA1B2D04404195A633E902578EFB15BE0DAE0CB7B946D8506A999ACF5884C2DEA36FB18E112066DC4BFC57E23FDE258FEF7")]
    public void HashSha512_ValidPassword_ShouldReturnHash(string password, string salt, string expectedHash)
    {
        //Arrange 
        var appSettings = Options.Create(new AppSettings {PasswordPepper = PasswordPepper});
        var cryptoService = new CryptoService(appSettings);

        // Act
        var actual = cryptoService.HashSha512(password, salt);

        // Assert
        actual.hashedPassword.Should().Be(expectedHash);
        actual.salt.Should().Be(salt);
    }

    [Theory]
    [InlineData("g4*vXL",
        new byte[]
        {
            173, 71, 177, 160, 120, 1, 15, 89, 192, 242, 176, 39, 158, 114, 181, 95, 150, 140, 93, 231, 159, 204,
            143, 226, 208, 78, 103, 100, 93, 128, 91, 151, 243, 208, 83, 25, 105, 58, 248, 144, 145, 9, 155, 126,
            121, 92, 165, 78, 204, 103, 119, 161, 194, 244, 158, 4, 137, 188, 7, 31, 212, 157, 251, 70
        },
        "3483DF981BC3D98CF5A1879CBD1F1650BC6AA8B66D323057EEAC367F62338C8FF33539B797FD3D684B158F03EB0E0B4A58E115D74B6F41F37543841A6E7315EF",
        "AD47B1A078010F59C0F2B0279E72B55F968C5DE79FCC8FE2D04E67645D805B97F3D05319693AF89091099B7E795CA54ECC6777A1C2F49E0489BC071FD49DFB46")]
    [InlineData("R*Ys9$GE", new byte[]
        {
            249, 205, 229, 167, 233, 156, 57, 118, 143, 192, 116, 178, 16, 186, 94, 146, 88, 16, 94, 129, 90, 61,
            20, 199, 176, 90, 49, 42, 184, 172, 122, 253, 157, 215, 128, 219, 255, 209, 192, 213, 78, 109, 9, 92,
            150, 30, 252, 70, 217, 178, 19, 202, 132, 1, 7, 179, 172, 42, 11, 203, 128, 0, 17, 85
        },
        "DC5A7BBB39325E1AD09BA5D8CF0FC90D7E6E7072145E34FD995F1EFCB8D3CE165AA42E166D8B78ACBB0C9607CA406D11182DB81AE5553A1416D1DDB2DA7432AE",
        "F9CDE5A7E99C39768FC074B210BA5E9258105E815A3D14C7B05A312AB8AC7AFD9DD780DBFFD1C0D54E6D095C961EFC46D9B213CA840107B3AC2A0BCB80001155")]
    [InlineData("7A(xPfRyXV5", new byte[]
        {
            242, 100, 205, 72, 29, 63, 151, 63, 47, 233, 42, 202, 235, 154, 42, 57, 221, 117, 147, 123, 101, 132,
            190, 210, 219, 252, 48, 55, 220, 67, 26, 51, 72, 164, 194, 10, 81, 115, 90, 24, 20, 126, 4, 143, 220, 31,
            127, 40, 148, 228, 204, 231, 99, 252, 243, 249, 86, 43, 206, 66, 20, 254, 95, 201
        },
        "7FB0DDF09983DC1F78D359303E35884AE6F06624702207F98C3F9C7AB5F51522F5154676D2B37427A5C98C9F4A9E42421E6DADD966CCE5DF0C312E76650A5020",
        "F264CD481D3F973F2FE92ACAEB9A2A39DD75937B6584BED2DBFC3037DC431A3348A4C20A51735A18147E048FDC1F7F2894E4CCE763FCF3F9562BCE4214FE5FC9")]
    public void HashHmac_ValidPassword_ShouldReturnHash(string password, byte[] key, string expectedHash,
        string expectedSalt)
    {
        // Act
        var actual = _cryptoService.HashHmac(password, key);

        // Assert
        actual.hashedPassword.Should().Be(expectedHash);
        actual.salt.Should().Be(expectedSalt);
    }

    [Theory]
    [InlineData("i5@VM")]
    [InlineData("W^jrgF@9")]
    [InlineData("s74XTrMng8AsFr2R9f")]
    public void AesEncryptToHexString_ValidString_ShouldEncryptToHexString(string value)
    {
        // Act
        var actual = _cryptoService.AesEncryptToHexString(value, EncryptionPassword);

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
        var actual = _cryptoService.AesDecryptToString(encrypted, EncryptionPassword);

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
        Action action = () => _cryptoService.AesDecryptToString(encrypted, EncryptionPassword);

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
        Action action = () => _cryptoService.AesDecryptToString(encrypted, EncryptionPassword);

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
        var actual = _cryptoService.AesDecryptToString(
            _cryptoService.AesEncryptToHexString(value, EncryptionPassword),
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
        var actual = _cryptoService.GenerateSalt(length);

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
        Action action = () => _cryptoService.GenerateSalt(length);

        // Assert
        action.Should().Throw<ArgumentException>().WithMessage("*Argument must be a positive number*");
    }

    [Theory]
    [InlineData("1b1a296f47e1829a", new byte[] {0x1b, 0x1a, 0x29, 0x6f, 0x47, 0xe1, 0x82, 0x9a})]
    [InlineData("3e74e8f8a78e5ef635b07103735316c6",
        new byte[]
        {
            0x3e, 0x74, 0xe8, 0xf8, 0xa7, 0x8e, 0x5e, 0xf6, 0x35, 0xb0, 0x71, 0x03, 0x73, 0x53, 0x16, 0xc6
        })]
    [InlineData("0103ef4bf2c0ac0ca4dd2b5a74d9969625eee49515968608968e55e139d4e08c",
        new byte[]
        {
            0x01, 0x03, 0xef, 0x4b, 0xf2, 0xc0, 0xac, 0x0c, 0xa4, 0xdd, 0x2b, 0x5a, 0x74, 0xd9, 0x96, 0x96, 0x25,
            0xee,
            0xe4, 0x95, 0x15, 0x96, 0x86, 0x08, 0x96, 0x8e, 0x55, 0xe1, 0x39, 0xd4, 0xe0, 0x8c
        })]
    public void HexStringToBytes_ValidHexString_ShouldReturnByteArray(string value, byte[] expectedValue)
    {
        // Act
        var actual = _cryptoService.HexStringToBytes(value);

        // Assert
        actual.Should().Equal(expectedValue);
    }

    [Fact]
    public void GenerateSalt_NullValue_ShouldThrowArgumentException()
    {
        // Act
        Action action = () => _cryptoService.HexStringToBytes(null!);

        // Assert
        action.Should().Throw<ArgumentException>().WithMessage("*Argument cannot be null*");
    }
}