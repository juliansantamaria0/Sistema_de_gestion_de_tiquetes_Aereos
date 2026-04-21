namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Domain.Aggregate;

public sealed class ReservationStatusHistoryService : IReservationStatusHistoryService
{
    private readonly RecordReservationStatusHistoryUseCase              _record;
    private readonly DeleteReservationStatusHistoryUseCase              _delete;
    private readonly GetAllReservationStatusHistoryUseCase              _getAll;
    private readonly GetReservationStatusHistoryByIdUseCase             _getById;
    private readonly GetReservationStatusHistoryByReservationUseCase    _getByReservation;

    public ReservationStatusHistoryService(
        RecordReservationStatusHistoryUseCase           record,
        DeleteReservationStatusHistoryUseCase           delete,
        GetAllReservationStatusHistoryUseCase           getAll,
        GetReservationStatusHistoryByIdUseCase          getById,
        GetReservationStatusHistoryByReservationUseCase getByReservation)
    {
        _record           = record;
        _delete           = delete;
        _getAll           = getAll;
        _getById          = getById;
        _getByReservation = getByReservation;
    }

    public async Task<ReservationStatusHistoryDto> RecordAsync(
        int               reservationId,
        int               reservationStatusId,
        string?           notes,
        CancellationToken cancellationToken = default)
    {
        var agg = await _record.ExecuteAsync(
            reservationId, reservationStatusId, notes, cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<ReservationStatusHistoryDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<ReservationStatusHistoryDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task<IEnumerable<ReservationStatusHistoryDto>> GetByReservationAsync(
        int               reservationId,
        CancellationToken cancellationToken = default)
    {
        var list = await _getByReservation.ExecuteAsync(reservationId, cancellationToken);
        return list.Select(ToDto);
    }

    private static ReservationStatusHistoryDto ToDto(ReservationStatusHistoryAggregate agg)
        => new(agg.Id.Value, agg.ReservationId, agg.ReservationStatusId, agg.ChangedAt, agg.Notes);
}
