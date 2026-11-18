using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reminders.Core.Database;
using Reminders.Core.Models;

namespace Reminders.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCore(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = PostgresConnectionString.FromConfiguration(configuration);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Не задана строка подключения к PostgreSQL: databaseUrl.");
        }

        services.AddDbContext<RemindersDbContext>(options =>
            options.UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<Security.PasswordHasher>();

        return services;
    }
}
