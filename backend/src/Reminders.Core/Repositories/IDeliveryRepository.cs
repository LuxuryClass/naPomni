using Reminders.Core.Models;

namespace Reminders.Core.Repositories;

public interface IDeliveryRepository
{
    void AddRange(IEnumerable<Delivery> deliveries);

    Task<IReadOnlyList<Delivery>> ClaimDueAsync(
        DateTime utcNow, DateTime lockedUntilUtc, int batchSize,
        CancellationToken cancellationToken);

    Task<bool> TryMarkDeliveredAsync(
        Guid deliveryId, Guid leaseToken, DateTime deliveredAtUtc,
        CancellationToken cancellationToken);

    Task<bool> TryScheduleRetryAsync(
        Guid deliveryId, Guid leaseToken, DateTime nextAttemptAtUtc,
        string error, CancellationToken cancellationToken);

    Task<bool> TryMarkFailedAsync(
        Guid deliveryId, Guid leaseToken, string error,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Delivery>> ListHistoryAsync(
        Guid teamId, DateTime fromUtc, int limit,
        CancellationToken cancellationToken);
}
