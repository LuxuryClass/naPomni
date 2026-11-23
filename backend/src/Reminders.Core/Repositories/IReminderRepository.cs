using Reminders.Core.Models;

namespace Reminders.Core.Repositories;

public interface IReminderRepository
{
    Task<Reminder?> FindByIdAsync(
        Guid teamId, Guid reminderId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Reminder>> ListByTeamIdAsync(
        Guid teamId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Reminder>> ListDueAsync(
        DateTime utcNow, int batchSize, CancellationToken cancellationToken);

    void Add(Reminder reminder);

    void Remove(Reminder reminder);
}
