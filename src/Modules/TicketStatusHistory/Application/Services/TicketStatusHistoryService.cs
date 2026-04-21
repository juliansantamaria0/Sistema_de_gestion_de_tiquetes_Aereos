namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Domain.Aggregate;

public sealed class TicketStatusHistoryService : ITicketStatusHistoryService
{
    private readonly RecordTicketStatusHistoryUseCase         _record;
    private readonly DeleteTicketStatusHistoryUseCase         _delete;
    private readonly GetAllTicketStatusHistoryUseCase         _getAll;
    private readonly GetTicketStatusHistoryByIdUseCase        _getById;
    private readonly GetTicketStatusHistoryByTicketUseCase    _getByTicket;

    public TicketStatusHistoryService(
        RecordTicketStatusHistoryUseCase      record,
        DeleteTicketStatusHistoryUseCase      delete,
        GetAllTicketStatusHistoryUseCase      getAll,
        GetTicketStatusHistoryByIdUseCase     getById,
        GetTicketStatusHistoryByTicketUseCase getByTicket)
    {
        _record      = record;
        _delete      = delete;
        _getAll      = getAll;
        _getById     = getById;
        _getByTicket = getByTicket;
    }

    public async Task<TicketStatusHistoryDto> RecordAsync(
        int               ticketId,
        int               ticketStatusId,
        string?           notes,
        CancellationToken cancellationToken = default)
    {
        var agg = await _record.ExecuteAsync(ticketId, ticketStatusId, notes, cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<TicketStatusHistoryDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<TicketStatusHistoryDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task<IEnumerable<TicketStatusHistoryDto>> GetByTicketAsync(
        int               ticketId,
        CancellationToken cancellationToken = default)
    {
        var list = await _getByTicket.ExecuteAsync(ticketId, cancellationToken);
        return list.Select(ToDto);
    }

    private static TicketStatusHistoryDto ToDto(TicketStatusHistoryAggregate agg)
        => new(agg.Id.Value, agg.TicketId, agg.TicketStatusId, agg.ChangedAt, agg.Notes);
}
