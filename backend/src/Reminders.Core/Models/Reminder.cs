using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Reminders.Core.Utils;

namespace Reminders.Core.Models
{
    public class Reminder
    {
        [Key]
        public Guid Id { get; set; }
        public Guid TeamId { get; set; }
        public Guid AuthorId { get; set; }
        [MaxLength(200)]
        public string Title { get; set; } = null!;
        [MaxLength(2000)]
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
