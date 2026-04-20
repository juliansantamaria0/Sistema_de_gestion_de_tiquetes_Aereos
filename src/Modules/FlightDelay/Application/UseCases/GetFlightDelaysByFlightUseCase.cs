namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Domain.Repositories;

/// <summary>
/// Obtiene todos los registros de retraso de un vuelo programado.
/// Un vuelo puede acumular múltiples retrasos durante su operación.
/// </summary>
public sealed class GetFlightDelaysByFlightUseCase
{
    private readonly IFlightDelayRepository _repository;

    public GetFlightDelaysByFlightUseCase(IFlightDelayRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<FlightDelayAggregate>> ExecuteAsync(
        int               scheduledFlightId,
        CancellationToken cancellationToken = default)
        => await _repository.GetByFlightAsync(scheduledFlightId, cancellationToken);
}
