namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Domain.ValueObject;

/// <summary>
/// Asignación de un empleado a un vuelo concreto con su rol operativo.
/// SQL: flight_crew.
///
/// 4NF: (scheduled_flight_id, employee_id) → crew_role_id.
/// No hay dependencias multivaluadas independientes — no viola 4NF.
///
/// Invariante clave: UNIQUE (scheduled_flight_id, employee_id).
/// Un empleado sólo puede tener UN rol en un vuelo dado.
///
/// El rol se puede reasignar mediante ReassignRole().
/// No existe updated_at en el DDL; si el negocio lo requiere, se elimina
/// y re-crea la asignación.
/// </summary>
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

    /// <summary>
    /// Reasigna el rol operativo del empleado en este vuelo.
    /// Solo crew_role_id puede cambiar; scheduled_flight_id y employee_id
    /// forman la clave de negocio y no son modificables.
    /// </summary>
    public void ReassignRole(int crewRoleId)
    {
        if (crewRoleId <= 0)
            throw new ArgumentException(
                "CrewRoleId must be a positive integer.", nameof(crewRoleId));

        CrewRoleId = crewRoleId;
    }
}
