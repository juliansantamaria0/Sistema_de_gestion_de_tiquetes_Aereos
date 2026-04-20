namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DelayReason.Infrastructure.Entity;

public sealed class DelayReasonEntity
{
    public int    Id       { get; set; }
    public string Name     { get; set; } = null!;
    public string Category { get; set; } = null!;
}
