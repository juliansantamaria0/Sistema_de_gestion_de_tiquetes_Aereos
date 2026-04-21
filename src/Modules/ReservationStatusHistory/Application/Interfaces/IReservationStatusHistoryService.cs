namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Application.Interfaces;

public interface IReservationStatusHistoryService
{
    Task<ReservationStatusHistoryDto?>             GetByIdAsync(int id,                                                        CancellationToken cancellationToken = default);
    Task<IEnumerable<ReservationStatusHistoryDto>> GetAllAsync(                                                                 CancellationToken cancellationToken = default);
    Task<IEnumerable<ReservationStatusHistoryDto>> GetByReservationAsync(int reservationId,                                    CancellationToken cancellationToken = default);
    Task<ReservationStatusHistoryDto>              RecordAsync(int reservationId, int reservationStatusId, string? notes,     CancellationToken cancellationToken = default);
    Task                                           DeleteAsync(int id,                                                        CancellationToken cancellationToken = default);
}

public sealed record ReservationStatusHistoryDto(
    int      Id,
    int      ReservationId,
    int      ReservationStatusId,
    DateTime ChangedAt,
    string?  Notes);
