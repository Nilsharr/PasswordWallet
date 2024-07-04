using System.Data.Common;
using FastEndpoints.Testing;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;

namespace IntegrationTests.AppFixtures;

public class PasswordWalletAppFixture : AppFixture<Program>
{
    private const string Image = "postgres:16.3-alpine";
    private const string Database = "PasswordWallet";
    private const string Username = "postgres";
    private const string Password = "root";

    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder().WithImage(Image)
        .WithDatabase(Database).WithUsername(Username).WithPassword(Password).Build();

    private DbConnection _dbConnection = default!;
    private Respawner _respawner = default!;

    protected override async Task PreSetupAsync()
    {
        await _dbContainer.StartAsync();
        _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());
    }

    protected override async Task SetupAsync()
    {
        await InitializeRespawner();
    }

    protected override void ConfigureApp(IWebHostBuilder webHostBuilder)
    {
        webHostBuilder.UseEnvironment(Environments.Development);
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        var descriptor =
            services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<PasswordWalletDbContext>));
        if (descriptor is not null)
        {
            services.Remove(descriptor);
        }

        services.AddDbContextPool<PasswordWalletDbContext>(options =>
            options.UseNpgsql(_dbContainer.GetConnectionString()).UseSnakeCaseNamingConvention());
    }

    protected override async Task TearDownAsync()
    {
        await _dbConnection.DisposeAsync();
        await _dbContainer.DisposeAsync();
    }

    public Task ResetDatabase()
    {
        return _respawner.ResetAsync(_dbConnection);
    }

    private async Task InitializeRespawner()
    {
        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"],
            TablesToIgnore = ["__EFMigrationsHistory"]
        });
    }
}