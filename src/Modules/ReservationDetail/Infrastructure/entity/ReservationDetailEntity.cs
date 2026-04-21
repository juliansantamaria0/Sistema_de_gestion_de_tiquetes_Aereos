namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Infrastructure.Entity;

public sealed class ReservationDetailEntity
{
    public int       Id                  { get; set; }
    public int       ReservationId       { get; set; }
    public int       PassengerId         { get; set; }
    public int       FlightSeatId        { get; set; }
    public int       FareTypeId          { get; set; }
    public DateTime  CreatedAt           { get; set; }
    public DateTime? UpdatedAt           { get; set; }
}
