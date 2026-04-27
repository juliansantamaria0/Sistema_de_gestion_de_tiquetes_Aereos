using Microsoft.Extensions.Configuration;

namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;

/// <summary>Resuelve la cadena MySQL en un solo lugar: configuración, variables de entorno, sin duplicar lógica.</summary>
public static class ConnectionStringResolver
{
    public static string GetRequiredMySqlConnectionString(IConfiguration configuration)
    {
        // 1) Variables de entorno explícitas (útiles en CI / Aiven sin tocar el repo)
        var c = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
        if (IsSet(c)) return c!.Trim();
        c = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING");
        if (IsSet(c)) return c!.Trim();

        // 2) IConfiguration (appsettings + posibles user-secrets, según el host)
        c = configuration.GetConnectionString("DefaultConnection");
        if (IsSet(c)) return c!.Trim();

        c = configuration.GetConnectionString("MySqlDB");
        if (IsSet(c)) return c!.Trim();

        c = configuration["ConnectionStrings:DefaultConnection"];
        if (IsSet(c)) return c!.Trim();

        throw new InvalidOperationException(
            "No se encontró la cadena de conexión MySQL. " +
            "Defina la variable de entorno ConnectionStrings__DefaultConnection o MYSQL_CONNECTION_STRING, " +
            "o añada ConnectionStrings:DefaultConnection en su configuración local (p. ej. appsettings.Development.json en la carpeta del proyecto, excluido de git).");
    }

    private static bool IsSet(string? s) => !string.IsNullOrWhiteSpace(s);
}
