namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Domain.Repositories;

public sealed class GetAllFlightCrewsUseCase
{
    private readonly IFlightCrewRepository _repository;

    public GetAllFlightCrewsUseCase(IFlightCrewRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<FlightCrewAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
