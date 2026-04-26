namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckIn.Application.Services;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckIn.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckIn.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckIn.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;

public sealed class CheckInService : ICheckInService
{
    private readonly CreateCheckInUseCase       _create;
    private readonly DeleteCheckInUseCase       _delete;
    private readonly GetAllCheckInsUseCase      _getAll;
    private readonly GetCheckInByIdUseCase      _getById;
    private readonly ChangeCheckInStatusUseCase _changeStatus;
    private readonly GetCheckInByTicketUseCase  _getByTicket;
    private readonly AppDbContext               _db;

    public CheckInService(
        CreateCheckInUseCase       create,
        DeleteCheckInUseCase       delete,
        GetAllCheckInsUseCase      getAll,
        GetCheckInByIdUseCase      getById,
        ChangeCheckInStatusUseCase changeStatus,
        GetCheckInByTicketUseCase  getByTicket,
        AppDbContext               db)
    {
        _create       = create;
        _delete       = delete;
        _getAll       = getAll;
        _getById      = getById;
        _changeStatus = changeStatus;
        _getByTicket  = getByTicket;
        _db           = db;
    }

    public async Task<CheckInDto> CreateAsync(
        int               ticketId,
        int               checkInStatusId,
        string?           counterNumber,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(
            ticketId, checkInStatusId, counterNumber, cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<CheckInDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        if (CurrentUser.IsAuthenticated && CurrentUser.CustomerId.HasValue)
        {
            var customerId = CurrentUser.CustomerId.Value;
            var ticketIds = await _db.Tickets
                .AsNoTracking()
                .Where(t => _db.ReservationDetails.Any(d =>
                    d.Id == t.ReservationDetailId &&
                    _db.Reservations.Any(r => r.CustomerId == customerId && r.Id == d.ReservationId)))
                .Select(t => t.Id)
                .ToListAsync(cancellationToken);

            var results = new List<CheckInDto>();
            foreach (var tid in ticketIds)
            {
                var checkIn = await _getByTicket.ExecuteAsync(tid, cancellationToken);
                if (checkIn is not null) results.Add(ToDto(checkIn));
            }
            return results;
        }
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<CheckInDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task ChangeStatusAsync(
        int               id,
        int               checkInStatusId,
        string?           counterNumber,
        CancellationToken cancellationToken = default)
        => await _changeStatus.ExecuteAsync(id, checkInStatusId, counterNumber, cancellationToken);

    public async Task<CheckInDto?> GetByTicketAsync(
        int               ticketId,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getByTicket.ExecuteAsync(ticketId, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    

    private static CheckInDto ToDto(CheckInAggregate agg)
        => new(
            agg.Id.Value,
            agg.TicketId,
            agg.CheckInTime,
            agg.CheckInStatusId,
            agg.CounterNumber);
}
