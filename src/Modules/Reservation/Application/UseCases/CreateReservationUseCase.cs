namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.UseCases;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Extensions;

public sealed class CreateReservationUseCase
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public CreateReservationUseCase(AppDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<ReservationAggregate> ExecuteAsync(
        string reservationCode,
        int customerId,
        int scheduledFlightId,
        int reservationStatusId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var normalizedCode = reservationCode.Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(normalizedCode))
            throw new InvalidOperationException("El código de reserva es obligatorio.");

        if (!await _context.Customers.AsNoTracking().AnyAsync(x => x.Id == customerId, cancellationToken))
            throw new InvalidOperationException($"No existe el cliente con id {customerId}.");

        if (!await _context.ScheduledFlights.AsNoTracking().AnyAsync(x => x.Id == scheduledFlightId, cancellationToken))
            throw new InvalidOperationException($"No existe el vuelo programado con id {scheduledFlightId}.");

        if (!await _context.ReservationStatuses.AsNoTracking().AnyAsync(x => x.Id == reservationStatusId, cancellationToken))
            throw new InvalidOperationException($"No existe el estado de reserva con id {reservationStatusId}.");

        if (await _context.Reservations.AsNoTracking().AnyAsync(x => x.ReservationCode == normalizedCode, cancellationToken))
            throw new InvalidOperationException($"Ya existe una reserva con el código {normalizedCode}.");

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
            ReservationCode = normalizedCode,
            CustomerId = customerId,
            ScheduledFlightId = scheduledFlightId,
            ReservationDate = now,
            ReservationStatusId = reservationStatusId,
            ConfirmedAt = null,
            CancelledAt = null,
            CreatedAt = now,
            UpdatedAt = null
        };

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        await _context.Reservations.AddAsync(reservationEntity, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        await _context.AddReservationStatusHistoryAsync(
            reservationEntity.Id,
            reservationStatusId,
            "Reserva creada",
            now,
            cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

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
