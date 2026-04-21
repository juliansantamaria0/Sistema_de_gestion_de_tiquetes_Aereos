namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Infrastructure.Entity;

public sealed class LoyaltyProgramEntity
{
    public int     Id             { get; set; }
    public int     AirlineId      { get; set; }
    public string  Name           { get; set; } = null!;
    public decimal MilesPerDollar { get; set; }
}
