namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Alias de compatibilidad para mantener código legado que use el nombre
/// en español de la clase de registro de dependencias.
/// </summary>
public static class DependencyInjeccion
{
    /// <summary>
    /// Redirige al método canónico <see cref="DependencyInjection.AddSharedInfrastructure"/>.
    /// </summary>
    public static IServiceCollection AddSharedInfrastructureLegacy(
        this IServiceCollection services,
        IConfiguration configuration)
        => DependencyInjection.AddSharedInfrastructure(services, configuration);
}
