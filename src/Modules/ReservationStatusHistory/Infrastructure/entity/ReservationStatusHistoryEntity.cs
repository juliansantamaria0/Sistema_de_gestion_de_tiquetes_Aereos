namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Infrastructure.Entity;

public sealed class ReservationStatusHistoryEntity
{
    public int      Id                  { get; set; }
    public int      ReservationId       { get; set; }
    public int      ReservationStatusId { get; set; }
    public DateTime ChangedAt           { get; set; }
    public string?  Notes               { get; set; }
}
