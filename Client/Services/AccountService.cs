using Microsoft.AspNetCore.Components;
using PasswordWallet.Shared.Dtos;

namespace PasswordWallet.Client.Services;

public interface IAccountService
{
    string? AuthorizationToken { get; set; }
    Task Login(LoginRequestDto request);
    void Logout();
}

public class AccountService : IAccountService
{
    private readonly ApiClient _apiClient;
    private readonly NavigationManager _navigationManager;

    //TODO store jwt token in cookie
    public string? AuthorizationToken { get; set; }

    public AccountService(ApiClient apiClient, NavigationManager navigationManager)
    {
        _apiClient = apiClient;
        _navigationManager = navigationManager;
    }

    public async Task Login(LoginRequestDto request)
    {
        AuthorizationToken = (await _apiClient.LoginAsync(request)).Token;
    }

    public void Logout()
    {
        AuthorizationToken = null;
        _navigationManager.NavigateTo("account/login");
    }
}