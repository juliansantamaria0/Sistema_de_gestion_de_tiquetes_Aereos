namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Infrastructure.Entity;

public sealed class PassengerEntity
{
    public int       Id                  { get; set; }
    public int       PersonId            { get; set; }
    public string?   FrequentFlyerNumber { get; set; }
    public int?      NationalityId       { get; set; }
    public DateTime  CreatedAt           { get; set; }
    public DateTime? UpdatedAt           { get; set; }
}
