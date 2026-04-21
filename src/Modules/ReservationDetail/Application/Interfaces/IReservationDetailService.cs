namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Application.Interfaces;

public interface IReservationDetailService
{
    Task<ReservationDetailDto?>             GetByIdAsync(int id,                                                              CancellationToken cancellationToken = default);
    Task<IEnumerable<ReservationDetailDto>> GetAllAsync(                                                                      CancellationToken cancellationToken = default);
    Task<IEnumerable<ReservationDetailDto>> GetByReservationAsync(int reservationId,                                          CancellationToken cancellationToken = default);
    Task<ReservationDetailDto>              CreateAsync(int reservationId, int passengerId, int flightSeatId, int fareTypeId,  CancellationToken cancellationToken = default);
    Task                                    ChangeFareTypeAsync(int id, int fareTypeId,                                       CancellationToken cancellationToken = default);
    Task                                    DeleteAsync(int id,                                                               CancellationToken cancellationToken = default);
}

public sealed record ReservationDetailDto(
    int      Id,
    int      ReservationId,
    int      PassengerId,
    int      FlightSeatId,
    int      FareTypeId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
