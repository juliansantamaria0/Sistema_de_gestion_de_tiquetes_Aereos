namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Domain.ValueObject;

public sealed class FlightCancellationId
{
    public int Value { get; }

    public FlightCancellationId(int value)
    {
        if (value <= 0)
            throw new ArgumentException("FlightCancellationId must be a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is FlightCancellationId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
