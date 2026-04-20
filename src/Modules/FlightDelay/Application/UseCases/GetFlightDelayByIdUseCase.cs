namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Domain.ValueObject;

public sealed class GetFlightDelayByIdUseCase
{
    private readonly IFlightDelayRepository _repository;

    public GetFlightDelayByIdUseCase(IFlightDelayRepository repository)
    {
        _repository = repository;
    }

    public async Task<FlightDelayAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new FlightDelayId(id), cancellationToken);
}
