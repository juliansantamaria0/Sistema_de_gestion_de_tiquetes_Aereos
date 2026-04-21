namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Application.UseCases;

public sealed class FlightStatusService : IFlightStatusService
{
    private readonly CreateFlightStatusUseCase   _create;
    private readonly DeleteFlightStatusUseCase   _delete;
    private readonly GetAllFlightStatusesUseCase _getAll;
    private readonly GetFlightStatusByIdUseCase  _getById;
    private readonly UpdateFlightStatusUseCase   _update;

    public FlightStatusService(
        CreateFlightStatusUseCase   create,
        DeleteFlightStatusUseCase   delete,
        GetAllFlightStatusesUseCase getAll,
        GetFlightStatusByIdUseCase  getById,
        UpdateFlightStatusUseCase   update)
    {
        _create  = create;
        _delete  = delete;
        _getAll  = getAll;
        _getById = getById;
        _update  = update;
    }

    public async Task<FlightStatusDto> CreateAsync(
        string            name,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(name, cancellationToken);
        return new FlightStatusDto(agg.Id.Value, agg.Name);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<FlightStatusDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(a => new FlightStatusDto(a.Id.Value, a.Name));
    }

    public async Task<FlightStatusDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : new FlightStatusDto(agg.Id.Value, agg.Name);
    }

    public async Task UpdateAsync(
        int               id,
        string            name,
        CancellationToken cancellationToken = default)
        => await _update.ExecuteAsync(id, name, cancellationToken);
}
