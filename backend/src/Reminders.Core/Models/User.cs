using System.ComponentModel.DataAnnotations;

namespace Reminders.Core.Models
{
    public class User 
    {
        [Key]
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? DeactivatedAt { get; set; }
        public List<TeamMember> Memberships { get; set; } = new();
    }
}
