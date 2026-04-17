namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Helpers;

/// <summary>
/// Métodos de extensión para registrar la infraestructura compartida
/// en el contenedor de dependencias de .NET 8.
/// </summary>
public static class DependencyInjeccion
{
    /// <summary>
    /// Registra el <see cref="AppDbContext"/> con Pomelo MySQL,
    /// convención snake_case, reintentos automáticos y el
    /// <see cref="IUnitOfWork"/> en el contenedor DI.
    /// </summary>
    /// <param name="services">Colección de servicios de la aplicación.</param>
    /// <param name="configuration">Configuración de la aplicación (appsettings).</param>
    /// <returns>La misma <see cref="IServiceCollection"/> para encadenamiento.</returns>
    /// <exception cref="InvalidOperationException">
    /// Se lanza si la cadena de conexión 'DefaultConnection' no está configurada.
    /// </exception>
    /// <example>
    /// <code>
    /// // En Program.cs:
    /// builder.Services.AddSharedInfrastructure(builder.Configuration);
    /// </code>
    /// </example>
    public static IServiceCollection AddSharedInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "La cadena de conexión 'DefaultConnection' no está configurada. " +
                "Verifique appsettings.json o las variables de entorno.");

        var serverVersion = MySqlVersionResolver.Resolve(connectionString);

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseMySql(connectionString, serverVersion, mySqlOptions =>
            {
                // Reintentos automáticos ante fallos transitorios de red/DB.
                mySqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);

                // Tiempo máximo de espera por comando SQL (segundos).
                mySqlOptions.CommandTimeout(60);
            })
            // Convierte automáticamente PascalCase (C#) ↔ snake_case (MySQL).
            // Requiere: EFCore.NamingConventions NuGet package.
            .UseSnakeCaseNamingConvention();


            // En desarrollo: logging detallado de SQL y datos sensibles.
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();

        });

        // IUnitOfWork se resuelve desde el mismo AppDbContext del scope actual.
        // Usar Scoped para que comparta la misma instancia dentro de una petición.
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
