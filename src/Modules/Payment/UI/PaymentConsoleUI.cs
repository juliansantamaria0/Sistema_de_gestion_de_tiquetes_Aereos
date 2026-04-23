
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;


namespace Sistema_de_gestion_de_tiquetes_Aereos.src.Modules.Payment.UI;

public sealed class PaymentConsoleUI : ReflectiveModuleUI<IPaymentService>
{
    public PaymentConsoleUI(IPaymentService service, IServiceProvider serviceProvider)
        : base("Payment", "Pagos", service, serviceProvider)
    {
    }
}