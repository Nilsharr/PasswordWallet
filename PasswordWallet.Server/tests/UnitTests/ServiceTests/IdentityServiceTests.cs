using Core.Constants;
using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;
using FluentAssertions;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace UnitTests.ServiceTests;

public class IdentityServiceTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IPasswordHasher<User> _passwordHasher = Substitute.For<IPasswordHasher<User>>();
    private readonly ICredentialService _credentialService = Substitute.For<ICredentialService>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();

    [Fact]
    public async Task CreateNewUser_Always_ShouldReturnUser()
    {
        const string username = "TestUser";
        const string password = "Lr6a6#aT4g";
        const string passwordHash = "83c98f1a4422b6acf819671e2be2a2aa9b1a320c0e369ea944d255cddc2b6c20";
        var tokenResult = new TokenResult("7eb0532e95144186b85d670f2ce44", DateTimeOffset.UtcNow);
        _tokenService.GenerateRefreshToken().Returns(tokenResult);
        _passwordHasher.HashPassword(Arg.Any<User>(), Arg.Any<string>()).Returns(passwordHash);
        var userService = CreateUserService();

        var result = await userService.CreateNewUser(username, password);

        result.Should().NotBeNull();
        result.Username.Should().Be(username);
        result.PasswordHash.Should().Be(passwordHash);
        result.Folders.Should().HaveCount(1);
        result.Folders[0].Name.Should().Be(FolderConstants.DefaultFolderName);
        result.Folders[0].Position.Should().Be(1);
    }

    [Fact]
    public async Task Authenticate_ValidLoginData_ShouldReturnUser()
    {
        const long userId = 1;
        const string username = "TestUser";
        const string password = "Lr6a6#aT4g";
        var user = new User { Id = userId, Username = username };
        _unitOfWork.UserRepository.GetByUsername(Arg.Any<string>()).Returns(user);
        _passwordHasher.VerifyHashedPassword(Arg.Any<User>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(PasswordVerificationResult.Success);
        var userService = CreateUserService();

        var result = await userService.Authenticate(username, password);

        result.Should().NotBeNull();
        result!.Id.Should().Be(userId);
        result.Username.Should().Be(username);
    }

    [Fact]
    public async Task Authenticate_UsernameNotExists_ShouldReturnNull()
    {
        const string username = "TestUser";
        const string password = "Lr6a6#aT4g";
        _unitOfWork.UserRepository.GetByUsername(Arg.Any<string>()).ReturnsNull();
        var userService = CreateUserService();

        var result = await userService.Authenticate(username, password);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Authenticate_PasswordDoesntMatch_ShouldReturnNull()
    {
        const string username = "TestUser";
        const string password = "Lr6a6#aT4g";
        var user = new User { Id = 1, Username = username };
        _unitOfWork.UserRepository.GetByUsername(Arg.Any<string>()).Returns(user);
        _passwordHasher.VerifyHashedPassword(Arg.Any<User>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(PasswordVerificationResult.Failed);
        var userService = CreateUserService();

        var result = await userService.Authenticate(username, password);

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAuthenticationResponse_Always_ShouldReturnAuthenticationResponse()
    {
        const long userId = 1;
        const string username = "Test123";
        const bool withLastLoginDates = false;
        const string accessToken = "8d94eae3288c5518fcba56252b22ce4b";
        var accessTokenExpiry = DateTimeOffset.UtcNow.AddMinutes(5);
        const string refreshToken = "2ad5a2c46583cdfd168fe249159003fd";
        var refreshTokenExpiry = DateTimeOffset.UtcNow.AddHours(6);
        _tokenService.GenerateJwtToken(Arg.Any<long>()).Returns(new TokenResult(accessToken, accessTokenExpiry));
        _tokenService.GenerateRefreshToken().Returns(new TokenResult(refreshToken, refreshTokenExpiry));

        var userService = CreateUserService();

        var result = await userService.CreateAuthenticationResponse(userId, username, withLastLoginDates);

        result.Username.Should().Be(username);
        result.AccessToken.Should().Be(accessToken);
        result.AccessTokenExpiry.Should().Be(accessTokenExpiry);
        result.RefreshToken.Should().Be(refreshToken);
        result.RefreshTokenExpiry.Should().Be(refreshTokenExpiry);
    }

    [Fact]
    public async Task CreateAuthenticationResponse_WithLastLoginDates_ShouldReturnLastLoginDates()
    {
        const long userId = 1;
        const string username = "Test123";
        const bool withLastLoginDates = true;
        var lastValidLoginDate = DateTimeOffset.UtcNow.AddHours(-6);
        var lastInvalidLoginDate = DateTimeOffset.UtcNow.AddDays(-2);
        var userService = CreateUserService();
        _tokenService.GenerateJwtToken(Arg.Any<long>()).Returns(new TokenResult("xyz", DateTimeOffset.UtcNow));
        _tokenService.GenerateRefreshToken().Returns(new TokenResult("xyz", DateTimeOffset.UtcNow));
        _unitOfWork.LoginHistoryRepository.GetLastLoginDates(Arg.Any<long>())
            .Returns((lastValidLoginDate, lastInvalidLoginDate));

        var result = await userService.CreateAuthenticationResponse(userId, username, withLastLoginDates);

        result.LastSuccessfulLogin.Should().Be(lastValidLoginDate);
        result.LastUnsuccessfulLogin.Should().Be(lastInvalidLoginDate);
    }

    [Fact]
    public async Task CreateAuthenticationResponse_WithoutLastLoginDates_ShouldReturnNulls()
    {
        const long userId = 1;
        const string username = "Test123";
        const bool withLastLoginDates = false;
        var userService = CreateUserService();
        _tokenService.GenerateJwtToken(Arg.Any<long>()).Returns(new TokenResult("xyz", DateTimeOffset.UtcNow));
        _tokenService.GenerateRefreshToken().Returns(new TokenResult("xyz", DateTimeOffset.UtcNow));
        _unitOfWork.LoginHistoryRepository.GetLastLoginDates(Arg.Any<long>()).Returns((null, null));

        var result = await userService.CreateAuthenticationResponse(userId, username, withLastLoginDates);

        result.LastSuccessfulLogin.Should().BeNull();
        result.LastUnsuccessfulLogin.Should().BeNull();
    }

    [Fact]
    public async Task ChangePassword_ValidCurrentPassword_ShouldUpdateUserPassword()
    {
        const long userId = 1;
        const string currentPassword = "uz#8mbhGW5";
        const string newPassword = "Xdp8c^Dp5h";
        const string newPasswordHash = "4b7133a2b0090dc006f93991335276d24e6d80cdf41873d0c17f2188554249a0";
        var user = new User { Id = userId, Username = "TestUser" };
        _unitOfWork.UserRepository.Get(Arg.Any<long>()).Returns(user);
        _passwordHasher.VerifyHashedPassword(Arg.Any<User>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(PasswordVerificationResult.Success);
        _passwordHasher.HashPassword(Arg.Any<User>(), Arg.Any<string>()).Returns(newPasswordHash);
        var userService = CreateUserService();

        var result = await userService.ChangePassword(userId, currentPassword, newPassword);

        user.PasswordHash.Should().Be(newPasswordHash);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ChangePassword_InvalidCurrentPassword_ShouldReturnFalse()
    {
        const long userId = 1;
        const string currentPassword = "caoB^cf71B";
        const string newPassword = "Xdp8c^Dp5h";
        var user = new User { Id = userId, Username = "TestUser" };
        _unitOfWork.UserRepository.Get(Arg.Any<long>()).Returns(user);
        _passwordHasher.VerifyHashedPassword(Arg.Any<User>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(PasswordVerificationResult.Failed);
        var userService = CreateUserService();

        var result = await userService.ChangePassword(userId, currentPassword, newPassword);

        result.Should().BeFalse();
    }

    private IdentityService CreateUserService() => new(_unitOfWork, _passwordHasher, _credentialService, _tokenService,
        Substitute.For<ILogger<IdentityService>>());
}