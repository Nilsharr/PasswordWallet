using Core.Entities;
using FluentAssertions;
using IntegrationTests.AppFixtures;
using IntegrationTests.BaseIntegrationTests;
using IntegrationTests.CollectionDefinitions;
using IntegrationTests.Fakes;

namespace IntegrationTests.RepositoryTests;

[Collection(RepositoryCollection.CollectionName)]
public class FolderRepositoryTests(RepositoryAppFixture appFixture)
    : RepositoryBaseIntegrationTest(appFixture)
{
    private User _user = default!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _user = await AddUser();
    }

    [Fact]
    public async Task GetAll_WithFolders_ShouldReturnAllFolders()
    {
        const int amount = 3;
        await AddFolders(_user.Id, amount);

        var result = await UnitOfWork.FolderRepository.GetAll(_user.Id);

        result.Should().HaveCount(amount);
    }

    [Fact]
    public async Task GetAll_NoFolders_ShouldReturnEmptyList()
    {
        var result = await UnitOfWork.FolderRepository.GetAll(_user.Id);

        result.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public async Task GetAll_Always_ShouldReturnFoldersOrderedAscendingByPosition()
    {
        await AddFolders(_user.Id, 3);

        var result = await UnitOfWork.FolderRepository.GetAll(_user.Id);

        result.Should().NotBeEmpty();
        result.Should().BeInAscendingOrder(x => x.Position);
    }

    [Fact]
    public async Task GetNextAvailablePosition_NoFolders_ShouldReturnOne()
    {
        var result = await UnitOfWork.FolderRepository.GetNextAvailablePosition(_user.Id);

        result.Should().Be(1);
    }

    [Fact]
    public async Task GetNextAvailablePosition_WithFolders_ShouldReturnNextPosition()
    {
        const int amount = 5;
        await AddFolders(_user.Id, amount);

        var result = await UnitOfWork.FolderRepository.GetNextAvailablePosition(_user.Id);

        result.Should().Be(amount + 1);
    }

    [Fact]
    public async Task IsFolderOwnedByUser_UserOwnsFolder_ShouldReturnTrue()
    {
        var folder = await AddFolder(_user.Id);

        var result = await UnitOfWork.FolderRepository.IsFolderOwnedByUser(_user.Id, folder.Id);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsFolderOwnedByUser_UserDoesNotOwnFolder_ShouldReturnFalse()
    {
        var otherUser = await AddUser();
        var folder = await AddFolder(_user.Id);

        var result = await UnitOfWork.FolderRepository.IsFolderOwnedByUser(otherUser.Id, folder.Id);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteUpdateName_Always_ShouldUpdateName()
    {
        const string newName = "Other";
        var folder = await AddFolder(_user.Id);

        await UnitOfWork.FolderRepository.ExecuteUpdateName(folder.Id, newName);
        var result = await UnitOfWork.FolderRepository.GetSingleWithoutTracking(x => x.Id == folder.Id);

        result!.Name.Should().Be(newName);
    }

    [Fact]
    public async Task ExecuteUpdatePosition_Always_ShouldUpdatePosition()
    {
        const long newPosition = 3;
        var folder = await AddFolder(_user.Id);
        await AddFolders(_user.Id, 5);

        await UnitOfWork.FolderRepository.ExecuteUpdatePosition(_user.Id, folder.Id, newPosition);
        var result = await UnitOfWork.FolderRepository.GetSingleWithoutTracking(x => x.Id == folder.Id);

        result!.Position.Should().Be(newPosition);
    }

    private async Task AddFolders(long userId, int amount)
    {
        var position = await UnitOfWork.FolderRepository.GetNextAvailablePosition(userId);
        for (var i = 0; i < amount; i++, position++)
        {
            var folder = new FolderFaker(userId, position).Generate();
            await UnitOfWork.FolderRepository.Add(folder);
        }

        await UnitOfWork.SaveChangesAsync();
    }
}