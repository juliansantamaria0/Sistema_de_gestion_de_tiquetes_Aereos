namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Domain.ValueObject;

public readonly record struct RolePermissionId(int Value)
{
    public static RolePermissionId New(int value)
    {
        if (value < 0)
            throw new ArgumentException("RolePermissionId must be zero or a positive integer.", nameof(value));
        return new RolePermissionId(value);
    }
    public override string ToString() => Value.ToString();
}
