namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Domain.ValueObject;

public sealed class RefundId
{
    public int Value { get; }

    public RefundId(int value)
    {
        if (value < 0)
            throw new ArgumentException("RefundId must be zero or a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is RefundId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
