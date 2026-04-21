namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Domain.ValueObject;

public sealed class TicketStatusHistoryId
{
    public int Value { get; }

    public TicketStatusHistoryId(int value)
    {
        if (value <= 0)
            throw new ArgumentException(
                "TicketStatusHistoryId must be a positive integer.", nameof(value));

        Value = value;
    }

    public override bool Equals(object? obj) =>
        obj is TicketStatusHistoryId other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
}
