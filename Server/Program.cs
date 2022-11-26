global using FastEndpoints;
using FastEndpoints.ClientGen;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.CodeGeneration.CSharp;
using PasswordWallet.Server.Data;
using PasswordWallet.Server.Services;
using PasswordWallet.Server.Utils;

//TODO: Store secrets

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICredentialsService, CredentialsService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddAuthenticationJWTBearer(builder.Configuration.GetSection("AppSettings")["JwtSigningKey"]);

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContextPool<PasswordWalletDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PasswordWalletConnection"))
        .UseSnakeCaseNamingConvention());

builder.Services.AddFastEndpoints();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerDoc(
        s => { s.DocumentName = "PasswordWalletApi"; },
        shortSchemaNames: true,
        removeEmptySchemas: true);
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseOpenApi();
    app.UseSwaggerUi3(s => s.ConfigureDefaults());
}

//app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints(c =>
{
    c.Endpoints.ShortNames = true;
    c.Serializer.Options.PropertyNamingPolicy = null;
});

//NOTE: run `dotnet run --generateclients true` to update the ApiClient 

await app.GenerateClientsAndExitAsync(
    documentName: "PasswordWalletApi",
    destinationPath: "../Client/HttpClient",
    csSettings: c =>
    {
        c.ClassName = "ApiClient";
        c.CSharpGeneratorSettings.Namespace = "PasswordWallet.Client";
        c.CSharpGeneratorSettings.JsonLibrary = CSharpJsonLibrary.SystemTextJson;
        c.GenerateDtoTypes = false;
        c.AdditionalNamespaceUsages = new[] {"PasswordWallet.Client.HttpClient", "PasswordWallet.Shared.Dtos"};
        c.ClientBaseClass = "BaseApiClient";
        c.UseHttpRequestMessageCreationMethod = true;
    },
    tsSettings: null);

app.Run();