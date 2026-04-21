namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Domain.ValueObject;

public sealed class FareTypeId
{
    public int Value { get; }

    public FareTypeId(int value)
    {
        if (value <= 0)
            throw new ArgumentException("FareTypeId must be a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is FareTypeId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
