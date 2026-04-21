namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Application.Interfaces;

public sealed class UpdateNationalityUseCase
{
    private readonly INationalityService _service;
    public UpdateNationalityUseCase(INationalityService service) => _service = service;
    public Task<NationalityDto> ExecuteAsync(int id, UpdateNationalityRequest request, CancellationToken ct = default)
        => _service.UpdateAsync(id, request, ct);
}
