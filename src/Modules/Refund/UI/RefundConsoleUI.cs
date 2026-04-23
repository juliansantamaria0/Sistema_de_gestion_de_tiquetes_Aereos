
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;


namespace Sistema_de_gestion_de_tiquetes_Aereos.src.Modules.Refund.UI;

public sealed class RefundConsoleUI : ReflectiveModuleUI<IRefundService>
{
    public RefundConsoleUI(IRefundService service, IServiceProvider serviceProvider)
        : base("Refund", "Reembolsos", service, serviceProvider)
    {
    }
}