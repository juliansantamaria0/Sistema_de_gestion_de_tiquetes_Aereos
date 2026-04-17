namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.Helpers;

using Microsoft.EntityFrameworkCore;
using MySqlConnector;

/// <summary>
/// Resuelve la versión exacta del servidor MySQL en tiempo de ejecución.
/// Evita hardcodear la versión del servidor en el código fuente.
/// </summary>
public static class MySqlVersionResolver
{
    /// <summary>
    /// Abre una conexión breve a la base de datos y consulta la versión
    /// del servidor. En caso de fallo, devuelve una versión mínima segura (8.0.21).
    /// </summary>
    /// <param name="connectionString">Cadena de conexión MySQL válida.</param>
    /// <returns>
    /// <see cref="ServerVersion"/> detectada automáticamente o
    /// <c>ServerVersion.Parse("8.0.21")</c> como fallback.
    /// </returns>
    public static ServerVersion Resolve(string connectionString)
    {
        try
        {
            return ServerVersion.AutoDetect(connectionString);
        }
        catch (Exception ex)
        {
            // En entornos CI/CD o diseño en frío la BD puede no estar disponible.
            // Fallback: MySQL 8.0.21 — versión mínima compatible con Pomelo 8.x
            Console.Error.WriteLine(
                $"[MySqlVersionResolver] AutoDetect failed: {ex.Message}. " +
                "Falling back to MySQL 8.0.21.");

            return ServerVersion.Parse("8.0.21-mysql");
        }
    }

    /// <summary>
    /// Versión de fallback explícita para usarse en tests o migraciones offline.
    /// </summary>
    public static ServerVersion Fallback => ServerVersion.Parse("8.0.21-mysql");
}
