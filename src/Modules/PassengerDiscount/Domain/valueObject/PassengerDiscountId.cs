namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Domain.ValueObject;

public sealed class PassengerDiscountId
{
    public int Value { get; }

    public PassengerDiscountId(int value)
    {
        if (value < 0)
            throw new ArgumentException("PassengerDiscountId must be zero or a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is PassengerDiscountId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
