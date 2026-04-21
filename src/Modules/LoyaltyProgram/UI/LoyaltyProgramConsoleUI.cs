namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Application.Interfaces;

public sealed class LoyaltyProgramConsoleUI
{
    private readonly ILoyaltyProgramService _service;

    public LoyaltyProgramConsoleUI(ILoyaltyProgramService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n========== LOYALTY PROGRAM MODULE ==========");
            Console.WriteLine("1. List all programs");
            Console.WriteLine("2. Get program by ID");
            Console.WriteLine("3. Get program by airline");
            Console.WriteLine("4. Create program");
            Console.WriteLine("5. Update program");
            Console.WriteLine("6. Delete program");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            var option = Console.ReadLine()?.Trim();

            switch (option)
            {
                case "1": await ListAllAsync();         break;
                case "2": await GetByIdAsync();         break;
                case "3": await GetByAirlineAsync();    break;
                case "4": await CreateAsync();          break;
                case "5": await UpdateAsync();          break;
                case "6": await DeleteAsync();          break;
                case "0": running = false;              break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    private async Task ListAllAsync()
    {
        var programs = await _service.GetAllAsync();
        Console.WriteLine("\n--- All Loyalty Programs ---");
        foreach (var p in programs)
            Console.WriteLine($"  [{p.Id}] {p.Name} | Airline:{p.AirlineId} | {p.MilesPerDollar:F2} miles/$");
    }

    private async Task GetByIdAsync()
    {
        Console.Write("Enter program ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        var p = await _service.GetByIdAsync(id);
        if (p is null) Console.WriteLine($"Program with ID {id} not found.");
        else           Console.WriteLine($"  [{p.Id}] {p.Name} | Airline:{p.AirlineId} | {p.MilesPerDollar:F2} miles/$");
    }

    private async Task GetByAirlineAsync()
    {
        Console.Write("Enter Airline ID: ");
        if (!int.TryParse(Console.ReadLine(), out int airlineId))
        { Console.WriteLine("Invalid ID."); return; }

        var p = await _service.GetByAirlineAsync(airlineId);
        if (p is null) Console.WriteLine($"No loyalty program for airline {airlineId}.");
        else           Console.WriteLine($"  [{p.Id}] {p.Name} | {p.MilesPerDollar:F2} miles/$");
    }

    private async Task CreateAsync()
    {
        Console.Write("Airline ID: ");
        if (!int.TryParse(Console.ReadLine(), out int airlineId)) { Console.WriteLine("Invalid."); return; }

        Console.Write("Program name (e.g. LifeMiles): ");
        var name = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(name)) { Console.WriteLine("Name cannot be empty."); return; }

        Console.Write("Miles per dollar (> 0, default 1): ");
        if (!decimal.TryParse(Console.ReadLine()?.Replace(',', '.'),
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out decimal mpd) || mpd <= 0)
            mpd = 1m;

        try
        {
            var created = await _service.CreateAsync(airlineId, name, mpd);
            Console.WriteLine($"Program created: [{created.Id}] {created.Name} | {created.MilesPerDollar:F2} miles/$");
        }
        catch (ArgumentException ex) { Console.WriteLine($"Validation error: {ex.Message}"); }
    }

    private async Task UpdateAsync()
    {
        Console.Write("Program ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("Invalid."); return; }

        Console.Write("New name: ");
        var name = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(name)) { Console.WriteLine("Name cannot be empty."); return; }

        Console.Write("New miles per dollar (> 0): ");
        if (!decimal.TryParse(Console.ReadLine()?.Replace(',', '.'),
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out decimal mpd) || mpd <= 0)
        { Console.WriteLine("Invalid value."); return; }

        try
        {
            await _service.UpdateAsync(id, name, mpd);
            Console.WriteLine("Program updated successfully.");
        }
        catch (ArgumentException ex) { Console.WriteLine($"Validation error: {ex.Message}"); }
    }

    private async Task DeleteAsync()
    {
        Console.Write("Program ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("Invalid."); return; }

        await _service.DeleteAsync(id);
        Console.WriteLine("Program deleted successfully.");
    }
}
