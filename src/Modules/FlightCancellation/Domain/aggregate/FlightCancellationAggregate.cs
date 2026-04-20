namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Domain.ValueObject;

/// <summary>
/// Registro de cancelación de un vuelo programado.
/// SQL: flight_cancellation. PK: cancellation_id.
///
/// Invariante clave: UNIQUE (scheduled_flight_id) — un vuelo solo puede
/// cancelarse una vez. La unicidad se garantiza a nivel de base de datos
/// y también operativamente: si ya existe un registro para el vuelo,
/// la BD rechazará el INSERT con violación de unique constraint.
///
/// cancelled_at se fija al momento del registro — inmutable.
/// notes es nullable: observaciones opcionales sobre la cancelación.
/// La única modificación válida es actualizar notes (UpdateNotes).
/// </summary>
public sealed class FlightCancellationAggregate
{
    public FlightCancellationId Id                    { get; private set; }
    public int                  ScheduledFlightId     { get; private set; }
    public int                  CancellationReasonId  { get; private set; }
    public DateTime             CancelledAt           { get; private set; }
    public string?              Notes                 { get; private set; }

    private FlightCancellationAggregate()
    {
        Id = null!;
    }

    public FlightCancellationAggregate(
        FlightCancellationId id,
        int                  scheduledFlightId,
        int                  cancellationReasonId,
        DateTime             cancelledAt,
        string?              notes = null)
    {
        if (scheduledFlightId <= 0)
            throw new ArgumentException(
                "ScheduledFlightId must be a positive integer.", nameof(scheduledFlightId));

        if (cancellationReasonId <= 0)
            throw new ArgumentException(
                "CancellationReasonId must be a positive integer.", nameof(cancellationReasonId));

        ValidateNotes(notes);

        Id                   = id;
        ScheduledFlightId    = scheduledFlightId;
        CancellationReasonId = cancellationReasonId;
        CancelledAt          = cancelledAt;
        Notes                = notes?.Trim();
    }

    /// <summary>
    /// Actualiza las notas adicionales de la cancelación.
    /// ScheduledFlightId, CancellationReasonId y CancelledAt son inmutables.
    /// </summary>
    public void UpdateNotes(string? notes)
    {
        ValidateNotes(notes);
        Notes = notes?.Trim();
    }

    // ── Validaciones privadas ─────────────────────────────────────────────────

    private static void ValidateNotes(string? notes)
    {
        if (notes is not null && notes.Trim().Length > 250)
            throw new ArgumentException(
                "Notes cannot exceed 250 characters.", nameof(notes));
    }
}
