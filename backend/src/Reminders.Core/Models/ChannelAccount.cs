using System.ComponentModel.DataAnnotations;
using Reminders.Core.Utils;

namespace Reminders.Core.Models
{
    public class ChannelAccount
    {
        [Key]
        public Guid Id { get; set; }

        public Guid TeamId { get; set; }
        public ChannelType Type { get; set; }
        public string ExternalId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string EncryptedSecret { get; set; } = null!;
        public long? UpdatesOffset { get; set; }
        public string? BindingCodeHash { get; set; }
        public DateTime? BindingCodeExpiresAt { get; set; }
        public string? BrokenReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ChannelTarget> Targets { get; set; } = new();
    }
}
