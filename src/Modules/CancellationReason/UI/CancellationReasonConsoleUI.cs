namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.CancellationReason.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.CancellationReason.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class CancellationReasonConsoleUI : ReflectiveModuleUI<ICancellationReasonService>
{
    public CancellationReasonConsoleUI(ICancellationReasonService service, IServiceProvider serviceProvider)
        : base("cancellation_reason", "Cancellation Reason Management", service, serviceProvider)
    {
    }
}
