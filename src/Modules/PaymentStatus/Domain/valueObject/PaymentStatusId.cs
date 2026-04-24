namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Domain.ValueObject;

public sealed class PaymentStatusId
{
    public int Value { get; }

    public PaymentStatusId(int value)
    {
        if (value < 0)
            throw new ArgumentException("PaymentStatusId must be zero or a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is PaymentStatusId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
