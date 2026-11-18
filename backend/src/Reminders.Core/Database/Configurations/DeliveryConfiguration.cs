using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reminders.Core.Models;
using Reminders.Core.Utils;

namespace Reminders.Core.Database.Configurations;

public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
{
    public void Configure(EntityTypeBuilder<Delivery> builder)
    {
        var sendingStatus = (int)DeliveryStatus.Sending;

        builder.ToTable("deliveries", table =>
        {
            table.HasCheckConstraint(
                "ck_deliveries_attempts_non_negative",
                "attempts >= 0");

            table.HasCheckConstraint(
                "ck_deliveries_lease",
                $"""
                (status = {sendingStatus} AND locked_until IS NOT NULL AND lease_token IS NOT NULL)
                OR
                (status <> {sendingStatus} AND locked_until IS NULL AND lease_token IS NULL)
                """);
        });

        builder.HasOne<ChannelTarget>()
            .WithMany()
            .HasForeignKey(delivery => new { delivery.TeamId, delivery.TargetId })
            .HasPrincipalKey(target => new { target.TeamId, target.Id })
            .OnDelete(DeleteBehavior.Restrict);
    }
}