using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Reminders.Core.Utils;

namespace Reminders.Core.Models
{
    [Index(nameof(NextAttemptAt))]
    public class Delivery
    {
        [Key]
        public Guid Id { get; set; }
        public Guid TeamId { get; set; }
        public Guid TargetId { get; set; }
        public Guid? ReminderId { get; set; }
        [DeleteBehavior(DeleteBehavior.SetNull)]
        public Reminder? Reminder { get; set; }
        public string Title { get; set; } = null!;
        public DateTime ScheduledAt { get; set; }
        public DeliveryStatus Status { get; set; }
        public int Attempts { get; set; }
        public DateTime NextAttemptAt { get; set; }
        public DateTime? LockedUntil { get; set; }
        public Guid? LeaseToken { get; set; }
        public string? LastError { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
