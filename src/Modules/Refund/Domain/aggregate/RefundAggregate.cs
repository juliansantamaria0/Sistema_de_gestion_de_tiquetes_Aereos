namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Domain.ValueObject;

/// <summary>
/// Solicitud de reembolso de un pago.
/// SQL: refund.
///
/// Invariantes:
///   - amount >= 0 (espejo del chk_refund_amount).
///   - payment_id, amount y requested_at son inmutables tras la creación.
///
/// Ciclo de vida típico: PENDING → APPROVED → PROCESSED
///                        PENDING → REJECTED
///
/// UpdateStatus(): única mutación válida.
///   - Registra processed_at cuando el estado indica que fue procesado/rechazado.
///   - reason: puede actualizarse al cambiar el estado.
/// </summary>
public sealed class RefundAggregate
{
    public RefundId  Id              { get; private set; }
    public int       PaymentId       { get; private set; }
    public int       RefundStatusId  { get; private set; }
    public decimal   Amount          { get; private set; }
    public DateTime  RequestedAt     { get; private set; }
    public DateTime? ProcessedAt     { get; private set; }
    public string?   Reason          { get; private set; }

    private RefundAggregate()
    {
        Id = null!;
    }

    public RefundAggregate(
        RefundId  id,
        int       paymentId,
        int       refundStatusId,
        decimal   amount,
        DateTime  requestedAt,
        DateTime? processedAt = null,
        string?   reason      = null)
    {
        if (paymentId <= 0)
            throw new ArgumentException(
                "PaymentId must be a positive integer.", nameof(paymentId));

        if (refundStatusId <= 0)
            throw new ArgumentException(
                "RefundStatusId must be a positive integer.", nameof(refundStatusId));

        ValidateAmount(amount);
        ValidateReason(reason);

        if (processedAt.HasValue && processedAt.Value < requestedAt)
            throw new ArgumentException(
                "processed_at cannot be earlier than requested_at.", nameof(processedAt));

        Id             = id;
        PaymentId      = paymentId;
        RefundStatusId = refundStatusId;
        Amount         = amount;
        RequestedAt    = requestedAt;
        ProcessedAt    = processedAt;
        Reason         = reason?.Trim();
    }

    /// <summary>
    /// Actualiza el estado del reembolso.
    /// Opcionalmente registra processed_at (cuando se aprueba, rechaza o procesa)
    /// y actualiza el motivo.
    /// payment_id, amount y requested_at son inmutables.
    /// </summary>
    public void UpdateStatus(int refundStatusId, DateTime? processedAt = null, string? reason = null)
    {
        if (refundStatusId <= 0)
            throw new ArgumentException(
                "RefundStatusId must be a positive integer.", nameof(refundStatusId));

        if (processedAt.HasValue && processedAt.Value < RequestedAt)
            throw new ArgumentException(
                "processed_at cannot be earlier than requested_at.", nameof(processedAt));

        ValidateReason(reason);

        RefundStatusId = refundStatusId;
        ProcessedAt    = processedAt ?? ProcessedAt;
        Reason         = reason?.Trim() ?? Reason;
    }

    // ── Validaciones privadas ─────────────────────────────────────────────────

    private static void ValidateAmount(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Amount must be >= 0.", nameof(amount));
    }

    private static void ValidateReason(string? reason)
    {
        if (reason is not null && reason.Trim().Length > 250)
            throw new ArgumentException(
                "Reason cannot exceed 250 characters.", nameof(reason));
    }
}
