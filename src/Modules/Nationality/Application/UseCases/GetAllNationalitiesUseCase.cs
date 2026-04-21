namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Application.Interfaces;

public sealed class GetAllNationalitiesUseCase
{
    private readonly INationalityService _service;
    public GetAllNationalitiesUseCase(INationalityService service) => _service = service;
    public Task<IReadOnlyList<NationalityDto>> ExecuteAsync(CancellationToken ct = default)
        => _service.GetAllAsync(ct);
}
