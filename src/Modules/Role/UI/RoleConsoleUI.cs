namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Role.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class RoleConsoleUI : IModuleUI
{
    private readonly IRoleService _service;

    public string Key   => "role";
    public string Title => "Role Management";

    public RoleConsoleUI(IRoleService service) => _service = service;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        bool running = true;
        while (running && !cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("\n========== ROLE MANAGEMENT ==========");
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
        if (!list.Any()) { Console.WriteLine("No roles found."); return; }
        Console.WriteLine($"\n{"ID",-6} {"Name",-30} {"Active",-8}");
        Console.WriteLine(new string('-', 46));
        foreach (var r in list)
            Console.WriteLine($"{r.RoleId,-6} {r.Name,-30} {r.IsActive,-8}");
    }

    private async Task GetByIdAsync(CancellationToken ct)
    {
        Console.Write("Role ID: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Invalid ID."); return; }
        var r = await _service.GetByIdAsync(id, ct);
        if (r is null) { Console.WriteLine("Not found."); return; }
        Console.WriteLine($"ID: {r.RoleId} | Name: {r.Name} | Active: {r.IsActive}");
    }

    private async Task CreateAsync(CancellationToken ct)
    {
        Console.Write("Name (e.g. ADMIN): ");
        var name = Console.ReadLine()?.Trim() ?? string.Empty;
        Console.Write("Is active? (y/n): ");
        var active = Console.ReadLine()?.Trim().ToLower() == "y";
        var created = await _service.CreateAsync(new CreateRoleRequest(name, active), ct);
        Console.WriteLine($"Created with ID: {created.RoleId}");
    }

    private async Task UpdateAsync(CancellationToken ct)
    {
        Console.Write("ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Invalid ID."); return; }
        Console.Write("New Name: ");
        var name = Console.ReadLine()?.Trim() ?? string.Empty;
        Console.Write("Is active? (y/n): ");
        var active = Console.ReadLine()?.Trim().ToLower() == "y";
        var updated = await _service.UpdateAsync(id, new UpdateRoleRequest(name, active), ct);
        Console.WriteLine($"Updated: {updated.Name}");
    }

    private async Task DeleteAsync(CancellationToken ct)
    {
        Console.Write("ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Invalid ID."); return; }
        Console.Write($"Confirm delete role {id}? (y/n): ");
        if (Console.ReadLine()?.Trim().ToLower() != "y") { Console.WriteLine("Cancelled."); return; }
        await _service.DeleteAsync(id, ct);
        Console.WriteLine("Deleted.");
    }
}
