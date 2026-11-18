using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reminders.Core.Models;

namespace Reminders.Core.Database.Configurations;

public class ReminderConfiguration : IEntityTypeConfiguration<Reminder>
{
    public void Configure(EntityTypeBuilder<Reminder> builder)
    {
        builder.ToTable("remiders", table =>
            table.HasCheckConstraint(
                "ck_reminders_repeat_settings",
                """
                (
                    repeat_unit IS NULL 
                    AND repeat_every IS NULL
                )
                OR
                (
                    repeat_unit IS NOT NULL 
                    AND repeat_every IS NOT NULL 
                    AND repeat_every > 0
                )
                """));

        builder.HasOne<Team>()
            .WithMany()
            .HasForeignKey(reminder => reminder.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(reminder => reminder.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
