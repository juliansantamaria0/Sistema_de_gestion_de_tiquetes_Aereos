namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Domain.ValueObject;

/// <summary>
/// Catálogo de estados operativos de un vuelo programado.
/// Valores esperados: SCHEDULED, ACTIVE, DELAYED, CANCELLED, COMPLETED.
/// Nombre normalizado a mayúsculas.
/// </summary>
public sealed class FlightStatusAggregate
{
    public FlightStatusId Id   { get; private set; }
    public string         Name { get; private set; }

    private FlightStatusAggregate()
    {
        Id   = null!;
        Name = null!;
    }

    public FlightStatusAggregate(FlightStatusId id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("FlightStatus name cannot be empty.", nameof(name));

        if (name.Trim().Length > 50)
            throw new ArgumentException(
                "FlightStatus name cannot exceed 50 characters.", nameof(name));

        Id   = id;
        Name = name.Trim().ToUpperInvariant();
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("FlightStatus name cannot be empty.", nameof(newName));

        if (newName.Trim().Length > 50)
            throw new ArgumentException(
                "FlightStatus name cannot exceed 50 characters.", nameof(newName));

        Name = newName.Trim().ToUpperInvariant();
    }
}
