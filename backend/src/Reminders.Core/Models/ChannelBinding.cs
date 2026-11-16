using System.ComponentModel.DataAnnotations;
using Reminders.Core.Utils;

namespace Reminders.Core.Models
{
    public class ChannelBinding
    {
        [Key]
        public Guid Id { get; set; }
        public Guid TeamId { get; set; }
        public ChannelType Type { get; set; }
        public string CodeHash { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public DateTime? UsedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
