namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Domain.Aggregate;

public sealed class FlightCabinPriceService : IFlightCabinPriceService
{
    private readonly CreateFlightCabinPriceUseCase       _create;
    private readonly DeleteFlightCabinPriceUseCase       _delete;
    private readonly GetAllFlightCabinPricesUseCase      _getAll;
    private readonly GetFlightCabinPriceByIdUseCase      _getById;
    private readonly UpdateFlightCabinPriceUseCase       _update;
    private readonly GetFlightCabinPricesByFlightUseCase _getByFlight;

    public FlightCabinPriceService(
        CreateFlightCabinPriceUseCase      create,
        DeleteFlightCabinPriceUseCase      delete,
        GetAllFlightCabinPricesUseCase     getAll,
        GetFlightCabinPriceByIdUseCase     getById,
        UpdateFlightCabinPriceUseCase      update,
        GetFlightCabinPricesByFlightUseCase getByFlight)
    {
        _create      = create;
        _delete      = delete;
        _getAll      = getAll;
        _getById     = getById;
        _update      = update;
        _getByFlight = getByFlight;
    }

    public async Task<FlightCabinPriceDto> CreateAsync(
        int scheduledFlightId, int cabinClassId, int fareTypeId, decimal price,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(
            scheduledFlightId, cabinClassId, fareTypeId, price, cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<FlightCabinPriceDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<FlightCabinPriceDto?> GetByIdAsync(
        int id, CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task UpdatePriceAsync(
        int id, decimal newPrice, CancellationToken cancellationToken = default)
        => await _update.ExecuteAsync(id, newPrice, cancellationToken);

    public async Task<IEnumerable<FlightCabinPriceDto>> GetByFlightAsync(
        int scheduledFlightId, CancellationToken cancellationToken = default)
    {
        var list = await _getByFlight.ExecuteAsync(scheduledFlightId, cancellationToken);
        return list.Select(ToDto);
    }

    private static FlightCabinPriceDto ToDto(FlightCabinPriceAggregate agg)
        => new(agg.Id.Value, agg.ScheduledFlightId, agg.CabinClassId, agg.FareTypeId, agg.Price);
}
