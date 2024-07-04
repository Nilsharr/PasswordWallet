using Bogus;
using Core.Entities;

namespace IntegrationTests.Fakes;

public sealed class LoginHistoryFaker : Faker<LoginHistory>
{
    public LoginHistoryFaker(long userId, bool correct = true, DateTimeOffset? date = null)
    {
        RuleFor(x => x.UserId, userId);
        RuleFor(x => x.Correct, correct);
        RuleFor(x => x.IpAddress, f => f.Internet.Ip());

        if (date is null)
        {
            RuleFor(x => x.Date, f => f.Date.RecentOffset().ToUniversalTime());
        }
        else
        {
            RuleFor(x => x.Date, date);
        }
    }
}