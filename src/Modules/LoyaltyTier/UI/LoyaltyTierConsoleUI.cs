namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class LoyaltyTierConsoleUI : ReflectiveModuleUI<ILoyaltyTierService>
{
    public LoyaltyTierConsoleUI(ILoyaltyTierService service, IServiceProvider serviceProvider)
        : base("loyalty_tier", "Loyalty Tier Management", service, serviceProvider)
    {
    }
}
