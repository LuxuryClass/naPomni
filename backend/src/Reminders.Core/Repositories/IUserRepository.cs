using Reminders.Core.Models;

namespace Reminders.Core.Repositories;

public interface IUserRepository
{
    Task<User?> FindByIdAsync(Guid userId, CancellationToken cancellationToken);

    Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken);

    void Add(User user);
}
