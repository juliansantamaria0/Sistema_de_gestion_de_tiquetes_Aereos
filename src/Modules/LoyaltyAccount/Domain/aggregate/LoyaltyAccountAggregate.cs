namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Domain.ValueObject;

/// <summary>
/// Cuenta de un pasajero en un programa de fidelización.
/// SQL: loyalty_account. [NC-4] id renombrado a loyalty_account_id.
///
/// [IR-3] FK compuesta (loyalty_program_id, loyalty_tier_id) garantiza que
///        el tier asignado pertenece al mismo programa de la cuenta.
/// UNIQUE: (passenger_id, loyalty_program_id) — un pasajero, una cuenta por programa.
/// CHECK:  available_miles <= total_miles — espejado en dominio.
///
/// Ciclo de vida:
///   - AddMiles()    → acumula millas (total + available).
///   - RedeemMiles() → resta millas disponibles (available solo).
///   - UpgradeTier() → promueve al pasajero a un tier superior.
///   - passenger_id, loyalty_program_id y joined_at son inmutables.
/// </summary>
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

    /// <summary>
    /// Acumula millas: incrementa total y available.
    /// </summary>
    public void AddMiles(int miles)
    {
        if (miles <= 0)
            throw new ArgumentException("Miles to add must be positive.", nameof(miles));

        TotalMiles     += miles;
        AvailableMiles += miles;
    }

    /// <summary>
    /// Redime millas: decrementa solo available (las millas históricas se preservan).
    /// </summary>
    public void RedeemMiles(int miles)
    {
        if (miles <= 0)
            throw new ArgumentException("Miles to redeem must be positive.", nameof(miles));

        if (miles > AvailableMiles)
            throw new InvalidOperationException(
                $"Insufficient available miles. Available: {AvailableMiles}, requested: {miles}.");

        AvailableMiles -= miles;
    }

    /// <summary>
    /// Promueve al pasajero a un nuevo tier.
    /// La integridad de que el tier pertenezca al mismo programa se garantiza
    /// mediante la FK compuesta [IR-3] en la base de datos.
    /// </summary>
    public void UpgradeTier(int loyaltyTierId)
    {
        if (loyaltyTierId <= 0)
            throw new ArgumentException(
                "LoyaltyTierId must be a positive integer.", nameof(loyaltyTierId));

        LoyaltyTierId = loyaltyTierId;
    }

    // ── Validaciones privadas ─────────────────────────────────────────────────

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
