namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.BaggageType.Domain.ValueObject;

public sealed class BaggageTypeId
{
    public int Value { get; }

    public BaggageTypeId(int value)
    {
        // 0 is used transiently before persistence assigns DB identity.
        if (value < 0)
            throw new ArgumentException("BaggageTypeId must be zero or a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is BaggageTypeId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
