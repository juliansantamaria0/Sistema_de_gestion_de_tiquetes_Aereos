namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Domain.Aggregate;

public sealed class FlightCrewService : IFlightCrewService
{
    private readonly CreateFlightCrewUseCase      _create;
    private readonly DeleteFlightCrewUseCase      _delete;
    private readonly GetAllFlightCrewsUseCase     _getAll;
    private readonly GetFlightCrewByIdUseCase     _getById;
    private readonly UpdateFlightCrewUseCase      _update;
    private readonly GetFlightCrewByFlightUseCase _getByFlight;

    public FlightCrewService(
        CreateFlightCrewUseCase      create,
        DeleteFlightCrewUseCase      delete,
        GetAllFlightCrewsUseCase     getAll,
        GetFlightCrewByIdUseCase     getById,
        UpdateFlightCrewUseCase      update,
        GetFlightCrewByFlightUseCase getByFlight)
    {
        _create      = create;
        _delete      = delete;
        _getAll      = getAll;
        _getById     = getById;
        _update      = update;
        _getByFlight = getByFlight;
    }

    public async Task<FlightCrewDto> CreateAsync(
        int               scheduledFlightId,
        int               employeeId,
        int               crewRoleId,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(scheduledFlightId, employeeId, crewRoleId, cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<FlightCrewDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<FlightCrewDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task UpdateAsync(
        int               id,
        int               crewRoleId,
        CancellationToken cancellationToken = default)
        => await _update.ExecuteAsync(id, crewRoleId, cancellationToken);

    public async Task<IEnumerable<FlightCrewDto>> GetByFlightAsync(
        int               scheduledFlightId,
        CancellationToken cancellationToken = default)
    {
        var list = await _getByFlight.ExecuteAsync(scheduledFlightId, cancellationToken);
        return list.Select(ToDto);
    }

    

    private static FlightCrewDto ToDto(FlightCrewAggregate agg)
        => new(agg.Id.Value, agg.ScheduledFlightId, agg.EmployeeId, agg.CrewRoleId, agg.CreatedAt);
}
