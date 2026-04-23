namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Domain.Aggregate;

public sealed class SeatMapService : ISeatMapService
{
    private readonly CreateSeatMapUseCase              _create;
    private readonly DeleteSeatMapUseCase              _delete;
    private readonly GetAllSeatMapsUseCase             _getAll;
    private readonly GetSeatMapByIdUseCase             _getById;
    private readonly UpdateSeatMapUseCase              _update;
    private readonly GetSeatMapsByAircraftTypeUseCase  _getByAircraftType;

    public SeatMapService(
        CreateSeatMapUseCase             create,
        DeleteSeatMapUseCase             delete,
        GetAllSeatMapsUseCase            getAll,
        GetSeatMapByIdUseCase            getById,
        UpdateSeatMapUseCase             update,
        GetSeatMapsByAircraftTypeUseCase getByAircraftType)
    {
        _create            = create;
        _delete            = delete;
        _getAll            = getAll;
        _getById           = getById;
        _update            = update;
        _getByAircraftType = getByAircraftType;
    }

    public async Task<SeatMapDto> CreateAsync(
        int               aircraftTypeId,
        string            seatNumber,
        int               cabinClassId,
        string?           seatFeatures,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(
            aircraftTypeId, seatNumber, cabinClassId, seatFeatures, cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<SeatMapDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<SeatMapDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task UpdateAsync(
        int               id,
        int               cabinClassId,
        string?           seatFeatures,
        CancellationToken cancellationToken = default)
        => await _update.ExecuteAsync(id, cabinClassId, seatFeatures, cancellationToken);

    public async Task<IEnumerable<SeatMapDto>> GetByAircraftTypeAsync(
        int               aircraftTypeId,
        CancellationToken cancellationToken = default)
    {
        var list = await _getByAircraftType.ExecuteAsync(aircraftTypeId, cancellationToken);
        return list.Select(ToDto);
    }

    

    private static SeatMapDto ToDto(SeatMapAggregate agg)
        => new(agg.Id.Value, agg.AircraftTypeId, agg.SeatNumber, agg.CabinClassId, agg.SeatFeatures);
}
