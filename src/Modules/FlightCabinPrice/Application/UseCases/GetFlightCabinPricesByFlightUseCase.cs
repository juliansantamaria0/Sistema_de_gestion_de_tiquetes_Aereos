namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Domain.Repositories;





public sealed class GetFlightCabinPricesByFlightUseCase
{
    private readonly IFlightCabinPriceRepository _repository;

    public GetFlightCabinPricesByFlightUseCase(IFlightCabinPriceRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<FlightCabinPriceAggregate>> ExecuteAsync(
        int scheduledFlightId, CancellationToken cancellationToken = default)
        => await _repository.GetByFlightAsync(scheduledFlightId, cancellationToken);
}
