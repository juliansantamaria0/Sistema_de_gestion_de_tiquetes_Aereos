namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Domain.Aggregate;

public sealed class RouteService : IRouteService
{
    private readonly CreateRouteUseCase         _create;
    private readonly DeleteRouteUseCase         _delete;
    private readonly GetAllRoutesUseCase        _getAll;
    private readonly GetRouteByIdUseCase        _getById;
    private readonly GetRouteByAirportsUseCase  _getByAirports;
    private readonly GetRoutesByOriginUseCase   _getByOrigin;

    public RouteService(
        CreateRouteUseCase        create,
        DeleteRouteUseCase        delete,
        GetAllRoutesUseCase       getAll,
        GetRouteByIdUseCase       getById,
        GetRouteByAirportsUseCase getByAirports,
        GetRoutesByOriginUseCase  getByOrigin)
    {
        _create        = create;
        _delete        = delete;
        _getAll        = getAll;
        _getById       = getById;
        _getByAirports = getByAirports;
        _getByOrigin   = getByOrigin;
    }

    public async Task<RouteDto> CreateAsync(
        int               originAirportId,
        int               destinationAirportId,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(
            originAirportId, destinationAirportId, cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<RouteDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<RouteDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task<RouteDto?> GetByAirportsAsync(
        int               originId,
        int               destinationId,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getByAirports.ExecuteAsync(originId, destinationId, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task<IEnumerable<RouteDto>> GetByOriginAsync(
        int               originAirportId,
        CancellationToken cancellationToken = default)
    {
        var list = await _getByOrigin.ExecuteAsync(originAirportId, cancellationToken);
        return list.Select(ToDto);
    }

    private static RouteDto ToDto(RouteAggregate agg)
        => new(agg.Id.Value, agg.OriginAirportId, agg.DestinationAirportId, agg.CreatedAt);
}
