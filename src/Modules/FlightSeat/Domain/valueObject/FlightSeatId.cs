namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Domain.ValueObject;

public sealed class FlightSeatId
{
    public int Value { get; }

    public FlightSeatId(int value)
    {
        if (value < 0)
            throw new ArgumentException("FlightSeatId must be zero or a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is FlightSeatId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
