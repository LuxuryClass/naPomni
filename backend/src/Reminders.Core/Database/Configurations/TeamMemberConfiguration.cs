using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reminders.Core.Models;

namespace Reminders.Core.Database.Configurations;

public class TeamMemberConfiguration : IEntityTypeConfiguration<TeamMember>
{
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
        builder.HasOne(member => member.Team)
            .WithMany(team => team.Members)
            .HasForeignKey(member => member.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(member => member.User)
            .WithMany(user => user.Memberships)
            .HasForeignKey(member => member.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}