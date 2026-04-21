namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Passenger.Application.Interfaces;

public sealed class PassengerConsoleUI
{
    private readonly IPassengerService _service;

    public PassengerConsoleUI(IPassengerService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n========== PASSENGER MODULE ==========");
            Console.WriteLine("1. List all passengers");
            Console.WriteLine("2. Get passenger by ID");
            Console.WriteLine("3. Get passenger by person ID");
            Console.WriteLine("4. Register passenger");
            Console.WriteLine("5. Update passenger");
            Console.WriteLine("6. Delete passenger");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            var option = Console.ReadLine()?.Trim();

            switch (option)
            {
                case "1": await ListAllAsync();        break;
                case "2": await GetByIdAsync();        break;
                case "3": await GetByPersonAsync();    break;
                case "4": await RegisterAsync();       break;
                case "5": await UpdateAsync();         break;
                case "6": await DeleteAsync();         break;
                case "0": running = false;             break;
                default: Console.WriteLine("Invalid option."); break;
            }
        }
    }

    private async Task ListAllAsync()
    {
        var passengers = await _service.GetAllAsync();
        Console.WriteLine("\n--- All Passengers ---");
        foreach (var p in passengers) PrintPassenger(p);
    }

    private async Task GetByIdAsync()
    {
        Console.Write("Enter passenger ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        var p = await _service.GetByIdAsync(id);
        if (p is null) Console.WriteLine($"Passenger with ID {id} not found.");
        else           PrintPassenger(p);
    }

    private async Task GetByPersonAsync()
    {
        Console.Write("Enter Person ID: ");
        if (!int.TryParse(Console.ReadLine(), out int personId))
        { Console.WriteLine("Invalid ID."); return; }

        var p = await _service.GetByPersonAsync(personId);
        if (p is null) Console.WriteLine($"No passenger profile for person {personId}.");
        else           PrintPassenger(p);
    }

    private async Task RegisterAsync()
    {
        Console.Write("Person ID: ");
        if (!int.TryParse(Console.ReadLine(), out int personId))
        { Console.WriteLine("Invalid."); return; }

        Console.Write("Frequent flyer number (optional): ");
        var ffnInput = Console.ReadLine()?.Trim();
        string? ffn = string.IsNullOrWhiteSpace(ffnInput) ? null : ffnInput;

        Console.Write("Nationality ID (optional): ");
        var natInput = Console.ReadLine()?.Trim();
        int? nationalityId = int.TryParse(natInput, out int n) && n > 0 ? n : null;

        try
        {
            var created = await _service.CreateAsync(personId, ffn, nationalityId);
            Console.WriteLine(
                $"Passenger registered: [{created.Id}] Person:{created.PersonId}" +
                (created.FrequentFlyerNumber is not null ? $" | FFN:{created.FrequentFlyerNumber}" : string.Empty));
        }
        catch (ArgumentException ex) { Console.WriteLine($"Validation error: {ex.Message}"); }
    }

    private async Task UpdateAsync()
    {
        Console.Write("Passenger ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid."); return; }

        Console.Write("New frequent flyer number (optional, Enter to clear): ");
        var ffnInput = Console.ReadLine()?.Trim();
        string? ffn = string.IsNullOrWhiteSpace(ffnInput) ? null : ffnInput;

        Console.Write("New Nationality ID (optional, Enter to clear): ");
        var natInput = Console.ReadLine()?.Trim();
        int? nationalityId = int.TryParse(natInput, out int n) && n > 0 ? n : null;

        try
        {
            await _service.UpdateAsync(id, ffn, nationalityId);
            Console.WriteLine("Passenger updated successfully.");
        }
        catch (ArgumentException ex) { Console.WriteLine($"Validation error: {ex.Message}"); }
    }

    private async Task DeleteAsync()
    {
        Console.Write("Passenger ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid."); return; }

        await _service.DeleteAsync(id);
        Console.WriteLine("Passenger deleted successfully.");
    }

    private static void PrintPassenger(PassengerDto p)
        => Console.WriteLine(
            $"  [{p.Id}] Person:{p.PersonId}" +
            (p.FrequentFlyerNumber is not null ? $" | FFN:{p.FrequentFlyerNumber}" : string.Empty) +
            (p.NationalityId.HasValue ? $" | Nationality:{p.NationalityId}" : string.Empty) +
            $" | Joined:{p.CreatedAt:yyyy-MM-dd}");
}
