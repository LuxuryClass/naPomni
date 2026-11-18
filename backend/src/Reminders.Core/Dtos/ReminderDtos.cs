using System.ComponentModel.DataAnnotations;
using Reminders.Core.Utils;
using Reminders.Core.Validation;

namespace Reminders.Core.Dtos;

public class ReminderCreateRequest
{
    [Required]
    [MaxLength(ValidationLimits.ReminderTitleMaxLength)]
    public string Title { get; set; } = null!;

    [MaxLength(ValidationLimits.ReminderDescriptionMaxLength)]
    public string? Description { get; set; }

    [Required]
    public DateTime? StartLocal { get; set; }

    [Required]
    public string TimeZoneId { get; set; } = null!;

    [EnumDataType(typeof(RepeatUnit))]
    public RepeatUnit? RepeatUnit { get; set; }

    [Range(1, int.MaxValue)]
    public int? RepeatEvery { get; set; }
}

public class ReminderUpdateRequest
{
    [Required]
    [MaxLength(ValidationLimits.ReminderTitleMaxLength)]
    public string Title { get; set; } = null!;

    [MaxLength(ValidationLimits.ReminderDescriptionMaxLength)]
    public string? Description { get; set; }

    [Required]
    public DateTime? StartLocal { get; set; }

    [Required]
    public string TimeZoneId { get; set; } = null!;

    [EnumDataType(typeof(RepeatUnit))]
    public RepeatUnit? RepeatUnit { get; set; }

    [Range(1, int.MaxValue)]
    public int? RepeatEvery { get; set; }

    [Required]
    public uint? Version { get; set; }
}

public class ReminderResponse
{
    public Guid ReminderId { get; set; }
    public Guid TeamId { get; set; }
    public Guid AuthorId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime StartLocal { get; set; }
    public string TimeZoneId { get; set; } = null!;
    public RepeatUnit? RepeatUnit { get; set; }
    public int? RepeatEvery { get; set; }
    public DateTime? NextFireAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public uint Version { get; set; }
}
