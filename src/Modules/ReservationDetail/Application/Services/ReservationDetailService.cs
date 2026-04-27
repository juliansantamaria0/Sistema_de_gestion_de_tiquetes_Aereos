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
        if (CurrentUser.IsAuthenticated && CurrentUser.CustomerId.HasValue)
        {
            var reservation = await _db.Reservations.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == reservationId, cancellationToken)
                ?? throw new InvalidOperationException("No se encontró la reserva indicada.");
            if (reservation.CustomerId != CurrentUser.CustomerId.Value)
                throw new InvalidOperationException("Solo puede gestionar asientos de sus propias reservas.");

            var customer = await _db.Customers.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == reservation.CustomerId, cancellationToken);
            if (customer is not null)
            {
                var pax = await _db.Passengers.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == passengerId, cancellationToken)
                    ?? throw new InvalidOperationException("Pasajero no encontrado.");
                if (pax.PersonId != customer.PersonId)
                    throw new InvalidOperationException("Solo puede asignar asiento usando su propio registro de pasajero (titular de la cuenta).");
            }
        }
        var agg = await _create.ExecuteAsync(
            reservationId, passengerId, flightSeatId, fareTypeId, cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await AssertDetailOwnedByCurrentCustomerIfNeededAsync(id, cancellationToken);
        await _delete.ExecuteAsync(id, cancellationToken);
    }

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
        if (CurrentUser.IsAuthenticated && CurrentUser.CustomerId.HasValue)
        {
            var detail = await _db.ReservationDetails.AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
            if (detail is null) return null;
            var res = await _db.Reservations.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == detail.ReservationId, cancellationToken);
            if (res is null || res.CustomerId != CurrentUser.CustomerId.Value)
                return null;
        }
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task ChangeFareTypeAsync(
        int               id,
        int               fareTypeId,
        CancellationToken cancellationToken = default)
    {
        await AssertDetailOwnedByCurrentCustomerIfNeededAsync(id, cancellationToken);
        await _update.ExecuteAsync(id, fareTypeId, cancellationToken);
    }

    public async Task<IEnumerable<ReservationDetailDto>> GetByReservationAsync(
        int               reservationId,
        CancellationToken cancellationToken = default)
    {
        var list = await _getByReservation.ExecuteAsync(reservationId, cancellationToken);
        return list.Select(ToDto);
    }

    private async Task AssertDetailOwnedByCurrentCustomerIfNeededAsync(int detailId, CancellationToken cancellationToken)
    {
        if (!CurrentUser.IsAuthenticated || !CurrentUser.CustomerId.HasValue) return;
        var customerId = CurrentUser.CustomerId.Value;
        var resId = await _db.ReservationDetails.AsNoTracking()
            .Where(d => d.Id == detailId)
            .Select(d => d.ReservationId)
            .FirstOrDefaultAsync(cancellationToken);
        if (resId == 0) throw new InvalidOperationException("No se encontró el detalle de reserva.");
        var ok = await _db.Reservations.AsNoTracking()
            .AnyAsync(r => r.Id == resId && r.CustomerId == customerId, cancellationToken);
        if (!ok) throw new InvalidOperationException("Solo puede modificar o eliminar asientos de sus propias reservas.");
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
