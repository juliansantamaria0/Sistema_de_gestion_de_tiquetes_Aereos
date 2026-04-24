namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Domain.ValueObject;

public readonly record struct DocumentTypeId(int Value)
{
    public static DocumentTypeId New(int value)
    {
        if (value < 0)
            throw new ArgumentException("DocumentTypeId must be zero or a positive integer.", nameof(value));
        return new DocumentTypeId(value);
    }
    public override string ToString() => Value.ToString();
}
