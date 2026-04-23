namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.Helpers;

using Microsoft.EntityFrameworkCore;
using MySqlConnector;





public static class MySqlVersionResolver
{
    
    
    
    
    
    
    
    
    
    public static ServerVersion Resolve(string connectionString)
    {
        try
        {
            return ServerVersion.AutoDetect(connectionString);
        }
        catch (Exception ex)
        {
            
            
            Console.Error.WriteLine(
                $"[MySqlVersionResolver] AutoDetect failed: {ex.Message}. " +
                "Falling back to MySQL 8.0.21.");

            return ServerVersion.Parse("8.0.21-mysql");
        }
    }

    
    
    
    public static ServerVersion Fallback => ServerVersion.Parse("8.0.21-mysql");
}
