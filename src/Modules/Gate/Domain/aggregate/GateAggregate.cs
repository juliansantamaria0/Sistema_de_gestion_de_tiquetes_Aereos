namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Domain.ValueObject;





public sealed class GateAggregate
{
    public GateId  Id         { get; private set; }
    public int     TerminalId { get; private set; }
    public string  Code       { get; private set; } = string.Empty;
    public bool    IsActive   { get; private set; }

    private GateAggregate() { }

    public static GateAggregate Create(int terminalId, string code, bool isActive = true)
    {
        if (terminalId <= 0) throw new ArgumentException("TerminalId must be positive.", nameof(terminalId));
        ValidateCode(code);

        return new GateAggregate
        {
            TerminalId = terminalId,
            Code       = code.Trim().ToUpperInvariant(),
            IsActive   = isActive
        };
    }

    public static GateAggregate Reconstitute(int id, int terminalId, string code, bool isActive) =>
        new()
        {
            Id         = GateId.New(id),
            TerminalId = terminalId,
            Code       = code,
            IsActive   = isActive
        };

    public void Update(int terminalId, string code, bool isActive)
    {
        if (terminalId <= 0) throw new ArgumentException("TerminalId must be positive.", nameof(terminalId));
        ValidateCode(code);

        TerminalId = terminalId;
        Code       = code.Trim().ToUpperInvariant();
        IsActive   = isActive;
    }

    public void Activate()  => IsActive = true;
    public void Deactivate() => IsActive = false;

    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Gate code cannot be empty.", nameof(code));
        if (code.Length > 10)               throw new ArgumentException("Gate code cannot exceed 10 characters.", nameof(code));
    }
}
