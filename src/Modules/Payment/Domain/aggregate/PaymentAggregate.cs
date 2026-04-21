namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Domain.ValueObject;

/// <summary>
/// Pago vinculado a una reserva O a un tiquete (XOR exclusivo).
/// SQL: payment. [TN-1] currency_id añadido para soporte multi-moneda.
///
/// Invariantes (espejo de los CHECKs del DDL):
///   1. amount >= 0 (chk_pay_amount).
///   2. XOR exclusivo (chk_pay_xor): reservation_id XOR ticket_id.
///      Un pago no puede estar vinculado a ambos ni a ninguno.
///
/// UpdateStatus(): única mutación válida — actualiza estado del pago,
/// transaction_reference y rejection_reason según el flujo de pago.
/// </summary>
public sealed class PaymentAggregate
{
    public PaymentId Id                   { get; private set; }
    public int?      ReservationId        { get; private set; }
    public int?      TicketId             { get; private set; }
    public int       CurrencyId           { get; private set; }
    public DateTime  PaymentDate          { get; private set; }
    public decimal   Amount               { get; private set; }
    public int       PaymentStatusId      { get; private set; }
    public int       PaymentMethodId      { get; private set; }
    public string?   TransactionReference { get; private set; }
    public string?   RejectionReason      { get; private set; }
    public DateTime  CreatedAt            { get; private set; }
    public DateTime? UpdatedAt            { get; private set; }

    private PaymentAggregate()
    {
        Id = null!;
    }

    public PaymentAggregate(
        PaymentId id,
        int?      reservationId,
        int?      ticketId,
        int       currencyId,
        DateTime  paymentDate,
        decimal   amount,
        int       paymentStatusId,
        int       paymentMethodId,
        string?   transactionReference,
        string?   rejectionReason,
        DateTime  createdAt,
        DateTime? updatedAt = null)
    {
        // XOR exclusivo: reservation_id XOR ticket_id
        ValidateXor(reservationId, ticketId);
        ValidateAmount(amount);
        ValidateForeignKeys(currencyId, paymentStatusId, paymentMethodId);
        ValidateTransactionReference(transactionReference);
        ValidateRejectionReason(rejectionReason);

        Id                   = id;
        ReservationId        = reservationId;
        TicketId             = ticketId;
        CurrencyId           = currencyId;
        PaymentDate          = paymentDate;
        Amount               = amount;
        PaymentStatusId      = paymentStatusId;
        PaymentMethodId      = paymentMethodId;
        TransactionReference = transactionReference?.Trim();
        RejectionReason      = rejectionReason?.Trim();
        CreatedAt            = createdAt;
        UpdatedAt            = updatedAt;
    }

    /// <summary>
    /// Actualiza el estado del pago junto con referencia y motivo de rechazo.
    /// ReservationId, TicketId, Amount, CurrencyId, PaymentDate y PaymentMethodId
    /// son inmutables tras la creación.
    /// </summary>
    public void UpdateStatus(
        int     paymentStatusId,
        string? transactionReference = null,
        string? rejectionReason      = null)
    {
        if (paymentStatusId <= 0)
            throw new ArgumentException(
                "PaymentStatusId must be a positive integer.", nameof(paymentStatusId));

        ValidateTransactionReference(transactionReference);
        ValidateRejectionReason(rejectionReason);

        PaymentStatusId      = paymentStatusId;
        TransactionReference = transactionReference?.Trim();
        RejectionReason      = rejectionReason?.Trim();
        UpdatedAt            = DateTime.UtcNow;
    }

    // ── Validaciones privadas ─────────────────────────────────────────────────

    private static void ValidateXor(int? reservationId, int? ticketId)
    {
        bool hasReservation = reservationId.HasValue;
        bool hasTicket      = ticketId.HasValue;

        if (hasReservation && hasTicket)
            throw new ArgumentException(
                "A payment cannot be linked to both a reservation and a ticket (XOR constraint).");

        if (!hasReservation && !hasTicket)
            throw new ArgumentException(
                "A payment must be linked to either a reservation or a ticket (XOR constraint).");

        if (reservationId.HasValue && reservationId.Value <= 0)
            throw new ArgumentException(
                "ReservationId must be a positive integer.", nameof(reservationId));

        if (ticketId.HasValue && ticketId.Value <= 0)
            throw new ArgumentException(
                "TicketId must be a positive integer.", nameof(ticketId));
    }

    private static void ValidateAmount(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Amount must be >= 0.", nameof(amount));
    }

    private static void ValidateForeignKeys(int currencyId, int paymentStatusId, int paymentMethodId)
    {
        if (currencyId <= 0)
            throw new ArgumentException("CurrencyId must be a positive integer.", nameof(currencyId));
        if (paymentStatusId <= 0)
            throw new ArgumentException("PaymentStatusId must be a positive integer.", nameof(paymentStatusId));
        if (paymentMethodId <= 0)
            throw new ArgumentException("PaymentMethodId must be a positive integer.", nameof(paymentMethodId));
    }

    private static void ValidateTransactionReference(string? reference)
    {
        if (reference is not null && reference.Trim().Length > 100)
            throw new ArgumentException(
                "TransactionReference cannot exceed 100 characters.", nameof(reference));
    }

    private static void ValidateRejectionReason(string? reason)
    {
        if (reason is not null && reason.Trim().Length > 250)
            throw new ArgumentException(
                "RejectionReason cannot exceed 250 characters.", nameof(reason));
    }
}
