using System.IdentityModel.Tokens.Jwt;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using PasswordWallet.Server.Entities;
using PasswordWallet.Server.Repositories;
using PasswordWallet.Server.Services;
using PasswordWallet.Server.Utils;
using PasswordWallet.Shared.Dtos;

namespace PasswordWallet.UnitTests.ServicesTests;

public class AuthServiceTests
{
    [Fact]
    public void GenerateJwtToken_AccountId_ShouldCreateValidToken()
    {
        // Arrange
        const int accountId = 7;

        var appSettings = Options.Create(new AppSettings
        {
            JwtSigningKey =
                "72acc023-1458-43cc-af51-50123b98f6f836392d91-e1ad-470c-89d3-c24b554a34e22443eeb2-5099-45db-9819-ee3ae720350a"
        });
        var accountRepository = new Mock<IAccountRepository>();
        var cryptoService = new Mock<ICryptoService>();

        var authService = new AuthService(appSettings, accountRepository.Object, cryptoService.Object);

        // Act
        var token = authService.GenerateJwtToken(accountId);
        JwtSecurityToken? validatedToken = null;

        var action = () =>
        {
            new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSettings.Value.JwtSigningKey)),
                ValidateIssuer = false,
                ValidateAudience = false
            }, out var outToken);
            validatedToken = outToken as JwtSecurityToken;
        };

        // Assert
        action.Should().NotThrow();
        validatedToken.Should().NotBeNull();
        validatedToken!.ValidFrom.Date.Should().Be(DateTime.Today.Date);
        validatedToken.ValidTo.Date.Should().Be(DateTime.Today.AddDays(1).Date);
        validatedToken.Claims.Should()
            .ContainSingle(x => x.Type == Constants.AccountIdClaim && x.Value == accountId.ToString());
    }

    [Fact]
    public async Task
        AreCredentialsValid_ValidCredentialsAndPasswordKeptAsHash_ShouldReturnTrueAndAccountIdAndUseSha512()
    {
        // Arrange
        const long accountId = 2;
        const string passwordHash =
            "8d9ba15292ec06f92c9c9a4373bd1d701819b519803fac89d3bbd3229694187b228b3efc42818f64890cbb184d7bb698c4035fb6ada3badad1f0973d64b97c72";
        const string salt = "3c622e473c1be769e4c995084c84a2b8836ff9a66a435196fa09ac4788d3a3b4";

        var loginRequest = new LoginRequestDto
        {
            Login = "test123",
            Password = "qwerty123"
        };

        var account = new Account
        {
            Id = accountId, Login = loginRequest.Login, IsPasswordKeptAsHash = true, PasswordHash = passwordHash,
            Salt = salt
        };

        var appSettings = Options.Create(new AppSettings {PasswordPepper = "f0738b1b7b68f8c74fcd9411b780ac59"});

        var accountRepository = new Mock<IAccountRepository>();
        accountRepository.Setup(x => x.Get(loginRequest.Login, default)).ReturnsAsync(account);

        var cryptoService = new Mock<ICryptoService>();
        cryptoService.Setup(x => x.HashSha512(loginRequest.Password, account.Salt))
            .Returns((passwordHash, account.Salt));

        var authService = new AuthService(appSettings, accountRepository.Object, cryptoService.Object);

        // Act
        var result = await authService.AreCredentialsValid(loginRequest);

        // Assert
        cryptoService.Verify(x => x.HashSha512(loginRequest.Password, account.Salt), Times.Once);
        result.valid.Should().Be(true);
        result.accountId.Should().Be(accountId);
    }

    [Fact]
    public async Task AreCredentialsValid_ValidCredentialsAndPasswordKeptAsHmac_ShouldReturnTrueAndAccountIdAndUseHmac()
    {
        // Arrange
        const long accountId = 3;
        const string passwordHash =
            "6e5a42d0e03f42922d9d5ebdcf8ff7f92f8b0f7ac668671c0ce325a74c055c2c57665f1b9e0cb822b7ad4e85707955fd5b8d3fb56e91a73ae74cef1de2a200b3";
        const string salt = "e13a5658fdb0f53badbfaf1d7f8c7bd76b78ae10a0aa0a086359d946e52bd574";
        var saltBytes = new byte[]
        {
            225, 58, 86, 88, 253, 176, 245, 59, 189, 250, 241, 215, 248, 199, 189, 118, 183, 138, 225, 10, 160, 160,
            160, 134, 53, 157, 70, 229, 82, 189, 87, 116
        };

        var loginRequest = new LoginRequestDto
        {
            Login = "test321",
            Password = "qwerty321"
        };

        var account = new Account
        {
            Id = accountId, Login = loginRequest.Login, IsPasswordKeptAsHash = false, PasswordHash = passwordHash,
            Salt = salt
        };

        var appSettings = Options.Create(new AppSettings());

        var accountRepository = new Mock<IAccountRepository>();
        accountRepository.Setup(x => x.Get(loginRequest.Login, default)).ReturnsAsync(account);

        var cryptoService = new Mock<ICryptoService>();
        cryptoService.Setup(x => x.HexStringToBytes(account.Salt)).Returns(saltBytes);
        cryptoService.Setup(x => x.HashHmac(loginRequest.Password, cryptoService.Object.HexStringToBytes(account.Salt)))
            .Returns((passwordHash, salt));

        var authService = new AuthService(appSettings, accountRepository.Object, cryptoService.Object);

        // Act
        var result = await authService.AreCredentialsValid(loginRequest);

        // Assert
        cryptoService.Verify(
            x => x.HashHmac(loginRequest.Password, cryptoService.Object.HexStringToBytes(account.Salt)),
            Times.Once);
        result.valid.Should().Be(true);
        result.accountId.Should().Be(accountId);
    }

    [Fact]
    public async Task AreCredentialsValid_NotExistingLogin_ShouldReturnFalseAndNullAccountId()
    {
        // Arrange
        const string passwordHash =
            "fa8ec7c278964de5056241c82df3dda2054592f451a8789ed70f092006368a5bf8cf358c809ddb481d7a761ca2fb90537d3b0297ca59cc3924f6cf936b503cf9";

        var loginRequest = new LoginRequestDto
        {
            Login = "test321",
            Password = passwordHash
        };

        var appSettings = Options.Create(new AppSettings());

        var accountRepository = new Mock<IAccountRepository>();
        accountRepository.Setup(x => x.Get(loginRequest.Login, default)).ReturnsAsync((Account) null!);

        var cryptoService = new Mock<ICryptoService>();

        var authService = new AuthService(appSettings, accountRepository.Object, cryptoService.Object);

        // Act
        var result = await authService.AreCredentialsValid(loginRequest);

        // Assert
        result.valid.Should().Be(false);
        result.accountId.Should().Be(null);
    }

    [Fact]
    public async Task AreCredentialsValid_InvalidPasswordSha512_ShouldReturnFalseAndAccountId()
    {
        // Arrange
        const long accountId = 5;
        const string validPasswordHash =
            "3a08298e6cc65124699eeec5b28249b1e84b2086f4b63068078619b51d105116a222f5a64dacd6779d3b28f53e29d40d1bef83f9904b6099abcbb9fd9941aaf5";
        const string invalidPasswordHash =
            "d5b296223747f8e74eecf3108b12065d5c03b26d1352f7b1a157c95c0179ad189f4bd3f88cf77d15e68a3e7dd97325de7bc5538495cbffedb34f4192ef432dca";
        const string salt = "2de0020d6cae5e63576b4d3f227d0268b23b2aad2445baea7c7b1d5fcc176176";


        var loginRequest = new LoginRequestDto
        {
            Login = "mondi123",
            Password = "xyz123"
        };

        var account = new Account
        {
            Id = accountId, Login = loginRequest.Login, IsPasswordKeptAsHash = true, PasswordHash = validPasswordHash,
            Salt = salt
        };

        var appSettings = Options.Create(new AppSettings {PasswordPepper = "f0738b1b7b68f8c74fcd9411b780ac59"});

        var accountRepository = new Mock<IAccountRepository>();
        accountRepository.Setup(x => x.Get(loginRequest.Login, default)).ReturnsAsync(account);

        var cryptoService = new Mock<ICryptoService>();
        cryptoService.Setup(x => x.HashSha512(loginRequest.Password, account.Salt))
            .Returns((invalidPasswordHash, account.Salt));

        var authService = new AuthService(appSettings, accountRepository.Object, cryptoService.Object);

        // Act
        var result = await authService.AreCredentialsValid(loginRequest);

        // Assert
        cryptoService.Verify(x => x.HashSha512(loginRequest.Password, account.Salt), Times.Once);
        result.valid.Should().Be(false);
        result.accountId.Should().Be(accountId);
    }

    [Fact]
    public async Task AreCredentialsValid_InvalidPasswordHmac_ShouldReturnFalseAndAccountId()
    {
        // Arrange
        const long accountId = 8;
        const string validPasswordHash =
            "b8bb31620275486688fc61ffb7bb59ed71bbc8e1a67f32d9e7344f3b2ce41548af5a9cbf30f32400f89c35f416afa28afb4ca0f24efd59ab845a825374ad49ee";
        const string invalidPasswordHash =
            "3d4eade7fb4d094b45a244296f4690980bf0e19965770b1aaad70ab4a51f23e33fa2af378025655a548fb5e512faf42b2a42046bfe9bb5aa9a3e42e32f4670e8";
        const string salt = "8c29918b8092d3308b56beece5de940689b4110fad7c96b938aaaba275ac38d8";
        var saltBytes = new byte[]
        {
            140, 41, 145, 139, 128, 146, 211, 48, 139, 86, 190, 236, 229, 222, 148, 6, 137, 180, 17, 15, 173, 199, 150,
            185, 56, 170, 171, 178, 117, 172, 56, 216
        };

        var loginRequest = new LoginRequestDto
        {
            Login = "arity245",
            Password = "fun6543"
        };

        var account = new Account
        {
            Id = accountId, Login = loginRequest.Login, IsPasswordKeptAsHash = false, PasswordHash = validPasswordHash,
            Salt = salt
        };

        var appSettings = Options.Create(new AppSettings());

        var accountRepository = new Mock<IAccountRepository>();
        accountRepository.Setup(x => x.Get(loginRequest.Login, default)).ReturnsAsync(account);

        var cryptoService = new Mock<ICryptoService>();
        cryptoService.Setup(x => x.HexStringToBytes(account.Salt)).Returns(saltBytes);
        cryptoService.Setup(x => x.HashHmac(loginRequest.Password, cryptoService.Object.HexStringToBytes(account.Salt)))
            .Returns((invalidPasswordHash, account.Salt));

        var authService = new AuthService(appSettings, accountRepository.Object, cryptoService.Object);

        // Act
        var result = await authService.AreCredentialsValid(loginRequest);

        // Assert
        cryptoService.Verify(
            x => x.HashHmac(loginRequest.Password, cryptoService.Object.HexStringToBytes(account.Salt)), Times.Once);
        result.valid.Should().Be(false);
        result.accountId.Should().Be(accountId);
    }

    [Fact]
    public async Task ChangePassword_PasswordKeptAsHash_ShouldChangePasswordUsingSha512AndUpdateCredentialsEncryption()
    {
        // Arrange
        const long accountId = 15;
        const string oldPasswordHash =
            "b605a9388d39f92cdbfc8d3b042a5e46ef388c6718c8d6b5210e283fe9ef4a3ca09f652665b52ad026a3b22ee3b3e657614a7739141f7d887c7fff1cb3d6b892";
        const string newPasswordHash =
            "0a900cb02a4fa771e55d69525ce119a0f2749d8bd43a8f7659c63d2a8d405c0f43b1fa600e4b2a0d04ff4ce456c5894b42c6c6b38b25fe28bc6d206efda79e25";
        const string newSalt = "d5886de97bf45effbe6a6d88d32db61f585e3951630e42c71422aad89970c025";
        const bool isPasswordKeptAsHash = true;
        const string newFirstCredentialPassword = "dd814a742967cc07c4b47f05ad0536ced8f9d16859e1f7851275d79c7284f42d";
        const string newSecondCredentialPassword = "aa708250d8b7948c00e38698ec5249d1efa7b79f18b7e8b229386c3fa1cfc731";


        var account = new Account
        {
            Id = accountId, Login = "test654", IsPasswordKeptAsHash = isPasswordKeptAsHash,
            PasswordHash = oldPasswordHash, Salt = "4161a2e5fd673f01ff680b1a15d8a4cf3d622682f34891fa3427ffe16a205462",
            Credentials = new List<Credentials>
            {
                new()
                {
                    AccountId = accountId, Login = "xyz",
                    Password = "18c694d0fb12313c4257edda8600bd64a1c8c30d49af44905c8491c3ad34ffff"
                },
                new()
                {
                    AccountId = accountId, Login = "pluto",
                    Password = "39b34d3420234dceedd59545942d5d0540b06c4d720f06efff31b2c3b8cd5880"
                }
            }
        };

        var expectedAccount = new Account
        {
            Id = accountId, Login = "test654", IsPasswordKeptAsHash = isPasswordKeptAsHash,
            PasswordHash = newPasswordHash, Salt = newSalt,
            Credentials = new List<Credentials>
            {
                new() {AccountId = accountId, Login = "xyz", Password = newFirstCredentialPassword},
                new() {AccountId = accountId, Login = "pluto", Password = newSecondCredentialPassword}
            }
        };

        var appSettings = Options.Create(new AppSettings {PasswordPepper = "4f03b00d2fe7edff9984a065bea65f70"});

        var accountRepository = new Mock<IAccountRepository>();
        accountRepository.Setup(x => x.GetWithCredentials(accountId, default)).ReturnsAsync(account);
        accountRepository.Setup(x => x.Update(account)).Verifiable();
        accountRepository.Setup(x => x.SaveChanges(default));

        var cryptoService = new Mock<ICryptoService>();
        cryptoService.Setup(x => x.HashSha512(It.IsAny<string>(), null)).Returns((newPasswordHash, newSalt));
        cryptoService.Setup(x => x.AesDecryptToString(It.IsAny<string>(), oldPasswordHash)).Returns(It.IsAny<string>());
        cryptoService.SetupSequence(x => x.AesEncryptToHexString(It.IsAny<string>(), newPasswordHash))
            .Returns(newFirstCredentialPassword).Returns(newSecondCredentialPassword);

        var authService = new AuthService(appSettings, accountRepository.Object, cryptoService.Object);

        // Act
        await authService.ChangePassword(accountId, It.IsAny<string>(), isPasswordKeptAsHash);

        // Assert
        accountRepository.Verify(x => x.Update(account), Times.Once);
        cryptoService.Verify(x => x.HashSha512(It.IsAny<string>(), null), Times.Once);
        cryptoService.Verify(x => x.AesDecryptToString(It.IsAny<string>(), oldPasswordHash), Times.Exactly(2));
        cryptoService.Verify(x => x.AesEncryptToHexString(It.IsAny<string>(), newPasswordHash), Times.Exactly(2));

        account.Should().BeEquivalentTo(expectedAccount);
    }

    [Fact]
    public async Task ChangePassword_PasswordKeptAsHmac_ShouldChangePasswordUsingHmacAndUpdateCredentialsEncryption()
    {
        // Arrange
        const long accountId = 20;
        const string oldPasswordHash =
            "406188c752c8294f3162a107f0ab070bb31cad1e0ea1f58a84fae7b8e344e23153dfa2946c538da9df81f5e93f967f3fa31a308efd9d774d3dfce50fce8248f3";
        const string newPasswordHash =
            "655e41f0bf6b71b0975c805da8eff0cf2a531878d1ba182135fac6120880ca73e73492de18ca8b139c8fa6672b66195ae92f01582e9bde93a045c3d2c38947cd";
        const string newSalt = "b4f88fbffbe04abf7f7cc022bea7c025e47959406312899b30dd15bba47bdff3";
        const bool isPasswordKeptAsHash = false;
        const string newFirstCredentialPassword = "9e78a69bd7358bb7ffafbc6cfa361f3ba4af3326236eff27b1f4fe376e66a1c4";
        const string newSecondCredentialPassword = "a145c7cb6f04015f0ce55749c7c927cbd4c689e569c837c604cd3f0ab53d9c55";


        var account = new Account
        {
            Id = accountId, Login = "byget222", IsPasswordKeptAsHash = isPasswordKeptAsHash,
            PasswordHash = oldPasswordHash, Salt = "1dfabc1cf39985e5963de50cc2f5c269e3cd950810a0d2a712e6abd1c37f419f",
            Credentials = new List<Credentials>
            {
                new()
                {
                    AccountId = accountId, Login = "qwerty",
                    Password = "193f392ff8825876c6d846189d3a302af89bfae552a81563f4c73d031d42e56d"
                },
                new()
                {
                    AccountId = accountId, Login = "moon234",
                    Password = "3157bddd884e139c96bc0ef3c05210c8bf65cbc333c4ed9026d6de24710bcbaa"
                }
            }
        };

        var expectedAccount = new Account
        {
            Id = accountId, Login = "byget222", IsPasswordKeptAsHash = isPasswordKeptAsHash,
            PasswordHash = newPasswordHash, Salt = newSalt,
            Credentials = new List<Credentials>
            {
                new() {AccountId = accountId, Login = "qwerty", Password = newFirstCredentialPassword},
                new() {AccountId = accountId, Login = "moon234", Password = newSecondCredentialPassword}
            }
        };

        var appSettings = Options.Create(new AppSettings());

        var accountRepository = new Mock<IAccountRepository>();
        accountRepository.Setup(x => x.GetWithCredentials(accountId, default)).ReturnsAsync(account);
        accountRepository.Setup(x => x.Update(account)).Verifiable();
        accountRepository.Setup(x => x.SaveChanges(default));

        var cryptoService = new Mock<ICryptoService>();
        cryptoService.Setup(x => x.HashHmac(It.IsAny<string>(), null)).Returns((newPasswordHash, newSalt));
        cryptoService.Setup(x => x.AesDecryptToString(It.IsAny<string>(), oldPasswordHash)).Returns(It.IsAny<string>());
        cryptoService.SetupSequence(x => x.AesEncryptToHexString(It.IsAny<string>(), newPasswordHash))
            .Returns(newFirstCredentialPassword).Returns(newSecondCredentialPassword);

        var authService = new AuthService(appSettings, accountRepository.Object, cryptoService.Object);

        // Act
        await authService.ChangePassword(accountId, It.IsAny<string>(), isPasswordKeptAsHash);

        // Assert
        accountRepository.Verify(x => x.Update(account), Times.Once);
        cryptoService.Verify(x => x.HashHmac(It.IsAny<string>(), null), Times.Once);
        cryptoService.Verify(x => x.AesDecryptToString(It.IsAny<string>(), oldPasswordHash), Times.Exactly(2));
        cryptoService.Verify(x => x.AesEncryptToHexString(It.IsAny<string>(), newPasswordHash), Times.Exactly(2));

        account.Should().BeEquivalentTo(expectedAccount);
    }
}