namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Application.Interfaces;

public interface INationalityService
{
    Task<NationalityDto>               CreateAsync(CreateNationalityRequest  request, CancellationToken ct = default);
    Task<NationalityDto?>              GetByIdAsync(int id,                           CancellationToken ct = default);
    Task<IReadOnlyList<NationalityDto>> GetAllAsync(CancellationToken ct = default);
    Task<NationalityDto>               UpdateAsync(int id, UpdateNationalityRequest request, CancellationToken ct = default);
    Task                               DeleteAsync(int id, CancellationToken ct = default);
}

public sealed record NationalityDto(int NationalityId, int CountryId, string Demonym);
public sealed record CreateNationalityRequest(int CountryId, string Demonym);
public sealed record UpdateNationalityRequest(int CountryId, string Demonym);
