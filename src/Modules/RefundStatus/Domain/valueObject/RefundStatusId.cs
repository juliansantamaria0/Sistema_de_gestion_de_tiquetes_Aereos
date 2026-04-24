namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Domain.ValueObject;

public sealed class RefundStatusId
{
    public int Value { get; }

    public RefundStatusId(int value)
    {
        if (value < 0)
            throw new ArgumentException("RefundStatusId must be zero or a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is RefundStatusId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
