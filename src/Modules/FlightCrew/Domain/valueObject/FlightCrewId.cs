namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Domain.ValueObject;

public sealed class FlightCrewId
{
    public int Value { get; }

    public FlightCrewId(int value)
    {
        if (value <= 0)
            throw new ArgumentException("FlightCrewId must be a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is FlightCrewId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
