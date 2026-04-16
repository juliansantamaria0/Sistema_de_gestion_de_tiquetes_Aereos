using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sistema_de_gestion_de_tiquetes_Aereos.src.shared.context;
using Sistema_de_gestion_de_tiquetes_Aereos.src.Shared.contracts;

namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "La cadena de conexión 'DefaultConnection' no está configurada. " +
                "Verifica appsettings.json o las variables de entorno.");

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