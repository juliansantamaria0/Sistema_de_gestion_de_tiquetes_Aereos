namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Domain.ValueObject;

public sealed class ReservationStatusId
{
    public int Value { get; }

    public ReservationStatusId(int value)
    {
        if (value < 0)
            throw new ArgumentException("ReservationStatusId must be zero or a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is ReservationStatusId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
