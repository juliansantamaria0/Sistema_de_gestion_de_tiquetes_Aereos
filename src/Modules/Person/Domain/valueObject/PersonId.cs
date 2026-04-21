namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Domain.ValueObject;

public sealed class PersonId
{
    public int Value { get; }

    public PersonId(int value)
    {
        if (value <= 0)
            throw new ArgumentException("PersonId must be a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is PersonId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
