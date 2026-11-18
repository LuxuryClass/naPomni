using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reminders.Core.Models;

namespace Reminders.Core.Database.Configurations
{
    public class ChannelBindingConfiguration : IEntityTypeConfiguration<ChannelBinding>
    {
        public void Configure(EntityTypeBuilder<ChannelBinding> builder)
        {
            builder.ToTable("channel_bindings", table =>
            {
                table.HasCheckConstraint(
                    "ck_channel_bindings_expires_at",
                    "expires_at > created_at");

                table.HasCheckConstraint(
                    "ck_channel_bindings_used_at",
                    "used_at IS NULL OR (used_at >= created_at AND used_at < expires_at)");
            });

            builder.HasOne<Team>()
                .WithMany()
                .HasForeignKey(binding => binding.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(binding => binding.CodeHash)
                .IsUnique();
        }
    }
}
