namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.ValueObject;

/// <summary>
/// Reserva de un cliente para un vuelo concreto.
/// SQL: reservation.
///
/// Invariantes (espejo de los CHECKs del DDL):
///   1. confirmed_at >= reservation_date  (si no es null)
///   2. cancelled_at >= reservation_date  (si no es null)
///   3. confirmed_at y cancelled_at son mutuamente excluyentes (CHECK chk_mutual_excl)
///
/// Ciclo de vida: PENDING → CONFIRMED o PENDING → CANCELLED.
/// Una reserva confirmada no puede cancelarse y viceversa
/// (exclusión mutua garantizada en dominio y en BD).
///
/// reservation_code: código único de negocio, normalizado a mayúsculas.
/// </summary>
public sealed class ReservationAggregate
{
    public ReservationId Id                  { get; private set; }
    public string        ReservationCode     { get; private set; }
    public int           CustomerId          { get; private set; }
    public int           ScheduledFlightId   { get; private set; }
    public DateTime      ReservationDate     { get; private set; }
    public int           ReservationStatusId { get; private set; }
    public DateTime?     ConfirmedAt         { get; private set; }
    public DateTime?     CancelledAt         { get; private set; }
    public DateTime      CreatedAt           { get; private set; }
    public DateTime?     UpdatedAt           { get; private set; }

    private ReservationAggregate()
    {
        Id              = null!;
        ReservationCode = null!;
    }

    public ReservationAggregate(
        ReservationId id,
        string        reservationCode,
        int           customerId,
        int           scheduledFlightId,
        DateTime      reservationDate,
        int           reservationStatusId,
        DateTime?     confirmedAt,
        DateTime?     cancelledAt,
        DateTime      createdAt,
        DateTime?     updatedAt = null)
    {
        ValidateCode(reservationCode);

        if (customerId <= 0)
            throw new ArgumentException("CustomerId must be a positive integer.", nameof(customerId));

        if (scheduledFlightId <= 0)
            throw new ArgumentException("ScheduledFlightId must be a positive integer.", nameof(scheduledFlightId));

        if (reservationStatusId <= 0)
            throw new ArgumentException("ReservationStatusId must be a positive integer.", nameof(reservationStatusId));

        ValidateTimestamps(reservationDate, confirmedAt, cancelledAt);

        Id                  = id;
        ReservationCode     = reservationCode.Trim().ToUpperInvariant();
        CustomerId          = customerId;
        ScheduledFlightId   = scheduledFlightId;
        ReservationDate     = reservationDate;
        ReservationStatusId = reservationStatusId;
        ConfirmedAt         = confirmedAt;
        CancelledAt         = cancelledAt;
        CreatedAt           = createdAt;
        UpdatedAt           = updatedAt;
    }

    /// <summary>
    /// Confirma la reserva.
    /// Precondición: no debe estar ya cancelada (exclusión mutua).
    /// Establece confirmed_at = now y actualiza el status al ID proporcionado.
    /// </summary>
    public void Confirm(int confirmedStatusId)
    {
        if (CancelledAt.HasValue)
            throw new InvalidOperationException(
                "Cannot confirm a reservation that has already been cancelled.");

        if (ConfirmedAt.HasValue)
            throw new InvalidOperationException(
                "Reservation is already confirmed.");

        if (confirmedStatusId <= 0)
            throw new ArgumentException(
                "ConfirmedStatusId must be a positive integer.", nameof(confirmedStatusId));

        var now = DateTime.UtcNow;

        if (now < ReservationDate)
            throw new InvalidOperationException(
                "confirmed_at cannot be earlier than reservation_date.");

        ReservationStatusId = confirmedStatusId;
        ConfirmedAt         = now;
        UpdatedAt           = now;
    }

    /// <summary>
    /// Cancela la reserva.
    /// Precondición: no debe estar ya confirmada (exclusión mutua).
    /// Establece cancelled_at = now y actualiza el status al ID proporcionado.
    /// </summary>
    public void Cancel(int cancelledStatusId)
    {
        if (ConfirmedAt.HasValue)
            throw new InvalidOperationException(
                "Cannot cancel a reservation that has already been confirmed.");

        if (CancelledAt.HasValue)
            throw new InvalidOperationException(
                "Reservation is already cancelled.");

        if (cancelledStatusId <= 0)
            throw new ArgumentException(
                "CancelledStatusId must be a positive integer.", nameof(cancelledStatusId));

        var now = DateTime.UtcNow;

        if (now < ReservationDate)
            throw new InvalidOperationException(
                "cancelled_at cannot be earlier than reservation_date.");

        ReservationStatusId = cancelledStatusId;
        CancelledAt         = now;
        UpdatedAt           = now;
    }

    /// <summary>
    /// Cambia el estado de la reserva sin afectar confirmed_at ni cancelled_at.
    /// Usado para transiciones intermedias de estado (ej: PENDING → ON_HOLD).
    /// </summary>
    public void ChangeStatus(int reservationStatusId)
    {
        if (reservationStatusId <= 0)
            throw new ArgumentException(
                "ReservationStatusId must be a positive integer.", nameof(reservationStatusId));

        ReservationStatusId = reservationStatusId;
        UpdatedAt           = DateTime.UtcNow;
    }

    // ── Validaciones privadas ─────────────────────────────────────────────────

    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("ReservationCode cannot be empty.", nameof(code));

        if (code.Trim().Length > 20)
            throw new ArgumentException("ReservationCode cannot exceed 20 characters.", nameof(code));
    }

    private static void ValidateTimestamps(
        DateTime  reservationDate,
        DateTime? confirmedAt,
        DateTime? cancelledAt)
    {
        // CHECK chk_confirmed_at
        if (confirmedAt.HasValue && confirmedAt.Value < reservationDate)
            throw new ArgumentException(
                "confirmed_at must be >= reservation_date.", nameof(confirmedAt));

        // CHECK chk_cancelled_at
        if (cancelledAt.HasValue && cancelledAt.Value < reservationDate)
            throw new ArgumentException(
                "cancelled_at must be >= reservation_date.", nameof(cancelledAt));

        // CHECK chk_mutual_excl
        if (confirmedAt.HasValue && cancelledAt.HasValue)
            throw new ArgumentException(
                "confirmed_at and cancelled_at are mutually exclusive.");
    }
}
