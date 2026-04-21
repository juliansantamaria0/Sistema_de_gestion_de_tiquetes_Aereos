namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Infrastructure.Entity;

public sealed class JobPositionEntity
{
    public int     JobPositionId { get; set; }
    public string  Name         { get; set; } = string.Empty;
    public string? Department   { get; set; }
}
