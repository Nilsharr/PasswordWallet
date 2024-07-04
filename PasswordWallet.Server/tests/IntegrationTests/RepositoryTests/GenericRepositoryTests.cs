using Core.Entities;
using FluentAssertions;
using IntegrationTests.AppFixtures;
using IntegrationTests.BaseIntegrationTests;
using IntegrationTests.CollectionDefinitions;
using IntegrationTests.Fakes;

namespace IntegrationTests.RepositoryTests;

[Collection(RepositoryCollection.CollectionName)]
public class GenericRepositoryTests(RepositoryAppFixture appFixture)
    : RepositoryBaseIntegrationTest(appFixture)
{
    [Fact]
    public async Task GetAll_NotEmptyTable_ShouldReturnAllRecords()
    {
        const int amount = 3;
        await AddUsers(amount);

        var result = await UnitOfWork.UserRepository.GetAll();

        result.Should().HaveCount(amount);
    }

    [Fact]
    public async Task GetAll_EmptyTable_ShouldReturnEmptyList()
    {
        var result = await UnitOfWork.UserRepository.GetAll();

        result.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public async Task Get_ExistingRecord_ShouldReturnRecord()
    {
        var added = await AddUser();

        var result = await UnitOfWork.UserRepository.Get(added.Id);

        result.Should().Be(added);
    }

    [Fact]
    public async Task Get_NotExistingRecord_ShouldReturnNull()
    {
        const long notExistingId = 5;

        var result = await UnitOfWork.UserRepository.Get(notExistingId);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetSingleWithoutTracking_ExistingRecord_ShouldReturnRecord()
    {
        var added = await AddUser();

        var result = await UnitOfWork.UserRepository.GetSingleWithoutTracking(x => x.Id == added.Id);

        result.Should().BeEquivalentTo(added);
    }

    [Fact]
    public async Task GetSingleWithoutTracking_NotExistingRecord_ShouldReturnNull()
    {
        const long notExistingId = 5;

        var result = await UnitOfWork.UserRepository.GetSingleWithoutTracking(x => x.Id == notExistingId);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetSingleWithoutTracking_Always_ShouldNotTrackChanges()
    {
        var added = await AddUser();
        var user = await UnitOfWork.UserRepository.GetSingleWithoutTracking(x => x.Id == added.Id);

        user!.Username = "ChangedUser";
        await UnitOfWork.SaveChangesAsync();
        var result = await UnitOfWork.UserRepository.GetSingleWithoutTracking(x => x.Id == added.Id);

        result!.Username.Should().Be(added.Username);
    }

    [Fact]
    public async Task Add_Always_ShouldAddNewRecord()
    {
        var user = new UserFaker().Generate();

        await UnitOfWork.UserRepository.Add(user);
        await UnitOfWork.SaveChangesAsync();

        user.Id.Should().BePositive();
    }

    [Fact]
    public async Task Delete_Always_ShouldDeleteRecord()
    {
        var added = await AddUser();

        await UnitOfWork.UserRepository.Delete(added);
        await UnitOfWork.SaveChangesAsync();
        var result = await UnitOfWork.UserRepository.GetSingleWithoutTracking(x => x.Id == added.Id);

        result.Should().BeNull();
    }

    [Fact]
    public async Task ExecuteDelete_Always_ShouldDeleteRecord()
    {
        var added = await AddUser();

        await UnitOfWork.UserRepository.ExecuteDelete(x => x.Id == added.Id);
        var result = await UnitOfWork.UserRepository.GetSingleWithoutTracking(x => x.Id == added.Id);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Exists_ExistingRecord_ShouldReturnTrue()
    {
        var added = await AddUser();

        var result = await UnitOfWork.UserRepository.Exists(x => x.Id == added.Id);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task Exists_NotExistingRecord_ShouldReturnFalse()
    {
        const long notExistingId = 10;

        var result = await UnitOfWork.UserRepository.Exists(x => x.Id == notExistingId);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task Count_NotEmptyTable_ShouldReturnCorrectCount()
    {
        const int amount = 3;
        await AddUsers(amount);

        var result = await UnitOfWork.UserRepository.Count();

        result.Should().Be(amount);
    }

    [Fact]
    public async Task Count_EmptyTable_ShouldReturnZero()
    {
        var result = await UnitOfWork.UserRepository.Count();

        result.Should().Be(0);
    }

    [Fact]
    public async Task Count_WithPredicate_ShouldReturnCorrectCount()
    {
        var user = await AddUser(new User { Username = "user123", PasswordHash = "123456" });
        await AddUser();

        var result = await UnitOfWork.UserRepository.Count(x => x.Username == user.Username);

        result.Should().Be(1);
    }

    private async Task AddUsers(int amount)
    {
        for (var i = 0; i < amount; i++)
        {
            var user = new UserFaker().Generate();
            await UnitOfWork.UserRepository.Add(user);
        }

        await UnitOfWork.SaveChangesAsync();
    }
}