using Core.Entities;
using Core.Interfaces.Repositories;
using FastEndpoints;
using FluentAssertions;
using Infrastructure.Identity;
using Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace UnitTests.ServiceTests;

public class TokenServiceTests
{
    private static readonly DateTimeOffset UtcDate = new(new DateTime(2022, 10, 5, 14, 30, 40));
    private static readonly TimeSpan AccessTokenLifeTime = new(0, 10, 0);
    private static readonly TimeSpan RefreshTokenLifeTime = new(12, 0, 0);

    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public void GenerateJwtToken_Always_ShouldReturnTokenWithExpiry()
    {
        Factory.RegisterTestServices(_ => { });
        const long userId = 1;
        var expectedExpiry = UtcDate.Add(AccessTokenLifeTime);
        var tokenService = CreateTokenService();

        var result = tokenService.GenerateJwtToken(userId);

        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrWhiteSpace();
        result.Expiry.Should().Be(expectedExpiry);
    }

    [Fact]
    public void GenerateRefreshToken_Always_ShouldReturnTokenWithExpiry()
    {
        var expectedExpiry = UtcDate.Add(RefreshTokenLifeTime);
        var tokenService = CreateTokenService();

        var result = tokenService.GenerateRefreshToken();

        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrWhiteSpace();
        result.Expiry.Should().Be(expectedExpiry);
    }

    [Theory]
    [InlineData(0.1)]
    [InlineData(0.5)]
    [InlineData(1)]
    [InlineData(6)]
    [InlineData(48)]
    public async Task RefreshAccessToken_ValidTokens_ShouldReturnNewToken(double hours)
    {
        Factory.RegisterTestServices(_ => { });
        const long userId = 1;
        const string refreshToken = "15117b282328146ac6afebaa8acd80e7";
        var refreshTokenExpiry = UtcDate.AddHours(hours);
        var user = new User { Id = userId, RefreshToken = refreshToken, RefreshTokenExpiry = refreshTokenExpiry };
        var tokenService = CreateTokenService();
        var accessToken = tokenService.GenerateJwtToken(userId);
        _unitOfWork.UserRepository.Get(Arg.Any<long>()).Returns(user);

        var result = await tokenService.RefreshAccessToken(accessToken.Token, refreshToken);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task RefreshAccessToken_InvalidAccessToken_ShouldReturnNull()
    {
        const string accessToken = "15117b282328146ac6afebaa8acd80e7";
        const string refreshToken = "15117b282328146ac6afebaa8acd80e7";
        var tokenService = CreateTokenService();

        var result = await tokenService.RefreshAccessToken(accessToken, refreshToken);

        result.Should().BeNull();
    }

    [Fact]
    public async Task RefreshAccessToken_NotExistingUser_ShouldReturnNull()
    {
        Factory.RegisterTestServices(_ => { });
        const string refreshToken = "15117b282328146ac6afebaa8acd80e7";
        var tokenService = CreateTokenService();
        var accessToken = tokenService.GenerateJwtToken(1);
        _unitOfWork.UserRepository.Get(Arg.Any<long>()).ReturnsNull();

        var result = await tokenService.RefreshAccessToken(accessToken.Token, refreshToken);

        result.Should().BeNull();
    }

    [Fact]
    public async Task RefreshAccessToken_NotMatchingRefreshToken_ShouldReturnNull()
    {
        Factory.RegisterTestServices(_ => { });
        const long userId = 1;
        const string refreshToken = "15117b282328146ac6afebaa8acd80e7";
        var user = new User { Id = userId, RefreshToken = "0800fc577294c34e0b28ad2839435945" };
        var tokenService = CreateTokenService();
        var accessToken = tokenService.GenerateJwtToken(userId);
        _unitOfWork.UserRepository.Get(Arg.Any<long>()).Returns(user);

        var result = await tokenService.RefreshAccessToken(accessToken.Token, refreshToken);

        result.Should().BeNull();
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(-0.5)]
    [InlineData(-1)]
    [InlineData(-6)]
    [InlineData(-48)]
    public async Task RefreshAccessToken_ExpiredRefreshToken_ShouldReturnNull(double hours)
    {
        Factory.RegisterTestServices(_ => { });
        const long userId = 1;
        const string refreshToken = "15117b282328146ac6afebaa8acd80e7";
        var user = new User { Id = userId, RefreshToken = refreshToken, RefreshTokenExpiry = UtcDate.AddHours(hours) };
        var tokenService = CreateTokenService();
        var accessToken = tokenService.GenerateJwtToken(userId);
        _unitOfWork.UserRepository.Get(Arg.Any<long>()).Returns(user);

        var result = await tokenService.RefreshAccessToken(accessToken.Token, refreshToken);

        result.Should().BeNull();
    }

    private TokenService CreateTokenService()
    {
        var jwtOptions = Options.Create(new JwtOptions
        {
            SigningKey = "3f99d53a-9140-4f7d-bbe6-beb81d28", AccessTokenLifeTime = AccessTokenLifeTime,
            RefreshTokenLifeTime = RefreshTokenLifeTime, Issuer = "https://localhost:7149",
            Audience = "https://localhost:7149"
        });
        var timeProvider = new FakeTimeProvider();
        timeProvider.SetUtcNow(UtcDate);

        var logger = Substitute.For<ILogger<TokenService>>();
        return new TokenService(_unitOfWork, jwtOptions, timeProvider, logger);
    }
}