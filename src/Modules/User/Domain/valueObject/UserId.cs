namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Domain.ValueObject;

public readonly record struct UserId(int Value)
{
    public static UserId New(int value)
    {
        if (value < 0)
            throw new ArgumentException("UserId must be zero or a positive integer.", nameof(value));
        return new UserId(value);
    }
    public override string ToString() => Value.ToString();
}
