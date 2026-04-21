namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Domain.Aggregate;

public sealed class TicketBaggageService : ITicketBaggageService
{
    private readonly CreateTicketBaggageUseCase         _create;
    private readonly DeleteTicketBaggageUseCase         _delete;
    private readonly GetAllTicketBaggagesUseCase        _getAll;
    private readonly GetTicketBaggageByIdUseCase        _getById;
    private readonly UpdateTicketBaggageUseCase         _update;
    private readonly GetTicketBaggagesByTicketUseCase   _getByTicket;

    public TicketBaggageService(
        CreateTicketBaggageUseCase       create,
        DeleteTicketBaggageUseCase       delete,
        GetAllTicketBaggagesUseCase      getAll,
        GetTicketBaggageByIdUseCase      getById,
        UpdateTicketBaggageUseCase       update,
        GetTicketBaggagesByTicketUseCase getByTicket)
    {
        _create      = create;
        _delete      = delete;
        _getAll      = getAll;
        _getById     = getById;
        _update      = update;
        _getByTicket = getByTicket;
    }

    public async Task<TicketBaggageDto> CreateAsync(
        int               ticketId,
        int               baggageTypeId,
        int               quantity,
        decimal           feeCharged,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(
            ticketId, baggageTypeId, quantity, feeCharged, cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<TicketBaggageDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<TicketBaggageDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task UpdateQuantityAndFeeAsync(
        int               id,
        int               quantity,
        decimal           feeCharged,
        CancellationToken cancellationToken = default)
        => await _update.ExecuteAsync(id, quantity, feeCharged, cancellationToken);

    public async Task<IEnumerable<TicketBaggageDto>> GetByTicketAsync(
        int               ticketId,
        CancellationToken cancellationToken = default)
    {
        var list = await _getByTicket.ExecuteAsync(ticketId, cancellationToken);
        return list.Select(ToDto);
    }

    // ── Mapper privado ────────────────────────────────────────────────────────

    private static TicketBaggageDto ToDto(TicketBaggageAggregate agg)
        => new(agg.Id.Value, agg.TicketId, agg.BaggageTypeId, agg.Quantity, agg.FeeCharged);
}
