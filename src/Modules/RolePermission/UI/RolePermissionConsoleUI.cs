namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RolePermission.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class RolePermissionConsoleUI : IModuleUI
{
    private readonly IRolePermissionService _service;

    public string Key   => "role_permission";
    public string Title => "Role-Permission Assignment";

    public RolePermissionConsoleUI(IRolePermissionService service) => _service = service;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        bool running = true;
        while (running && !cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("\n========== ROLE-PERMISSION MANAGEMENT ==========");
            Console.WriteLine("1. List all assignments");
            Console.WriteLine("2. Get assignment by ID");
            Console.WriteLine("3. List permissions of a role");
            Console.WriteLine("4. Assign permission to role");
            Console.WriteLine("5. Revoke permission (delete)");
            Console.WriteLine("0. Back");
            Console.Write("Select: ");
            var opt = Console.ReadLine()?.Trim();
            try
            {
                switch (opt)
                {
                    case "1": await ListAllAsync(cancellationToken);        break;
                    case "2": await GetByIdAsync(cancellationToken);        break;
                    case "3": await ListByRoleAsync(cancellationToken);     break;
                    case "4": await AssignAsync(cancellationToken);         break;
                    case "5": await RevokeAsync(cancellationToken);         break;
                    case "0": running = false;                              break;
                    default:  Console.WriteLine("Invalid option.");         break;
                }
            }
            catch (Exception ex) { Console.WriteLine($"[ERROR] {ex.Message}"); }
        }
    }

    private async Task ListAllAsync(CancellationToken ct)
    {
        var list = await _service.GetAllAsync(ct);
        if (!list.Any()) { Console.WriteLine("No assignments found."); return; }
        Console.WriteLine($"\n{"ID",-6} {"RoleId",-8} {"PermissionId",-14}");
        Console.WriteLine(new string('-', 30));
        foreach (var rp in list)
            Console.WriteLine($"{rp.RolePermissionId,-6} {rp.RoleId,-8} {rp.PermissionId,-14}");
    }

    private async Task GetByIdAsync(CancellationToken ct)
    {
        Console.Write("Assignment ID: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Invalid ID."); return; }
        var rp = await _service.GetByIdAsync(id, ct);
        if (rp is null) { Console.WriteLine("Not found."); return; }
        Console.WriteLine($"ID: {rp.RolePermissionId} | RoleId: {rp.RoleId} | PermissionId: {rp.PermissionId}");
    }

    private async Task ListByRoleAsync(CancellationToken ct)
    {
        Console.Write("Role ID: ");
        if (!int.TryParse(Console.ReadLine(), out var roleId)) { Console.WriteLine("Invalid ID."); return; }
        var list = await _service.GetByRoleIdAsync(roleId, ct);
        if (!list.Any()) { Console.WriteLine("No permissions assigned to this role."); return; }
        Console.WriteLine($"Permissions for role {roleId}:");
        foreach (var rp in list)
            Console.WriteLine($"  Assignment ID: {rp.RolePermissionId} | PermissionId: {rp.PermissionId}");
    }

    private async Task AssignAsync(CancellationToken ct)
    {
        Console.Write("Role ID: ");
        if (!int.TryParse(Console.ReadLine(), out var roleId)) { Console.WriteLine("Invalid ID."); return; }
        Console.Write("Permission ID: ");
        if (!int.TryParse(Console.ReadLine(), out var permId)) { Console.WriteLine("Invalid ID."); return; }
        var created = await _service.CreateAsync(new CreateRolePermissionRequest(roleId, permId), ct);
        Console.WriteLine($"Assigned: Role {roleId} ← Permission {permId} (ID: {created.RolePermissionId})");
    }

    private async Task RevokeAsync(CancellationToken ct)
    {
        Console.Write("Assignment ID to revoke: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Invalid ID."); return; }
        Console.Write($"Confirm revoke assignment {id}? (y/n): ");
        if (Console.ReadLine()?.Trim().ToLower() != "y") { Console.WriteLine("Cancelled."); return; }
        await _service.DeleteAsync(id, ct);
        Console.WriteLine("Assignment revoked.");
    }
}
