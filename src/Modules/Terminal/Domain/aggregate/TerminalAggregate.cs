namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Domain.ValueObject;





public sealed class TerminalAggregate
{
    public TerminalId Id              { get; private set; }
    public int        AirportId       { get; private set; }
    public string     Name            { get; private set; } = string.Empty;
    public bool       IsInternational { get; private set; }
    public DateTime   CreatedAt       { get; private set; }

    private TerminalAggregate() { }

    public static TerminalAggregate Create(int airportId, string name, bool isInternational)
    {
        if (airportId <= 0)  throw new ArgumentException("AirportId must be positive.", nameof(airportId));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Terminal name cannot be empty.", nameof(name));
        if (name.Length > 50) throw new ArgumentException("Terminal name cannot exceed 50 characters.", nameof(name));

        return new TerminalAggregate
        {
            AirportId       = airportId,
            Name            = name.Trim(),
            IsInternational = isInternational,
            CreatedAt       = DateTime.UtcNow
        };
    }

    public static TerminalAggregate Reconstitute(int id, int airportId, string name, bool isInternational, DateTime createdAt) =>
        new()
        {
            Id              = TerminalId.New(id),
            AirportId       = airportId,
            Name            = name,
            IsInternational = isInternational,
            CreatedAt       = createdAt
        };

    public void Update(int airportId, string name, bool isInternational)
    {
        if (airportId <= 0)  throw new ArgumentException("AirportId must be positive.", nameof(airportId));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Terminal name cannot be empty.", nameof(name));
        if (name.Length > 50) throw new ArgumentException("Terminal name cannot exceed 50 characters.", nameof(name));

        AirportId       = airportId;
        Name            = name.Trim();
        IsInternational = isInternational;
    }
}
