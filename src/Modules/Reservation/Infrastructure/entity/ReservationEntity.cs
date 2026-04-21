namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Infrastructure.Entity;

public sealed class ReservationEntity
{
    public int       Id                  { get; set; }
    public string    ReservationCode     { get; set; } = null!;
    public int       CustomerId          { get; set; }
    public int       ScheduledFlightId   { get; set; }
    public DateTime  ReservationDate     { get; set; }
    public int       ReservationStatusId { get; set; }
    public DateTime? ConfirmedAt         { get; set; }
    public DateTime? CancelledAt         { get; set; }
    public DateTime  CreatedAt           { get; set; }
    public DateTime? UpdatedAt           { get; set; }
}
