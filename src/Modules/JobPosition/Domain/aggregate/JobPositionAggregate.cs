namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Domain.ValueObject;

/// <summary>
/// Puesto laboral estructural del empleado en la aerolínea [TN-3].
/// Distinto de crew_role (rol operativo en un vuelo concreto).
/// Ejemplos: PILOT, CABIN_CREW, GROUND_STAFF, MAINTENANCE, ADMIN.
/// </summary>
public sealed class JobPositionAggregate
{
    public JobPositionId Id         { get; private set; }
    public string        Name       { get; private set; } = string.Empty;
    public string?       Department { get; private set; }

    private JobPositionAggregate() { }

    public static JobPositionAggregate Create(string name, string? department = null)
    {
        ValidateName(name);
        return new JobPositionAggregate
        {
            Name       = name.Trim(),
            Department = department?.Trim()
        };
    }

    public static JobPositionAggregate Reconstitute(int id, string name, string? department) =>
        new() { Id = JobPositionId.New(id), Name = name, Department = department };

    public void Update(string name, string? department)
    {
        ValidateName(name);
        Name       = name.Trim();
        Department = department?.Trim();
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Job position name cannot be empty.", nameof(name));
        if (name.Length > 80)
            throw new ArgumentException("Job position name cannot exceed 80 characters.", nameof(name));
    }
}
