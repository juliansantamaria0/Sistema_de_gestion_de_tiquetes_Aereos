namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Domain.ValueObject;

public sealed class SeatStatusId
{
    public int Value { get; }

    public SeatStatusId(int value)
    {
        if (value < 0)
            throw new ArgumentException("SeatStatusId must be zero or a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is SeatStatusId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
