namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RefundStatus.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class RefundStatusConsoleUI : ReflectiveModuleUI<IRefundStatusService>
{
    public RefundStatusConsoleUI(IRefundStatusService service, IServiceProvider serviceProvider)
        : base("refund_status", "Refund Status Management", service, serviceProvider)
    {
    }
}
