namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Helpers;

/// <summary>
/// Fábrica de diseño para que las herramientas de EF Core (dotnet-ef)
/// puedan crear instancias del contexto sin pasar por el host de la aplicación.
/// Necesario para ejecutar: dotnet ef migrations add / dotnet ef database update.
/// </summary>
public sealed class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // Busca appsettings.json en el directorio de trabajo actual.
        // Ajusta la ruta base si tu proyecto tiene una estructura diferente.
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found in appsettings.json.");

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseMySql(
            connectionString,
            MySqlVersionResolver.Resolve(connectionString),
            mySqlOptions =>
            {
                mySqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null);
            })
            .UseSnakeCaseNamingConvention();

        return new AppDbContext(optionsBuilder.Options);
    }
}
