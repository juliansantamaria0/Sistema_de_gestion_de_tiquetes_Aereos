namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Domain.ValueObject;

/// <summary>
/// Catálogo de tipos de contacto de emergencia.
/// Valores esperados: EMERGENCY, SECONDARY.
/// Nombre normalizado a mayúsculas para consistencia con el catálogo SQL.
/// </summary>
public sealed class ContactTypeAggregate
{
    public ContactTypeId Id   { get; private set; }
    public string        Name { get; private set; }

    private ContactTypeAggregate()
    {
        Id   = null!;
        Name = null!;
    }

    public ContactTypeAggregate(ContactTypeId id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("ContactType name cannot be empty.", nameof(name));

        if (name.Trim().Length > 50)
            throw new ArgumentException(
                "ContactType name cannot exceed 50 characters.", nameof(name));

        Id   = id;
        Name = name.Trim().ToUpperInvariant();
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("ContactType name cannot be empty.", nameof(newName));

        if (newName.Trim().Length > 50)
            throw new ArgumentException(
                "ContactType name cannot exceed 50 characters.", nameof(newName));

        Name = newName.Trim().ToUpperInvariant();
    }
}
