using Bogus;
using Core.Entities;

namespace IntegrationTests.Fakes;

public sealed class UserFaker : Faker<User>
{
    public UserFaker(string? refreshToken = null, DateTimeOffset? refreshTokenExpiry = null,
        DateTimeOffset? lockoutTime = null)
    {
        RuleFor(x => x.Username, f => f.Internet.UserName());
        RuleFor(x => x.PasswordHash, f => f.Internet.Password());
        RuleFor(x => x.RefreshToken, refreshToken);
        RuleFor(x => x.RefreshTokenExpiry, refreshTokenExpiry);
        RuleFor(x => x.LockoutTime, lockoutTime);
    }
}