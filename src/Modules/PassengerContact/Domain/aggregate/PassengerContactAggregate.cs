namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Domain.ValueObject;

/// <summary>
/// Contacto de emergencia de un pasajero. [NC-10] id renombrado.
/// SQL: passenger_contact.
///
/// 4NF: (passenger_id, contact_type_id) → full_name, phone, relationship.
///      Cada contacto tiene UN tipo — sin MVD independientes.
/// UNIQUE: (passenger_id, contact_type_id).
///
/// passenger_id y contact_type_id son la clave de negocio — inmutables.
/// Update(): modifica full_name, phone y relationship.
/// </summary>
public sealed class PassengerContactAggregate
{
    public PassengerContactId Id            { get; private set; }
    public int                PassengerId   { get; private set; }
    public int                ContactTypeId { get; private set; }
    public string             FullName      { get; private set; }
    public string             Phone         { get; private set; }
    public string?            Relationship  { get; private set; }

    private PassengerContactAggregate()
    {
        Id       = null!;
        FullName = null!;
        Phone    = null!;
    }

    public PassengerContactAggregate(
        PassengerContactId id,
        int                passengerId,
        int                contactTypeId,
        string             fullName,
        string             phone,
        string?            relationship = null)
    {
        if (passengerId <= 0)
            throw new ArgumentException(
                "PassengerId must be a positive integer.", nameof(passengerId));

        if (contactTypeId <= 0)
            throw new ArgumentException(
                "ContactTypeId must be a positive integer.", nameof(contactTypeId));

        ValidateFullName(fullName);
        ValidatePhone(phone);
        ValidateRelationship(relationship);

        Id            = id;
        PassengerId   = passengerId;
        ContactTypeId = contactTypeId;
        FullName      = fullName.Trim();
        Phone         = phone.Trim();
        Relationship  = relationship?.Trim();
    }

    /// <summary>
    /// Actualiza los datos de contacto.
    /// PassengerId y ContactTypeId son la clave de negocio — inmutables.
    /// </summary>
    public void Update(string fullName, string phone, string? relationship)
    {
        ValidateFullName(fullName);
        ValidatePhone(phone);
        ValidateRelationship(relationship);

        FullName     = fullName.Trim();
        Phone        = phone.Trim();
        Relationship = relationship?.Trim();
    }

    // ── Validaciones privadas ─────────────────────────────────────────────────

    private static void ValidateFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("FullName cannot be empty.", nameof(fullName));

        if (fullName.Trim().Length > 200)
            throw new ArgumentException(
                "FullName cannot exceed 200 characters.", nameof(fullName));
    }

    private static void ValidatePhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone cannot be empty.", nameof(phone));

        if (phone.Trim().Length > 30)
            throw new ArgumentException(
                "Phone cannot exceed 30 characters.", nameof(phone));
    }

    private static void ValidateRelationship(string? relationship)
    {
        if (relationship is not null && relationship.Trim().Length > 50)
            throw new ArgumentException(
                "Relationship cannot exceed 50 characters.", nameof(relationship));
    }
}
