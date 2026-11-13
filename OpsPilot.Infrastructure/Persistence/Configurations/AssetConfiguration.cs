using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpsPilot.Domain.Entities;

namespace OpsPilot.Infrastructure.Persistence.Configurations;

public sealed class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable("Assets");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AssetTag)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.AssetTag)
            .IsUnique();

        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Manufacturer)
            .HasMaxLength(100);

        builder.Property(x => x.Model)
            .HasMaxLength(100);

        builder.Property(x => x.SerialNumber)
            .HasMaxLength(100);

        builder.HasIndex(x => x.SerialNumber);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.Category);

        builder.HasIndex(x => x.AssignedToUserId);
    }
}
