using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Reminders.Core.Database;

public static class PostgresConnectionString
{
    public static string? FromConfiguration(IConfiguration configuration)
    {
        var value = configuration["databaseUrl"];
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (!value.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) &&
            !value.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                return new NpgsqlConnectionStringBuilder(value).ConnectionString;
            }
            catch (ArgumentException)
            {
                throw new InvalidOperationException("Некорректная строка подключения PostgreSQL в databaseUrl.");
            }
        }

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
            string.IsNullOrEmpty(uri.Host))
        {
            throw new InvalidOperationException("Некорректный адрес PostgreSQL в databaseUrl.");
        }

        var credentials = uri.GetComponents(UriComponents.UserInfo, UriFormat.UriEscaped).Split(':', 2);
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Database = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/'))
        };

        if (uri.Port == 0)
            throw new InvalidOperationException("Некорректный порт PostgreSQL в databaseUrl.");
        if (!uri.IsDefaultPort && uri.Port > 0)
            builder.Port = uri.Port;
        if (credentials.Length > 0 && credentials[0].Length > 0)
            builder.Username = Uri.UnescapeDataString(credentials[0]);
        if (credentials.Length > 1)
            builder.Password = Uri.UnescapeDataString(credentials[1]);

        foreach (var item in uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var parameter = item.Split('=', 2);
            if (parameter.Length != 2)
                throw new InvalidOperationException("Некорректный параметр PostgreSQL в databaseUrl.");

            var name = Uri.UnescapeDataString(parameter[0]).Replace('_', ' ');
            var parameterValue = Uri.UnescapeDataString(parameter[1]);

            try
            {
                if (name.Equals("sslmode", StringComparison.OrdinalIgnoreCase) ||
                    name.Equals("ssl mode", StringComparison.OrdinalIgnoreCase))
                {
                    var mode = parameterValue.Replace("-", "").Replace("_", "");
                    if (mode.Any(char.IsDigit) || !Enum.TryParse<SslMode>(mode, ignoreCase: true, out var sslMode) ||
                        !Enum.IsDefined(sslMode))
                    {
                        throw new ArgumentException("Некорректный режим SSL.");
                    }

                    builder.SslMode = sslMode;
                }
                else
                {
                    var option = name.ToLowerInvariant() switch
                    {
                        "sslrootcert" => "Root Certificate",
                        "sslcert" => "SSL Certificate",
                        "sslkey" => "SSL Key",
                        _ => name
                    };
                    builder[option] = parameterValue;
                }
            }
            catch (ArgumentException)
            {
                throw new InvalidOperationException("Некорректный параметр PostgreSQL в databaseUrl.");
            }
        }

        return builder.ConnectionString;
    }
}
