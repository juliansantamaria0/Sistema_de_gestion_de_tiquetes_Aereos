namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Domain.ValueObject;














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

    
    
    
    
    
    public void ChangeStatus(int seatStatusId)
    {
        if (seatStatusId <= 0)
            throw new ArgumentException(
                "SeatStatusId must be a positive integer.", nameof(seatStatusId));

        SeatStatusId = seatStatusId;
        UpdatedAt    = DateTime.UtcNow;
    }
}
