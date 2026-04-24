namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Domain.ValueObject;

public sealed class PassengerId
{
    public int Value { get; }

    public PassengerId(int value)
    {
        if (value < 0)
            throw new ArgumentException("PassengerId must be zero or a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is PassengerId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
