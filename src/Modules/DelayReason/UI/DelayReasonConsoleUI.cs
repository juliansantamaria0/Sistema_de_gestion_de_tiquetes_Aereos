namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DelayReason.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DelayReason.Application.Interfaces;

public sealed class DelayReasonConsoleUI
{
    private readonly IDelayReasonService _service;

    public DelayReasonConsoleUI(IDelayReasonService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n========== DELAY REASON MODULE ==========");
            Console.WriteLine("1. List all delay reasons");
            Console.WriteLine("2. Get delay reason by ID");
            Console.WriteLine("3. Create delay reason");
            Console.WriteLine("4. Update delay reason");
            Console.WriteLine("5. Delete delay reason");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            var option = Console.ReadLine()?.Trim();

            switch (option)
            {
                case "1": await ListAllAsync();  break;
                case "2": await GetByIdAsync();  break;
                case "3": await CreateAsync();   break;
                case "4": await UpdateAsync();   break;
                case "5": await DeleteAsync();   break;
                case "0": running = false;       break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    // ── Handlers ──────────────────────────────────────────────────────────────

    private async Task ListAllAsync()
    {
        var reasons = await _service.GetAllAsync();
        Console.WriteLine("\n--- All Delay Reasons ---");

        foreach (var r in reasons)
            Console.WriteLine($"  [{r.Id}] [{r.Category}] {r.Name}");
    }

    private async Task GetByIdAsync()
    {
        Console.Write("Enter delay reason ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        var reason = await _service.GetByIdAsync(id);

        if (reason is null)
            Console.WriteLine($"Delay reason with ID {id} not found.");
        else
            Console.WriteLine($"  [{reason.Id}] [{reason.Category}] {reason.Name}");
    }

    private async Task CreateAsync()
    {
        Console.Write("Name (e.g. WEATHER, TECHNICAL, ATC, CREW, COMMERCIAL): ");
        var name = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Name cannot be empty.");
            return;
        }

        Console.Write("Category (e.g. METEOROLOGICAL, MECHANICAL, OPERATIONAL): ");
        var category = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(category))
        {
            Console.WriteLine("Category cannot be empty.");
            return;
        }

        var created = await _service.CreateAsync(name, category);
        Console.WriteLine($"Delay reason created: [{created.Id}] [{created.Category}] {created.Name}");
    }

    private async Task UpdateAsync()
    {
        Console.Write("Enter delay reason ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        Console.Write("New name: ");
        var name = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Name cannot be empty.");
            return;
        }

        Console.Write("New category: ");
        var category = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(category))
        {
            Console.WriteLine("Category cannot be empty.");
            return;
        }

        await _service.UpdateAsync(id, name, category);
        Console.WriteLine("Delay reason updated successfully.");
    }

    private async Task DeleteAsync()
    {
        Console.Write("Enter delay reason ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        await _service.DeleteAsync(id);
        Console.WriteLine("Delay reason deleted successfully.");
    }
}
