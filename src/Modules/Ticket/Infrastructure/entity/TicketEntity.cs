namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Infrastructure.Entity;

public sealed class TicketEntity
{
    public int       Id                  { get; set; }
    public string    TicketCode          { get; set; } = null!;
    public int       ReservationDetailId { get; set; }
    public DateTime  IssueDate           { get; set; }
    public int       TicketStatusId      { get; set; }
    public DateTime  CreatedAt           { get; set; }
    public DateTime? UpdatedAt           { get; set; }
}
