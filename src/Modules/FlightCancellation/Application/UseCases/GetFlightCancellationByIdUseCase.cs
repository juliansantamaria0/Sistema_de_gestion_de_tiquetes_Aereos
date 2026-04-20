namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Domain.ValueObject;

public sealed class GetFlightCancellationByIdUseCase
{
    private readonly IFlightCancellationRepository _repository;

    public GetFlightCancellationByIdUseCase(IFlightCancellationRepository repository)
    {
        _repository = repository;
    }

    public async Task<FlightCancellationAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new FlightCancellationId(id), cancellationToken);
}
