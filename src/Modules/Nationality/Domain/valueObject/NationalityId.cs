namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Domain.ValueObject;

public readonly record struct NationalityId(int Value)
{
    public static NationalityId New(int value)
    {
        if (value <= 0)
            throw new ArgumentException("NationalityId must be a positive integer.", nameof(value));
        return new NationalityId(value);
    }
    public override string ToString() => Value.ToString();
}
