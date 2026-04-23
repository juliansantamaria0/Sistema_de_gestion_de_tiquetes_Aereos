namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Infrastructure.Entity;

public sealed class FlightSeatEntity
{
    public int       Id                { get; set; }
    public int       ScheduledFlightId { get; set; }
    public int       SeatMapId         { get; set; }
    public int       SeatStatusId      { get; set; }
    public byte[] Version { get; set; } = null!;
    public DateTime  CreatedAt         { get; set; }
    public DateTime? UpdatedAt         { get; set; }
}
