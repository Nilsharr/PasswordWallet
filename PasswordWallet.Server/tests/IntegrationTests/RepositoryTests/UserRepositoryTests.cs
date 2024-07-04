using Core.Models;
using FluentAssertions;
using FluentAssertions.Extensions;
using IntegrationTests.AppFixtures;
using IntegrationTests.BaseIntegrationTests;
using IntegrationTests.CollectionDefinitions;
using IntegrationTests.Fakes;

namespace IntegrationTests.RepositoryTests;

[Collection(RepositoryCollection.CollectionName)]
public class UserRepositoryTests(RepositoryAppFixture appFixture)
    : RepositoryBaseIntegrationTest(appFixture)
{
    [Fact]
    public async Task GetByUsername_UsernameExists_ShouldReturnUser()
    {
        var user = await AddUser();

        var result = await UnitOfWork.UserRepository.GetByUsername(user.Username);

        result.Should().Be(user);
    }

    [Fact]
    public async Task GetByUsername_UsernameNotExists_ShouldReturnNull()
    {
        const string notExistingUsername = "qwerty";

        var result = await UnitOfWork.UserRepository.GetByUsername(notExistingUsername);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPassword_UserExists_ShouldReturnPassword()
    {
        var user = await AddUser();

        var result = await UnitOfWork.UserRepository.GetPassword(user.Id);

        result.Should().Be(user.PasswordHash);
    }

    [Fact]
    public async Task GetPassword_UserNotExists_ShouldReturnNull()
    {
        const long notExistingId = 5;

        var result = await UnitOfWork.UserRepository.GetPassword(notExistingId);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetLockoutTime_UserWithLockoutTime_ShouldReturnLockoutTime()
    {
        var lockoutTime = DateTimeOffset.UtcNow.AddHours(12);
        var user = await AddUser(new UserFaker(null, null, lockoutTime).Generate());

        var result = await UnitOfWork.UserRepository.GetLockoutTime(user.Username);

        result.Should().BeCloseTo(lockoutTime, 10.Milliseconds());
    }

    [Fact]
    public async Task GetLockoutTime_UserWithoutLockoutTime_ShouldReturnNull()
    {
        var user = await AddUser();

        var result = await UnitOfWork.UserRepository.GetLockoutTime(user.Username);

        result.Should().BeNull();
    }

    [Fact]
    public async Task ExistsWithUsername_UserWithUsernameExists_ShouldReturnTrue()
    {
        var user = await AddUser();

        var result = await UnitOfWork.UserRepository.ExistsWithUsername(user.Username);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsWithUsername_UserWithUsernameDoesntExists_ShouldReturnFalse()
    {
        const string randomName = "randomName";

        var result = await UnitOfWork.UserRepository.ExistsWithUsername(randomName);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteUpdateRefreshToken_Always_ShouldUpdateRefreshTokenAndExpiry()
    {
        const string updatedRefreshToken = "03998c2dd4e08d04620d8b3faf7000a0";
        var updatedExpiry = DateTimeOffset.UtcNow.AddHours(24);
        var user = await AddUser(new UserFaker(updatedRefreshToken, updatedExpiry).Generate());

        await UnitOfWork.UserRepository.ExecuteUpdateRefreshToken(user.Id,
            new TokenResult(updatedRefreshToken, updatedExpiry));
        var result = await UnitOfWork.UserRepository.GetSingleWithoutTracking(x => x.Id == user.Id);

        result!.RefreshToken.Should().Be(updatedRefreshToken);
        result.RefreshTokenExpiry.Should().BeCloseTo(updatedExpiry, 10.Microseconds());
    }

    [Fact]
    public async Task ExecuteRevokeRefreshToken_Always_ShouldRemoveRefreshTokenAndExpiry()
    {
        const string refreshToken = "dd1db57d3705538b55288a0de35ef2af";
        var refreshTokenExpiry = DateTimeOffset.UtcNow.AddHours(12);
        var user = await AddUser(new UserFaker(refreshToken, refreshTokenExpiry).Generate());

        await UnitOfWork.UserRepository.ExecuteRevokeRefreshToken(user.Id);
        var result = await UnitOfWork.UserRepository.GetSingleWithoutTracking(x => x.Id == user.Id);

        result!.RefreshToken.Should().BeNull();
        result.RefreshTokenExpiry.Should().BeNull();
    }
}