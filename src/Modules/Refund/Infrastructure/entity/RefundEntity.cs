namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Infrastructure.Entity;

public sealed class RefundEntity
{
    public int       Id             { get; set; }
    public int       PaymentId      { get; set; }
    public int       RefundStatusId { get; set; }
    public decimal   Amount         { get; set; }
    public DateTime  RequestedAt    { get; set; }
    public DateTime? ProcessedAt    { get; set; }
    public string?   Reason         { get; set; }
}
