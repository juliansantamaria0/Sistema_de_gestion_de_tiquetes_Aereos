namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.UI;

using System.Security.Cryptography;
using System.Text;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.User.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class UserConsoleUI : IModuleUI
{
    private readonly IUserService _service;

    public string Key   => "user";
    public string Title => "User Management";

    public UserConsoleUI(IUserService service) => _service = service;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        bool running = true;
        while (running && !cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("\n========== USER MANAGEMENT ==========");
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
        if (!list.Any()) { Console.WriteLine("No users found."); return; }
        Console.WriteLine($"\n{"ID",-6} {"Username",-25} {"PersonId",-10} {"RoleId",-8} {"Active",-8}");
        Console.WriteLine(new string('-', 59));
        foreach (var u in list)
            Console.WriteLine($"{u.UserId,-6} {u.Username,-25} {u.PersonId,-10} {u.RoleId,-8} {u.IsActive,-8}");
    }

    private async Task GetByIdAsync(CancellationToken ct)
    {
        Console.Write("User ID: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Invalid ID."); return; }
        var u = await _service.GetByIdAsync(id, ct);
        if (u is null) { Console.WriteLine("Not found."); return; }
        Console.WriteLine($"ID: {u.UserId} | Username: {u.Username} | PersonId: {u.PersonId} | RoleId: {u.RoleId} | Active: {u.IsActive}");
    }

    private async Task CreateAsync(CancellationToken ct)
    {
        Console.Write("Person ID: ");
        if (!int.TryParse(Console.ReadLine(), out var personId)) { Console.WriteLine("Invalid ID."); return; }
        Console.Write("Role ID: ");
        if (!int.TryParse(Console.ReadLine(), out var roleId)) { Console.WriteLine("Invalid ID."); return; }
        Console.Write("Username: ");
        var username = Console.ReadLine()?.Trim() ?? string.Empty;
        Console.Write("Password: ");
        var password = Console.ReadLine() ?? string.Empty;
        var hash = HashPassword(password);
        Console.Write("Is active? (y/n): ");
        var active = Console.ReadLine()?.Trim().ToLower() == "y";
        var created = await _service.CreateAsync(
            new CreateUserRequest(personId, roleId, username, hash, active), ct);
        Console.WriteLine($"User created with ID: {created.UserId}");
    }

    private async Task UpdateAsync(CancellationToken ct)
    {
        Console.Write("User ID: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Invalid ID."); return; }
        Console.Write("New Role ID: ");
        if (!int.TryParse(Console.ReadLine(), out var roleId)) { Console.WriteLine("Invalid ID."); return; }
        Console.Write("New Username: ");
        var username = Console.ReadLine()?.Trim() ?? string.Empty;
        Console.Write("Is active? (y/n): ");
        var active = Console.ReadLine()?.Trim().ToLower() == "y";
        var updated = await _service.UpdateAsync(id, new UpdateUserRequest(roleId, username, active), ct);
        Console.WriteLine($"Updated: {updated.Username}");
    }

    private async Task DeleteAsync(CancellationToken ct)
    {
        Console.Write("User ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Invalid ID."); return; }
        Console.Write($"Confirm delete user {id}? (y/n): ");
        if (Console.ReadLine()?.Trim().ToLower() != "y") { Console.WriteLine("Cancelled."); return; }
        await _service.DeleteAsync(id, ct);
        Console.WriteLine("User deleted.");
    }

    /// <summary>SHA-256 simple. En producción usar BCrypt o Argon2.</summary>
    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
