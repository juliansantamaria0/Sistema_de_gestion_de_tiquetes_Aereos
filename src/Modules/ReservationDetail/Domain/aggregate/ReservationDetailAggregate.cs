namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.ValueObject;

/// <summary>
/// Una línea de reserva: un pasajero + un asiento + una tarifa dentro de una reserva.
/// SQL: reservation_detail.
///
/// 4NF: reservation_id →→ passenger_id y →→ flight_seat_id NO son independientes;
/// cada pasajero tiene UN asiento en esa reserva — no viola 4NF.
///
/// Invariantes clave:
///   - UNIQUE (reservation_id, passenger_id) — un pasajero solo una vez por reserva.
///   - UNIQUE (reservation_id, flight_seat_id) — un asiento solo a un pasajero.
///   - fare_amount fue eliminado — se obtiene vía JOIN con flight_cabin_price.
///
/// La única actualización válida es cambiar la tarifa (ChangeFareType).
/// El asiento y el pasajero son la clave de negocio y no pueden cambiarse.
/// El trigger RF-6 en la BD verifica que el asiento esté AVAILABLE antes de insertar.
/// </summary>
public sealed class ReservationDetailAggregate
{
    public ReservationDetailId Id                  { get; private set; }
    public int                 ReservationId       { get; private set; }
    public int                 PassengerId         { get; private set; }
    public int                 FlightSeatId        { get; private set; }
    public int                 FareTypeId          { get; private set; }
    public DateTime            CreatedAt           { get; private set; }
    public DateTime?           UpdatedAt           { get; private set; }

    private ReservationDetailAggregate()
    {
        Id = null!;
    }

    public ReservationDetailAggregate(
        ReservationDetailId id,
        int                 reservationId,
        int                 passengerId,
        int                 flightSeatId,
        int                 fareTypeId,
        DateTime            createdAt,
        DateTime?           updatedAt = null)
    {
        if (reservationId <= 0)
            throw new ArgumentException(
                "ReservationId must be a positive integer.", nameof(reservationId));

        if (passengerId <= 0)
            throw new ArgumentException(
                "PassengerId must be a positive integer.", nameof(passengerId));

        if (flightSeatId <= 0)
            throw new ArgumentException(
                "FlightSeatId must be a positive integer.", nameof(flightSeatId));

        if (fareTypeId <= 0)
            throw new ArgumentException(
                "FareTypeId must be a positive integer.", nameof(fareTypeId));

        Id            = id;
        ReservationId = reservationId;
        PassengerId   = passengerId;
        FlightSeatId  = flightSeatId;
        FareTypeId    = fareTypeId;
        CreatedAt     = createdAt;
        UpdatedAt     = updatedAt;
    }

    /// <summary>
    /// Cambia la tarifa seleccionada por el pasajero en esta línea de reserva.
    /// ReservationId, PassengerId y FlightSeatId son la clave de negocio — inmutables.
    /// </summary>
    public void ChangeFareType(int fareTypeId)
    {
        if (fareTypeId <= 0)
            throw new ArgumentException(
                "FareTypeId must be a positive integer.", nameof(fareTypeId));

        FareTypeId = fareTypeId;
        UpdatedAt  = DateTime.UtcNow;
    }
}
