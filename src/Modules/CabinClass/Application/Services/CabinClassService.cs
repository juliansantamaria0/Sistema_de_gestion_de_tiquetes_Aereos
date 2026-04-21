namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CabinClass.Application.UseCases;

public sealed class CabinClassService : ICabinClassService
{
    private readonly CreateCabinClassUseCase   _create;
    private readonly DeleteCabinClassUseCase   _delete;
    private readonly GetAllCabinClassesUseCase _getAll;
    private readonly GetCabinClassByIdUseCase  _getById;
    private readonly UpdateCabinClassUseCase   _update;

    public CabinClassService(
        CreateCabinClassUseCase   create,
        DeleteCabinClassUseCase   delete,
        GetAllCabinClassesUseCase getAll,
        GetCabinClassByIdUseCase  getById,
        UpdateCabinClassUseCase   update)
    {
        _create  = create;
        _delete  = delete;
        _getAll  = getAll;
        _getById = getById;
        _update  = update;
    }

    public async Task<CabinClassDto> CreateAsync(
        string name, CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(name, cancellationToken);
        return new CabinClassDto(agg.Id.Value, agg.Name);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<CabinClassDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(a => new CabinClassDto(a.Id.Value, a.Name));
    }

    public async Task<CabinClassDto?> GetByIdAsync(
        int id, CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : new CabinClassDto(agg.Id.Value, agg.Name);
    }

    public async Task UpdateAsync(
        int id, string name, CancellationToken cancellationToken = default)
        => await _update.ExecuteAsync(id, name, cancellationToken);
}
