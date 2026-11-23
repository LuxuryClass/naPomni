using Reminders.Core.Models;

namespace Reminders.Core.Repositories;

public interface IChannelRepository
{
    Task<ChannelBinding?> FindBindingByCodeHashAsync(
        string codeHash, CancellationToken cancellationToken);

    Task<ChannelTarget?> FindTargetAsync(
        Guid teamId, Guid targetId, CancellationToken cancellationToken);

    Task<IReadOnlyList<ChannelTarget>> ListTargetsAsync(
        Guid teamId, CancellationToken cancellationToken);

    void AddBinding(ChannelBinding binding);

    void AddTarget(ChannelTarget target);

    void RemoveTarget(ChannelTarget target);
}
