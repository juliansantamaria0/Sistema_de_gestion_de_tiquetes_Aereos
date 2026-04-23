namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Domain.Repositories;







public sealed class GetFlightCancellationByFlightUseCase
{
    private readonly IFlightCancellationRepository _repository;

    public GetFlightCancellationByFlightUseCase(IFlightCancellationRepository repository)
    {
        _repository = repository;
    }

    public async Task<FlightCancellationAggregate?> ExecuteAsync(
        int               scheduledFlightId,
        CancellationToken cancellationToken = default)
        => await _repository.GetByFlightAsync(scheduledFlightId, cancellationToken);
}
