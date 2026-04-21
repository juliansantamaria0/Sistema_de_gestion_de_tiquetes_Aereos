namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Infrastructure.Entity;

public sealed class PermissionEntity
{
    public int     PermissionId { get; set; }
    public string  Name        { get; set; } = string.Empty;
    public string? Description { get; set; }
}
