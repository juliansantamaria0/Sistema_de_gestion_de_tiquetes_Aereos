namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Domain.ValueObject;

public sealed class PassengerContactId
{
    public int Value { get; }

    public PassengerContactId(int value)
    {
        if (value < 0)
            throw new ArgumentException(
                "PassengerContactId must be zero or a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is PassengerContactId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
