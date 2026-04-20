namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Terminal.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

/// <summary>Interfaz de consola para el módulo Terminal.</summary>
public sealed class TerminalConsoleUI : IModuleUI
{
    private readonly ITerminalService _service;

    public string Key   => "terminal";
    public string Title => "Terminal Management";

    public TerminalConsoleUI(ITerminalService service) => _service = service;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        bool running = true;
        while (running && !cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("\n========== TERMINAL MANAGEMENT ==========");
            Console.WriteLine("1. List all terminals");
            Console.WriteLine("2. Get terminal by ID");
            Console.WriteLine("3. Create terminal");
            Console.WriteLine("4. Update terminal");
            Console.WriteLine("5. Delete terminal");
            Console.WriteLine("0. Back to main menu");
            Console.Write("Select an option: ");

            var option = Console.ReadLine()?.Trim();
            try
            {
                switch (option)
                {
                    case "1": await ListAllAsync(cancellationToken);  break;
                    case "2": await GetByIdAsync(cancellationToken);  break;
                    case "3": await CreateAsync(cancellationToken);   break;
                    case "4": await UpdateAsync(cancellationToken);   break;
                    case "5": await DeleteAsync(cancellationToken);   break;
                    case "0": running = false;                        break;
                    default:  Console.WriteLine("Invalid option.");   break;
                }
            }
            catch (Exception ex) { Console.WriteLine($"[ERROR] {ex.Message}"); }
        }
    }

    private async Task ListAllAsync(CancellationToken ct)
    {
        var list = await _service.GetAllAsync(ct);
        if (!list.Any()) { Console.WriteLine("No terminals found."); return; }
        Console.WriteLine($"\n{"ID",-6} {"AirportId",-10} {"Name",-25} {"International",-15} {"CreatedAt",-22}");
        Console.WriteLine(new string('-', 80));
        foreach (var t in list)
            Console.WriteLine($"{t.TerminalId,-6} {t.AirportId,-10} {t.Name,-25} {t.IsInternational,-15} {t.CreatedAt:yyyy-MM-dd HH:mm}");
    }

    private async Task GetByIdAsync(CancellationToken ct)
    {
        Console.Write("Enter Terminal ID: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Invalid ID."); return; }
        var t = await _service.GetByIdAsync(id, ct);
        if (t is null) { Console.WriteLine("Terminal not found."); return; }
        Console.WriteLine($"\nID: {t.TerminalId} | AirportId: {t.AirportId} | Name: {t.Name} | International: {t.IsInternational}");
    }

    private async Task CreateAsync(CancellationToken ct)
    {
        Console.Write("Airport ID: ");
        if (!int.TryParse(Console.ReadLine(), out var airportId)) { Console.WriteLine("Invalid airport ID."); return; }
        Console.Write("Terminal name: ");
        var name = Console.ReadLine()?.Trim() ?? string.Empty;
        Console.Write("Is international? (y/n): ");
        var isInt = Console.ReadLine()?.Trim().ToLower() == "y";

        var created = await _service.CreateAsync(new CreateTerminalRequest(airportId, name, isInt), ct);
        Console.WriteLine($"Terminal created with ID: {created.TerminalId}");
    }

    private async Task UpdateAsync(CancellationToken ct)
    {
        Console.Write("Terminal ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Invalid ID."); return; }
        Console.Write("New Airport ID: ");
        if (!int.TryParse(Console.ReadLine(), out var airportId)) { Console.WriteLine("Invalid airport ID."); return; }
        Console.Write("New name: ");
        var name = Console.ReadLine()?.Trim() ?? string.Empty;
        Console.Write("Is international? (y/n): ");
        var isInt = Console.ReadLine()?.Trim().ToLower() == "y";

        var updated = await _service.UpdateAsync(id, new UpdateTerminalRequest(airportId, name, isInt), ct);
        Console.WriteLine($"Terminal updated: {updated.Name}");
    }

    private async Task DeleteAsync(CancellationToken ct)
    {
        Console.Write("Terminal ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Invalid ID."); return; }
        Console.Write($"Confirm delete terminal {id}? (y/n): ");
        if (Console.ReadLine()?.Trim().ToLower() != "y") { Console.WriteLine("Cancelled."); return; }
        await _service.DeleteAsync(id, ct);
        Console.WriteLine("Terminal deleted successfully.");
    }
}
