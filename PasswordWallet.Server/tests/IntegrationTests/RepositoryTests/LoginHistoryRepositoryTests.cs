using Core.Entities;
using Core.Enums;
using FluentAssertions;
using FluentAssertions.Extensions;
using IntegrationTests.AppFixtures;
using IntegrationTests.BaseIntegrationTests;
using IntegrationTests.CollectionDefinitions;
using IntegrationTests.Fakes;

namespace IntegrationTests.RepositoryTests;

[Collection(RepositoryCollection.CollectionName)]
public class LoginHistoryRepositoryTests(RepositoryAppFixture appFixture)
    : RepositoryBaseIntegrationTest(appFixture)
{
    private User _user = default!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _user = await AddUser();
    }

    [Fact]
    public async Task GetAll_WithLoginHistory_ShouldReturnFilledPaginatedList()
    {
        const int amount = 6;
        const int pageSize = 20;
        await AddLoginHistories(_user.Id, amount, true);

        var result = await UnitOfWork.LoginHistoryRepository.GetAll(_user.Id);

        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(pageSize);
        result.TotalCount.Should().Be(amount);
        result.Items.Should().HaveCount(amount);
        result.TotalPages.Should().Be(1);
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public async Task GetAll_NoLoginHistory_ShouldReturnEmptyPaginatedList()
    {
        const int pageSize = 20;

        var result = await UnitOfWork.LoginHistoryRepository.GetAll(_user.Id);

        result.PageNumber.Should().Be(0);
        result.PageSize.Should().Be(pageSize);
        result.TotalCount.Should().Be(0);
        result.Items.Should().BeEmpty();
        result.TotalPages.Should().Be(0);
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public async Task GetAll_WithPageSizeLowerThanTotalCount_ShouldReturnListWithAmountOfItemsEqualToPageSize()
    {
        const int amount = 10;
        const int pageNumber = 1;
        const int pageSize = 5;
        await AddLoginHistories(_user.Id, amount, true);

        var result = await UnitOfWork.LoginHistoryRepository.GetAll(_user.Id, pageNumber, pageSize);

        result.PageNumber.Should().Be(pageNumber);
        result.PageSize.Should().Be(pageSize);
        result.TotalCount.Should().Be(amount);
        result.Items.Should().HaveCount(pageSize);
        result.TotalPages.Should().Be(amount / pageSize);
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public async Task GetAll_WithFilter_ShouldReturnFilteredPageHistory()
    {
        const int pageNumber = 1;
        const int pageSize = 15;
        const int amountOfGoodLogins = 3;
        const int amountOfBadLogins = 7;
        const bool correctFilter = false;
        await AddLoginHistories(_user.Id, amountOfGoodLogins, true);
        await AddLoginHistories(_user.Id, amountOfBadLogins, false);

        var result =
            await UnitOfWork.LoginHistoryRepository.GetAll(_user.Id, pageNumber, pageSize, correct: correctFilter);

        result.TotalCount.Should().Be(amountOfBadLogins);
        result.Items.Should().HaveCount(amountOfBadLogins);
        result.Items.Should().AllSatisfy(x => x.Correct.Should().Be(correctFilter));
    }

    [Fact]
    public async Task GetAll_OrderedAscending_ShouldReturnLoginHistoryInAscendingOrder()
    {
        const int count = 5;
        await AddLoginHistories(_user.Id, count, true);

        var result = await UnitOfWork.LoginHistoryRepository.GetAll(_user.Id, sortDirection: SortDirection.Asc);

        result.Items.Count.Should().Be(count);
        result.Items.Should().BeInAscendingOrder(x => x.Id);
    }

    [Fact]
    public async Task GetAll_OrderedDescending_ShouldReturnLoginHistoryInDescendingOrder()
    {
        const int count = 5;
        await AddLoginHistories(_user.Id, count, true);

        var result = await UnitOfWork.LoginHistoryRepository.GetAll(_user.Id, sortDirection: SortDirection.Desc);

        result.Items.Count.Should().Be(count);
        result.Items.Should().BeInDescendingOrder(x => x.Id);
    }

    [Fact]
    public async Task GetLastLoginDates_HasLoginHistory_ShouldReturnLastDates()
    {
        var lastValidLoginDate = DateTimeOffset.UtcNow.AddHours(-4);
        var lastInvalidLoginDate = DateTimeOffset.UtcNow.AddHours(-12);

        await UnitOfWork.LoginHistoryRepository.Add(
            new LoginHistoryFaker(_user.Id, true, lastValidLoginDate).Generate());
        await UnitOfWork.LoginHistoryRepository.Add(
            new LoginHistoryFaker(_user.Id, true, lastValidLoginDate.AddDays(-1)).Generate());
        await UnitOfWork.LoginHistoryRepository.Add(new LoginHistoryFaker(_user.Id, false, lastInvalidLoginDate)
            .Generate());
        await UnitOfWork.LoginHistoryRepository.Add(
            new LoginHistoryFaker(_user.Id, false, lastInvalidLoginDate.AddDays(-2)).Generate());

        await UnitOfWork.SaveChangesAsync();

        var result = await UnitOfWork.LoginHistoryRepository.GetLastLoginDates(_user.Id);

        result.lastValid.Should().BeCloseTo(lastValidLoginDate, 10.Microseconds());
        result.lastInvalid.Should().BeCloseTo(lastInvalidLoginDate, 10.Microseconds());
    }

    [Fact]
    public async Task GetLastLoginDates_NoLoginHistory_ShouldReturnNulls()
    {
        var result = await UnitOfWork.LoginHistoryRepository.GetLastLoginDates(_user.Id);

        result.lastValid.Should().BeNull();
        result.lastInvalid.Should().BeNull();
    }

    [Fact]
    public async Task GetLastLoginDates_OnlyCorrectLogins_ShouldReturnLastDateForCorrectAndNullForIncorrect()
    {
        var lastValidLoginDate = DateTimeOffset.UtcNow.AddHours(-6);
        await UnitOfWork.LoginHistoryRepository.Add(
            new LoginHistoryFaker(_user.Id, true, lastValidLoginDate).Generate());
        await UnitOfWork.SaveChangesAsync();

        var result = await UnitOfWork.LoginHistoryRepository.GetLastLoginDates(_user.Id);

        result.lastValid.Should().BeCloseTo(lastValidLoginDate, 10.Microseconds());
        result.lastInvalid.Should().BeNull();
    }

    [Fact]
    public async Task GetLastLoginDates_OnlyIncorrectLogins_ShouldReturnNullForCorrectAndLastDateForIncorrect()
    {
        var lastInvalidLoginDate = DateTimeOffset.UtcNow.AddDays(-3);
        await UnitOfWork.LoginHistoryRepository.Add(new LoginHistoryFaker(_user.Id, false, lastInvalidLoginDate)
            .Generate());
        await UnitOfWork.SaveChangesAsync();
        var result = await UnitOfWork.LoginHistoryRepository.GetLastLoginDates(_user.Id);

        result.lastValid.Should().BeNull();
        result.lastInvalid.Should().BeCloseTo(lastInvalidLoginDate, 10.Microseconds());
    }

    [Fact]
    public async Task GetAmountOfLogins_HasLoginHistory_ShouldReturnAmountOfLogins()
    {
        const int amountOfGoodLogins = 4;
        const int amountOfBadLogins = 2;
        await AddLoginHistories(_user.Id, amountOfGoodLogins, true);
        await AddLoginHistories(_user.Id, amountOfBadLogins, false);

        var result = await UnitOfWork.LoginHistoryRepository.GetAmountOfLogins(_user.Id);

        result.valid.Should().Be(amountOfGoodLogins);
        result.invalid.Should().Be(amountOfBadLogins);
    }

    [Fact]
    public async Task GetAmountOfLogins_NoLoginHistory_ShouldReturnZeros()
    {
        var result = await UnitOfWork.LoginHistoryRepository.GetAmountOfLogins(_user.Id);

        result.valid.Should().Be(0);
        result.invalid.Should().Be(0);
    }

    [Fact]
    public async Task GetAmountOfLogins_OnlyCorrectLogins_ShouldReturnAmountForCorrectAndZeroForIncorrect()
    {
        const int amountOfGoodLogins = 3;
        await AddLoginHistories(_user.Id, amountOfGoodLogins, true);

        var result = await UnitOfWork.LoginHistoryRepository.GetAmountOfLogins(_user.Id);

        result.valid.Should().Be(amountOfGoodLogins);
        result.invalid.Should().Be(0);
    }

    [Fact]
    public async Task GetAmountOfLogins_OnlyIncorrectLogins_ShouldReturnZeroForCorrectAndAmountForIncorrect()
    {
        const int amountOfBadLogins = 5;
        await AddLoginHistories(_user.Id, amountOfBadLogins, false);

        var result = await UnitOfWork.LoginHistoryRepository.GetAmountOfLogins(_user.Id);

        result.valid.Should().Be(0);
        result.invalid.Should().Be(amountOfBadLogins);
    }

    private async Task AddLoginHistories(long userId, int amount, bool correct)
    {
        for (var i = 0; i < amount; i++)
        {
            var loginHistory = new LoginHistoryFaker(userId, correct).Generate();
            await UnitOfWork.LoginHistoryRepository.Add(loginHistory);
        }

        await UnitOfWork.SaveChangesAsync();
    }
}