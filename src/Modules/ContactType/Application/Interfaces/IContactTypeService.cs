namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Application.Interfaces;

public interface IContactTypeService
{
    Task<ContactTypeDto?>             GetByIdAsync(int id,            CancellationToken cancellationToken = default);
    Task<IEnumerable<ContactTypeDto>> GetAllAsync(                    CancellationToken cancellationToken = default);
    Task<ContactTypeDto>              CreateAsync(string name,        CancellationToken cancellationToken = default);
    Task                              UpdateAsync(int id, string name,CancellationToken cancellationToken = default);
    Task                              DeleteAsync(int id,             CancellationToken cancellationToken = default);
}

public sealed record ContactTypeDto(int Id, string Name);
