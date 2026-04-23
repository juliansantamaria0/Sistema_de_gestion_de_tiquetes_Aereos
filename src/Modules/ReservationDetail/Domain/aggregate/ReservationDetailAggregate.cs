namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationDetail.Domain.ValueObject;

















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

    
    
    
    
    public void ChangeFareType(int fareTypeId)
    {
        if (fareTypeId <= 0)
            throw new ArgumentException(
                "FareTypeId must be a positive integer.", nameof(fareTypeId));

        FareTypeId = fareTypeId;
        UpdatedAt  = DateTime.UtcNow;
    }
}
