namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Domain.Repositories;

/// <summary>
/// Obtiene todos los asientos (con su estado actual) de un vuelo programado.
/// </summary>
public sealed class GetFlightSeatsByFlightUseCase
{
    private readonly IFlightSeatRepository _repository;

    public GetFlightSeatsByFlightUseCase(IFlightSeatRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<FlightSeatAggregate>> ExecuteAsync(
        int               scheduledFlightId,
        CancellationToken cancellationToken = default)
        => await _repository.GetByFlightAsync(scheduledFlightId, cancellationToken);
}
