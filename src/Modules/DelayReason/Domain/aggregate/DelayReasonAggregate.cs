namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DelayReason.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DelayReason.Domain.ValueObject;









public sealed class DelayReasonAggregate
{
    public DelayReasonId Id       { get; private set; }
    public string        Name     { get; private set; }
    public string        Category { get; private set; }

    private DelayReasonAggregate()
    {
        Id       = null!;
        Name     = null!;
        Category = null!;
    }

    public DelayReasonAggregate(DelayReasonId id, string name, string category)
    {
        ValidateName(name);
        ValidateCategory(category);

        Id       = id;
        Name     = name.Trim().ToUpperInvariant();
        Category = category.Trim().ToUpperInvariant();
    }

    public void Update(string name, string category)
    {
        ValidateName(name);
        ValidateCategory(category);

        Name     = name.Trim().ToUpperInvariant();
        Category = category.Trim().ToUpperInvariant();
    }

    

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("DelayReason name cannot be empty.", nameof(name));

        if (name.Trim().Length > 80)
            throw new ArgumentException("DelayReason name cannot exceed 80 characters.", nameof(name));
    }

    private static void ValidateCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("DelayReason category cannot be empty.", nameof(category));

        if (category.Trim().Length > 50)
            throw new ArgumentException("DelayReason category cannot exceed 50 characters.", nameof(category));
    }
}
