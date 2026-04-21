namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.Repositories;

/// <summary>
/// Obtiene todos los vuelos programados para una fecha concreta.
/// Punto de entrada clave para la operación diaria del aeropuerto.
/// </summary>
public sealed class GetScheduledFlightsByDateUseCase
{
    private readonly IScheduledFlightRepository _repository;

    public GetScheduledFlightsByDateUseCase(IScheduledFlightRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ScheduledFlightAggregate>> ExecuteAsync(
        DateOnly          date,
        CancellationToken cancellationToken = default)
        => await _repository.GetByDateAsync(date, cancellationToken);
}
