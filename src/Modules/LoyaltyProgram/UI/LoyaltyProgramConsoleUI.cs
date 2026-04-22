namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class LoyaltyProgramConsoleUI : ReflectiveModuleUI<ILoyaltyProgramService>
{
    public LoyaltyProgramConsoleUI(ILoyaltyProgramService service, IServiceProvider serviceProvider)
        : base("loyalty_program", "Loyalty Program Management", service, serviceProvider)
    {
    }
}
