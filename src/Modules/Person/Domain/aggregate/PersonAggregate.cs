namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Domain.ValueObject;















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
