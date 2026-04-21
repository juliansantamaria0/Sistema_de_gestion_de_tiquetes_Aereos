namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.Aggregate;

public sealed class TicketService : ITicketService
{
    private readonly CreateTicketUseCase                  _create;
    private readonly DeleteTicketUseCase                  _delete;
    private readonly GetAllTicketsUseCase                 _getAll;
    private readonly GetTicketByIdUseCase                 _getById;
    private readonly ChangeTicketStatusUseCase            _changeStatus;
    private readonly GetTicketByReservationDetailUseCase  _getByDetail;

    public TicketService(
        CreateTicketUseCase                 create,
        DeleteTicketUseCase                 delete,
        GetAllTicketsUseCase                getAll,
        GetTicketByIdUseCase                getById,
        ChangeTicketStatusUseCase           changeStatus,
        GetTicketByReservationDetailUseCase getByDetail)
    {
        _create       = create;
        _delete       = delete;
        _getAll       = getAll;
        _getById      = getById;
        _changeStatus = changeStatus;
        _getByDetail  = getByDetail;
    }

    public async Task<TicketDto> CreateAsync(
        string            ticketCode,
        int               reservationDetailId,
        int               ticketStatusId,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(ticketCode, reservationDetailId, ticketStatusId, cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<TicketDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<TicketDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task ChangeStatusAsync(
        int               id,
        int               ticketStatusId,
        CancellationToken cancellationToken = default)
        => await _changeStatus.ExecuteAsync(id, ticketStatusId, cancellationToken);

    public async Task<TicketDto?> GetByReservationDetailAsync(
        int               reservationDetailId,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getByDetail.ExecuteAsync(reservationDetailId, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    // ── Mapper privado ────────────────────────────────────────────────────────

    private static TicketDto ToDto(TicketAggregate agg)
        => new(
            agg.Id.Value,
            agg.TicketCode,
            agg.ReservationDetailId,
            agg.IssueDate,
            agg.TicketStatusId,
            agg.CreatedAt,
            agg.UpdatedAt);
}
