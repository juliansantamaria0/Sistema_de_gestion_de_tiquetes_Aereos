namespace Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;





public interface IUnitOfWork
{
    
    
    
    
    
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}
