namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Domain.ValueObject;

/// <summary>
/// Precio por combinación (vuelo + clase de cabina + tipo de tarifa).
/// SQL: flight_cabin_price.
///
/// 4NF ANÁLISIS:
///   MVD1: scheduled_flight_id →→ cabin_class_id (un vuelo ofrece varias clases)
///   MVD2: scheduled_flight_id →→ fare_type_id   (un vuelo ofrece varias tarifas)
///   ¿Son independientes? SÍ → violaría 4NF si se almacenaran por separado.
///   SOLUCIÓN: PK incluye las tres FKs → cada precio es para la combinación
///   específica (vuelo, clase, tarifa) → NO viola 4NF.
///
/// UNIQUE: (scheduled_flight_id, cabin_class_id, fare_type_id).
/// CHECK:  price >= 0 — espejado en dominio.
///
/// La terna (scheduled_flight_id, cabin_class_id, fare_type_id) es la
/// clave de negocio — inmutable.
/// UpdatePrice(): única mutación válida.
/// </summary>
public sealed class FlightCabinPriceAggregate
{
    public FlightCabinPriceId Id                { get; private set; }
    public int                ScheduledFlightId { get; private set; }
    public int                CabinClassId      { get; private set; }
    public int                FareTypeId        { get; private set; }
    public decimal            Price             { get; private set; }

    private FlightCabinPriceAggregate()
    {
        Id = null!;
    }

    public FlightCabinPriceAggregate(
        FlightCabinPriceId id,
        int                scheduledFlightId,
        int                cabinClassId,
        int                fareTypeId,
        decimal            price)
    {
        if (scheduledFlightId <= 0)
            throw new ArgumentException(
                "ScheduledFlightId must be a positive integer.", nameof(scheduledFlightId));

        if (cabinClassId <= 0)
            throw new ArgumentException(
                "CabinClassId must be a positive integer.", nameof(cabinClassId));

        if (fareTypeId <= 0)
            throw new ArgumentException(
                "FareTypeId must be a positive integer.", nameof(fareTypeId));

        ValidatePrice(price);

        Id                = id;
        ScheduledFlightId = scheduledFlightId;
        CabinClassId      = cabinClassId;
        FareTypeId        = fareTypeId;
        Price             = price;
    }

    /// <summary>
    /// Actualiza el precio de esta combinación (vuelo + clase + tarifa).
    /// La terna de FKs es la clave de negocio — inmutable.
    /// </summary>
    public void UpdatePrice(decimal newPrice)
    {
        ValidatePrice(newPrice);
        Price = newPrice;
    }

    // ── Validaciones privadas ─────────────────────────────────────────────────

    private static void ValidatePrice(decimal price)
    {
        if (price < 0)
            throw new ArgumentException(
                "Price must be >= 0. [chk_fcp_price]", nameof(price));
    }
}
