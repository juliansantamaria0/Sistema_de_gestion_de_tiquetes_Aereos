namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Domain.ValueObject;

public sealed class TicketId
{
    public int Value { get; }

    public TicketId(int value)
    {
        if (value < 0)
            throw new ArgumentException("TicketId must be zero or a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is TicketId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
