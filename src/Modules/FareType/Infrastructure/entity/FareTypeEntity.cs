namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Infrastructure.Entity;

public sealed class FareTypeEntity
{
    public int    Id                  { get; set; }
    public string Name                { get; set; } = null!;
    public bool   IsRefundable        { get; set; }
    public bool   IsChangeable        { get; set; }
    public int    AdvancePurchaseDays { get; set; }
    public bool   BaggageIncluded     { get; set; }
}
