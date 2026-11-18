using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reminders.Core.Models;
using Reminders.Core.Validation;

namespace Reminders.Core.Database.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ToTable("teams", table => 
            table.HasCheckConstraint(
                "ck_teams_invite_fields_together",
                "(invite_token_hash IS NULL) = (invite_expires_at IS NULL)"));

        builder.Property(team => team.Name)
            .HasMaxLength(ValidationLimits.TeamNameMaxLength);
    }
}