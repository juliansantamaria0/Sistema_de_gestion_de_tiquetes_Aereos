namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gate.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

/// <summary>Interfaz de consola para el módulo Gate.</summary>
public sealed class GateConsoleUI : IModuleUI
{
    private readonly IGateService _service;

    public string Key   => "gate";
    public string Title => "Gate Management";

    public GateConsoleUI(IGateService service) => _service = service;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        bool running = true;
        while (running && !cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("\n========== GATE MANAGEMENT ==========");
            Console.WriteLine("1. List all gates");
            Console.WriteLine("2. Get gate by ID");
            Console.WriteLine("3. Create gate");
            Console.WriteLine("4. Update gate");
            Console.WriteLine("5. Delete gate");
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
        if (!list.Any()) { Console.WriteLine("No gates found."); return; }
        Console.WriteLine($"\n{"ID",-6} {"TerminalId",-12} {"Code",-12} {"Active",-8}");
        Console.WriteLine(new string('-', 40));
        foreach (var g in list)
            Console.WriteLine($"{g.GateId,-6} {g.TerminalId,-12} {g.Code,-12} {g.IsActive,-8}");
    }

    private async Task GetByIdAsync(CancellationToken ct)
    {
        Console.Write("Enter Gate ID: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Invalid ID."); return; }
        var g = await _service.GetByIdAsync(id, ct);
        if (g is null) { Console.WriteLine("Gate not found."); return; }
        Console.WriteLine($"\nID: {g.GateId} | TerminalId: {g.TerminalId} | Code: {g.Code} | Active: {g.IsActive}");
    }

    private async Task CreateAsync(CancellationToken ct)
    {
        Console.Write("Terminal ID: ");
        if (!int.TryParse(Console.ReadLine(), out var terminalId)) { Console.WriteLine("Invalid terminal ID."); return; }
        Console.Write("Gate code: ");
        var code = Console.ReadLine()?.Trim() ?? string.Empty;
        Console.Write("Is active? (y/n): ");
        var isActive = Console.ReadLine()?.Trim().ToLower() == "y";

        var created = await _service.CreateAsync(new CreateGateRequest(terminalId, code, isActive), ct);
        Console.WriteLine($"Gate created with ID: {created.GateId}");
    }

    private async Task UpdateAsync(CancellationToken ct)
    {
        Console.Write("Gate ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Invalid ID."); return; }
        Console.Write("New Terminal ID: ");
        if (!int.TryParse(Console.ReadLine(), out var terminalId)) { Console.WriteLine("Invalid terminal ID."); return; }
        Console.Write("New code: ");
        var code = Console.ReadLine()?.Trim() ?? string.Empty;
        Console.Write("Is active? (y/n): ");
        var isActive = Console.ReadLine()?.Trim().ToLower() == "y";

        var updated = await _service.UpdateAsync(id, new UpdateGateRequest(terminalId, code, isActive), ct);
        Console.WriteLine($"Gate updated: {updated.Code}");
    }

    private async Task DeleteAsync(CancellationToken ct)
    {
        Console.Write("Gate ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Invalid ID."); return; }
        Console.Write($"Confirm delete gate {id}? (y/n): ");
        if (Console.ReadLine()?.Trim().ToLower() != "y") { Console.WriteLine("Cancelled."); return; }
        await _service.DeleteAsync(id, ct);
        Console.WriteLine("Gate deleted successfully.");
    }
}
