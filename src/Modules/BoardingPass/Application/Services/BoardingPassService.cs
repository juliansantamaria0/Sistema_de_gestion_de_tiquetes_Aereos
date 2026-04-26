namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Application.Services;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;

public sealed class BoardingPassService : IBoardingPassService
{
    private readonly CreateBoardingPassUseCase       _create;
    private readonly DeleteBoardingPassUseCase       _delete;
    private readonly GetAllBoardingPassesUseCase     _getAll;
    private readonly GetBoardingPassByIdUseCase      _getById;
    private readonly UpdateBoardingPassUseCase       _update;
    private readonly GetBoardingPassByCheckInUseCase _getByCheckIn;
    private readonly AppDbContext                    _db;

    public BoardingPassService(
        CreateBoardingPassUseCase       create,
        DeleteBoardingPassUseCase       delete,
        GetAllBoardingPassesUseCase     getAll,
        GetBoardingPassByIdUseCase      getById,
        UpdateBoardingPassUseCase       update,
        GetBoardingPassByCheckInUseCase getByCheckIn,
        AppDbContext                    db)
    {
        _create       = create;
        _delete       = delete;
        _getAll       = getAll;
        _getById      = getById;
        _update       = update;
        _getByCheckIn = getByCheckIn;
        _db           = db;
    }

    public async Task<BoardingPassDto> CreateAsync(
        int               checkInId,
        int?              gateId,
        string?           boardingGroup,
        int               flightSeatId,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(
            checkInId, gateId, boardingGroup, flightSeatId, cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<BoardingPassDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        if (CurrentUser.IsAuthenticated && CurrentUser.CustomerId.HasValue)
        {
            var customerId = CurrentUser.CustomerId.Value;
            var checkInIds = await _db.CheckIns
                .AsNoTracking()
                .Where(c => _db.Tickets.Any(t =>
                    t.Id == c.TicketId &&
                    _db.ReservationDetails.Any(d =>
                        d.Id == t.ReservationDetailId &&
                        _db.Reservations.Any(r => r.CustomerId == customerId && r.Id == d.ReservationId))))
                .Select(c => c.Id)
                .ToListAsync(cancellationToken);

            var results = new List<BoardingPassDto>();
            foreach (var cid in checkInIds)
            {
                var bp = await _getByCheckIn.ExecuteAsync(cid, cancellationToken);
                if (bp is not null) results.Add(ToDto(bp));
            }
            return results;
        }
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<BoardingPassDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task UpdateAsync(
        int               id,
        int?              gateId,
        string?           boardingGroup,
        CancellationToken cancellationToken = default)
        => await _update.ExecuteAsync(id, gateId, boardingGroup, cancellationToken);

    public async Task<BoardingPassDto?> GetByCheckInAsync(
        int               checkInId,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getByCheckIn.ExecuteAsync(checkInId, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    

    private static BoardingPassDto ToDto(BoardingPassAggregate agg)
        => new(agg.Id.Value, agg.CheckInId, agg.GateId, agg.BoardingGroup, agg.FlightSeatId);
}
