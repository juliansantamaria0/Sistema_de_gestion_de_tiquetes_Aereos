namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Domain.ValueObject;

/// <summary>
/// Nivel del programa de fidelización (Classic, Silver, Gold, Diamond).
/// SQL: loyalty_tier. [NC-5] id renombrado a loyalty_tier_id.
///
/// UNIQUE: (loyalty_program_id, name) — nivel único por nombre dentro del programa.
/// UNIQUE: (loyalty_program_id, loyalty_tier_id) — [IR-3] soporte FK compuesta
///         en loyalty_account para garantizar que el tier pertenece al mismo programa.
///
/// Invariantes:
///   - min_miles >= 0.
///   - loyalty_program_id es la clave de contexto — inmutable.
///   - benefits es nullable (TEXT en SQL).
///
/// Update(): modifica nombre, millas mínimas y beneficios.
/// </summary>
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

    /// <summary>
    /// Actualiza el nombre, las millas mínimas y los beneficios del tier.
    /// loyalty_program_id es la clave de contexto — inmutable.
    /// </summary>
    public void Update(string name, int minMiles, string? benefits)
    {
        ValidateName(name);
        ValidateMinMiles(minMiles);

        Name     = name.Trim();
        MinMiles = minMiles;
        Benefits = benefits?.Trim();
    }

    // ── Validaciones privadas ─────────────────────────────────────────────────

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
