namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Domain.ValueObject;






public sealed class NationalityAggregate
{
    public NationalityId Id        { get; private set; }
    public int           CountryId { get; private set; }
    public string        Demonym   { get; private set; } = string.Empty;

    private NationalityAggregate() { }

    public static NationalityAggregate Create(int countryId, string demonym)
    {
        Validate(countryId, demonym);
        return new NationalityAggregate
        {
            CountryId = countryId,
            Demonym   = demonym.Trim()
        };
    }

    public static NationalityAggregate Reconstitute(int id, int countryId, string demonym) =>
        new() { Id = NationalityId.New(id), CountryId = countryId, Demonym = demonym };

    public void Update(int countryId, string demonym)
    {
        Validate(countryId, demonym);
        CountryId = countryId;
        Demonym   = demonym.Trim();
    }

    private static void Validate(int countryId, string demonym)
    {
        if (countryId <= 0)
            throw new ArgumentException("CountryId must be positive.", nameof(countryId));
        if (string.IsNullOrWhiteSpace(demonym))
            throw new ArgumentException("Demonym cannot be empty.", nameof(demonym));
        if (demonym.Length > 80)
            throw new ArgumentException("Demonym cannot exceed 80 characters.", nameof(demonym));
    }
}
