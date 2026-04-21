namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Domain.ValueObject;

/// <summary>
/// Registro de cambio de estado de una reserva (RF-14).
/// SQL: reservation_status_history. [NC-8] id renombrado.
///
/// Tabla de auditoría — INMUTABLE tras inserción.
/// UNIQUE: (reservation_id, reservation_status_id, changed_at).
/// notes: VARCHAR(250) NULL — contexto opcional del cambio.
/// </summary>
public sealed class ReservationStatusHistoryAggregate
{
    public ReservationStatusHistoryId Id                  { get; private set; }
    public int                        ReservationId       { get; private set; }
    public int                        ReservationStatusId { get; private set; }
    public DateTime                   ChangedAt           { get; private set; }
    public string?                    Notes               { get; private set; }

    private ReservationStatusHistoryAggregate() { Id = null!; }

    public ReservationStatusHistoryAggregate(
        ReservationStatusHistoryId id,
        int                        reservationId,
        int                        reservationStatusId,
        DateTime                   changedAt,
        string?                    notes = null)
    {
        if (reservationId <= 0)
            throw new ArgumentException(
                "ReservationId must be a positive integer.", nameof(reservationId));

        if (reservationStatusId <= 0)
            throw new ArgumentException(
                "ReservationStatusId must be a positive integer.", nameof(reservationStatusId));

        if (notes is not null && notes.Trim().Length > 250)
            throw new ArgumentException(
                "Notes cannot exceed 250 characters.", nameof(notes));

        Id                  = id;
        ReservationId       = reservationId;
        ReservationStatusId = reservationStatusId;
        ChangedAt           = changedAt;
        Notes               = notes?.Trim();
    }
}
