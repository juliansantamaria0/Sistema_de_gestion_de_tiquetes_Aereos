using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;


namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? configuration.GetConnectionString("MySqlDB")
            ?? throw new InvalidOperationException(
                "No se encontró una cadena de conexión válida. " +
                "Configura 'DefaultConnection' o 'MySqlDB' en appsettings.json o variables de entorno.");

        var serverVersion = ServerVersion.AutoDetect(connectionString);

        services.AddDbContext<AppDbContext>(options =>
        {
            options
                .UseMySql(connectionString, serverVersion, mySqlOptions =>
                {
                    mySqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);
                })
                

                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()

            ;
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}