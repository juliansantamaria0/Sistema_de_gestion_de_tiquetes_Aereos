namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DelayReason.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DelayReason.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class DelayReasonConsoleUI : ReflectiveModuleUI<IDelayReasonService>
{
    public DelayReasonConsoleUI(IDelayReasonService service, IServiceProvider serviceProvider)
        : base("delay_reason", "Delay Reason Management", service, serviceProvider)
    {
    }
}
