namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Infrastructure.Entity;

public sealed class NationalityEntity
{
    public int    NationalityId { get; set; }
    public int    CountryId     { get; set; }
    public string Demonym       { get; set; } = string.Empty;
}
