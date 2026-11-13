using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpsPilot.Domain.Entities;

namespace OpsPilot.Infrastructure.Persistence.Configurations;

public sealed class IncidentConfiguration : IEntityTypeConfiguration<Incident>
{
    public void Configure(EntityTypeBuilder<Incident> builder)
    {
        builder.ToTable("Incidents");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(5000)
            .IsRequired();

        builder.Property(x => x.ResolutionNotes)
            .HasMaxLength(5000);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.Priority);

        builder.HasIndex(x => x.CreatedAtUtc);

        builder.HasIndex(x => x.AssignedTechnicianId);

        builder.HasIndex(x => new { x.Status, x.Priority });

        builder.HasOne<Asset>()
            .WithMany()
            .HasForeignKey(x => x.AssetId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
