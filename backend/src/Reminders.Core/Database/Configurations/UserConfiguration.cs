using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reminders.Core.Models;

namespace Reminders.Core.Database.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", table =>
            table.HasCheckConstraint("ck_users_email_normalized", "email = lower(btrim(email))"));

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Email)
            .HasMaxLength(254)
            .IsRequired();

        builder.HasIndex(user => user.Email).IsUnique();

        builder.Property(user => user.PasswordHash).IsRequired();
        builder.Property(user => user.CreatedAt).IsRequired();
        builder.Property(user => user.DeactivatedAt).IsRequired(false);
    }
}
