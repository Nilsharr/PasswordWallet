using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PasswordWallet.Client;
using PasswordWallet.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAlertService, AlertService>();

builder.Services.AddSingleton(new ApiClient(
    builder.HostEnvironment.BaseAddress,
    new HttpClient {BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)}));

await builder.Build().RunAsync();