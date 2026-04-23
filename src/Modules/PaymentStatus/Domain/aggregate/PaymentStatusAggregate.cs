namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Domain.ValueObject;






public sealed class PaymentStatusAggregate
{
    public PaymentStatusId Id   { get; private set; }
    public string          Name { get; private set; }

    private PaymentStatusAggregate()
    {
        Id   = null!;
        Name = null!;
    }

    public PaymentStatusAggregate(PaymentStatusId id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("PaymentStatus name cannot be empty.", nameof(name));

        if (name.Trim().Length > 50)
            throw new ArgumentException(
                "PaymentStatus name cannot exceed 50 characters.", nameof(name));

        Id   = id;
        Name = name.Trim().ToUpperInvariant();
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("PaymentStatus name cannot be empty.", nameof(newName));

        if (newName.Trim().Length > 50)
            throw new ArgumentException(
                "PaymentStatus name cannot exceed 50 characters.", nameof(newName));

        Name = newName.Trim().ToUpperInvariant();
    }
}
