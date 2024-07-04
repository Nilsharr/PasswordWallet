using Core.Entities;
using FluentAssertions;
using IntegrationTests.AppFixtures;
using IntegrationTests.BaseIntegrationTests;
using IntegrationTests.CollectionDefinitions;
using IntegrationTests.Fakes;

namespace IntegrationTests.RepositoryTests;

[Collection(RepositoryCollection.CollectionName)]
public class CredentialRepositoryTests(RepositoryAppFixture appFixture)
    : RepositoryBaseIntegrationTest(appFixture)
{
    private User _user = default!;
    private Folder _folder = default!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _user = await AddUser();
        _folder = await AddFolder(_user.Id);
    }

    [Fact]
    public async Task GetAll_WithCredentials_ShouldReturnFilledPaginatedList()
    {
        const int amount = 8;
        const int pageNumber = 1;
        const int pageSize = 20;
        await AddCredentials(_folder.Id, amount);

        var result = await UnitOfWork.CredentialRepository.GetAll(_folder.Id, pageNumber, pageSize);

        result.PageNumber.Should().Be(pageNumber);
        result.PageSize.Should().Be(pageSize);
        result.TotalCount.Should().Be(amount);
        result.Items.Should().HaveCount(amount);
        result.TotalPages.Should().Be(1);
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public async Task GetAll_NoCredentials_ShouldReturnEmptyPaginatedList()
    {
        const int pageNumber = 1;
        const int pageSize = 20;

        var result = await UnitOfWork.CredentialRepository.GetAll(_folder.Id, pageNumber, pageSize);

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
        const int amount = 15;
        const int pageNumber = 1;
        const int pageSize = 5;
        await AddCredentials(_folder.Id, amount);

        var result = await UnitOfWork.CredentialRepository.GetAll(_folder.Id, pageNumber, pageSize);

        result.PageNumber.Should().Be(pageNumber);
        result.PageSize.Should().Be(pageSize);
        result.TotalCount.Should().Be(amount);
        result.Items.Should().HaveCount(pageSize);
        result.TotalPages.Should().Be(amount / pageSize);
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public async Task GetAll_Always_ShouldReturnCredentialsOrderedAscendingByPosition()
    {
        const int amount = 10;
        const int pageNumber = 1;
        const int pageSize = 20;
        await AddCredentials(_folder.Id, amount);

        var result = await UnitOfWork.CredentialRepository.GetAll(_folder.Id, pageNumber, pageSize);

        result.Items.Should().NotBeEmpty();
        result.Items.Should().BeInAscendingOrder(x => x.Position);
    }

    [Fact]
    public async Task GetPassword_PasswordNotNull_ShouldReturnPassword()
    {
        var credential = await AddCredential(_folder.Id);

        var result = await UnitOfWork.CredentialRepository.GetPassword(credential.Id);

        result.Should().Be(credential.Password);
    }

    [Fact]
    public async Task GetPassword_PasswordNull_ShouldReturnNull()
    {
        var credential = new CredentialFaker(_folder.Id, 1).Generate();
        credential.Password = null;
        await AddCredential(credential);

        var result = await UnitOfWork.CredentialRepository.GetPassword(credential.Id);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUserCredentials_Always_ShouldReturnCredentialsFromAllFolders()
    {
        const int amountInFirstFolder = 4;
        const int amountInSecondFolder = 2;
        var anotherFolder = await AddFolder(_user.Id);
        await AddCredentials(_folder.Id, amountInFirstFolder);
        await AddCredentials(anotherFolder.Id, amountInSecondFolder);

        var result = await UnitOfWork.CredentialRepository.GetUserCredentials(_user.Id);

        result.Should().HaveCount(amountInFirstFolder + amountInSecondFolder);
    }

    [Fact]
    public async Task GetNextAvailablePosition_NoCredentials_ShouldReturnOne()
    {
        var result = await UnitOfWork.CredentialRepository.GetNextAvailablePosition(_folder.Id);

        result.Should().Be(1);
    }

    [Fact]
    public async Task GetNextAvailablePosition_WithCredentials_ShouldReturnNextPosition()
    {
        const int amount = 3;
        await AddCredentials(_folder.Id, amount);

        var result = await UnitOfWork.CredentialRepository.GetNextAvailablePosition(_folder.Id);

        result.Should().Be(amount + 1);
    }

    [Fact]
    public async Task IsCredentialOwnedByUser_UserOwnsCredential_ShouldReturnTrue()
    {
        var credential = await AddCredential(_folder.Id);

        var result = await UnitOfWork.CredentialRepository.IsCredentialOwnedByUser(_user.Id, credential.Id);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCredentialOwnedByUser_UserDoesNotOwnCredential_ShouldReturnFalse()
    {
        var credential = await AddCredential(_folder.Id);
        var otherUser = await AddUser();

        var result = await UnitOfWork.CredentialRepository.IsCredentialOwnedByUser(otherUser.Id, credential.Id);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteUpdateFolder_Always_ShouldUpdateFolderId()
    {
        var anotherFolder = await AddFolder(_user.Id);
        var credential = await AddCredential(_folder.Id);

        await UnitOfWork.CredentialRepository.ExecuteUpdateFolder(credential.Id, anotherFolder.Id);
        var result = await UnitOfWork.CredentialRepository.GetSingleWithoutTracking(x => x.Id == credential.Id);

        result!.FolderId.Should().Be(anotherFolder.Id);
    }

    [Fact]
    public async Task ExecuteUpdateFolder_Always_ShouldSetCorrectPosition()
    {
        const int amount = 4;
        var credential = await AddCredential(_folder.Id);
        var anotherFolder = await AddFolder(_user.Id);
        await AddCredentials(anotherFolder.Id, amount);

        await UnitOfWork.CredentialRepository.ExecuteUpdateFolder(credential.Id, anotherFolder.Id);
        var result = await UnitOfWork.CredentialRepository.GetSingleWithoutTracking(x => x.Id == credential.Id);

        result!.Position.Should().Be(amount + 1);
    }

    [Fact]
    public async Task ExecuteUpdatePosition_Always_ShouldUpdatePosition()
    {
        const long newPosition = 4;
        var credential = await AddCredential(_folder.Id);
        await AddCredentials(_folder.Id, 9);

        await UnitOfWork.CredentialRepository.ExecuteUpdatePosition(credential.Id, newPosition);
        var result = await UnitOfWork.CredentialRepository.GetSingleWithoutTracking(x => x.Id == credential.Id);

        result!.Position.Should().Be(newPosition);
    }

    private async Task AddCredentials(Guid folderId, int amount)
    {
        var position = await UnitOfWork.CredentialRepository.GetNextAvailablePosition(folderId);
        for (var i = 0; i < amount; i++, position++)
        {
            var credential = new CredentialFaker(folderId, position).Generate();
            await UnitOfWork.CredentialRepository.Add(credential);
        }

        await UnitOfWork.SaveChangesAsync();
    }
}