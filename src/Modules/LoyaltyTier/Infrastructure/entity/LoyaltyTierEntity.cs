namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Infrastructure.Entity;

public sealed class LoyaltyTierEntity
{
    public int     Id               { get; set; }
    public int     LoyaltyProgramId { get; set; }
    public string  Name             { get; set; } = null!;
    public int     MinMiles         { get; set; }
    public string? Benefits         { get; set; }

    public ICollection<Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Infrastructure.Entity.LoyaltyAccountEntity> LoyaltyAccounts { get; set; } = new List<Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Infrastructure.Entity.LoyaltyAccountEntity>();
}
