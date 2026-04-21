namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.Repositories;

/// <summary>
/// Obtiene todas las instancias de vuelo asociadas a un vuelo base.
/// Útil para ver el historial de operaciones de un vuelo recurrente.
/// </summary>
public sealed class GetScheduledFlightsByBaseFlightUseCase
{
    private readonly IScheduledFlightRepository _repository;

    public GetScheduledFlightsByBaseFlightUseCase(IScheduledFlightRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ScheduledFlightAggregate>> ExecuteAsync(
        int               baseFlightId,
        CancellationToken cancellationToken = default)
        => await _repository.GetByBaseFlightAsync(baseFlightId, cancellationToken);
}
