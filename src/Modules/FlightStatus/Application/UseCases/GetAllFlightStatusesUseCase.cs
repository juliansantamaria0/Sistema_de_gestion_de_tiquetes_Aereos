namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Domain.Repositories;

public sealed class GetAllFlightStatusesUseCase
{
    private readonly IFlightStatusRepository _repository;

    public GetAllFlightStatusesUseCase(IFlightStatusRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<FlightStatusAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
