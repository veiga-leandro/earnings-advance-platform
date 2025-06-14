using Earnings.Advance.Platform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Earnings.Advance.Platform.Infrastructure.Configurations
{
    /// <summary>
    /// Entity Framework configuration to AdvanceRequest
    /// </summary>
    public class AdvanceRequestConfiguration : IEntityTypeConfiguration<AdvanceRequest>
    {
        public void Configure(EntityTypeBuilder<AdvanceRequest> builder)
        {
            builder.ToTable("AdvanceRequests");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CreatorId)
                .IsRequired()
                .HasColumnType("uniqueidentifier");

            builder.Property(x => x.RequestedAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.Fee)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.NetAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.RequestDate)
                .IsRequired();

            builder.Property(x => x.ProcessedDate);

            // Index to better perfomance
            builder.HasIndex(x => x.CreatorId);
            builder.HasIndex(x => new { x.CreatorId, x.Status });
        }
    }
}
