namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;

using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;







public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    
    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
