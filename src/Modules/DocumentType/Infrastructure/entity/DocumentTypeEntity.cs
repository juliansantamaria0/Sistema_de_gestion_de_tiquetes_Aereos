namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Infrastructure.Entity;


public sealed class DocumentTypeEntity
{
    public int    DocumentTypeId { get; set; }
    public string Name          { get; set; } = string.Empty;
    public string Code          { get; set; } = string.Empty;
}
