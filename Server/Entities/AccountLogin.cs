using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PasswordWallet.Server.Entities;

public class AccountLogin : IEntityTypeConfiguration<AccountLogin>
{
    public long Id { get; set; }
    public DateTime Time { get; }
    public bool Correct { get; set; }

    public long AccountId { get; set; }
    public long LoginIpAddressId { get; set; }

    public Account Account { get; set; } = default!;
    public LoginIpAddress LoginIpAddress { get; set; } = default!;

    void IEntityTypeConfiguration<AccountLogin>.Configure(EntityTypeBuilder<AccountLogin> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Time).IsRequired().HasDefaultValueSql("now()");
        builder.Property(x => x.Correct).IsRequired();
        builder.Property(x => x.AccountId).IsRequired();
        builder.Property(x => x.LoginIpAddressId).IsRequired();
    }
}