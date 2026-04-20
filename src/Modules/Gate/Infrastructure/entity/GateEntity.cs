namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Infrastructure.Entity;

/// <summary>Entidad EF Core para la tabla <c>gate</c>.</summary>
public sealed class GateEntity
{
    public int    GateId     { get; set; }
    public int    TerminalId { get; set; }
    public string Code       { get; set; } = string.Empty;
    public bool   IsActive   { get; set; }
}
