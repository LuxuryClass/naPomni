using Microsoft.EntityFrameworkCore;
using Reminders.Core.Models;

namespace Reminders.Core.Database;

public class RemindersDbContext : DbContext
{
    public RemindersDbContext(DbContextOptions<RemindersDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
    public DbSet<Reminder> Reminders => Set<Reminder>();
    public DbSet<ChannelBinding> ChannelBindings => Set<ChannelBinding>();
    public DbSet<ChannelTarget> ChannelTargets => Set<ChannelTarget>();
    public DbSet<TelegramBotState> TelegramBotStates => Set<TelegramBotState>();
    public DbSet<Delivery> Deliveries => Set<Delivery>();
}

