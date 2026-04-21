namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Infrastructure.Entity;

public sealed class PaymentEntity
{
    public int       Id                   { get; set; }
    public int?      ReservationId        { get; set; }
    public int?      TicketId             { get; set; }
    public int       CurrencyId           { get; set; }
    public DateTime  PaymentDate          { get; set; }
    public decimal   Amount               { get; set; }
    public int       PaymentStatusId      { get; set; }
    public int       PaymentMethodId      { get; set; }
    public string?   TransactionReference { get; set; }
    public string?   RejectionReason      { get; set; }
    public DateTime  CreatedAt            { get; set; }
    public DateTime? UpdatedAt            { get; set; }
}
