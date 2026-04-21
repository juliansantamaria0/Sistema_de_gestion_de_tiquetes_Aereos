namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Domain.Repositories;

/// <summary>
/// Obtiene únicamente los asientos disponibles de un vuelo programado.
/// Caso de uso clave para el proceso de reserva: permite al cliente
/// elegir entre los asientos libres antes de confirmar.
/// </summary>
public sealed class GetAvailableFlightSeatsByFlightUseCase
{
    private readonly IFlightSeatRepository _repository;

    public GetAvailableFlightSeatsByFlightUseCase(IFlightSeatRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<FlightSeatAggregate>> ExecuteAsync(
        int               scheduledFlightId,
        CancellationToken cancellationToken = default)
        => await _repository.GetAvailableByFlightAsync(scheduledFlightId, cancellationToken);
}
