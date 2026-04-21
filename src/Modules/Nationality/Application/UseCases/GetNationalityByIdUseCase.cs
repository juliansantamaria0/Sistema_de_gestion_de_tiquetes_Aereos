namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Application.Interfaces;

public sealed class GetNationalityByIdUseCase
{
    private readonly INationalityService _service;
    public GetNationalityByIdUseCase(INationalityService service) => _service = service;
    public Task<NationalityDto?> ExecuteAsync(int id, CancellationToken ct = default)
        => _service.GetByIdAsync(id, ct);
}
