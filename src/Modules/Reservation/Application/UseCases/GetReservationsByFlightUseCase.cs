namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.Repositories;

/// <summary>
/// Obtiene todas las reservas de un vuelo programado.
/// Caso de uso clave para la gestión operativa del vuelo.
/// </summary>
public sealed class GetReservationsByFlightUseCase
{
    private readonly IReservationRepository _repository;

    public GetReservationsByFlightUseCase(IReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ReservationAggregate>> ExecuteAsync(
        int               scheduledFlightId,
        CancellationToken cancellationToken = default)
        => await _repository.GetByFlightAsync(scheduledFlightId, cancellationToken);
}
