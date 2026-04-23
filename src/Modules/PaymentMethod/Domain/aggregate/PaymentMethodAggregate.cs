namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Domain.Aggregate;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Domain.ValueObject;






public sealed class PaymentMethodAggregate
{
    public PaymentMethodId Id   { get; private set; }
    public string          Name { get; private set; }

    private PaymentMethodAggregate()
    {
        Id   = null!;
        Name = null!;
    }

    public PaymentMethodAggregate(PaymentMethodId id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("PaymentMethod name cannot be empty.", nameof(name));

        if (name.Trim().Length > 50)
            throw new ArgumentException(
                "PaymentMethod name cannot exceed 50 characters.", nameof(name));

        Id   = id;
        Name = name.Trim().ToUpperInvariant();
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("PaymentMethod name cannot be empty.", nameof(newName));

        if (newName.Trim().Length > 50)
            throw new ArgumentException(
                "PaymentMethod name cannot exceed 50 characters.", nameof(newName));

        Name = newName.Trim().ToUpperInvariant();
    }
}
