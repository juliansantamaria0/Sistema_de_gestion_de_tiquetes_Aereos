namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Infrastructure.Entity;

public sealed class LoyaltyAccountEntity
{
    public int      Id               { get; set; }
    public int      PassengerId      { get; set; }
    public int      LoyaltyProgramId { get; set; }
    public int      LoyaltyTierId    { get; set; }
    public int      TotalMiles       { get; set; }
    public int      AvailableMiles   { get; set; }
    public DateTime JoinedAt         { get; set; }

    // public Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Infrastructure.Entity.LoyaltyTierEntity? LoyaltyTier { get; set; }
}
