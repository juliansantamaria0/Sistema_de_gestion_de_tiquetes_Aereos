namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Domain.ValueObject;

public sealed class GetFlightCrewByIdUseCase
{
    private readonly IFlightCrewRepository _repository;

    public GetFlightCrewByIdUseCase(IFlightCrewRepository repository)
    {
        _repository = repository;
    }

    public async Task<FlightCrewAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new FlightCrewId(id), cancellationToken);
}
