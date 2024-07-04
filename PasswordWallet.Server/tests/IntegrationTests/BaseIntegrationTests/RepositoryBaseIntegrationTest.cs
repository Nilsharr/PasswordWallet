using Core.Entities;
using Core.Interfaces.Repositories;
using IntegrationTests.AppFixtures;
using IntegrationTests.Fakes;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.BaseIntegrationTests;

public class RepositoryBaseIntegrationTest : BaseIntegrationTest
{
    protected readonly IUnitOfWork UnitOfWork;

    protected RepositoryBaseIntegrationTest(PasswordWalletAppFixture appFixture) : base(appFixture)
    {
        var scope = appFixture.Services.CreateScope();
        UnitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    }

    protected async Task<User> AddUser(User? user = null)
    {
        user ??= new UserFaker().Generate();
        await UnitOfWork.UserRepository.Add(user);
        await UnitOfWork.SaveChangesAsync();
        return user;
    }

    protected async Task<Folder> AddFolder(long userId)
    {
        var position = await UnitOfWork.FolderRepository.GetNextAvailablePosition(userId);
        var folder = new FolderFaker(userId, position).Generate();
        await UnitOfWork.FolderRepository.Add(folder);
        await UnitOfWork.SaveChangesAsync();
        return folder;
    }

    protected async Task<Credential> AddCredential(Guid folderId)
    {
        var position = await UnitOfWork.CredentialRepository.GetNextAvailablePosition(folderId);
        var credential = new CredentialFaker(folderId, position).Generate();
        await UnitOfWork.CredentialRepository.Add(credential);
        await UnitOfWork.SaveChangesAsync();
        return credential;
    }

    protected async Task<Credential> AddCredential(Credential credential)
    {
        await UnitOfWork.CredentialRepository.Add(credential);
        await UnitOfWork.SaveChangesAsync();
        return credential;
    }
}