namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Domain.ValueObject;











public sealed class FareTypeAggregate
{
    public FareTypeId Id                  { get; private set; }
    public string     Name                { get; private set; }
    public bool       IsRefundable        { get; private set; }
    public bool       IsChangeable        { get; private set; }
    public int        AdvancePurchaseDays { get; private set; }
    public bool       BaggageIncluded     { get; private set; }

    private FareTypeAggregate()
    {
        Id   = null!;
        Name = null!;
    }

    public FareTypeAggregate(
        FareTypeId id,
        string     name,
        bool       isRefundable,
        bool       isChangeable,
        int        advancePurchaseDays,
        bool       baggageIncluded)
    {
        ValidateName(name);
        ValidateAdvancePurchaseDays(advancePurchaseDays);

        Id                  = id;
        Name                = name.Trim();
        IsRefundable        = isRefundable;
        IsChangeable        = isChangeable;
        AdvancePurchaseDays = advancePurchaseDays;
        BaggageIncluded     = baggageIncluded;
    }

    
    public void Update(
        string name,
        bool   isRefundable,
        bool   isChangeable,
        int    advancePurchaseDays,
        bool   baggageIncluded)
    {
        ValidateName(name);
        ValidateAdvancePurchaseDays(advancePurchaseDays);

        Name                = name.Trim();
        IsRefundable        = isRefundable;
        IsChangeable        = isChangeable;
        AdvancePurchaseDays = advancePurchaseDays;
        BaggageIncluded     = baggageIncluded;
    }

    

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("FareType name cannot be empty.", nameof(name));

        if (name.Trim().Length > 50)
            throw new ArgumentException(
                "FareType name cannot exceed 50 characters.", nameof(name));
    }

    private static void ValidateAdvancePurchaseDays(int days)
    {
        if (days < 0)
            throw new ArgumentException(
                "AdvancePurchaseDays must be >= 0.", nameof(days));
    }
}
