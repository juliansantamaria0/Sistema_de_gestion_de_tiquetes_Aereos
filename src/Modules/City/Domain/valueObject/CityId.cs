namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.City.Domain.ValueObject;




public readonly record struct CityId(int Value)
{
    public static CityId New(int value)
    {
        if (value < 0)
            throw new ArgumentException("CityId must be zero or a positive integer.", nameof(value));
        return new CityId(value);
    }

    public override string ToString() => Value.ToString();
}
