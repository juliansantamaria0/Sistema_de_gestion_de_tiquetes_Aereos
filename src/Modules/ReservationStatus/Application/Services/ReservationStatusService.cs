namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Application.UseCases;

public sealed class ReservationStatusService : IReservationStatusService
{
    private readonly CreateReservationStatusUseCase   _create;
    private readonly DeleteReservationStatusUseCase   _delete;
    private readonly GetAllReservationStatusesUseCase _getAll;
    private readonly GetReservationStatusByIdUseCase  _getById;
    private readonly UpdateReservationStatusUseCase   _update;

    public ReservationStatusService(
        CreateReservationStatusUseCase   create,
        DeleteReservationStatusUseCase   delete,
        GetAllReservationStatusesUseCase getAll,
        GetReservationStatusByIdUseCase  getById,
        UpdateReservationStatusUseCase   update)
    {
        _create  = create;
        _delete  = delete;
        _getAll  = getAll;
        _getById = getById;
        _update  = update;
    }

    public async Task<ReservationStatusDto> CreateAsync(
        string            name,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(name, cancellationToken);
        return new ReservationStatusDto(agg.Id.Value, agg.Name);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<ReservationStatusDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(a => new ReservationStatusDto(a.Id.Value, a.Name));
    }

    public async Task<ReservationStatusDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : new ReservationStatusDto(agg.Id.Value, agg.Name);
    }

    public async Task UpdateAsync(
        int               id,
        string            name,
        CancellationToken cancellationToken = default)
        => await _update.ExecuteAsync(id, name, cancellationToken);
}
