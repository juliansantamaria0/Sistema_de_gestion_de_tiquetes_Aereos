namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.ValueObject;












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

    
    
    
    
    public void ChangeStatus(int ticketStatusId)
    {
        if (ticketStatusId <= 0)
            throw new ArgumentException(
                "TicketStatusId must be a positive integer.", nameof(ticketStatusId));

        TicketStatusId = ticketStatusId;
        UpdatedAt      = DateTime.UtcNow;
    }

    

    private static void ValidateTicketCode(string ticketCode)
    {
        if (string.IsNullOrWhiteSpace(ticketCode))
            throw new ArgumentException("TicketCode cannot be empty.", nameof(ticketCode));

        if (ticketCode.Trim().Length > 30)
            throw new ArgumentException(
                "TicketCode cannot exceed 30 characters.", nameof(ticketCode));
    }
}
