namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Domain.ValueObject;

public readonly record struct EmployeeId(int Value)
{
    public static EmployeeId New(int value)
    {
        if (value < 0)
            throw new ArgumentException("EmployeeId must be zero or a positive integer.", nameof(value));
        return new EmployeeId(value);
    }
    public override string ToString() => Value.ToString();
}
