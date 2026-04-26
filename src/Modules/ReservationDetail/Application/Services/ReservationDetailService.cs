namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Application.Services;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;

public sealed class ReservationDetailService : IReservationDetailService
{
    private readonly CreateReservationDetailUseCase              _create;
    private readonly DeleteReservationDetailUseCase              _delete;
    private readonly GetAllReservationDetailsUseCase             _getAll;
    private readonly GetReservationDetailByIdUseCase             _getById;
    private readonly UpdateReservationDetailUseCase              _update;
    private readonly GetReservationDetailsByReservationUseCase   _getByReservation;
    private readonly AppDbContext                                _db;

    public ReservationDetailService(
        CreateReservationDetailUseCase            create,
        DeleteReservationDetailUseCase            delete,
        GetAllReservationDetailsUseCase           getAll,
        GetReservationDetailByIdUseCase           getById,
        UpdateReservationDetailUseCase            update,
        GetReservationDetailsByReservationUseCase getByReservation,
        AppDbContext                              db)
    {
        _create           = create;
        _delete           = delete;
        _getAll           = getAll;
        _getById          = getById;
        _update           = update;
        _getByReservation = getByReservation;
        _db               = db;
    }

    public async Task<ReservationDetailDto> CreateAsync(
        int               reservationId,
        int               passengerId,
        int               flightSeatId,
        int               fareTypeId,
        CancellationToken cancellationToken = default)
    {
        var agg = await _create.ExecuteAsync(
            reservationId, passengerId, flightSeatId, fareTypeId, cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<ReservationDetailDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        if (CurrentUser.IsAuthenticated && CurrentUser.CustomerId.HasValue)
        {
            var customerId = CurrentUser.CustomerId.Value;
            var reservationIds = await _db.Reservations
                .AsNoTracking()
                .Where(r => r.CustomerId == customerId)
                .Select(r => r.Id)
                .ToListAsync(cancellationToken);

            var results = new List<ReservationDetailDto>();
            foreach (var rid in reservationIds)
            {
                var details = await _getByReservation.ExecuteAsync(rid, cancellationToken);
                results.AddRange(details.Select(ToDto));
            }
            return results;
        }
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<ReservationDetailDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task ChangeFareTypeAsync(
        int               id,
        int               fareTypeId,
        CancellationToken cancellationToken = default)
        => await _update.ExecuteAsync(id, fareTypeId, cancellationToken);

    public async Task<IEnumerable<ReservationDetailDto>> GetByReservationAsync(
        int               reservationId,
        CancellationToken cancellationToken = default)
    {
        var list = await _getByReservation.ExecuteAsync(reservationId, cancellationToken);
        return list.Select(ToDto);
    }

    

    private static ReservationDetailDto ToDto(ReservationDetailAggregate agg)
        => new(
            agg.Id.Value,
            agg.ReservationId,
            agg.PassengerId,
            agg.FlightSeatId,
            agg.FareTypeId,
            agg.CreatedAt,
            agg.UpdatedAt);
}
