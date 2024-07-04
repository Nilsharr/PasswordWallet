using Api.Endpoints.v1.User.GetLoginHistory;
using Core.Entities;
using Core.Models;
using FluentAssertions;

namespace UnitTests.MappingTests;

public class GetLoginHistoryMapperTests
{
    private readonly GetLoginHistoryMapper _mapper = new();

    [Fact]
    public void MapLoginHistoryPaginatedListToResponse_Always_ShouldMapProperties()
    {
        const int pageNumber = 1;
        const int pageSize = 20;
        const int totalCount = 50;
        List<LoginHistory> loginHistories =
        [
            new LoginHistory { Id = 1, Date = DateTimeOffset.UtcNow, Correct = true, IpAddress = "192.168.0.0" },
            new LoginHistory { Id = 2, Date = DateTimeOffset.UtcNow, Correct = false, IpAddress = "192.168.0.0" },
            new LoginHistory { Id = 3, Date = DateTimeOffset.UtcNow, Correct = true, IpAddress = "192.168.0.0" }
        ];
        var paginatedList = new PaginatedList<LoginHistory>(pageNumber, pageSize, totalCount, loginHistories);

        var result = _mapper.FromEntities(paginatedList);

        result.Should().NotBeNull();
        result.PageNumber.Should().Be(pageNumber);
        result.PageSize.Should().Be(pageSize);
        result.TotalCount.Should().Be(totalCount);
        result.Items.Should().HaveCount(loginHistories.Count);

        foreach (var (loginHistoryResponse, loginHistory) in result.Items.Zip(loginHistories))
        {
            loginHistoryResponse.Id.Should().Be(loginHistory.Id);
            loginHistoryResponse.Date.Should().Be(loginHistory.Date);
            loginHistoryResponse.Correct.Should().Be(loginHistory.Correct);
            loginHistoryResponse.IpAddress.Should().Be(loginHistory.IpAddress);
        }
    }

    [Fact]
    public void MapLoginHistoryPaginatedListToResponse_EmptyList_ShouldReturnEmptyResponseList()
    {
        const int pageNumber = 1;
        const int pageSize = 20;
        const int totalCount = 0;
        var paginatedList =
            new PaginatedList<LoginHistory>(pageNumber, pageSize, totalCount, Array.Empty<LoginHistory>());

        var result = _mapper.FromEntities(paginatedList);

        result.Should().NotBeNull();
        result.PageNumber.Should().Be(pageNumber);
        result.PageSize.Should().Be(pageSize);
        result.TotalCount.Should().Be(totalCount);
        result.HasNextPage.Should().BeFalse();
        result.HasNextPage.Should().BeFalse();
        result.Items.Should().BeEmpty();
    }
}