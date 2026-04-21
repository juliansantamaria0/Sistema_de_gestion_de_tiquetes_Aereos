namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatus.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatus.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatus.Application.UseCases;

public sealed class TicketStatusService : ITicketStatusService
{
    private readonly CreateTicketStatusUseCase   _create;
    private readonly DeleteTicketStatusUseCase   _delete;
    private readonly GetAllTicketStatusesUseCase _getAll;
    private readonly GetTicketStatusByIdUseCase  _getById;
    private readonly UpdateTicketStatusUseCase   _update;

    public TicketStatusService(
        CreateTicketStatusUseCase  create,
        DeleteTicketStatusUseCase  delete,
        GetAllTicketStatusesUseCase getAll,
        GetTicketStatusByIdUseCase getById,
        UpdateTicketStatusUseCase  update)
    {
        _create  = create;
        _delete  = delete;
        _getAll  = getAll;
        _getById = getById;
        _update  = update;
    }

    public async Task<TicketStatusDto> CreateAsync(
        string            name,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(name, cancellationToken);
        return new TicketStatusDto(agg.Id.Value, agg.Name);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<TicketStatusDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(a => new TicketStatusDto(a.Id.Value, a.Name));
    }

    public async Task<TicketStatusDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : new TicketStatusDto(agg.Id.Value, agg.Name);
    }

    public async Task UpdateAsync(
        int               id,
        string            name,
        CancellationToken cancellationToken = default)
        => await _update.ExecuteAsync(id, name, cancellationToken);
}
