namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Infrastructure.Entity;

public sealed class SeatMapEntity
{
    public int     Id             { get; set; }
    public int     AircraftTypeId { get; set; }
    public string  SeatNumber     { get; set; } = null!;
    public int     CabinClassId   { get; set; }
    public string? SeatFeatures   { get; set; }
}
