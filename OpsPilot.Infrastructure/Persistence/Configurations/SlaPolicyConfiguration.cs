using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpsPilot.Domain.Entities;

namespace OpsPilot.Infrastructure.Persistence.Configurations;

public sealed class SlaPolicyConfiguration : IEntityTypeConfiguration<SlaPolicy>
{
    public void Configure(EntityTypeBuilder<SlaPolicy> builder)
    {
        builder.ToTable("SlaPolicies");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Priority).IsRequired();
        builder.Property(x => x.ResponseTargetMinutes).IsRequired();
        builder.Property(x => x.ResolutionTargetMinutes).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();

        builder.HasIndex(x => x.Priority).IsUnique();
    }
}
