namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Domain.ValueObject;










public sealed class RouteScheduleAggregate
{
    public RouteScheduleId Id            { get; private set; }
    public int             BaseFlightId  { get; private set; }
    public byte            DayOfWeek     { get; private set; }
    public TimeOnly        DepartureTime { get; private set; }

    private RouteScheduleAggregate()
    {
        Id = null!;
    }

    public RouteScheduleAggregate(
        RouteScheduleId id,
        int             baseFlightId,
        byte            dayOfWeek,
        TimeOnly        departureTime)
    {
        if (baseFlightId <= 0)
            throw new ArgumentException("BaseFlightId must be a positive integer.", nameof(baseFlightId));

        if (dayOfWeek < 1 || dayOfWeek > 7)
            throw new ArgumentOutOfRangeException(
                nameof(dayOfWeek), dayOfWeek,
                "DayOfWeek must be between 1 (Monday) and 7 (Sunday) — ISO 8601.");

        Id            = id;
        BaseFlightId  = baseFlightId;
        DayOfWeek     = dayOfWeek;
        DepartureTime = departureTime;
    }

    
    
    
    
    
    public void Update(byte dayOfWeek, TimeOnly departureTime)
    {
        if (dayOfWeek < 1 || dayOfWeek > 7)
            throw new ArgumentOutOfRangeException(
                nameof(dayOfWeek), dayOfWeek,
                "DayOfWeek must be between 1 (Monday) and 7 (Sunday) — ISO 8601.");

        DayOfWeek     = dayOfWeek;
        DepartureTime = departureTime;
    }

    
    public string DayOfWeekName => DayOfWeek switch
    {
        1 => "Monday",
        2 => "Tuesday",
        3 => "Wednesday",
        4 => "Thursday",
        5 => "Friday",
        6 => "Saturday",
        7 => "Sunday",
        _ => "Unknown"
    };
}
