namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Infrastructure.Entity;

/// <summary>Entidad EF Core para la tabla <c>terminal</c>.</summary>
public sealed class TerminalEntity
{
    public int      TerminalId      { get; set; }
    public int      AirportId       { get; set; }
    public string   Name            { get; set; } = string.Empty;
    public bool     IsInternational { get; set; }
    public DateTime CreatedAt       { get; set; }
}
