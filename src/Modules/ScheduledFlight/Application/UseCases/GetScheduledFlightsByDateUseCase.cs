namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.Repositories;





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
