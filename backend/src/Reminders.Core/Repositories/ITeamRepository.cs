using Reminders.Core.Models;

namespace Reminders.Core.Repositories;

public interface ITeamRepository
{
    Task<Team?> FindByIdAsync(Guid teamId, CancellationToken cancellationToken);

    Task<IReadOnlyList<TeamMember>> ListMembershipsByUserIdAsync(
        Guid userId, CancellationToken cancellationToken);

    Task<TeamMember?> FindMemberAsync(
        Guid teamId, Guid userId, CancellationToken cancellationToken);

    Task<IReadOnlyList<TeamMember>> ListMembersAsync(
        Guid teamId, CancellationToken cancellationToken);

    void Add(Team team);

    void AddMember(TeamMember member);

    void RemoveMember(TeamMember member);
}
