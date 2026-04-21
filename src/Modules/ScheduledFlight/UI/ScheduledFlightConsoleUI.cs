namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ScheduledFlight.Application.Interfaces;

public sealed class ScheduledFlightConsoleUI
{
    private readonly IScheduledFlightService _service;

    public ScheduledFlightConsoleUI(IScheduledFlightService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n========== SCHEDULED FLIGHT MODULE ==========");
            Console.WriteLine("1. List all scheduled flights");
            Console.WriteLine("2. Get scheduled flight by ID");
            Console.WriteLine("3. List by base flight");
            Console.WriteLine("4. List by departure date");
            Console.WriteLine("5. Create scheduled flight");
            Console.WriteLine("6. Update scheduled flight");
            Console.WriteLine("7. Delete scheduled flight");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            var option = Console.ReadLine()?.Trim();

            switch (option)
            {
                case "1": await ListAllAsync();          break;
                case "2": await GetByIdAsync();          break;
                case "3": await ListByBaseFlightAsync(); break;
                case "4": await ListByDateAsync();       break;
                case "5": await CreateAsync();           break;
                case "6": await UpdateAsync();           break;
                case "7": await DeleteAsync();           break;
                case "0": running = false;               break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    // ── Handlers ──────────────────────────────────────────────────────────────

    private async Task ListAllAsync()
    {
        var flights = await _service.GetAllAsync();
        Console.WriteLine("\n--- All Scheduled Flights ---");

        foreach (var f in flights)
            PrintFlight(f);
    }

    private async Task GetByIdAsync()
    {
        Console.Write("Enter scheduled flight ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        var flight = await _service.GetByIdAsync(id);

        if (flight is null)
            Console.WriteLine($"Scheduled flight with ID {id} not found.");
        else
            PrintFlight(flight);
    }

    private async Task ListByBaseFlightAsync()
    {
        Console.Write("Enter Base Flight ID: ");
        if (!int.TryParse(Console.ReadLine(), out int baseFlightId))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        var flights = await _service.GetByBaseFlightAsync(baseFlightId);
        Console.WriteLine($"\n--- Scheduled Flights for Base Flight {baseFlightId} ---");

        foreach (var f in flights)
            PrintFlight(f);
    }

    private async Task ListByDateAsync()
    {
        Console.Write("Enter departure date (yyyy-MM-dd): ");
        if (!DateOnly.TryParse(Console.ReadLine()?.Trim(), out DateOnly date))
        {
            Console.WriteLine("Invalid date format. Use yyyy-MM-dd.");
            return;
        }

        var flights = await _service.GetByDateAsync(date);
        Console.WriteLine($"\n--- Scheduled Flights on {date:yyyy-MM-dd} ---");

        foreach (var f in flights)
            PrintFlight(f);
    }

    private async Task CreateAsync()
    {
        Console.Write("Base Flight ID: ");
        if (!int.TryParse(Console.ReadLine(), out int baseFlightId))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("Aircraft ID: ");
        if (!int.TryParse(Console.ReadLine(), out int aircraftId))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("Gate ID (leave blank if not assigned): ");
        var gateInput = Console.ReadLine()?.Trim();
        int? gateId   = int.TryParse(gateInput, out int gParsed) ? gParsed : null;

        Console.Write("Departure date (yyyy-MM-dd): ");
        if (!DateOnly.TryParse(Console.ReadLine()?.Trim(), out DateOnly depDate))
        { Console.WriteLine("Invalid date."); return; }

        Console.Write("Departure time (HH:mm): ");
        if (!TimeOnly.TryParse(Console.ReadLine()?.Trim(), out TimeOnly depTime))
        { Console.WriteLine("Invalid time."); return; }

        Console.Write("Estimated arrival datetime (yyyy-MM-dd HH:mm): ");
        if (!DateTime.TryParse(Console.ReadLine()?.Trim(), out DateTime eta))
        { Console.WriteLine("Invalid datetime."); return; }

        Console.Write("Flight Status ID: ");
        if (!int.TryParse(Console.ReadLine(), out int statusId))
        { Console.WriteLine("Invalid ID."); return; }

        var request = new CreateScheduledFlightRequest(
            baseFlightId, aircraftId, gateId, depDate, depTime, eta, statusId);

        var created = await _service.CreateAsync(request);
        Console.WriteLine($"Scheduled flight created: [{created.Id}] " +
                          $"{created.DepartureDate:yyyy-MM-dd} {created.DepartureTime:HH:mm}");
    }

    private async Task UpdateAsync()
    {
        Console.Write("Scheduled flight ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("New Aircraft ID: ");
        if (!int.TryParse(Console.ReadLine(), out int aircraftId))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("New Gate ID (leave blank to clear): ");
        var gateInput = Console.ReadLine()?.Trim();
        int? gateId   = int.TryParse(gateInput, out int gParsed) ? gParsed : null;

        Console.Write("New departure date (yyyy-MM-dd): ");
        if (!DateOnly.TryParse(Console.ReadLine()?.Trim(), out DateOnly depDate))
        { Console.WriteLine("Invalid date."); return; }

        Console.Write("New departure time (HH:mm): ");
        if (!TimeOnly.TryParse(Console.ReadLine()?.Trim(), out TimeOnly depTime))
        { Console.WriteLine("Invalid time."); return; }

        Console.Write("New estimated arrival (yyyy-MM-dd HH:mm): ");
        if (!DateTime.TryParse(Console.ReadLine()?.Trim(), out DateTime eta))
        { Console.WriteLine("Invalid datetime."); return; }

        Console.Write("New Flight Status ID: ");
        if (!int.TryParse(Console.ReadLine(), out int statusId))
        { Console.WriteLine("Invalid ID."); return; }

        var request = new UpdateScheduledFlightRequest(
            aircraftId, gateId, depDate, depTime, eta, statusId);

        await _service.UpdateAsync(id, request);
        Console.WriteLine("Scheduled flight updated successfully.");
    }

    private async Task DeleteAsync()
    {
        Console.Write("Enter scheduled flight ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        await _service.DeleteAsync(id);
        Console.WriteLine("Scheduled flight deleted successfully.");
    }

    // ── Helpers de presentación ───────────────────────────────────────────────

    private static void PrintFlight(ScheduledFlightDto f)
        => Console.WriteLine(
            $"  [{f.Id}] BaseFlight: {f.BaseFlightId} | Aircraft: {f.AircraftId} | " +
            $"Gate: {(f.GateId.HasValue ? f.GateId.ToString() : "N/A")} | " +
            $"Departs: {f.DepartureDate:yyyy-MM-dd} {f.DepartureTime:HH:mm} | " +
            $"ETA: {f.EstimatedArrivalDatetime:yyyy-MM-dd HH:mm} | " +
            $"Status: {f.FlightStatusId}");
}
