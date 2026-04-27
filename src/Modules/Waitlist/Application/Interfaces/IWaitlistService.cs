namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Waitlist.Application.Interfaces;

public interface IWaitlistService
{
    Task<WaitlistEntryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<WaitlistEntryDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<WaitlistEntryDto>> GetByFlightAsync(int scheduledFlightId, CancellationToken cancellationToken = default);

    Task<WaitlistEntryDto> AddAsync(
        int reservationId,
        int scheduledFlightId,
        int passengerId,
        int fareTypeId,
        CancellationToken cancellationToken = default);

    Task CancelEntryAsync(int id, CancellationToken cancellationToken = default);
}

public sealed record WaitlistEntryDto(
    int      Id,
    int      ReservationId,
    int      ScheduledFlightId,
    int      PassengerId,
    int      FareTypeId,
    DateTime FechaSolicitud,
    int      Prioridad,
    string   Estado,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

