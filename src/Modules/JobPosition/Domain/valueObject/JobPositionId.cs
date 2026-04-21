namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Domain.ValueObject;

public readonly record struct JobPositionId(int Value)
{
    public static JobPositionId New(int value)
    {
        if (value <= 0)
            throw new ArgumentException("JobPositionId must be a positive integer.", nameof(value));
        return new JobPositionId(value);
    }
    public override string ToString() => Value.ToString();
}
