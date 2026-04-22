namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class PaymentMethodConsoleUI : ReflectiveModuleUI<IPaymentMethodService>
{
    public PaymentMethodConsoleUI(IPaymentMethodService service, IServiceProvider serviceProvider)
        : base("payment_method", "Payment Method Management", service, serviceProvider)
    {
    }
}
