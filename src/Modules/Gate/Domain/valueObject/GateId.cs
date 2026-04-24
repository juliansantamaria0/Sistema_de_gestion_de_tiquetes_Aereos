namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Domain.ValueObject;


public readonly record struct GateId(int Value)
{
    public static GateId New(int value)
    {
        if (value < 0)
            throw new ArgumentException("GateId must be zero or a positive integer.", nameof(value));
        return new GateId(value);
    }

    public override string ToString() => Value.ToString();
}
