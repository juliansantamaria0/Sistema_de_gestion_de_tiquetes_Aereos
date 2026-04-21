namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Infrastructure.Entity;

public sealed class RoleEntity
{
    public int    RoleId   { get; set; }
    public string Name     { get; set; } = string.Empty;
    public bool   IsActive { get; set; }
}
