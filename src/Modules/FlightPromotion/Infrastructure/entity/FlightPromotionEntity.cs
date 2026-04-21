namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Infrastructure.Entity;

public sealed class FlightPromotionEntity
{
    public int Id                { get; set; }
    public int ScheduledFlightId { get; set; }
    public int PromotionId       { get; set; }
}
