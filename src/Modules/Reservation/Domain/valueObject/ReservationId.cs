namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Domain.ValueObject;

public sealed class ReservationId
{
    public int Value { get; }

    public ReservationId(int value)
    {
        if (value < 0)
            throw new ArgumentException("ReservationId must be zero or a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is ReservationId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
