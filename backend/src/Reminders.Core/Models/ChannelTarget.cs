using System.ComponentModel.DataAnnotations;

namespace Reminders.Core.Models
{
    public class ChannelTarget
    {
        [Key]
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string Address { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? BrokenReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public ChannelAccount Account { get; set; } = null!;
    }
}
