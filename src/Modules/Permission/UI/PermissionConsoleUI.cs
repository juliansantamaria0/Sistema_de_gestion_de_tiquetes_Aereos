namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Permission.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class PermissionConsoleUI : IModuleUI
{
    private readonly IPermissionService _service;

    public string Key   => "permission";
    public string Title => "Permission Management";

    public PermissionConsoleUI(IPermissionService service) => _service = service;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        bool running = true;
        while (running && !cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("\n========== PERMISSION MANAGEMENT ==========");
            Console.WriteLine("1. List all");
            Console.WriteLine("2. Get by ID");
            Console.WriteLine("3. Create");
            Console.WriteLine("4. Update");
            Console.WriteLine("5. Delete");
            Console.WriteLine("0. Back");
            Console.Write("Select: ");
            var opt = Console.ReadLine()?.Trim();
            try
            {
                switch (opt)
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
        if (!list.Any()) { Console.WriteLine("No permissions found."); return; }
        Console.WriteLine($"\n{"ID",-6} {"Name",-40} {"Description",-40}");
        Console.WriteLine(new string('-', 88));
        foreach (var p in list)
            Console.WriteLine($"{p.PermissionId,-6} {p.Name,-40} {p.Description ?? "-",-40}");
    }

    private async Task GetByIdAsync(CancellationToken ct)
    {
        Console.Write("Permission ID: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Invalid ID."); return; }
        var p = await _service.GetByIdAsync(id, ct);
        if (p is null) { Console.WriteLine("Not found."); return; }
        Console.WriteLine($"ID: {p.PermissionId} | Name: {p.Name} | Desc: {p.Description ?? "-"}");
    }

    private async Task CreateAsync(CancellationToken ct)
    {
        Console.Write("Name (e.g. flights.read): ");
        var name = Console.ReadLine()?.Trim() ?? string.Empty;
        Console.Write("Description (optional, Enter to skip): ");
        var desc = Console.ReadLine()?.Trim();
        var created = await _service.CreateAsync(
            new CreatePermissionRequest(name, string.IsNullOrWhiteSpace(desc) ? null : desc), ct);
        Console.WriteLine($"Created with ID: {created.PermissionId}");
    }

    private async Task UpdateAsync(CancellationToken ct)
    {
        Console.Write("ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Invalid ID."); return; }
        Console.Write("New Name: ");
        var name = Console.ReadLine()?.Trim() ?? string.Empty;
        Console.Write("New Description (optional): ");
        var desc = Console.ReadLine()?.Trim();
        var updated = await _service.UpdateAsync(id,
            new UpdatePermissionRequest(name, string.IsNullOrWhiteSpace(desc) ? null : desc), ct);
        Console.WriteLine($"Updated: {updated.Name}");
    }

    private async Task DeleteAsync(CancellationToken ct)
    {
        Console.Write("ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Invalid ID."); return; }
        Console.Write($"Confirm delete permission {id}? (y/n): ");
        if (Console.ReadLine()?.Trim().ToLower() != "y") { Console.WriteLine("Cancelled."); return; }
        await _service.DeleteAsync(id, ct);
        Console.WriteLine("Deleted.");
    }
}
