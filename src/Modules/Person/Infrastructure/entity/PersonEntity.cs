namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Infrastructure.Entity;

public sealed class PersonEntity
{
    public int       Id             { get; set; }
    public int       DocumentTypeId { get; set; }
    public string    DocumentNumber { get; set; } = null!;
    public string    FirstName      { get; set; } = null!;
    public string    LastName       { get; set; } = null!;
    public DateOnly? BirthDate      { get; set; }
    public int?      GenderId       { get; set; }
    public DateTime  CreatedAt      { get; set; }
    public DateTime? UpdatedAt      { get; set; }
}
