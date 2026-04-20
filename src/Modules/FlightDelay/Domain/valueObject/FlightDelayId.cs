namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Domain.ValueObject;

public sealed class FlightDelayId
{
    public int Value { get; }

    public FlightDelayId(int value)
    {
        if (value <= 0)
            throw new ArgumentException("FlightDelayId must be a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is FlightDelayId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
