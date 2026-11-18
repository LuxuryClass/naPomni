namespace Reminders.Core.Validation;

internal static class ValidationLimits
{
    public const int EmailMaxLength = 254;

    public const int PasswordMinLength = 8;
    public const int PasswordMaxLength = 100;

    public const int TeamNameMaxLength = 100;

    public const int ReminderTitleMaxLength = 200;
    public const int ReminderDescriptionMaxLength = 2000;
}
