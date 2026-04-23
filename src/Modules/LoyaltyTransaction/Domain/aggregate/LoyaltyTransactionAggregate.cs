namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Domain.ValueObject;













public sealed class LoyaltyTransactionAggregate
{
    public static readonly string TypeEarn   = "EARN";
    public static readonly string TypeRedeem = "REDEEM";

    public LoyaltyTransactionId Id               { get; private set; }
    public int                  LoyaltyAccountId { get; private set; }
    public int?                 TicketId         { get; private set; }
    public string               TransactionType  { get; private set; }
    public int                  Miles            { get; private set; }
    public DateTime             TransactionDate  { get; private set; }

    private LoyaltyTransactionAggregate()
    {
        Id              = null!;
        TransactionType = null!;
    }

    public LoyaltyTransactionAggregate(
        LoyaltyTransactionId id,
        int                  loyaltyAccountId,
        int?                 ticketId,
        string               transactionType,
        int                  miles,
        DateTime             transactionDate)
    {
        if (loyaltyAccountId <= 0)
            throw new ArgumentException(
                "LoyaltyAccountId must be a positive integer.", nameof(loyaltyAccountId));

        if (ticketId.HasValue && ticketId.Value <= 0)
            throw new ArgumentException(
                "TicketId must be a positive integer when provided.", nameof(ticketId));

        ValidateTransactionType(transactionType);
        ValidateMiles(miles);

        Id               = id;
        LoyaltyAccountId = loyaltyAccountId;
        TicketId         = ticketId;
        TransactionType  = transactionType.Trim().ToUpperInvariant();
        Miles            = miles;
        TransactionDate  = transactionDate;
    }

    

    private static void ValidateTransactionType(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException(
                "TransactionType cannot be empty.", nameof(type));

        var normalized = type.Trim().ToUpperInvariant();
        if (normalized != TypeEarn && normalized != TypeRedeem)
            throw new ArgumentException(
                $"TransactionType must be '{TypeEarn}' or '{TypeRedeem}'. [chk_ltx_type]",
                nameof(type));
    }

    private static void ValidateMiles(int miles)
    {
        if (miles <= 0)
            throw new ArgumentException(
                "Miles must be greater than 0. [chk_ltx_miles]", nameof(miles));
    }
}
