namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Domain.ValueObject;
















public sealed class LoyaltyTierAggregate
{
    public LoyaltyTierId Id               { get; private set; }
    public int           LoyaltyProgramId { get; private set; }
    public string        Name             { get; private set; }
    public int           MinMiles         { get; private set; }
    public string?       Benefits         { get; private set; }

    private LoyaltyTierAggregate()
    {
        Id   = null!;
        Name = null!;
    }

    public LoyaltyTierAggregate(
        LoyaltyTierId id,
        int           loyaltyProgramId,
        string        name,
        int           minMiles,
        string?       benefits = null)
    {
        if (loyaltyProgramId <= 0)
            throw new ArgumentException(
                "LoyaltyProgramId must be a positive integer.", nameof(loyaltyProgramId));

        ValidateName(name);
        ValidateMinMiles(minMiles);

        Id               = id;
        LoyaltyProgramId = loyaltyProgramId;
        Name             = name.Trim();
        MinMiles         = minMiles;
        Benefits         = benefits?.Trim();
    }

    
    
    
    
    public void Update(string name, int minMiles, string? benefits)
    {
        ValidateName(name);
        ValidateMinMiles(minMiles);

        Name     = name.Trim();
        MinMiles = minMiles;
        Benefits = benefits?.Trim();
    }

    

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("LoyaltyTier name cannot be empty.", nameof(name));

        if (name.Trim().Length > 50)
            throw new ArgumentException(
                "LoyaltyTier name cannot exceed 50 characters.", nameof(name));
    }

    private static void ValidateMinMiles(int minMiles)
    {
        if (minMiles < 0)
            throw new ArgumentException(
                "MinMiles must be >= 0.", nameof(minMiles));
    }
}
