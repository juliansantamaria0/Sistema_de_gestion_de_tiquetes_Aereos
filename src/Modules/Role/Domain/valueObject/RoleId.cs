namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Domain.ValueObject;

public readonly record struct RoleId(int Value)
{
    public static RoleId New(int value)
    {
        if (value <= 0)
            throw new ArgumentException("RoleId must be a positive integer.", nameof(value));
        return new RoleId(value);
    }
    public override string ToString() => Value.ToString();
}
