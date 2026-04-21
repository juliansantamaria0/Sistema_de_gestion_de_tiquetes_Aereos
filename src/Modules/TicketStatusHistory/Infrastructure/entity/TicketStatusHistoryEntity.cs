namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Infrastructure.Entity;

public sealed class TicketStatusHistoryEntity
{
    public int      Id             { get; set; }
    public int      TicketId       { get; set; }
    public int      TicketStatusId { get; set; }
    public DateTime ChangedAt      { get; set; }
    public string?  Notes          { get; set; }
}
