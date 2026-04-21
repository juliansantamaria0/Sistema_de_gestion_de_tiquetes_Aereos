namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatus.Domain.ValueObject;

/// <summary>
/// Catálogo de estados del ciclo de vida de una reserva.
/// Valores esperados en producción: PENDING, CONFIRMED, CANCELLED.
/// El nombre se normaliza a mayúsculas para consistencia con el catálogo SQL.
/// </summary>
public sealed class ReservationStatusAggregate
{
    public ReservationStatusId Id   { get; private set; }
    public string              Name { get; private set; }

    private ReservationStatusAggregate()
    {
        Id   = null!;
        Name = null!;
    }

    public ReservationStatusAggregate(ReservationStatusId id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("ReservationStatus name cannot be empty.", nameof(name));

        if (name.Trim().Length > 50)
            throw new ArgumentException("ReservationStatus name cannot exceed 50 characters.", nameof(name));

        Id   = id;
        Name = name.Trim().ToUpperInvariant();
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("ReservationStatus name cannot be empty.", nameof(newName));

        if (newName.Trim().Length > 50)
            throw new ArgumentException("ReservationStatus name cannot exceed 50 characters.", nameof(newName));

        Name = newName.Trim().ToUpperInvariant();
    }
}
