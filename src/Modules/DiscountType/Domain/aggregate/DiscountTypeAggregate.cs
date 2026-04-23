namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Domain.ValueObject;








public sealed class DiscountTypeAggregate
{
    public DiscountTypeId Id         { get; private set; }
    public string         Name       { get; private set; }
    public decimal        Percentage { get; private set; }
    public int?           AgeMin     { get; private set; }
    public int?           AgeMax     { get; private set; }

    private DiscountTypeAggregate()
    {
        Id   = null!;
        Name = null!;
    }

    public DiscountTypeAggregate(
        DiscountTypeId id,
        string         name,
        decimal        percentage,
        int?           ageMin,
        int?           ageMax)
    {
        ValidateName(name);
        ValidatePercentage(percentage);
        ValidateAgeRange(ageMin, ageMax);

        Id         = id;
        Name       = name.Trim();
        Percentage = percentage;
        AgeMin     = ageMin;
        AgeMax     = ageMax;
    }

    public void Update(string name, decimal percentage, int? ageMin, int? ageMax)
    {
        ValidateName(name);
        ValidatePercentage(percentage);
        ValidateAgeRange(ageMin, ageMax);

        Name       = name.Trim();
        Percentage = percentage;
        AgeMin     = ageMin;
        AgeMax     = ageMax;
    }

    

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("DiscountType name cannot be empty.", nameof(name));

        if (name.Trim().Length > 50)
            throw new ArgumentException(
                "DiscountType name cannot exceed 50 characters.", nameof(name));
    }

    private static void ValidatePercentage(decimal percentage)
    {
        if (percentage < 0 || percentage > 100)
            throw new ArgumentException(
                "Percentage must be between 0 and 100. [chk_dt_pct]", nameof(percentage));
    }

    private static void ValidateAgeRange(int? ageMin, int? ageMax)
    {
        if (ageMin.HasValue && ageMin.Value < 0)
            throw new ArgumentException("AgeMin must be >= 0.", nameof(ageMin));

        if (ageMax.HasValue && ageMax.Value < 0)
            throw new ArgumentException("AgeMax must be >= 0.", nameof(ageMax));

        if (ageMin.HasValue && ageMax.HasValue && ageMin.Value > ageMax.Value)
            throw new ArgumentException(
                "AgeMin cannot be greater than AgeMax.", nameof(ageMin));
    }
}
