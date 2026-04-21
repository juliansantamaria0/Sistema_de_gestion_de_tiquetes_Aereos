namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Domain.ValueObject;

/// <summary>
/// Catálogo de estados de reembolso.
/// Valores esperados: PENDING, APPROVED, REJECTED, PROCESSED.
/// Nombre normalizado a mayúsculas para consistencia con el catálogo SQL.
/// </summary>
public sealed class RefundStatusAggregate
{
    public RefundStatusId Id   { get; private set; }
    public string         Name { get; private set; }

    private RefundStatusAggregate()
    {
        Id   = null!;
        Name = null!;
    }

    public RefundStatusAggregate(RefundStatusId id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("RefundStatus name cannot be empty.", nameof(name));

        if (name.Trim().Length > 50)
            throw new ArgumentException(
                "RefundStatus name cannot exceed 50 characters.", nameof(name));

        Id   = id;
        Name = name.Trim().ToUpperInvariant();
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("RefundStatus name cannot be empty.", nameof(newName));

        if (newName.Trim().Length > 50)
            throw new ArgumentException(
                "RefundStatus name cannot exceed 50 characters.", nameof(newName));

        Name = newName.Trim().ToUpperInvariant();
    }
}
