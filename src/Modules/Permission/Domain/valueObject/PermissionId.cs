namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Domain.ValueObject;

public readonly record struct PermissionId(int Value)
{
    public static PermissionId New(int value)
    {
        if (value < 0)
            throw new ArgumentException("PermissionId must be zero or a positive integer.", nameof(value));
        return new PermissionId(value);
    }
    public override string ToString() => Value.ToString();
}
