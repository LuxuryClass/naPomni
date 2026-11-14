using System.ComponentModel.DataAnnotations;

namespace Reminders.Core.Models
{
    public class Team
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? InviteTokenHash { get; set; }
        public DateTime? InviteExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<TeamMember> Members { get; set; } = new();
    }
}
