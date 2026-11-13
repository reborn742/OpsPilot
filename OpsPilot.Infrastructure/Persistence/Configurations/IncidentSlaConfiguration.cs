using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpsPilot.Domain.Entities;

namespace OpsPilot.Infrastructure.Persistence.Configurations;

public sealed class IncidentSlaConfiguration : IEntityTypeConfiguration<IncidentSla>
{
    public void Configure(EntityTypeBuilder<IncidentSla> builder)
    {
        builder.ToTable("IncidentSlas");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Priority).IsRequired();
        builder.Property(x => x.ResponseTargetMinutes).IsRequired();
        builder.Property(x => x.ResolutionTargetMinutes).IsRequired();

        builder.Property(x => x.ResponseDueAtUtc).IsRequired();
        builder.Property(x => x.ResolutionDueAtUtc).IsRequired();

        builder.HasIndex(x => x.IncidentId).IsUnique();
        builder.HasIndex(x => x.ResponseDueAtUtc);
        builder.HasIndex(x => x.ResolutionDueAtUtc);

        builder.HasOne<Incident>()
            .WithOne()
            .HasForeignKey<IncidentSla>(x => x.IncidentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
