namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Domain.ValueObject;

public sealed class FlightStatusId
{
    public int Value { get; }

    public FlightStatusId(int value)
    {
        if (value <= 0)
            throw new ArgumentException("FlightStatusId must be a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is FlightStatusId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
