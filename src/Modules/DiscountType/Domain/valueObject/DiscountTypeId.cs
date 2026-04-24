namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Domain.ValueObject;

public sealed class DiscountTypeId
{
    public int Value { get; }

    public DiscountTypeId(int value)
    {
        if (value < 0)
            throw new ArgumentException("DiscountTypeId must be zero or a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is DiscountTypeId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
