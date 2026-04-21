namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.ValueObject;

/// <summary>
/// Tiquete aéreo emitido para un pasajero sobre una línea de reserva.
/// SQL: ticket.
///
/// UNIQUE: ticket_code — código de negocio del tiquete.
/// UNIQUE: reservation_detail_id — un tiquete por línea de reserva.
///
/// ticket_code normalizado a mayúsculas.
/// issue_date y reservation_detail_id son inmutables tras la emisión.
/// ChangeStatus(): única mutación válida sobre el ciclo de vida del tiquete.
/// </summary>
public sealed class TicketAggregate
{
    public TicketId  Id                  { get; private set; }
    public string    TicketCode          { get; private set; }
    public int       ReservationDetailId { get; private set; }
    public DateTime  IssueDate           { get; private set; }
    public int       TicketStatusId      { get; private set; }
    public DateTime  CreatedAt           { get; private set; }
    public DateTime? UpdatedAt           { get; private set; }

    private TicketAggregate()
    {
        Id         = null!;
        TicketCode = null!;
    }

    public TicketAggregate(
        TicketId  id,
        string    ticketCode,
        int       reservationDetailId,
        DateTime  issueDate,
        int       ticketStatusId,
        DateTime  createdAt,
        DateTime? updatedAt = null)
    {
        ValidateTicketCode(ticketCode);

        if (reservationDetailId <= 0)
            throw new ArgumentException(
                "ReservationDetailId must be a positive integer.", nameof(reservationDetailId));

        if (ticketStatusId <= 0)
            throw new ArgumentException(
                "TicketStatusId must be a positive integer.", nameof(ticketStatusId));

        Id                  = id;
        TicketCode          = ticketCode.Trim().ToUpperInvariant();
        ReservationDetailId = reservationDetailId;
        IssueDate           = issueDate;
        TicketStatusId      = ticketStatusId;
        CreatedAt           = createdAt;
        UpdatedAt           = updatedAt;
    }

    /// <summary>
    /// Cambia el estado del tiquete (ISSUED → USED, ISSUED → CANCELLED, etc.).
    /// ticket_code, reservation_detail_id e issue_date son inmutables.
    /// </summary>
    public void ChangeStatus(int ticketStatusId)
    {
        if (ticketStatusId <= 0)
            throw new ArgumentException(
                "TicketStatusId must be a positive integer.", nameof(ticketStatusId));

        TicketStatusId = ticketStatusId;
        UpdatedAt      = DateTime.UtcNow;
    }

    // ── Validaciones privadas ─────────────────────────────────────────────────

    private static void ValidateTicketCode(string ticketCode)
    {
        if (string.IsNullOrWhiteSpace(ticketCode))
            throw new ArgumentException("TicketCode cannot be empty.", nameof(ticketCode));

        if (ticketCode.Trim().Length > 30)
            throw new ArgumentException(
                "TicketCode cannot exceed 30 characters.", nameof(ticketCode));
    }
}
