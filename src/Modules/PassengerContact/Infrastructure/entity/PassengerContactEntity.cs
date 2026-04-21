namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Infrastructure.Entity;

public sealed class PassengerContactEntity
{
    public int     Id            { get; set; }
    public int     PassengerId   { get; set; }
    public int     ContactTypeId { get; set; }
    public string  FullName      { get; set; } = null!;
    public string  Phone         { get; set; } = null!;
    public string? Relationship  { get; set; }
}
