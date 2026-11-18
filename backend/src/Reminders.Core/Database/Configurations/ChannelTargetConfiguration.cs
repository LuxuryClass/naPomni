using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reminders.Core.Models;

namespace Reminders.Core.Database.Configurations;

public class ChanelTargetConfiguration : IEntityTypeConfiguration<ChannelTarget>
{
    public void Configure(EntityTypeBuilder<ChannelTarget> builder)
    {
        builder.HasOne(target => target.Team)
            .WithMany()
            .HasForeignKey(target => target.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(target => new { target.Type, target.Address })
            .IsUnique();
    }
}
