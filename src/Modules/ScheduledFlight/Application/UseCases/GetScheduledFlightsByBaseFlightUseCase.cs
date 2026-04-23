namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.Repositories;





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
