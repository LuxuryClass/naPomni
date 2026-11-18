using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Reminders.Core.Utils;
using Reminders.Core.Validation;

namespace Reminders.Core.Models
{
    public class Reminder
    {
        [Key]
        public Guid Id { get; set; }
        public Guid TeamId { get; set; }
        public Guid AuthorId { get; set; }

        [MaxLength(ValidationLimits.ReminderTitleMaxLength)]
        public string Title { get; set; } = null!;

        [MaxLength(ValidationLimits.ReminderDescriptionMaxLength)]
        public string? Description { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime StartLocal { get; set; }
        public string TimeZoneId { get; set; } = null!;
        public RepeatUnit? RepeatUnit { get; set; }
        public int? RepeatEvery { get; set; }
        public int NextOccurrenceNumber { get; set; }
        public DateTime? NextFireAt { get; set; }
        public DateTime CreatedAt { get; set; }

        [Timestamp]
        public uint Version { get; set; }
    }
}
