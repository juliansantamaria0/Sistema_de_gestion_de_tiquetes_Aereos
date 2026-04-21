namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Domain.ValueObject;

/// <summary>
/// Registro de cambio de estado de un tiquete (RF-14).
/// SQL: ticket_status_history. [NC-8] id renombrado.
/// Tabla de auditoría — INMUTABLE tras inserción.
/// UNIQUE: (ticket_id, ticket_status_id, changed_at).
/// </summary>
public sealed class TicketStatusHistoryAggregate
{
    public TicketStatusHistoryId Id             { get; private set; }
    public int                   TicketId       { get; private set; }
    public int                   TicketStatusId { get; private set; }
    public DateTime              ChangedAt      { get; private set; }
    public string?               Notes          { get; private set; }

    private TicketStatusHistoryAggregate() { Id = null!; }

    public TicketStatusHistoryAggregate(
        TicketStatusHistoryId id,
        int                   ticketId,
        int                   ticketStatusId,
        DateTime              changedAt,
        string?               notes = null)
    {
        if (ticketId <= 0)
            throw new ArgumentException("TicketId must be a positive integer.", nameof(ticketId));

        if (ticketStatusId <= 0)
            throw new ArgumentException("TicketStatusId must be a positive integer.", nameof(ticketStatusId));

        if (notes is not null && notes.Trim().Length > 250)
            throw new ArgumentException("Notes cannot exceed 250 characters.", nameof(notes));

        Id             = id;
        TicketId       = ticketId;
        TicketStatusId = ticketStatusId;
        ChangedAt      = changedAt;
        Notes          = notes?.Trim();
    }
}
