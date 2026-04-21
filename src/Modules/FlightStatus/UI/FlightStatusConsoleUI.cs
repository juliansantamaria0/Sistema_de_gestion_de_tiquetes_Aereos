namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightStatus.Application.Interfaces;

public sealed class FlightStatusConsoleUI
{
    private readonly IFlightStatusService _service;

    public FlightStatusConsoleUI(IFlightStatusService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n========== FLIGHT STATUS MODULE ==========");
            Console.WriteLine("1. List all flight statuses");
            Console.WriteLine("2. Get flight status by ID");
            Console.WriteLine("3. Create flight status");
            Console.WriteLine("4. Update flight status");
            Console.WriteLine("5. Delete flight status");
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
                default: Console.WriteLine("Invalid option."); break;
            }
        }
    }

    private async Task ListAllAsync()
    {
        var statuses = await _service.GetAllAsync();
        Console.WriteLine("\n--- All Flight Statuses ---");
        foreach (var s in statuses)
            Console.WriteLine($"  [{s.Id}] {s.Name}");
    }

    private async Task GetByIdAsync()
    {
        Console.Write("Enter flight status ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        var s = await _service.GetByIdAsync(id);
        if (s is null) Console.WriteLine($"Flight status with ID {id} not found.");
        else           Console.WriteLine($"  [{s.Id}] {s.Name}");
    }

    private async Task CreateAsync()
    {
        Console.Write("Enter name (e.g. SCHEDULED, ACTIVE, DELAYED, CANCELLED, COMPLETED): ");
        var name = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(name)) { Console.WriteLine("Name cannot be empty."); return; }

        var created = await _service.CreateAsync(name);
        Console.WriteLine($"Flight status created: [{created.Id}] {created.Name}");
    }

    private async Task UpdateAsync()
    {
        Console.Write("Enter flight status ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("Enter new name: ");
        var newName = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(newName)) { Console.WriteLine("Name cannot be empty."); return; }

        await _service.UpdateAsync(id, newName);
        Console.WriteLine("Flight status updated successfully.");
    }

    private async Task DeleteAsync()
    {
        Console.Write("Enter flight status ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        await _service.DeleteAsync(id);
        Console.WriteLine("Flight status deleted successfully.");
    }
}
