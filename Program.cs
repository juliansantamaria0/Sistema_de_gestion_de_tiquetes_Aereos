using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Infrastructure;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.ui;


using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Geography;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Fleet;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightOps;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Users;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Booking;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticketing;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Finance;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Loyalty;
using sistema_de_gestion_de_tiquetes_Aereos.src.shared.ui;


var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()          
    .Build();


var services = new ServiceCollection();

services
    .AddSharedInfrastructure(configuration)   
    .AddGeographyModule()
    .AddFleetModule()
    .AddFlightOpsModule()
    .AddUsersModule()
    .AddBookingModule()
    .AddTicketingModule()
    .AddFinanceModule()
    .AddLoyaltyModule();

var provider = services.BuildServiceProvider();

// ── Menú principal
var modules = provider.GetServices<IModuleUI>().ToList();

while (true)
{
    Console.WriteLine("\n╔══════════════════════════════════╗");
    Console.WriteLine("║  Sistema de Tiquetes Aéreos      ║");
    Console.WriteLine("╠══════════════════════════════════╣");
    foreach (var m in modules)
        Console.WriteLine($"║  [{m.Key,-4}] {m.Title,-26}║");
    Console.WriteLine("║  [q   ] Salir                    ║");
    Console.WriteLine("╚══════════════════════════════════╝");
    Console.Write("Selecciona: ");

    var input = Console.ReadLine()?.Trim().ToLowerInvariant();

    if (input is "q" or "quit" or "exit") break;

    var module = modules.FirstOrDefault(m => m.Key == input);
    if (module is not null)
        await module.RunAsync();
    else
        Console.WriteLine("Opción no válida. Intenta de nuevo.");
}

Console.WriteLine("Hasta luego.");