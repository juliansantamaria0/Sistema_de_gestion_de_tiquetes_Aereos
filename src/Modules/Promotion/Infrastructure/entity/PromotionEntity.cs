namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Infrastructure.Entity;

public sealed class PromotionEntity
{
    public int      Id          { get; set; }
    public int      AirlineId   { get; set; }
    public string   Name        { get; set; } = null!;
    public decimal  DiscountPct { get; set; }
    public DateOnly ValidFrom   { get; set; }
    public DateOnly ValidUntil  { get; set; }
}
