namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Domain.ValueObject;

public sealed class GenderId
{
    public int Value { get; }

    public GenderId(int value)
    {
        if (value < 0)
            throw new ArgumentException("GenderId must be zero or a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is GenderId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
