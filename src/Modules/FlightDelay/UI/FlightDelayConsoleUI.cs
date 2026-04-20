namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightDelay.Application.Interfaces;

public sealed class FlightDelayConsoleUI
{
    private readonly IFlightDelayService _service;

    public FlightDelayConsoleUI(IFlightDelayService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n========== FLIGHT DELAY MODULE ==========");
            Console.WriteLine("1. List all delay records");
            Console.WriteLine("2. Get delay by ID");
            Console.WriteLine("3. List delays by flight");
            Console.WriteLine("4. Report delay");
            Console.WriteLine("5. Adjust delay minutes");
            Console.WriteLine("6. Delete delay record");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            var option = Console.ReadLine()?.Trim();

            switch (option)
            {
                case "1": await ListAllAsync();          break;
                case "2": await GetByIdAsync();          break;
                case "3": await ListByFlightAsync();     break;
                case "4": await ReportDelayAsync();      break;
                case "5": await AdjustDelayAsync();      break;
                case "6": await DeleteDelayAsync();      break;
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
        var delays = await _service.GetAllAsync();
        Console.WriteLine("\n--- All Flight Delay Records ---");
        foreach (var d in delays) PrintDelay(d);
    }

    private async Task GetByIdAsync()
    {
        Console.Write("Enter delay record ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        var delay = await _service.GetByIdAsync(id);
        if (delay is null) Console.WriteLine($"Flight delay with ID {id} not found.");
        else               PrintDelay(delay);
    }

    private async Task ListByFlightAsync()
    {
        Console.Write("Enter Scheduled Flight ID: ");
        if (!int.TryParse(Console.ReadLine(), out int flightId))
        { Console.WriteLine("Invalid ID."); return; }

        var delays = await _service.GetByFlightAsync(flightId);
        var list   = delays.ToList();
        Console.WriteLine($"\n--- Delay Records for Flight {flightId} ---");

        int totalMinutes = list.Sum(d => d.DelayMinutes);
        foreach (var d in list) PrintDelay(d);
        Console.WriteLine($"  Total accumulated delay: {totalMinutes} min ({totalMinutes / 60}h {totalMinutes % 60}m)");
    }

    private async Task ReportDelayAsync()
    {
        Console.Write("Scheduled Flight ID: ");
        if (!int.TryParse(Console.ReadLine(), out int flightId))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("Delay Reason ID: ");
        if (!int.TryParse(Console.ReadLine(), out int reasonId))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("Delay in minutes (> 0): ");
        if (!int.TryParse(Console.ReadLine(), out int minutes))
        { Console.WriteLine("Invalid value."); return; }

        try
        {
            var created = await _service.CreateAsync(flightId, reasonId, minutes);
            Console.WriteLine(
                $"Delay reported: [{created.Id}] Flight {created.ScheduledFlightId} | " +
                $"Reason {created.DelayReasonId} | {created.DelayMinutes} min | " +
                $"Reported: {created.ReportedAt:yyyy-MM-dd HH:mm}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Validation error: {ex.Message}");
        }
    }

    private async Task AdjustDelayAsync()
    {
        Console.Write("Delay record ID to adjust: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("Corrected delay in minutes (> 0): ");
        if (!int.TryParse(Console.ReadLine(), out int minutes))
        { Console.WriteLine("Invalid value."); return; }

        try
        {
            await _service.AdjustDelayAsync(id, minutes);
            Console.WriteLine($"Delay adjusted to {minutes} minutes.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Validation error: {ex.Message}");
        }
    }

    private async Task DeleteDelayAsync()
    {
        Console.Write("Delay record ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        await _service.DeleteAsync(id);
        Console.WriteLine("Delay record deleted successfully.");
    }

    // ── Helpers de presentación ───────────────────────────────────────────────

    private static void PrintDelay(FlightDelayDto d)
        => Console.WriteLine(
            $"  [{d.Id}] Flight: {d.ScheduledFlightId} | " +
            $"Reason: {d.DelayReasonId} | {d.DelayMinutes} min | " +
            $"Reported: {d.ReportedAt:yyyy-MM-dd HH:mm}");
}
