using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config;

public class CredentialConfiguration : IEntityTypeConfiguration<Credential>
{
    public void Configure(EntityTypeBuilder<Credential> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Password).HasMaxLength(512);
        builder.Property(x => x.Username).HasMaxLength(60);
        builder.Property(x => x.WebAddress).HasMaxLength(256);
        builder.Property(x => x.Description).HasMaxLength(512);
        // added unique constraint with AddDeferredUniqueConstraint extension method
        builder.Property(x => x.Position).IsRequired();
        builder.Property(x => x.FolderId).IsRequired();
    }
}