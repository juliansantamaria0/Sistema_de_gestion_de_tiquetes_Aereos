namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Domain.ValueObject;






public sealed class DocumentTypeAggregate
{
    public DocumentTypeId Id   { get; private set; }
    public string         Name { get; private set; } = string.Empty;
    public string         Code { get; private set; } = string.Empty;

    private DocumentTypeAggregate() { }

    public static DocumentTypeAggregate Create(string name, string code)
    {
        ValidateName(name);
        ValidateCode(code);
        return new DocumentTypeAggregate
        {
            Name = name.Trim(),
            Code = code.Trim().ToUpperInvariant()
        };
    }

    public static DocumentTypeAggregate Reconstitute(int id, string name, string code) =>
        new() { Id = DocumentTypeId.New(id), Name = name, Code = code };

    public void Update(string name, string code)
    {
        ValidateName(name);
        ValidateCode(code);
        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Document type name cannot be empty.", nameof(name));
        if (name.Length > 50)
            throw new ArgumentException("Document type name cannot exceed 50 characters.", nameof(name));
    }

    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Document type code cannot be empty.", nameof(code));
        if (code.Length > 10)
            throw new ArgumentException("Document type code cannot exceed 10 characters.", nameof(code));
    }
}
