using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reminders.Core.Models;

namespace Reminders.Core.Database.Configurations
{
    public class TelegramBotStateConfiguration : IEntityTypeConfiguration<TelegramBotState>
    {
        private const int SingletonId = 1;

        public void Configure(EntityTypeBuilder<TelegramBotState> builder)
        {
            builder.ToTable("telegram_bot_state", table =>
            table.HasCheckConstraint(
                "ck_telegram_bot_state_singleton",
                $"id = {SingletonId}"));

            builder.Property(state => state.Id)
                .ValueGeneratedNever();
        }
    }
}
