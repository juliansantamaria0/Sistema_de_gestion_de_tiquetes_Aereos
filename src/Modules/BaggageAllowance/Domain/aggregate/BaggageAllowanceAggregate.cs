namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.BaggageAllowance.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.BaggageAllowance.Domain.ValueObject;












public sealed class BaggageAllowanceAggregate
{
    public BaggageAllowanceId Id             { get; private set; }
    public int                CabinClassId   { get; private set; }
    public int                FareTypeId     { get; private set; }
    public int                CarryOnPieces  { get; private set; }
    public decimal            CarryOnKg      { get; private set; }
    public int                CheckedPieces  { get; private set; }
    public decimal            CheckedKg      { get; private set; }

    private BaggageAllowanceAggregate()
    {
        Id = null!;
    }

    public BaggageAllowanceAggregate(
        BaggageAllowanceId id,
        int                cabinClassId,
        int                fareTypeId,
        int                carryOnPieces,
        decimal            carryOnKg,
        int                checkedPieces,
        decimal            checkedKg)
    {
        if (cabinClassId <= 0)
            throw new ArgumentException("CabinClassId must be a positive integer.", nameof(cabinClassId));

        if (fareTypeId <= 0)
            throw new ArgumentException("FareTypeId must be a positive integer.", nameof(fareTypeId));

        ValidateLimits(carryOnPieces, carryOnKg, checkedPieces, checkedKg);

        Id            = id;
        CabinClassId  = cabinClassId;
        FareTypeId    = fareTypeId;
        CarryOnPieces = carryOnPieces;
        CarryOnKg     = carryOnKg;
        CheckedPieces = checkedPieces;
        CheckedKg     = checkedKg;
    }

    
    
    
    
    public void Update(int carryOnPieces, decimal carryOnKg, int checkedPieces, decimal checkedKg)
    {
        ValidateLimits(carryOnPieces, carryOnKg, checkedPieces, checkedKg);

        CarryOnPieces = carryOnPieces;
        CarryOnKg     = carryOnKg;
        CheckedPieces = checkedPieces;
        CheckedKg     = checkedKg;
    }

    

    private static void ValidateLimits(
        int     carryOnPieces,
        decimal carryOnKg,
        int     checkedPieces,
        decimal checkedKg)
    {
        if (carryOnPieces < 0)
            throw new ArgumentException("CarryOnPieces must be >= 0.", nameof(carryOnPieces));

        if (carryOnKg < 0)
            throw new ArgumentException("CarryOnKg must be >= 0.", nameof(carryOnKg));

        if (checkedPieces < 0)
            throw new ArgumentException("CheckedPieces must be >= 0.", nameof(checkedPieces));

        if (checkedKg < 0)
            throw new ArgumentException("CheckedKg must be >= 0.", nameof(checkedKg));
    }
}
