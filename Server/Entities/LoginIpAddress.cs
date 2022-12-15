using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PasswordWallet.Server.Entities;

public class LoginIpAddress : IEntityTypeConfiguration<LoginIpAddress>
{
    public long Id { get; set; }
    public IPAddress IpAddress { get; set; } = default!;
    public long AmountOfGoodLogins { get; set; }
    public long AmountOfBadLogins { get; set; }
    public int SubsequentBadLogins { get; set; }
    public DateTime? TemporaryLock { get; set; }
    public bool PermanentLock { get; set; }

    public IList<AccountLogin> AccountLogins { get; set; } = new List<AccountLogin>();

    void IEntityTypeConfiguration<LoginIpAddress>.Configure(EntityTypeBuilder<LoginIpAddress> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.IpAddress).IsRequired();
        builder.Property(x => x.AmountOfGoodLogins).IsRequired().HasDefaultValue(0);
        builder.Property(x => x.AmountOfBadLogins).IsRequired().HasDefaultValue(0);
        builder.Property(x => x.SubsequentBadLogins).IsRequired().HasDefaultValue(0);
        builder.Property(x => x.TemporaryLock);
        builder.Property(x => x.PermanentLock).IsRequired().HasDefaultValue(false);
    }
}