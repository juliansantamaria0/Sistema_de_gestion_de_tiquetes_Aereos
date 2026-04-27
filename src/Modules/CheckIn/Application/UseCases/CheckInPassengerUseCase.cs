namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckIn.Application.UseCases;

using Microsoft.EntityFrameworkCore;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BoardingPass.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckIn.Application.Results;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckIn.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckIn.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckIn.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Constants;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Context;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Extensions;

public sealed class CheckInPassengerUseCase
{
    private static readonly string[] AllowedTicketStatusNames = new[] { "PAID", "PAGADO", "ISSUED" };
    private static readonly string[] EnabledFlightStatusNames = new[] { "SCHEDULED", "BOARDING", "HABILITADO" };
    private const string CheckedInTicketStatusName  = "CHECKED_IN";
    private const string CheckedInCheckInStatusName = "CHECKED_IN";

    private readonly AppDbContext            _context;
    private readonly IUnitOfWork             _unitOfWork;
    private readonly ICheckInRepository      _checkInRepository;
    private readonly IBoardingPassRepository _boardingPassRepository;
    private readonly IFlightSeatRepository   _flightSeatRepository;

    public CheckInPassengerUseCase(
        AppDbContext            context,
        IUnitOfWork             unitOfWork,
        ICheckInRepository      checkInRepository,
        IBoardingPassRepository boardingPassRepository,
        IFlightSeatRepository   flightSeatRepository)
    {
        _context                = context;
        _unitOfWork             = unitOfWork;
        _checkInRepository      = checkInRepository;
        _boardingPassRepository = boardingPassRepository;
        _flightSeatRepository   = flightSeatRepository;
    }

    public async Task<CheckInPassengerResult> ExecuteAsync(
        int               ticketId,
        string?           counterNumber,
        CancellationToken cancellationToken = default)
    {
        if (ticketId <= 0)
            throw new InvalidOperationException("El identificador del tiquete debe ser un entero positivo.");

        var ticket = await _context.Tickets
            .FirstOrDefaultAsync(t => t.Id == ticketId, cancellationToken)
            ?? throw new InvalidOperationException($"No existe el tiquete con id {ticketId}.");

        var ticketStatus = await _context.TicketStatuses
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == ticket.TicketStatusId, cancellationToken)
            ?? throw new InvalidOperationException("No se pudo determinar el estado del tiquete.");

        if (!AllowedTicketStatusNames.Contains(ticketStatus.Name, StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                $"El check-in solo está disponible para tiquetes pagados. Estado actual: '{ticketStatus.Name}'.");

        if (await _context.CheckIns.AsNoTracking().AnyAsync(c => c.TicketId == ticketId, cancellationToken))
            throw new InvalidOperationException("Este tiquete ya tiene un check-in registrado.");

        var detail = await _context.ReservationDetails
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == ticket.ReservationDetailId, cancellationToken)
            ?? throw new InvalidOperationException(
                $"No se encontró el detalle de reserva del tiquete (id {ticket.ReservationDetailId}).");

        var reservation = await _context.Reservations
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == detail.ReservationId, cancellationToken)
            ?? throw new InvalidOperationException(
                $"No se encontró la reserva asociada al tiquete (id {detail.ReservationId}).");

        var scheduledFlight = await _context.ScheduledFlights
            .AsNoTracking()
            .FirstOrDefaultAsync(sf => sf.Id == reservation.ScheduledFlightId, cancellationToken)
            ?? throw new InvalidOperationException(
                $"No se encontró el vuelo programado de la reserva (id {reservation.ScheduledFlightId}).");

        var flightStatusName = await _context.FlightStatuses
            .AsNoTracking()
            .Where(s => s.Id == scheduledFlight.FlightStatusId)
            .Select(s => s.Name)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException("No se pudo determinar el estado del vuelo.");

        if (!EnabledFlightStatusNames.Contains(flightStatusName, StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                $"El vuelo no está habilitado para check-in. Estado actual del vuelo: '{flightStatusName}'.");

        var flightSeatId = await ResolveOrAssignSeatAsync(
            detail.FlightSeatId,
            scheduledFlight.Id,
            cancellationToken);

        var checkedInTicketStatusId = await _context.TicketStatuses
            .AsNoTracking()
            .Where(s => s.Name == CheckedInTicketStatusName)
            .Select(s => s.Id)
            .FirstOrDefaultAsync(cancellationToken);
        if (checkedInTicketStatusId == 0)
            throw new InvalidOperationException(
                $"No existe el estado de tiquete '{CheckedInTicketStatusName}' en el catálogo. Ejecute el seed de datos.");

        var checkedInCheckInStatusId = await _context.CheckInStatuses
            .AsNoTracking()
            .Where(s => s.Name == CheckedInCheckInStatusName)
            .Select(s => s.Id)
            .FirstOrDefaultAsync(cancellationToken);
        if (checkedInCheckInStatusId == 0)
            throw new InvalidOperationException(
                $"No existe el estado de check-in '{CheckedInCheckInStatusName}' en el catálogo.");

        var boardingPassCode = await GenerateUniqueBoardingPassCodeAsync(cancellationToken);

        var now = DateTime.UtcNow;
        int                    persistedCheckInId     = 0;
        int                    persistedBoardingPassId = 0;
        BoardingPassAggregate? boardingPass            = null;

        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            await MarkSeatAsOccupiedAsync(flightSeatId, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            var checkInAgg = new CheckInAggregate(
                new CheckInId(0),
                ticketId,
                now,
                checkedInCheckInStatusId,
                counterNumber);
            await _checkInRepository.AddAsync(checkInAgg, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            // El Id real lo asigna MySQL al hacer commit; lo recuperamos por TicketId (uq_check_in_ticket).
            var persistedCheckIn = await _checkInRepository.GetByTicketAsync(ticketId, cancellationToken)
                ?? throw new InvalidOperationException(
                    $"No se pudo recuperar el check-in recién creado para el tiquete {ticketId}.");
            persistedCheckInId = persistedCheckIn.Id.Value;

            boardingPass = new BoardingPassAggregate(
                new BoardingPassId(0),
                boardingPassCode,
                persistedCheckInId,
                scheduledFlight.GateId,
                BuildBoardingGroup(detail.FlightSeatId, flightSeatId),
                flightSeatId,
                now);
            await _boardingPassRepository.AddAsync(boardingPass, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            // Mismo patrón que con el CheckIn: releemos el pase por CheckInId (uq_boarding_pass_check_in)
            // para obtener el Id que asignó MySQL y no devolver 0 a la capa de presentación.
            var persistedBoardingPass = await _boardingPassRepository.GetByCheckInAsync(persistedCheckInId, cancellationToken)
                ?? throw new InvalidOperationException(
                    $"No se pudo recuperar el pase de abordar recién creado para el check-in {persistedCheckInId}.");
            persistedBoardingPassId = persistedBoardingPass.Id.Value;

            ticket.TicketStatusId = checkedInTicketStatusId;
            ticket.UpdatedAt      = now;
            await _context.AddTicketStatusHistoryAsync(
                ticket.Id,
                checkedInTicketStatusId,
                "Check-in realizado",
                now,
                cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        });

        return new CheckInPassengerResult(
            CheckInId:        persistedCheckInId,
            TicketId:         ticketId,
            FlightSeatId:     flightSeatId,
            BoardingPassId:   persistedBoardingPassId,
            BoardingPassCode: boardingPass!.BoardingPassCode,
            GateId:           boardingPass.GateId,
            BoardingGroup:    boardingPass.BoardingGroup,
            IssuedAt:         boardingPass.IssuedAt);
    }

    private async Task<int> ResolveOrAssignSeatAsync(
        int               currentSeatId,
        int               scheduledFlightId,
        CancellationToken cancellationToken)
    {
        if (currentSeatId > 0)
        {
            var seat = await _context.FlightSeats
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == currentSeatId, cancellationToken);
            if (seat is not null && seat.ScheduledFlightId == scheduledFlightId)
                return seat.Id;
        }

        var available = (await _flightSeatRepository.GetAvailableByFlightAsync(scheduledFlightId, cancellationToken))
            .FirstOrDefault();

        if (available is null)
            throw new InvalidOperationException(
                "No hay asientos disponibles en este vuelo para asignar al check-in.");

        return available.Id.Value;
    }

    private async Task MarkSeatAsOccupiedAsync(int flightSeatId, CancellationToken cancellationToken)
    {
        var occupiedStatusId = await _context.SeatStatuses
            .AsNoTracking()
            .Where(s => s.Name == SeatStatusNames.Occupied)
            .Select(s => s.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (occupiedStatusId == 0)
            return;

        var entity = await _context.FlightSeats
            .FirstOrDefaultAsync(s => s.Id == flightSeatId, cancellationToken);
        if (entity is null) return;

        entity.SeatStatusId = occupiedStatusId;
        entity.UpdatedAt    = DateTime.UtcNow;
        _context.FlightSeats.Update(entity);
    }

    private async Task<string> GenerateUniqueBoardingPassCodeAsync(CancellationToken cancellationToken)
    {
        for (var i = 0; i < 5; i++)
        {
            var candidate = "BP-" + Guid.NewGuid().ToString("N")[..10].ToUpperInvariant();
            if (!await _boardingPassRepository.BoardingPassCodeExistsAsync(candidate, cancellationToken))
                return candidate;
        }

        return "BP-" + DateTime.UtcNow.Ticks.ToString("X");
    }

    private static string BuildBoardingGroup(int originalSeatId, int finalSeatId)
    {
        // Si el asiento se asignó automáticamente (no venía en el detalle), marcamos grupo C.
        return originalSeatId == finalSeatId && originalSeatId > 0 ? "A" : "C";
    }
}
