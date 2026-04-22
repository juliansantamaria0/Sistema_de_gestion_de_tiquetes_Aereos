namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class LoyaltyAccountConsoleUI : ReflectiveModuleUI<ILoyaltyAccountService>
{
    public LoyaltyAccountConsoleUI(ILoyaltyAccountService service, IServiceProvider serviceProvider)
        : base("loyalty_account", "Loyalty Account Management", service, serviceProvider)
    {
    }
}
