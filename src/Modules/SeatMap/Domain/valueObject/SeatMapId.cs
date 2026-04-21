namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Domain.ValueObject;

public sealed class SeatMapId
{
    public int Value { get; }

    public SeatMapId(int value)
    {
        if (value <= 0)
            throw new ArgumentException("SeatMapId must be a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is SeatMapId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
