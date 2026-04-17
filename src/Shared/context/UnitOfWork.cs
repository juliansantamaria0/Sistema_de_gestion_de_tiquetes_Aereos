namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

/// <summary>
/// Implementación concreta de <see cref="IUnitOfWork"/>.
/// Envuelve <see cref="AppDbContext.SaveChangesAsync"/> para desacoplar
/// los módulos de la dependencia directa a EF Core.
/// Se registra con ciclo de vida Scoped junto al DbContext.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
