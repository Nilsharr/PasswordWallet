using Microsoft.EntityFrameworkCore;
using PasswordWallet.Server.Entities;

namespace PasswordWallet.Server.Data;

public class PasswordWalletDbContext : DbContext
{
    public PasswordWalletDbContext(DbContextOptions<PasswordWalletDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PasswordWalletDbContext).Assembly);
    }

    public DbSet<Account> Account { get; set; } = default!;
    public DbSet<Credential> Credential { get; set; } = default!;
    public DbSet<AccountLogin> AccountLogin { get; set; } = default!;
    public DbSet<LoginIpAddress> LoginIpAddress { get; set; } = default!;
}