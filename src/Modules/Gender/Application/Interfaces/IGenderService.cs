namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Application.Interfaces;

public interface IGenderService
{
    Task<GenderDto?>             GetByIdAsync(int id,            CancellationToken cancellationToken = default);
    Task<IEnumerable<GenderDto>> GetAllAsync(                    CancellationToken cancellationToken = default);
    Task<GenderDto>              CreateAsync(string name,        CancellationToken cancellationToken = default);
    Task                         UpdateAsync(int id, string name,CancellationToken cancellationToken = default);
    Task                         DeleteAsync(int id,             CancellationToken cancellationToken = default);
}

public sealed record GenderDto(int Id, string Name);
