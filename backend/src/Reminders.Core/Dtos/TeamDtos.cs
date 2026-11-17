using System.ComponentModel.DataAnnotations;
using Reminders.Core.Utils;

namespace Reminders.Core.Dtos;

public class TeamCreateRequest
{
    [Required]
    [MaxLength(100)]        
    public string Name { get; set; } = null!;
}

public class TeamResponse
{
    public Guid TeamId { get; set; }
    public string Name { get; set; } = null!;
    public MemberRole Role { get; set; }
}

public class TeamMemberResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = null!;
    public MemberRole Role { get; set; }
}

