namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Domain.ValueObject;
















public sealed class LoyaltyAccountAggregate
{
    public LoyaltyAccountId Id               { get; private set; }
    public int              PassengerId      { get; private set; }
    public int              LoyaltyProgramId { get; private set; }
    public int              LoyaltyTierId    { get; private set; }
    public int              TotalMiles       { get; private set; }
    public int              AvailableMiles   { get; private set; }
    public DateTime         JoinedAt         { get; private set; }

    private LoyaltyAccountAggregate()
    {
        Id = null!;
    }

    public LoyaltyAccountAggregate(
        LoyaltyAccountId id,
        int              passengerId,
        int              loyaltyProgramId,
        int              loyaltyTierId,
        int              totalMiles,
        int              availableMiles,
        DateTime         joinedAt)
    {
        if (passengerId <= 0)
            throw new ArgumentException(
                "PassengerId must be a positive integer.", nameof(passengerId));

        if (loyaltyProgramId <= 0)
            throw new ArgumentException(
                "LoyaltyProgramId must be a positive integer.", nameof(loyaltyProgramId));

        if (loyaltyTierId <= 0)
            throw new ArgumentException(
                "LoyaltyTierId must be a positive integer.", nameof(loyaltyTierId));

        ValidateMiles(totalMiles, availableMiles);

        Id               = id;
        PassengerId      = passengerId;
        LoyaltyProgramId = loyaltyProgramId;
        LoyaltyTierId    = loyaltyTierId;
        TotalMiles       = totalMiles;
        AvailableMiles   = availableMiles;
        JoinedAt         = joinedAt;
    }

    
    
    
    public void AddMiles(int miles)
    {
        if (miles <= 0)
            throw new ArgumentException("Miles to add must be positive.", nameof(miles));

        TotalMiles     += miles;
        AvailableMiles += miles;
    }

    
    
    
    public void RedeemMiles(int miles)
    {
        if (miles <= 0)
            throw new ArgumentException("Miles to redeem must be positive.", nameof(miles));

        if (miles > AvailableMiles)
            throw new InvalidOperationException(
                $"Insufficient available miles. Available: {AvailableMiles}, requested: {miles}.");

        AvailableMiles -= miles;
    }

    
    
    
    
    
    public void UpgradeTier(int loyaltyTierId)
    {
        if (loyaltyTierId <= 0)
            throw new ArgumentException(
                "LoyaltyTierId must be a positive integer.", nameof(loyaltyTierId));

        LoyaltyTierId = loyaltyTierId;
    }

    

    private static void ValidateMiles(int totalMiles, int availableMiles)
    {
        if (totalMiles < 0)
            throw new ArgumentException("TotalMiles must be >= 0.", nameof(totalMiles));

        if (availableMiles < 0)
            throw new ArgumentException("AvailableMiles must be >= 0.", nameof(availableMiles));

        if (availableMiles > totalMiles)
            throw new ArgumentException(
                "AvailableMiles cannot exceed TotalMiles. [chk_la_miles]");
    }
}
