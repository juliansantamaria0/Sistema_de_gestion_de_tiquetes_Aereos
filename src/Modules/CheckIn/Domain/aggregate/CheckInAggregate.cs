namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckIn.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CheckIn.Domain.ValueObject;












public sealed class CheckInAggregate
{
    public CheckInId Id               { get; private set; }
    public int       TicketId         { get; private set; }
    public DateTime  CheckInTime      { get; private set; }
    public int       CheckInStatusId  { get; private set; }
    public string?   CounterNumber    { get; private set; }

    private CheckInAggregate()
    {
        Id = null!;
    }

    public CheckInAggregate(
        CheckInId id,
        int       ticketId,
        DateTime  checkInTime,
        int       checkInStatusId,
        string?   counterNumber = null)
    {
        if (ticketId <= 0)
            throw new ArgumentException(
                "TicketId must be a positive integer.", nameof(ticketId));

        if (checkInStatusId <= 0)
            throw new ArgumentException(
                "CheckInStatusId must be a positive integer.", nameof(checkInStatusId));

        ValidateCounterNumber(counterNumber);

        Id              = id;
        TicketId        = ticketId;
        CheckInTime     = checkInTime;
        CheckInStatusId = checkInStatusId;
        CounterNumber   = counterNumber?.Trim();
    }

    
    
    
    
    public void ChangeStatus(int checkInStatusId, string? counterNumber = null)
    {
        if (checkInStatusId <= 0)
            throw new ArgumentException(
                "CheckInStatusId must be a positive integer.", nameof(checkInStatusId));

        ValidateCounterNumber(counterNumber);

        CheckInStatusId = checkInStatusId;
        CounterNumber   = counterNumber?.Trim() ?? CounterNumber;
    }

    

    private static void ValidateCounterNumber(string? counterNumber)
    {
        if (counterNumber is not null && counterNumber.Trim().Length > 20)
            throw new ArgumentException(
                "CounterNumber cannot exceed 20 characters.", nameof(counterNumber));
    }
}
