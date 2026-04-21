namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.ValueObject;

public sealed class GetScheduledFlightByIdUseCase
{
    private readonly IScheduledFlightRepository _repository;

    public GetScheduledFlightByIdUseCase(IScheduledFlightRepository repository)
    {
        _repository = repository;
    }

    public async Task<ScheduledFlightAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new ScheduledFlightId(id), cancellationToken);
}
