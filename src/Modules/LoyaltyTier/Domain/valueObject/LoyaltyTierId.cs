namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Domain.ValueObject;

public sealed class LoyaltyTierId
{
    public int Value { get; }

    public LoyaltyTierId(int value)
    {
        if (value < 0)
            throw new ArgumentException("LoyaltyTierId must be zero or a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is LoyaltyTierId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
