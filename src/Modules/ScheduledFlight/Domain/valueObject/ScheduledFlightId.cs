namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Domain.ValueObject;

public sealed class ScheduledFlightId
{
    public int Value { get; }

    public ScheduledFlightId(int value)
    {
        if (value <= 0)
            throw new ArgumentException("ScheduledFlightId must be a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is ScheduledFlightId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
