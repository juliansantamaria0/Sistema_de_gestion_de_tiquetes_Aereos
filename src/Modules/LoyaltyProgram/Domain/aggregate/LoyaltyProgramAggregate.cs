namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Domain.ValueObject;

/// <summary>
/// Programa de fidelización de una aerolínea.
/// SQL: loyalty_program. [NC-6] id renombrado a loyalty_program_id.
///
/// UNIQUE: airline_id — una aerolínea tiene un único programa.
/// UNIQUE: name — el nombre del programa es único globalmente.
///
/// Invariantes:
///   - miles_per_dollar > 0 — un programa que no acumula millas no tiene sentido.
///   - airline_id es inmutable (la aerolínea propietaria no cambia).
///
/// Update(): modifica nombre y tasa de acumulación.
/// </summary>
public sealed class LoyaltyProgramAggregate
{
    public LoyaltyProgramId Id             { get; private set; }
    public int              AirlineId      { get; private set; }
    public string           Name           { get; private set; }
    public decimal          MilesPerDollar { get; private set; }

    private LoyaltyProgramAggregate()
    {
        Id   = null!;
        Name = null!;
    }

    public LoyaltyProgramAggregate(
        LoyaltyProgramId id,
        int              airlineId,
        string           name,
        decimal          milesPerDollar)
    {
        if (airlineId <= 0)
            throw new ArgumentException(
                "AirlineId must be a positive integer.", nameof(airlineId));

        ValidateName(name);
        ValidateMilesPerDollar(milesPerDollar);

        Id             = id;
        AirlineId      = airlineId;
        Name           = name.Trim();
        MilesPerDollar = milesPerDollar;
    }

    /// <summary>
    /// Actualiza el nombre y la tasa de acumulación de millas.
    /// airline_id es inmutable — la aerolínea propietaria no cambia.
    /// </summary>
    public void Update(string name, decimal milesPerDollar)
    {
        ValidateName(name);
        ValidateMilesPerDollar(milesPerDollar);

        Name           = name.Trim();
        MilesPerDollar = milesPerDollar;
    }

    // ── Validaciones privadas ─────────────────────────────────────────────────

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("LoyaltyProgram name cannot be empty.", nameof(name));

        if (name.Trim().Length > 100)
            throw new ArgumentException(
                "LoyaltyProgram name cannot exceed 100 characters.", nameof(name));
    }

    private static void ValidateMilesPerDollar(decimal milesPerDollar)
    {
        if (milesPerDollar <= 0)
            throw new ArgumentException(
                "MilesPerDollar must be greater than 0.", nameof(milesPerDollar));
    }
}
