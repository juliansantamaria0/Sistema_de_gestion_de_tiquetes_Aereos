namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Domain.Repositories;






public sealed class GetAvailableFlightSeatsByFlightUseCase
{
    private readonly IFlightSeatRepository _repository;

    public GetAvailableFlightSeatsByFlightUseCase(IFlightSeatRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<FlightSeatAggregate>> ExecuteAsync(
        int               scheduledFlightId,
        CancellationToken cancellationToken = default)
        => await _repository.GetAvailableByFlightAsync(scheduledFlightId, cancellationToken);
}
