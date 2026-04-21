namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.JobPosition.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class JobPositionConsoleUI : IModuleUI
{
    private readonly IJobPositionService _service;

    public string Key   => "job_position";
    public string Title => "Job Position Management";

    public JobPositionConsoleUI(IJobPositionService service) => _service = service;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        bool running = true;
        while (running && !cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("\n========== JOB POSITION MANAGEMENT ==========");
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
        if (!list.Any()) { Console.WriteLine("No job positions found."); return; }
        Console.WriteLine($"\n{"ID",-6} {"Name",-30} {"Department",-25}");
        Console.WriteLine(new string('-', 63));
        foreach (var j in list)
            Console.WriteLine($"{j.JobPositionId,-6} {j.Name,-30} {j.Department ?? "-",-25}");
    }

    private async Task GetByIdAsync(CancellationToken ct)
    {
        Console.Write("Job Position ID: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Invalid ID."); return; }
        var j = await _service.GetByIdAsync(id, ct);
        if (j is null) { Console.WriteLine("Not found."); return; }
        Console.WriteLine($"ID: {j.JobPositionId} | Name: {j.Name} | Dept: {j.Department ?? "-"}");
    }

    private async Task CreateAsync(CancellationToken ct)
    {
        Console.Write("Name (e.g. PILOT): ");
        var name = Console.ReadLine()?.Trim() ?? string.Empty;
        Console.Write("Department (optional): ");
        var dept = Console.ReadLine()?.Trim();
        var created = await _service.CreateAsync(
            new CreateJobPositionRequest(name, string.IsNullOrWhiteSpace(dept) ? null : dept), ct);
        Console.WriteLine($"Created with ID: {created.JobPositionId}");
    }

    private async Task UpdateAsync(CancellationToken ct)
    {
        Console.Write("ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Invalid ID."); return; }
        Console.Write("New Name: ");
        var name = Console.ReadLine()?.Trim() ?? string.Empty;
        Console.Write("New Department (optional): ");
        var dept = Console.ReadLine()?.Trim();
        var updated = await _service.UpdateAsync(id,
            new UpdateJobPositionRequest(name, string.IsNullOrWhiteSpace(dept) ? null : dept), ct);
        Console.WriteLine($"Updated: {updated.Name}");
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
