namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Application.Services;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;

public sealed class TicketService : ITicketService
{
    private readonly CreateTicketUseCase                  _create;
    private readonly DeleteTicketUseCase                  _delete;
    private readonly GetAllTicketsUseCase                 _getAll;
    private readonly GetTicketByIdUseCase                 _getById;
    private readonly ChangeTicketStatusUseCase            _changeStatus;
    private readonly GetTicketByReservationDetailUseCase  _getByDetail;
    private readonly AppDbContext                         _db;

    public TicketService(
        CreateTicketUseCase                 create,
        DeleteTicketUseCase                 delete,
        GetAllTicketsUseCase                getAll,
        GetTicketByIdUseCase                getById,
        ChangeTicketStatusUseCase           changeStatus,
        GetTicketByReservationDetailUseCase getByDetail,
        AppDbContext                        db)
    {
        _create       = create;
        _delete       = delete;
        _getAll       = getAll;
        _getById      = getById;
        _changeStatus = changeStatus;
        _getByDetail  = getByDetail;
        _db           = db;
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
        if (CurrentUser.IsAuthenticated && CurrentUser.CustomerId.HasValue)
        {
            var customerId = CurrentUser.CustomerId.Value;
            var detailIds = await _db.ReservationDetails
                .AsNoTracking()
                .Where(d => _db.Reservations.Any(r => r.CustomerId == customerId && r.Id == d.ReservationId))
                .Select(d => d.Id)
                .ToListAsync(cancellationToken);

            var results = new List<TicketDto>();
            foreach (var did in detailIds)
            {
                var ticket = await _getByDetail.ExecuteAsync(did, cancellationToken);
                if (ticket is not null) results.Add(ToDto(ticket));
            }
            return results;
        }
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
