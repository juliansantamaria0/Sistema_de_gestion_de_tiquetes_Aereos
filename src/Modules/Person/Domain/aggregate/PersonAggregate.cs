namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Domain.ValueObject;

/// <summary>
/// Entidad base: atributos de identidad de cualquier persona.
/// SQL: person.
///
/// 4NF: todos los atributos dependen de person_id como superclave.
/// UNIQUE: (document_type_id, document_number).
///
/// birth_date usa DateOnly (.NET 8) — columna DATE en MySQL.
/// Pomelo 8.x mapea DATE ↔ DateOnly de forma nativa.
///
/// document_type_id y document_number son inmutables tras la creación
/// (el documento de identidad no cambia).
/// Update(): modifica first_name, last_name, birth_date y gender_id.
/// </summary>
public sealed class PersonAggregate
{
    public PersonId  Id             { get; private set; }
    public int       DocumentTypeId { get; private set; }
    public string    DocumentNumber { get; private set; }
    public string    FirstName      { get; private set; }
    public string    LastName       { get; private set; }
    public DateOnly? BirthDate      { get; private set; }
    public int?      GenderId       { get; private set; }
    public DateTime  CreatedAt      { get; private set; }
    public DateTime? UpdatedAt      { get; private set; }

    private PersonAggregate()
    {
        Id             = null!;
        DocumentNumber = null!;
        FirstName      = null!;
        LastName       = null!;
    }

    public PersonAggregate(
        PersonId  id,
        int       documentTypeId,
        string    documentNumber,
        string    firstName,
        string    lastName,
        DateOnly? birthDate,
        int?      genderId,
        DateTime  createdAt,
        DateTime? updatedAt = null)
    {
        if (documentTypeId <= 0)
            throw new ArgumentException(
                "DocumentTypeId must be a positive integer.", nameof(documentTypeId));

        ValidateDocumentNumber(documentNumber);
        ValidateName(firstName, nameof(firstName));
        ValidateName(lastName,  nameof(lastName));
        ValidateGenderId(genderId);
        ValidateBirthDate(birthDate);

        Id             = id;
        DocumentTypeId = documentTypeId;
        DocumentNumber = documentNumber.Trim();
        FirstName      = firstName.Trim();
        LastName       = lastName.Trim();
        BirthDate      = birthDate;
        GenderId       = genderId;
        CreatedAt      = createdAt;
        UpdatedAt      = updatedAt;
    }

    /// <summary>
    /// Actualiza los datos personales mutables.
    /// DocumentTypeId y DocumentNumber son la clave de identidad — inmutables.
    /// </summary>
    public void Update(
        string    firstName,
        string    lastName,
        DateOnly? birthDate,
        int?      genderId)
    {
        ValidateName(firstName, nameof(firstName));
        ValidateName(lastName,  nameof(lastName));
        ValidateGenderId(genderId);
        ValidateBirthDate(birthDate);

        FirstName = firstName.Trim();
        LastName  = lastName.Trim();
        BirthDate = birthDate;
        GenderId  = genderId;
        UpdatedAt = DateTime.UtcNow;
    }

    // ── Validaciones privadas ─────────────────────────────────────────────────

    private static void ValidateDocumentNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("DocumentNumber cannot be empty.", nameof(number));

        if (number.Trim().Length > 30)
            throw new ArgumentException(
                "DocumentNumber cannot exceed 30 characters.", nameof(number));
    }

    private static void ValidateName(string name, string paramName)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"{paramName} cannot be empty.", paramName);

        if (name.Trim().Length > 100)
            throw new ArgumentException(
                $"{paramName} cannot exceed 100 characters.", paramName);
    }

    private static void ValidateGenderId(int? genderId)
    {
        if (genderId.HasValue && genderId.Value <= 0)
            throw new ArgumentException(
                "GenderId must be a positive integer when provided.", nameof(genderId));
    }

    private static void ValidateBirthDate(DateOnly? birthDate)
    {
        if (birthDate.HasValue && birthDate.Value > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentException(
                "BirthDate cannot be in the future.", nameof(birthDate));
    }
}
