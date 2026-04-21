namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Domain.ValueObject;

/// <summary>
/// Asignación de un permiso a un rol (tabla de unión M:N).
/// Invariante: (RoleId, PermissionId) deben ser únicos y positivos.
/// Update no tiene sentido semántico en esta tabla: se elimina y recrea.
/// </summary>
public sealed class RolePermissionAggregate
{
    public RolePermissionId Id           { get; private set; }
    public int              RoleId       { get; private set; }
    public int              PermissionId { get; private set; }

    private RolePermissionAggregate() { }

    public static RolePermissionAggregate Create(int roleId, int permissionId)
    {
        if (roleId <= 0)       throw new ArgumentException("RoleId must be positive.", nameof(roleId));
        if (permissionId <= 0) throw new ArgumentException("PermissionId must be positive.", nameof(permissionId));
        return new RolePermissionAggregate { RoleId = roleId, PermissionId = permissionId };
    }

    public static RolePermissionAggregate Reconstitute(int id, int roleId, int permissionId) =>
        new() { Id = RolePermissionId.New(id), RoleId = roleId, PermissionId = permissionId };
}
