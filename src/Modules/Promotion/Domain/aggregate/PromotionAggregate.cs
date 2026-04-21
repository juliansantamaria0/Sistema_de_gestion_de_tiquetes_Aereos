namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Domain.ValueObject;

/// <summary>
/// Promoción de aerolínea con descuento por porcentaje y vigencia.
/// SQL: promotion.
///
/// valid_from y valid_until usan DateOnly (.NET 8) — columnas DATE en MySQL.
/// Pomelo 8.x mapea DATE ↔ DateOnly de forma nativa.
///
/// CHECKs del DDL espejados:
///   - discount_pct IN [0, 100]       [chk_promo_pct]
///   - valid_until >= valid_from       [chk_promo_dates]
///
/// airline_id es la clave de contexto — inmutable.
/// Update(): modifica nombre, porcentaje y rango de fechas.
/// </summary>
public sealed class PromotionAggregate
{
    public PromotionId Id          { get; private set; }
    public int         AirlineId   { get; private set; }
    public string      Name        { get; private set; }
    public decimal     DiscountPct { get; private set; }
    public DateOnly    ValidFrom   { get; private set; }
    public DateOnly    ValidUntil  { get; private set; }

    private PromotionAggregate()
    {
        Id   = null!;
        Name = null!;
    }

    public PromotionAggregate(
        PromotionId id,
        int         airlineId,
        string      name,
        decimal     discountPct,
        DateOnly    validFrom,
        DateOnly    validUntil)
    {
        if (airlineId <= 0)
            throw new ArgumentException(
                "AirlineId must be a positive integer.", nameof(airlineId));

        ValidateName(name);
        ValidateDiscountPct(discountPct);
        ValidateDates(validFrom, validUntil);

        Id          = id;
        AirlineId   = airlineId;
        Name        = name.Trim();
        DiscountPct = discountPct;
        ValidFrom   = validFrom;
        ValidUntil  = validUntil;
    }

    /// <summary>
    /// Actualiza nombre, porcentaje y rango de vigencia.
    /// AirlineId es la clave de contexto — inmutable.
    /// </summary>
    public void Update(string name, decimal discountPct, DateOnly validFrom, DateOnly validUntil)
    {
        ValidateName(name);
        ValidateDiscountPct(discountPct);
        ValidateDates(validFrom, validUntil);

        Name        = name.Trim();
        DiscountPct = discountPct;
        ValidFrom   = validFrom;
        ValidUntil  = validUntil;
    }

    // ── Validaciones privadas ─────────────────────────────────────────────────

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Promotion name cannot be empty.", nameof(name));

        if (name.Trim().Length > 100)
            throw new ArgumentException(
                "Promotion name cannot exceed 100 characters.", nameof(name));
    }

    private static void ValidateDiscountPct(decimal discountPct)
    {
        if (discountPct < 0 || discountPct > 100)
            throw new ArgumentException(
                "DiscountPct must be between 0 and 100. [chk_promo_pct]",
                nameof(discountPct));
    }

    private static void ValidateDates(DateOnly validFrom, DateOnly validUntil)
    {
        if (validUntil < validFrom)
            throw new ArgumentException(
                "ValidUntil must be >= ValidFrom. [chk_promo_dates]",
                nameof(validUntil));
    }
}
