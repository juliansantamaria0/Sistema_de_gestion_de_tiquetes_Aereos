namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Application.UseCases;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Infrastructure.Entity;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateReservationDetailUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AppDbContext _context;

    public CreateReservationDetailUseCase(IUnitOfWork unitOfWork, AppDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<ReservationDetailAggregate> ExecuteAsync(
        int reservationId,
        int passengerId,
        int flightSeatId,
        int fareTypeId,
        CancellationToken cancellationToken = default)
    {
        var reservation = await _context.Reservations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == reservationId, cancellationToken)
            ?? throw new InvalidOperationException($"No existe la reserva con id {reservationId}.");

        if (reservation.CancelledAt.HasValue)
            throw new InvalidOperationException("No se pueden agregar detalles a una reserva cancelada.");
        if (reservation.ConfirmedAt.HasValue)
            throw new InvalidOperationException("No se pueden agregar detalles a una reserva ya confirmada.");

        if (!await _context.Passengers.AsNoTracking().AnyAsync(x => x.Id == passengerId, cancellationToken))
            throw new InvalidOperationException($"No existe el pasajero con id {passengerId}.");

        if (!await _context.FareTypes.AsNoTracking().AnyAsync(x => x.Id == fareTypeId, cancellationToken))
            throw new InvalidOperationException($"No existe la tarifa con id {fareTypeId}.");

        var seat = await _context.FlightSeats
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == flightSeatId, cancellationToken)
            ?? throw new InvalidOperationException($"No existe el asiento de vuelo con id {flightSeatId}.");

        if (seat.ScheduledFlightId != reservation.ScheduledFlightId)
            throw new InvalidOperationException("El asiento seleccionado no pertenece al mismo vuelo programado de la reserva.");

        var availableStatusId = await _context.SeatStatuses
            .AsNoTracking()
            .Where(x => x.Name == "AVAILABLE")
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (availableStatusId <= 0)
            throw new InvalidOperationException("No existe el estado de asiento 'AVAILABLE'.");

        if (seat.SeatStatusId != availableStatusId)
            throw new InvalidOperationException("El asiento seleccionado ya no está disponible.");

        if (await _context.ReservationDetails.AsNoTracking().AnyAsync(x => x.ReservationId == reservationId && x.FlightSeatId == flightSeatId, cancellationToken))
            throw new InvalidOperationException("Ese asiento ya está asociado a esta reserva.");

        var seatAlreadyReserved = await _context.ReservationDetails
            .AsNoTracking()
            .Join(
                _context.Reservations.AsNoTracking(),
                detail => detail.ReservationId,
                existingReservation => existingReservation.Id,
                (detail, existingReservation) => new { detail.FlightSeatId, existingReservation.CancelledAt })
            .AnyAsync(x => x.FlightSeatId == flightSeatId && x.CancelledAt == null, cancellationToken);

        if (seatAlreadyReserved)
            throw new InvalidOperationException("Ese asiento ya está reservado en otra reserva activa.");

        if (await _context.ReservationDetails.AsNoTracking().AnyAsync(x => x.ReservationId == reservationId && x.PassengerId == passengerId, cancellationToken))
            throw new InvalidOperationException("Ese pasajero ya tiene un asiento asignado dentro de la reserva.");

        var now = DateTime.UtcNow;
        var entity = new ReservationDetailEntity
        {
            ReservationId = reservationId,
            PassengerId = passengerId,
            FlightSeatId = flightSeatId,
            FareTypeId = fareTypeId,
            CreatedAt = now,
            UpdatedAt = null
        };

        await _context.ReservationDetails.AddAsync(entity, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new ReservationDetailAggregate(
            new ReservationDetailId(entity.Id),
            entity.ReservationId,
            entity.PassengerId,
            entity.FlightSeatId,
            entity.FareTypeId,
            entity.CreatedAt,
            entity.UpdatedAt);
    }
}
