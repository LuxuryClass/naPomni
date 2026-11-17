
using System.ComponentModel.DataAnnotations;

namespace Reminders.Core.Dtos;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(254)]
    public string Email { get; set; } = null!;
    [Required]
    [MinLength(8)]
    [MaxLength(100)]
    public string Password { get; set; } = null!;
}

public class LoginRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(254)]
    public string Email { get; set; } = null!;
    [Required]
    [MaxLength(100)]
    public string Password { get; set; } = null!;
}

public class UserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
}