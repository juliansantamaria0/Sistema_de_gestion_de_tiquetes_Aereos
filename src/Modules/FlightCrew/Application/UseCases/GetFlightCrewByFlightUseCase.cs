namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Domain.Repositories;

/// <summary>
/// Obtiene toda la tripulación asignada a un vuelo programado.
/// Caso de uso clave para la gestión operativa del vuelo.
/// </summary>
public sealed class GetFlightCrewByFlightUseCase
{
    private readonly IFlightCrewRepository _repository;

    public GetFlightCrewByFlightUseCase(IFlightCrewRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<FlightCrewAggregate>> ExecuteAsync(
        int               scheduledFlightId,
        CancellationToken cancellationToken = default)
        => await _repository.GetByFlightAsync(scheduledFlightId, cancellationToken);
}
