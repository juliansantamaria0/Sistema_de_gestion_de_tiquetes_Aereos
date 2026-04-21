namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Domain.Repositories;

/// <summary>
/// Obtiene todos los precios (por clase y tarifa) disponibles en un vuelo.
/// Caso de uso clave para mostrar la tabla de precios al cliente.
/// </summary>
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
