using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection") ??
            configuration.GetConnectionString("MySqlDB") ??
            configuration["ConnectionStrings__DefaultConnection"] ??
            throw new InvalidOperationException(
                "No se encontro la cadena de conexion 'DefaultConnection' o 'MySqlDB'.");

        var configuredServerVersion = configuration["MySqlServerVersion"];
        var serverVersion = string.IsNullOrWhiteSpace(configuredServerVersion)
            ? ServerVersion.AutoDetect(connectionString)
            : ServerVersion.Parse(configuredServerVersion);

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
                .UseSnakeCaseNamingConvention();
#if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
#endif
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        RegisterByConventions(services, Assembly.GetExecutingAssembly());

        return services;
    }

    private static void RegisterByConventions(IServiceCollection services, Assembly assembly)
    {
        var concreteTypes = assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .ToList();

        // Registra todos los casos de uso y otras clases concretas para resolución por constructor.
        foreach (var type in concreteTypes.Where(t => t.Namespace?.Contains(".UseCases") == true))
        {
            services.AddScoped(type);
        }

        foreach (var type in concreteTypes.Where(t => typeof(IModuleUI).IsAssignableFrom(t)))
        {
            services.AddScoped(typeof(IModuleUI), type);
            services.AddScoped(type);
        }

        foreach (var type in concreteTypes)
        {
            var interfaces = type.GetInterfaces()
                .Where(i => !IsFrameworkInterface(i))
                .ToList();

            foreach (var @interface in interfaces)
            {
                if (@interface == typeof(IModuleUI))
                    continue;

                services.AddScoped(@interface, type);
            }
        }
    }

    private static bool IsFrameworkInterface(Type type)
        => type.Namespace is not null &&
           (type.Namespace.StartsWith("System", StringComparison.Ordinal) ||
            type.Namespace.StartsWith("Microsoft", StringComparison.Ordinal));
}
