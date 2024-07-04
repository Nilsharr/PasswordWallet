using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config;

public class FolderConfiguration : IEntityTypeConfiguration<Folder>
{
    public void Configure(EntityTypeBuilder<Folder> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(64);
        // added unique constraint with AddDeferredUniqueConstraint extension method
        builder.Property(x => x.Position).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
    }
}