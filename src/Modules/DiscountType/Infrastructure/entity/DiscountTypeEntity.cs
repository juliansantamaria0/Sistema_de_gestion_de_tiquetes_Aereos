namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Infrastructure.Entity;

public sealed class DiscountTypeEntity
{
    public int     Id         { get; set; }
    public string  Name       { get; set; } = null!;
    public decimal Percentage { get; set; }
    public int?    AgeMin     { get; set; }
    public int?    AgeMax     { get; set; }
}
