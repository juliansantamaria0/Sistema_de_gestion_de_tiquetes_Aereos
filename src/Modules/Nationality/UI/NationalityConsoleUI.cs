namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Nationality.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class NationalityConsoleUI : IModuleUI
{
    private readonly INationalityService _service;

    public string Key   => "nationality";
    public string Title => "Nationality Management";

    public NationalityConsoleUI(INationalityService service) => _service = service;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        bool running = true;
        while (running && !cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("\n========== NATIONALITY MANAGEMENT ==========");
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
        if (!list.Any()) { Console.WriteLine("No nationalities found."); return; }
        Console.WriteLine($"\n{"ID",-6} {"CountryId",-10} {"Demonym",-30}");
        Console.WriteLine(new string('-', 48));
        foreach (var n in list)
            Console.WriteLine($"{n.NationalityId,-6} {n.CountryId,-10} {n.Demonym,-30}");
    }

    private async Task GetByIdAsync(CancellationToken ct)
    {
        Console.Write("Nationality ID: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Invalid ID."); return; }
        var n = await _service.GetByIdAsync(id, ct);
        if (n is null) { Console.WriteLine("Not found."); return; }
        Console.WriteLine($"ID: {n.NationalityId} | CountryId: {n.CountryId} | Demonym: {n.Demonym}");
    }

    private async Task CreateAsync(CancellationToken ct)
    {
        Console.Write("Country ID: ");
        if (!int.TryParse(Console.ReadLine(), out var cId)) { Console.WriteLine("Invalid ID."); return; }
        Console.Write("Demonym (e.g. Colombian): ");
        var demonym = Console.ReadLine()?.Trim() ?? string.Empty;
        var created = await _service.CreateAsync(new CreateNationalityRequest(cId, demonym), ct);
        Console.WriteLine($"Created with ID: {created.NationalityId}");
    }

    private async Task UpdateAsync(CancellationToken ct)
    {
        Console.Write("ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Invalid ID."); return; }
        Console.Write("New Country ID: ");
        if (!int.TryParse(Console.ReadLine(), out var cId)) { Console.WriteLine("Invalid ID."); return; }
        Console.Write("New Demonym: ");
        var demonym = Console.ReadLine()?.Trim() ?? string.Empty;
        var updated = await _service.UpdateAsync(id, new UpdateNationalityRequest(cId, demonym), ct);
        Console.WriteLine($"Updated: {updated.Demonym}");
    }

    private async Task DeleteAsync(CancellationToken ct)
    {
        Console.Write("ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Invalid ID."); return; }
        Console.Write($"Confirm delete {id}? (y/n): ");
        if (Console.ReadLine()?.Trim().ToLower() != "y") { Console.WriteLine("Cancelled."); return; }
        await _service.DeleteAsync(id, ct);
        Console.WriteLine("Deleted.");
    }
}
