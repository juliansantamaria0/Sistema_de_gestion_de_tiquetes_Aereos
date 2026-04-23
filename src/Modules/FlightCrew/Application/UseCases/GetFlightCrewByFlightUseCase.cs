namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Domain.Repositories;





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
