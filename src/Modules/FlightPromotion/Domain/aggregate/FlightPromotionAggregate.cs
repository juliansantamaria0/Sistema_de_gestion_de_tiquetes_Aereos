namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Domain.ValueObject;










public sealed class FlightPromotionAggregate
{
    public FlightPromotionId Id                { get; private set; }
    public int               ScheduledFlightId { get; private set; }
    public int               PromotionId       { get; private set; }

    private FlightPromotionAggregate()
    {
        Id = null!;
    }

    public FlightPromotionAggregate(
        FlightPromotionId id,
        int               scheduledFlightId,
        int               promotionId)
    {
        if (scheduledFlightId <= 0)
            throw new ArgumentException(
                "ScheduledFlightId must be a positive integer.", nameof(scheduledFlightId));

        if (promotionId <= 0)
            throw new ArgumentException(
                "PromotionId must be a positive integer.", nameof(promotionId));

        Id                = id;
        ScheduledFlightId = scheduledFlightId;
        PromotionId       = promotionId;
    }
}
