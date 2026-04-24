namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Domain.ValueObject;

public sealed class RouteId
{
    public int Value { get; }

    public RouteId(int value)
    {
        if (value < 0)
            throw new ArgumentException("RouteId must be zero or a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is RouteId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
