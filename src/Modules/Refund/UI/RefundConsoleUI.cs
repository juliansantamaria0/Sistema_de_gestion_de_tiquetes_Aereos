namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class RefundConsoleUI : ReflectiveModuleUI<IRefundService>
{
    public RefundConsoleUI(IRefundService service, IServiceProvider serviceProvider)
        : base("refund", "Refund Management", service, serviceProvider)
    {
    }
}
