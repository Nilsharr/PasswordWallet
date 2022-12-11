using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PasswordWallet.Server.Entities;

public class Credentials : IEntityTypeConfiguration<Credentials>
{
    public long Id { get; set; }
    public string Password { get; set; } = default!;
    public string? Login { get; set; }
    public string? WebAddress { get; set; }
    public string? Description { get; set; }
    public long AccountId { get; set; }
    public Account Account { get; set; } = default!;

    void IEntityTypeConfiguration<Credentials>.Configure(EntityTypeBuilder<Credentials> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Password).IsRequired().HasMaxLength(512);
        builder.Property(x => x.Login).HasMaxLength(30);
        builder.Property(x => x.WebAddress).HasMaxLength(256);
        builder.Property(x => x.Description).HasMaxLength(256);
        builder.Property(x => x.AccountId).IsRequired();
    }
}