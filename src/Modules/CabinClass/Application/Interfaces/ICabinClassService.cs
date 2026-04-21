namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Application.Interfaces;

public interface ICabinClassService
{
    Task<CabinClassDto?>             GetByIdAsync(int id,            CancellationToken cancellationToken = default);
    Task<IEnumerable<CabinClassDto>> GetAllAsync(                    CancellationToken cancellationToken = default);
    Task<CabinClassDto>              CreateAsync(string name,        CancellationToken cancellationToken = default);
    Task                             UpdateAsync(int id, string name,CancellationToken cancellationToken = default);
    Task                             DeleteAsync(int id,             CancellationToken cancellationToken = default);
}

public sealed record CabinClassDto(int Id, string Name);
