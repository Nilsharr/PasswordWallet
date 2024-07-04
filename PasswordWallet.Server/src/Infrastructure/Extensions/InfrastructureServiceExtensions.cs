using Core.Constants;
using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using FastEndpoints.Security;
using Infrastructure.Cryptography;
using Infrastructure.Data;
using Infrastructure.Data.Repositories;
using Infrastructure.Identity;
using Infrastructure.Options;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddDatabase(configuration)
            .AddRepositories()
            .AddOptions(configuration)
            .AddAuthentication(configuration)
            .AddCryptography();
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PasswordWalletConnection");
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        services.AddDbContextPool<PasswordWalletDbContext>(options =>
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ILoginHistoryRepository, LoginHistoryRepository>();
        services.AddScoped<ICredentialRepository, CredentialRepository>();
        services.AddScoped<IFolderRepository, FolderRepository>();

        return services;
    }

    private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptionsWithValidateOnStart<JwtOptions>()
            .Bind(configuration.GetRequiredSection(JwtOptions.SectionName));
        services.AddOptionsWithValidateOnStart<LoginSecurityOptions>()
            .Bind(configuration.GetRequiredSection(LoginSecurityOptions.SectionName));

        return services;
    }

    private static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();
        ArgumentNullException.ThrowIfNull(jwtOptions);
        ArgumentException.ThrowIfNullOrWhiteSpace(jwtOptions.SigningKey);

        services.AddAuthenticationJwtBearer(s => s.SigningKey = jwtOptions.SigningKey, o =>
            {
                o.TokenValidationParameters.ValidIssuer = jwtOptions.Issuer;
                o.TokenValidationParameters.ValidAudience = jwtOptions.Audience;
            })
            .AddAuthorizationBuilder()
            .AddPolicy(AuthenticationConstants.UserPolicy, x => x.RequireClaim(AuthenticationConstants.UserIdClaim));

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        return services;
    }

    private static IServiceCollection AddCryptography(this IServiceCollection services)
    {
        services.AddDataProtection().UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration());
        services.AddScoped<IEncryptionService, EncryptionService>();
        return services;
    }
}