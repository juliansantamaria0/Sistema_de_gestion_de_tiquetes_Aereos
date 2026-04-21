namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Domain.ValueObject;

public sealed class ContactTypeId
{
    public int Value { get; }

    public ContactTypeId(int value)
    {
        if (value <= 0)
            throw new ArgumentException("ContactTypeId must be a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is ContactTypeId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
