namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Domain.ValueObject;

public sealed class GetFlightStatusByIdUseCase
{
    private readonly IFlightStatusRepository _repository;

    public GetFlightStatusByIdUseCase(IFlightStatusRepository repository)
    {
        _repository = repository;
    }

    public async Task<FlightStatusAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new FlightStatusId(id), cancellationToken);
}
