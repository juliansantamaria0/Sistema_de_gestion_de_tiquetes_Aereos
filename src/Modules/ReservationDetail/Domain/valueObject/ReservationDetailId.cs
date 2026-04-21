namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.ValueObject;

public sealed class ReservationDetailId
{
    public int Value { get; }

    public ReservationDetailId(int value)
    {
        if (value <= 0)
            throw new ArgumentException("ReservationDetailId must be a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is ReservationDetailId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
