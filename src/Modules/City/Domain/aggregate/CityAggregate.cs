namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.City.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.City.Domain.ValueObject;





public sealed class CityAggregate
{
    public CityId Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int CountryId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private CityAggregate() { }

    
    public static CityAggregate Create(string name, int countryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("City name cannot be empty.", nameof(name));
        if (name.Length > 100)
            throw new ArgumentException("City name cannot exceed 100 characters.", nameof(name));
        if (countryId <= 0)
            throw new ArgumentException("CountryId must be a positive integer.", nameof(countryId));

        return new CityAggregate
        {
            Name      = name.Trim(),
            CountryId = countryId,
            CreatedAt = DateTime.UtcNow
        };
    }

    
    public static CityAggregate Reconstitute(int id, string name, int countryId, DateTime createdAt)
    {
        return new CityAggregate
        {
            Id        = CityId.New(id),
            Name      = name,
            CountryId = countryId,
            CreatedAt = createdAt
        };
    }

    
    public void Update(string name, int countryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("City name cannot be empty.", nameof(name));
        if (name.Length > 100)
            throw new ArgumentException("City name cannot exceed 100 characters.", nameof(name));
        if (countryId <= 0)
            throw new ArgumentException("CountryId must be a positive integer.", nameof(countryId));

        Name      = name.Trim();
        CountryId = countryId;
    }
}
