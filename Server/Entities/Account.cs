using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PasswordWallet.Server.Entities;

public class Account : IEntityTypeConfiguration<Account>
{
    public int Id { get; set; }
    public string Login { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string Salt { get; set; } = default!;
    public bool IsPasswordKeptAsHash { get; set; }
    public List<Credentials> Credentials { get; set; } = default!;

    void IEntityTypeConfiguration<Account>.Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Login).IsRequired().HasMaxLength(30);
        builder.Property(x => x.PasswordHash).IsRequired().HasMaxLength(512);
        builder.Property(x => x.Salt).HasMaxLength(256);
        builder.Property(x => x.IsPasswordKeptAsHash).IsRequired();
        builder.HasIndex(x => x.Login).IsUnique();
    }
}