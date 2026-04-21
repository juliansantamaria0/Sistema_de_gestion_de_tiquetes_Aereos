namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Domain.ValueObject;

public sealed class PaymentId
{
    public int Value { get; }

    public PaymentId(int value)
    {
        if (value <= 0)
            throw new ArgumentException("PaymentId must be a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is PaymentId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
