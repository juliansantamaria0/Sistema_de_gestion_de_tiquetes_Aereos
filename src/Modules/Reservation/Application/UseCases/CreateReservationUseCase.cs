namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.UseCases;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateReservationUseCase
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork  _unitOfWork;

    public CreateReservationUseCase(AppDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<ReservationAggregate> ExecuteAsync(
        string            reservationCode,
        int               customerId,
        int               scheduledFlightId,
        int               reservationStatusId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        if (!await _context.Customers.AsNoTracking().AnyAsync(x => x.Id == customerId, cancellationToken))
            throw new InvalidOperationException($"No existe el cliente con id {customerId}.");

        if (!await _context.ScheduledFlights.AsNoTracking().AnyAsync(x => x.Id == scheduledFlightId, cancellationToken))
            throw new InvalidOperationException($"No existe el vuelo programado con id {scheduledFlightId}.");

        if (!await _context.ReservationStatuses.AsNoTracking().AnyAsync(x => x.Id == reservationStatusId, cancellationToken))
            throw new InvalidOperationException($"No existe el estado de reserva con id {reservationStatusId}.");

        var availableSeats = await _context.FlightSeats
            .AsNoTracking()
            .Join(
                _context.SeatStatuses.AsNoTracking(),
                seat => seat.SeatStatusId,
                status => status.Id,
                (seat, status) => new { seat.ScheduledFlightId, StatusName = status.Name })
            .CountAsync(x => x.ScheduledFlightId == scheduledFlightId && x.StatusName == "AVAILABLE", cancellationToken);

        if (availableSeats <= 0)
            throw new InvalidOperationException("El vuelo no tiene asientos disponibles para crear nuevas reservas.");

        var reservationEntity = new ReservationEntity
        {
            ReservationCode = reservationCode.Trim().ToUpperInvariant(),
            CustomerId = customerId,
            ScheduledFlightId = scheduledFlightId,
            ReservationDate = now,
            ReservationStatusId = reservationStatusId,
            ConfirmedAt = null,
            CancelledAt = null,
            CreatedAt = now,
            UpdatedAt = null
        };

        await _context.Reservations.AddAsync(reservationEntity, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        await _context.ReservationStatusHistories.AddAsync(new ReservationStatusHistoryEntity
        {
            ReservationId = reservationEntity.Id,
            ReservationStatusId = reservationStatusId,
            ChangedAt = now,
            Notes = "Reserva creada"
        }, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new ReservationAggregate(
            new ReservationId(reservationEntity.Id),
            reservationEntity.ReservationCode,
            reservationEntity.CustomerId,
            reservationEntity.ScheduledFlightId,
            reservationEntity.ReservationDate,
            reservationEntity.ReservationStatusId,
            reservationEntity.ConfirmedAt,
            reservationEntity.CancelledAt,
            reservationEntity.CreatedAt,
            reservationEntity.UpdatedAt);
    }
}
