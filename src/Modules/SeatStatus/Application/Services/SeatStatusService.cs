namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Application.UseCases;

public sealed class SeatStatusService : ISeatStatusService
{
    private readonly CreateSeatStatusUseCase   _create;
    private readonly DeleteSeatStatusUseCase   _delete;
    private readonly GetAllSeatStatusesUseCase _getAll;
    private readonly GetSeatStatusByIdUseCase  _getById;
    private readonly UpdateSeatStatusUseCase   _update;

    public SeatStatusService(
        CreateSeatStatusUseCase   create,
        DeleteSeatStatusUseCase   delete,
        GetAllSeatStatusesUseCase getAll,
        GetSeatStatusByIdUseCase  getById,
        UpdateSeatStatusUseCase   update)
    {
        _create  = create;
        _delete  = delete;
        _getAll  = getAll;
        _getById = getById;
        _update  = update;
    }

    public async Task<SeatStatusDto> CreateAsync(
        string            name,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(name, cancellationToken);
        return new SeatStatusDto(agg.Id.Value, agg.Name);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<SeatStatusDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(a => new SeatStatusDto(a.Id.Value, a.Name));
    }

    public async Task<SeatStatusDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : new SeatStatusDto(agg.Id.Value, agg.Name);
    }

    public async Task UpdateAsync(
        int               id,
        string            name,
        CancellationToken cancellationToken = default)
        => await _update.ExecuteAsync(id, name, cancellationToken);
}
