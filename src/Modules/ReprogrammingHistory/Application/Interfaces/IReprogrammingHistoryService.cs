namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReprogrammingHistory.Application.Interfaces;

public interface IReprogrammingHistoryService
{
    Task<IEnumerable<ReprogrammingHistoryDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ReprogrammingHistoryDto>> GetByReservationAsync(int reservationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ReprogrammingHistoryDto>> GetByFlightAsync(int scheduledFlightId, CancellationToken cancellationToken = default);
}

public sealed record ReprogrammingHistoryDto(
    int      Id,
    int      ReservationId,
    int      VueloAnteriorId,
    int      NuevoVueloId,
    DateTime FechaCambio,
    string?  Motivo,
    DateTime CreatedAt);

