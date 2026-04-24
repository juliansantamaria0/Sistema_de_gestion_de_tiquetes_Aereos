namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatusHistory.Domain.ValueObject;

public sealed class FlightStatusHistoryId
{
    public int Value { get; }

    public FlightStatusHistoryId(int value)
    {
        if (value < 0)
            throw new ArgumentException(
                "FlightStatusHistoryId must be zero or a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is FlightStatusHistoryId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
