using PasswordWallet.Client.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Net;

namespace PasswordWallet.Client.Helpers;

public class AppRouteView : RouteView
{
    [Inject] public NavigationManager NavigationManager { get; set; } = default!;

    [Inject] public IAccountService AccountService { get; set; } = default!;

    protected override void Render(RenderTreeBuilder builder)
    {
        var authorize = Attribute.GetCustomAttribute(RouteData.PageType, typeof(AuthorizeAttribute)) is not null;
        switch (authorize)
        {
            case true when AccountService.AuthorizationToken is null:
            {
                RedirectToLogin();
                break;
            }
            case true when !JwtHelper.CheckTokenIsValid(AccountService.AuthorizationToken!):
            {
                AccountService.AuthorizationToken = null;
                RedirectToLogin();
                break;
            }
            default:
                base.Render(builder);
                break;
        }
    }

    private void RedirectToLogin()
    {
        var returnUrl = WebUtility.UrlEncode(new Uri(NavigationManager.Uri).PathAndQuery);
        NavigationManager.NavigateTo($"account/login?returnUrl={returnUrl}");
    }
}