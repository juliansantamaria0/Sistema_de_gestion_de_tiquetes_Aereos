namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Infrastructure.Entity;

public sealed class LoyaltyTransactionEntity
{
    public int      Id               { get; set; }
    public int      LoyaltyAccountId { get; set; }
    public int?     TicketId         { get; set; }
    public string   TransactionType  { get; set; } = null!;
    public int      Miles            { get; set; }
    public DateTime TransactionDate  { get; set; }
}
