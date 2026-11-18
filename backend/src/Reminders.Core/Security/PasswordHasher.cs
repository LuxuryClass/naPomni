using Microsoft.AspNetCore.Identity;
using Reminders.Core.Models;

namespace Reminders.Core.Security;

public class PasswordHasher(IPasswordHasher<User> identityHasher)
{
    public string HashPassword(User user, string password)
    {
        return identityHasher.HashPassword(user, password);
    }

    public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
    {
        return identityHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
    }
}
