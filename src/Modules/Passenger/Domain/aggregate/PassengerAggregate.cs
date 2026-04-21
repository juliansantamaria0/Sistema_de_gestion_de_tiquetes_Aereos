namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Domain.ValueObject;

/// <summary>
/// Rol pasajero: quien viaja.
/// SQL: passenger.
///
/// UNIQUE: person_id — una persona, un rol pasajero.
/// UNIQUE: frequent_flyer_number (cuando no es null).
/// nationality_id: para documentación aduanera (distinto de país de residencia).
///
/// person_id es inmutable — la persona asociada no cambia.
/// Update(): modifica frequent_flyer_number y nationality_id.
/// </summary>
public sealed class PassengerAggregate
{
    public PassengerId Id                  { get; private set; }
    public int         PersonId            { get; private set; }
    public string?     FrequentFlyerNumber { get; private set; }
    public int?        NationalityId       { get; private set; }
    public DateTime    CreatedAt           { get; private set; }
    public DateTime?   UpdatedAt           { get; private set; }

    private PassengerAggregate()
    {
        Id = null!;
    }

    public PassengerAggregate(
        PassengerId id,
        int         personId,
        string?     frequentFlyerNumber,
        int?        nationalityId,
        DateTime    createdAt,
        DateTime?   updatedAt = null)
    {
        if (personId <= 0)
            throw new ArgumentException(
                "PersonId must be a positive integer.", nameof(personId));

        ValidateFrequentFlyerNumber(frequentFlyerNumber);
        ValidateNationalityId(nationalityId);

        Id                  = id;
        PersonId            = personId;
        FrequentFlyerNumber = frequentFlyerNumber?.Trim().ToUpperInvariant();
        NationalityId       = nationalityId;
        CreatedAt           = createdAt;
        UpdatedAt           = updatedAt;
    }

    /// <summary>
    /// Actualiza el número de viajero frecuente y la nacionalidad.
    /// PersonId es inmutable.
    /// </summary>
    public void Update(string? frequentFlyerNumber, int? nationalityId)
    {
        ValidateFrequentFlyerNumber(frequentFlyerNumber);
        ValidateNationalityId(nationalityId);

        FrequentFlyerNumber = frequentFlyerNumber?.Trim().ToUpperInvariant();
        NationalityId       = nationalityId;
        UpdatedAt           = DateTime.UtcNow;
    }

    // ── Validaciones privadas ─────────────────────────────────────────────────

    private static void ValidateFrequentFlyerNumber(string? number)
    {
        if (number is not null && number.Trim().Length > 30)
            throw new ArgumentException(
                "FrequentFlyerNumber cannot exceed 30 characters.", nameof(number));
    }

    private static void ValidateNationalityId(int? nationalityId)
    {
        if (nationalityId.HasValue && nationalityId.Value <= 0)
            throw new ArgumentException(
                "NationalityId must be a positive integer when provided.", nameof(nationalityId));
    }
}
