using Microsoft.EntityFrameworkCore;
using Reminders.Core.Utils;

namespace Reminders.Core.Models;

[PrimaryKey(nameof(TeamId), nameof(UserId))]
public class TeamMember
{
    public Guid TeamId { get; set; }
    public Guid UserId { get; set; }
    public MemberRole Role { get; set; }
    public DateTime JoinedAt { get; set; }
    public Team Team { get; set; } = null!;
    public User User { get; set; } = null!;
}
