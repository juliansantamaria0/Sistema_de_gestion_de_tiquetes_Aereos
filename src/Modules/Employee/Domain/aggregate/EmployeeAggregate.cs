namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Domain.ValueObject;

/// <summary>
/// Empleado de la aerolínea — extiende Person con datos laborales.
/// hire_date usa DateOnly (.NET 8). job_position_id es nullable.
/// </summary>
public sealed class EmployeeAggregate
{
    public EmployeeId Id            { get; private set; }
    public int        PersonId      { get; private set; }
    public int        AirlineId     { get; private set; }
    public int?       JobPositionId { get; private set; }
    public DateOnly   HireDate      { get; private set; }
    public bool       IsActive      { get; private set; }
    public DateTime   CreatedAt     { get; private set; }
    public DateTime?  UpdatedAt     { get; private set; }

    private EmployeeAggregate() { }

    public static EmployeeAggregate Create(int personId, int airlineId, DateOnly hireDate,
        int? jobPositionId = null, bool isActive = true)
    {
        Validate(personId, airlineId, hireDate);
        return new EmployeeAggregate
        {
            PersonId      = personId,
            AirlineId     = airlineId,
            JobPositionId = jobPositionId,
            HireDate      = hireDate,
            IsActive      = isActive,
            CreatedAt     = DateTime.UtcNow
        };
    }

    public static EmployeeAggregate Reconstitute(int id, int personId, int airlineId,
        int? jobPositionId, DateOnly hireDate, bool isActive, DateTime createdAt, DateTime? updatedAt) =>
        new()
        {
            Id            = EmployeeId.New(id),
            PersonId      = personId,
            AirlineId     = airlineId,
            JobPositionId = jobPositionId,
            HireDate      = hireDate,
            IsActive      = isActive,
            CreatedAt     = createdAt,
            UpdatedAt     = updatedAt
        };

    public void Update(int airlineId, int? jobPositionId, DateOnly hireDate, bool isActive)
    {
        if (airlineId <= 0) throw new ArgumentException("AirlineId must be positive.", nameof(airlineId));
        if (hireDate > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentException("HireDate cannot be in the future.", nameof(hireDate));
        AirlineId     = airlineId;
        JobPositionId = jobPositionId;
        HireDate      = hireDate;
        IsActive      = isActive;
        UpdatedAt     = DateTime.UtcNow;
    }

    private static void Validate(int personId, int airlineId, DateOnly hireDate)
    {
        if (personId <= 0)  throw new ArgumentException("PersonId must be positive.", nameof(personId));
        if (airlineId <= 0) throw new ArgumentException("AirlineId must be positive.", nameof(airlineId));
        if (hireDate > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentException("HireDate cannot be in the future.", nameof(hireDate));
    }
}
