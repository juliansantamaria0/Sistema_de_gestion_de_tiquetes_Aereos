using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Helpers;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // La cadena se resuelve al crear el DbContext (IConfiguration ya incluye variables de entorno y JSON).
        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var config = serviceProvider.GetRequiredService<IConfiguration>();
            var connectionString = ConnectionStringResolver.GetRequiredMySqlConnectionString(config);

            var configuredServerVersion = config["MySqlServerVersion"];
            var serverVersion = string.IsNullOrWhiteSpace(configuredServerVersion)
                ? MySqlVersionResolver.Resolve(connectionString)
                : ServerVersion.Parse(configuredServerVersion);

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
            .Where(t => t is { IsClass: true, IsAbstract: false, DeclaringType: null })
            .ToList();

        foreach (var type in concreteTypes.Where(t => t.Namespace?.Contains(".UseCases", StringComparison.Ordinal) == true))
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
                .Where(i => !IsFrameworkInterface(i) && i.Namespace?.StartsWith("Sistema_de_gestion_de_tiquetes_Aereos", StringComparison.Ordinal) == true)
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
