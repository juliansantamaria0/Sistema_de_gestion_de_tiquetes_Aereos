namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

/// <summary>
/// Contrato de unidad de trabajo. Permite confirmar transacciones
/// sin acoplar la capa de aplicación a EF Core directamente.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Persiste todos los cambios pendientes en la base de datos.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Número de filas afectadas.</returns>
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}
