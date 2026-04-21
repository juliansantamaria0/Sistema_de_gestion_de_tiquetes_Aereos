namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Application.Interfaces;

public interface IFlightSeatService
{
    Task<FlightSeatDto?>             GetByIdAsync(int id,                                          CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightSeatDto>> GetAllAsync(                                                  CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightSeatDto>> GetByFlightAsync(int scheduledFlightId,                       CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightSeatDto>> GetAvailableByFlightAsync(int scheduledFlightId,              CancellationToken cancellationToken = default);
    Task<FlightSeatDto>              CreateAsync(int scheduledFlightId, int seatMapId, int seatStatusId, CancellationToken cancellationToken = default);
    Task                             ChangeStatusAsync(int id, int seatStatusId,                   CancellationToken cancellationToken = default);
    Task                             DeleteAsync(int id,                                           CancellationToken cancellationToken = default);
}

public sealed record FlightSeatDto(
    int      Id,
    int      ScheduledFlightId,
    int      SeatMapId,
    int      SeatStatusId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
