namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Domain.ValueObject;

public sealed class GetFlightCabinPriceByIdUseCase
{
    private readonly IFlightCabinPriceRepository _repository;

    public GetFlightCabinPriceByIdUseCase(IFlightCabinPriceRepository repository)
    {
        _repository = repository;
    }

    public async Task<FlightCabinPriceAggregate?> ExecuteAsync(
        int id, CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new FlightCabinPriceId(id), cancellationToken);
}
