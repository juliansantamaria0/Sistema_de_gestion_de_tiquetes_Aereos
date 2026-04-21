namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Domain.ValueObject;

public sealed class LoyaltyProgramId
{
    public int Value { get; }

    public LoyaltyProgramId(int value)
    {
        if (value <= 0)
            throw new ArgumentException("LoyaltyProgramId must be a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is LoyaltyProgramId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
