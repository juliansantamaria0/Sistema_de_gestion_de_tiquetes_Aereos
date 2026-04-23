namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Domain.Repositories;

public sealed class CreateTicketUseCase
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IReservationDetailRepository _reservationDetailRepository;
    private readonly IPaymentRepository _paymentRepository;

    public CreateTicketUseCase(
        ITicketRepository ticketRepository,
        IReservationRepository reservationRepository,
        IReservationDetailRepository reservationDetailRepository,
        IPaymentRepository paymentRepository)
    {
        _ticketRepository = ticketRepository;
        _reservationRepository = reservationRepository;
        _reservationDetailRepository = reservationDetailRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task<TicketAggregate> ExecuteAsync(
        string ticketCode,
        int reservationDetailId,
        int ticketStatusId,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = ticketCode.Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(normalizedCode))
            throw new InvalidOperationException("El código del tiquete es obligatorio.");

        var detail = await _reservationDetailRepository.GetByIdAsync(
            new ReservationDetailId(reservationDetailId),
            cancellationToken)
            ?? throw new InvalidOperationException($"No existe el detalle de reserva con id {reservationDetailId}.");

        var reservation = await _reservationRepository.GetByIdAsync(
            new ReservationId(detail.ReservationId),
            cancellationToken)
            ?? throw new InvalidOperationException($"No existe la reserva asociada al detalle {reservationDetailId}.");

        if (!reservation.ConfirmedAt.HasValue || reservation.CancelledAt.HasValue)
            throw new InvalidOperationException("Solo se pueden emitir tiquetes desde reservas válidas y confirmadas.");

        if (await _ticketRepository.TicketExistsForReservationDetailAsync(reservationDetailId, cancellationToken))
            throw new InvalidOperationException("Ya existe un tiquete emitido para este detalle de reserva.");

        if (await _ticketRepository.TicketCodeExistsAsync(normalizedCode, cancellationToken))
            throw new InvalidOperationException($"Ya existe un tiquete con el código {normalizedCode}.");

        if (!await _ticketRepository.TicketStatusExistsAsync(ticketStatusId, cancellationToken))
            throw new InvalidOperationException($"No existe el estado de tiquete con id {ticketStatusId}.");

        var expectedTotal = await _reservationRepository.GetQuotedFareTotalForReservationAsync(
            detail.ReservationId,
            cancellationToken);
        var paidTotal = await _paymentRepository.SumApprovedPaymentsForReservationAsync(
            detail.ReservationId,
            cancellationToken);

        if (paidTotal <= 0m || paidTotal < expectedTotal)
            throw new Exception("No se puede emitir el tiquete: Faltan pagos asociados o fondos insuficientes.");

        return await _ticketRepository.IssueTicketWithHistoryAsync(
            normalizedCode,
            reservationDetailId,
            ticketStatusId,
            cancellationToken);
    }
}
