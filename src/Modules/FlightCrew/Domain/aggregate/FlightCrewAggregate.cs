namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Domain.ValueObject;















public sealed class FlightCrewAggregate
{
    public FlightCrewId Id                { get; private set; }
    public int          ScheduledFlightId { get; private set; }
    public int          EmployeeId        { get; private set; }
    public int          CrewRoleId        { get; private set; }
    public DateTime     CreatedAt         { get; private set; }

    private FlightCrewAggregate()
    {
        Id = null!;
    }

    public FlightCrewAggregate(
        FlightCrewId id,
        int          scheduledFlightId,
        int          employeeId,
        int          crewRoleId,
        DateTime     createdAt)
    {
        if (scheduledFlightId <= 0)
            throw new ArgumentException(
                "ScheduledFlightId must be a positive integer.", nameof(scheduledFlightId));

        if (employeeId <= 0)
            throw new ArgumentException(
                "EmployeeId must be a positive integer.", nameof(employeeId));

        if (crewRoleId <= 0)
            throw new ArgumentException(
                "CrewRoleId must be a positive integer.", nameof(crewRoleId));

        Id                = id;
        ScheduledFlightId = scheduledFlightId;
        EmployeeId        = employeeId;
        CrewRoleId        = crewRoleId;
        CreatedAt         = createdAt;
    }

    
    
    
    
    
    public void ReassignRole(int crewRoleId)
    {
        if (crewRoleId <= 0)
            throw new ArgumentException(
                "CrewRoleId must be a positive integer.", nameof(crewRoleId));

        CrewRoleId = crewRoleId;
    }
}
