namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatStatus.Domain.ValueObject;

/// <summary>
/// Catálogo de estados de un asiento en un vuelo.
/// Valores esperados en producción: AVAILABLE, OCCUPIED, BLOCKED.
/// El nombre se normaliza a mayúsculas para consistencia con el catálogo SQL.
/// </summary>
public sealed class SeatStatusAggregate
{
    public SeatStatusId Id   { get; private set; }
    public string       Name { get; private set; }

    private SeatStatusAggregate()
    {
        Id   = null!;
        Name = null!;
    }

    public SeatStatusAggregate(SeatStatusId id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("SeatStatus name cannot be empty.", nameof(name));

        if (name.Trim().Length > 50)
            throw new ArgumentException("SeatStatus name cannot exceed 50 characters.", nameof(name));

        Id   = id;
        Name = name.Trim().ToUpperInvariant();
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("SeatStatus name cannot be empty.", nameof(newName));

        if (newName.Trim().Length > 50)
            throw new ArgumentException("SeatStatus name cannot exceed 50 characters.", nameof(newName));

        Name = newName.Trim().ToUpperInvariant();
    }
}
