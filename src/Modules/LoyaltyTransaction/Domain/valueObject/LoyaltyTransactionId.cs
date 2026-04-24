namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Domain.ValueObject;

public sealed class LoyaltyTransactionId
{
    public int Value { get; }

    public LoyaltyTransactionId(int value)
    {
        if (value < 0)
            throw new ArgumentException("LoyaltyTransactionId must be zero or a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is LoyaltyTransactionId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
