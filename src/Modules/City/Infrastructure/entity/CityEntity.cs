namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.City.Infrastructure.Entity;





public sealed class CityEntity
{
    public int      CityId    { get; set; }
    public string   Name      { get; set; } = string.Empty;
    public int      CountryId { get; set; }
    public DateTime CreatedAt { get; set; }
}
