using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpsPilot.Domain.Entities;

namespace OpsPilot.Infrastructure.Persistence.Configurations;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ActorDisplayName)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.Action)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.EntityType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.EntityId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Details)
            .HasMaxLength(2000);

        builder.HasIndex(x => x.OccurredAtUtc);
        builder.HasIndex(x => new { x.EntityType, x.EntityId });
        builder.HasIndex(x => x.ActorUserId);
    }
}
