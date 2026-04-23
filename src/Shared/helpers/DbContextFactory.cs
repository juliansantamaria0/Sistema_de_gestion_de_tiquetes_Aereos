namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.Helpers;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;





public static class DbContextFactory
{
    
    
    
    
    
    
    
    
    
    
    
    
    
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

    
    
    
    
    
    
    public static AppDbContext CreateFromOptions(DbContextOptions<AppDbContext> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return new AppDbContext(options);
    }
}
