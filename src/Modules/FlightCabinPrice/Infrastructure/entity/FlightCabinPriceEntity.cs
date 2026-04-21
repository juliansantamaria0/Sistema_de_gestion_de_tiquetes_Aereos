namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Infrastructure.Entity;

public sealed class FlightCabinPriceEntity
{
    public int     Id                { get; set; }
    public int     ScheduledFlightId { get; set; }
    public int     CabinClassId      { get; set; }
    public int     FareTypeId        { get; set; }
    public decimal Price             { get; set; }
}
