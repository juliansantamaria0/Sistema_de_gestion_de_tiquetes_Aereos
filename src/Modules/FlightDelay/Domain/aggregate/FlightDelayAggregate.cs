namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Domain.ValueObject;

/// <summary>
/// Registro de un retraso en un vuelo programado.
/// SQL: flight_delay. [NC-7] id renombrado a flight_delay_id.
///
/// Invariante: delay_minutes > 0 (espejo del CHECK SQL).
/// Un vuelo puede tener múltiples registros de retraso (acumulación).
/// reported_at se fija al momento del reporte — no es modificable.
///
/// AdjustDelay(): única mutación válida — corrige minutos de retraso
/// reportados incorrectamente sin borrar el registro.
/// </summary>
public sealed class FlightDelayAggregate
{
    public FlightDelayId Id                { get; private set; }
    public int           ScheduledFlightId { get; private set; }
    public int           DelayReasonId     { get; private set; }
    public int           DelayMinutes      { get; private set; }
    public DateTime      ReportedAt        { get; private set; }

    private FlightDelayAggregate()
    {
        Id = null!;
    }

    public FlightDelayAggregate(
        FlightDelayId id,
        int           scheduledFlightId,
        int           delayReasonId,
        int           delayMinutes,
        DateTime      reportedAt)
    {
        if (scheduledFlightId <= 0)
            throw new ArgumentException(
                "ScheduledFlightId must be a positive integer.", nameof(scheduledFlightId));

        if (delayReasonId <= 0)
            throw new ArgumentException(
                "DelayReasonId must be a positive integer.", nameof(delayReasonId));

        ValidateDelayMinutes(delayMinutes);

        Id                = id;
        ScheduledFlightId = scheduledFlightId;
        DelayReasonId     = delayReasonId;
        DelayMinutes      = delayMinutes;
        ReportedAt        = reportedAt;
    }

    /// <summary>
    /// Corrige los minutos de retraso reportados incorrectamente.
    /// scheduled_flight_id, delay_reason_id y reported_at son inmutables.
    /// </summary>
    public void AdjustDelay(int delayMinutes)
    {
        ValidateDelayMinutes(delayMinutes);
        DelayMinutes = delayMinutes;
    }

    // ── Validaciones privadas ─────────────────────────────────────────────────

    private static void ValidateDelayMinutes(int minutes)
    {
        if (minutes <= 0)
            throw new ArgumentException(
                "DelayMinutes must be greater than 0.", nameof(minutes));
    }
}
