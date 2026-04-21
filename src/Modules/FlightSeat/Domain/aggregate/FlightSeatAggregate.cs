namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Domain.ValueObject;

/// <summary>
/// Estado dinámico de un asiento concreto en un vuelo programado.
/// SQL: flight_seat.
///
/// [IR-1] seat_map_id FK garantiza que el asiento exista en el mapa
/// estático del tipo de aeronave asignado al vuelo.
/// cabin_class_id fue eliminado del DDL — se obtiene vía seat_map.
///
/// UNIQUE: (scheduled_flight_id, seat_map_id).
/// La única operación de negocio es cambiar el estado del asiento
/// (AVAILABLE → OCCUPIED → BLOCKED) a través de ChangeStatus().
/// scheduled_flight_id y seat_map_id forman la clave de negocio — inmutables.
/// </summary>
public sealed class FlightSeatAggregate
{
    public FlightSeatId Id                { get; private set; }
    public int          ScheduledFlightId { get; private set; }
    public int          SeatMapId         { get; private set; }
    public int          SeatStatusId      { get; private set; }
    public DateTime     CreatedAt         { get; private set; }
    public DateTime?    UpdatedAt         { get; private set; }

    private FlightSeatAggregate()
    {
        Id = null!;
    }

    public FlightSeatAggregate(
        FlightSeatId id,
        int          scheduledFlightId,
        int          seatMapId,
        int          seatStatusId,
        DateTime     createdAt,
        DateTime?    updatedAt = null)
    {
        if (scheduledFlightId <= 0)
            throw new ArgumentException(
                "ScheduledFlightId must be a positive integer.", nameof(scheduledFlightId));

        if (seatMapId <= 0)
            throw new ArgumentException(
                "SeatMapId must be a positive integer.", nameof(seatMapId));

        if (seatStatusId <= 0)
            throw new ArgumentException(
                "SeatStatusId must be a positive integer.", nameof(seatStatusId));

        Id                = id;
        ScheduledFlightId = scheduledFlightId;
        SeatMapId         = seatMapId;
        SeatStatusId      = seatStatusId;
        CreatedAt         = createdAt;
        UpdatedAt         = updatedAt;
    }

    /// <summary>
    /// Cambia el estado del asiento en este vuelo.
    /// Es la única operación de negocio válida sobre flight_seat.
    /// Ejemplo: AVAILABLE → OCCUPIED (al reservar), OCCUPIED → AVAILABLE (al cancelar).
    /// </summary>
    public void ChangeStatus(int seatStatusId)
    {
        if (seatStatusId <= 0)
            throw new ArgumentException(
                "SeatStatusId must be a positive integer.", nameof(seatStatusId));

        SeatStatusId = seatStatusId;
        UpdatedAt    = DateTime.UtcNow;
    }
}
