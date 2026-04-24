namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Domain.ValueObject;

public sealed class FlightPromotionId
{
    public int Value { get; }

    public FlightPromotionId(int value)
    {
        if (value < 0)
            throw new ArgumentException(
                "FlightPromotionId must be zero or a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is FlightPromotionId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
