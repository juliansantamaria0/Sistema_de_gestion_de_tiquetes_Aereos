namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentStatus.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class PaymentStatusConsoleUI : ReflectiveModuleUI<IPaymentStatusService>
{
    public PaymentStatusConsoleUI(IPaymentStatusService service, IServiceProvider serviceProvider)
        : base("payment_status", "Payment Status Management", service, serviceProvider)
    {
    }
}
