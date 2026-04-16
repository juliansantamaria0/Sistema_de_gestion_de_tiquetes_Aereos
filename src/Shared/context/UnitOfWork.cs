using Sistema_de_gestion_de_tiquetes_Aereos.src.Shared.contracts;

namespace Sistema_de_gestion_de_tiquetes_Aereos.src.shared.context;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;

    public UnitOfWork(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
