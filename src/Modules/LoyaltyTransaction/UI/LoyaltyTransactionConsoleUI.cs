namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class LoyaltyTransactionConsoleUI : ReflectiveModuleUI<ILoyaltyTransactionService>
{
    public LoyaltyTransactionConsoleUI(ILoyaltyTransactionService service, IServiceProvider serviceProvider)
        : base("loyalty_transaction", "Loyalty Transaction Management", service, serviceProvider)
    {
    }
}
