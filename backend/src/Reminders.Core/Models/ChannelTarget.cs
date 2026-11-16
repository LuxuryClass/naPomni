using System.ComponentModel.DataAnnotations;
using Reminders.Core.Utils;

namespace Reminders.Core.Models
{
    public class ChannelTarget
    {
        [Key]
        public Guid Id { get; set; }
        public Guid TeamId { get; set; }
        public ChannelType Type { get; set; }
        public string Address { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? BrokenReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public Team Team { get; set; } = null!;
    }
}
