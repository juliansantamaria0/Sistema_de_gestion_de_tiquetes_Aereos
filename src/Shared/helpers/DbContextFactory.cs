namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.Helpers;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

/// <summary>
/// Helper para crear instancias de <see cref="AppDbContext"/> fuera del
/// contenedor DI (pruebas de integración, scripts utilitarios, jobs).
/// </summary>
public static class DbContextFactory
{
    /// <summary>
    /// Crea y retorna un <see cref="AppDbContext"/> listo para usar.
    /// La instancia NO está gestionada por el contenedor DI;
    /// el llamador es responsable de hacer Dispose (usar <c>await using</c>).
    /// </summary>
    /// <param name="connectionString">Cadena de conexión MySQL.</param>
    /// <returns>Nueva instancia de <see cref="AppDbContext"/>.</returns>
    /// <example>
    /// <code>
    /// await using var context = DbContextFactory.Create(connectionString);
    /// var countries = await context.Countries.ToListAsync();
    /// </code>
    /// </example>
    public static AppDbContext Create(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException(
                "Connection string cannot be null or empty.", nameof(connectionString));

        var serverVersion = MySqlVersionResolver.Resolve(connectionString);

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql(connectionString, serverVersion, mySqlOptions =>
            {
                mySqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null);
            })
            .UseSnakeCaseNamingConvention()
            .Options;

        return new AppDbContext(options);
    }

    /// <summary>
    /// Crea un contexto a partir de un <see cref="DbContextOptions{TContext}"/>
    /// ya configurado. Útil para tests con InMemory o SQLite.
    /// </summary>
    /// <param name="options">Opciones pre-configuradas.</param>
    /// <returns>Nueva instancia de <see cref="AppDbContext"/>.</returns>
    public static AppDbContext CreateFromOptions(DbContextOptions<AppDbContext> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return new AppDbContext(options);
    }
}
