namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Domain.ValueObject;

/// <summary>Value Object que representa el identificador único de una terminal.</summary>
public readonly record struct TerminalId(int Value)
{
    public static TerminalId New(int value)
    {
        if (value <= 0)
            throw new ArgumentException("TerminalId must be a positive integer.", nameof(value));
        return new TerminalId(value);
    }

    public override string ToString() => Value.ToString();
}
