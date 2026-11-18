using System.ComponentModel.DataAnnotations;
using Reminders.Core.Validation;

namespace Reminders.Core.Dtos;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(ValidationLimits.EmailMaxLength)]
    public string Email { get; set; } = null!;

    [Required]
    [MinLength(ValidationLimits.PasswordMinLength)]
    [MaxLength(ValidationLimits.PasswordMaxLength)]
    public string Password { get; set; } = null!;
}

public class LoginRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(ValidationLimits.EmailMaxLength)]
    public string Email { get; set; } = null!;

    [Required]
    [MaxLength(ValidationLimits.PasswordMaxLength)]
    public string Password { get; set; } = null!;
}

public class UserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
}
