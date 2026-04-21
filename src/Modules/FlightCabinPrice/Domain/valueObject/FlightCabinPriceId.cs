namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Domain.ValueObject;

public sealed class FlightCabinPriceId
{
    public int Value { get; }

    public FlightCabinPriceId(int value)
    {
        if (value <= 0)
            throw new ArgumentException(
                "FlightCabinPriceId must be a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is FlightCabinPriceId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
