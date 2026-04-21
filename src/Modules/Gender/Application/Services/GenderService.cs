namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Application.UseCases;

public sealed class GenderService : IGenderService
{
    private readonly CreateGenderUseCase  _create;
    private readonly DeleteGenderUseCase  _delete;
    private readonly GetAllGendersUseCase _getAll;
    private readonly GetGenderByIdUseCase _getById;
    private readonly UpdateGenderUseCase  _update;

    public GenderService(
        CreateGenderUseCase  create,
        DeleteGenderUseCase  delete,
        GetAllGendersUseCase getAll,
        GetGenderByIdUseCase getById,
        UpdateGenderUseCase  update)
    {
        _create  = create;
        _delete  = delete;
        _getAll  = getAll;
        _getById = getById;
        _update  = update;
    }

    public async Task<GenderDto> CreateAsync(string name, CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(name, cancellationToken);
        return new GenderDto(agg.Id.Value, agg.Name);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<GenderDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(a => new GenderDto(a.Id.Value, a.Name));
    }

    public async Task<GenderDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : new GenderDto(agg.Id.Value, agg.Name);
    }

    public async Task UpdateAsync(int id, string name, CancellationToken cancellationToken = default)
        => await _update.ExecuteAsync(id, name, cancellationToken);
}
