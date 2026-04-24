namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Domain.ValueObject;

public sealed class PaymentMethodId
{
    public int Value { get; }

    public PaymentMethodId(int value)
    {
        if (value < 0)
            throw new ArgumentException("PaymentMethodId must be zero or a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is PaymentMethodId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
