namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Domain.ValueObject;

public sealed class LoyaltyAccountId
{
    public int Value { get; }

    public LoyaltyAccountId(int value)
    {
        if (value <= 0)
            throw new ArgumentException("LoyaltyAccountId must be a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is LoyaltyAccountId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
