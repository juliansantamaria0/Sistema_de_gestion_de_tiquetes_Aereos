namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Domain.ValueObject;

public sealed class TicketBaggageId
{
    public int Value { get; }

    public TicketBaggageId(int value)
    {
        if (value <= 0)
            throw new ArgumentException("TicketBaggageId must be a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is TicketBaggageId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
