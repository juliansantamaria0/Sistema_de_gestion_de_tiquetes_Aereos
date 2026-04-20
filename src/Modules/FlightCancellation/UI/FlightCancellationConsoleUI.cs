namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCancellation.Application.Interfaces;

public sealed class FlightCancellationConsoleUI
{
    private readonly IFlightCancellationService _service;

    public FlightCancellationConsoleUI(IFlightCancellationService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n========== FLIGHT CANCELLATION MODULE ==========");
            Console.WriteLine("1. List all cancellations");
            Console.WriteLine("2. Get cancellation by ID");
            Console.WriteLine("3. Check if flight is cancelled");
            Console.WriteLine("4. Register cancellation");
            Console.WriteLine("5. Update notes");
            Console.WriteLine("6. Delete cancellation record");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            var option = Console.ReadLine()?.Trim();

            switch (option)
            {
                case "1": await ListAllAsync();          break;
                case "2": await GetByIdAsync();          break;
                case "3": await CheckByFlightAsync();    break;
                case "4": await RegisterAsync();         break;
                case "5": await UpdateNotesAsync();      break;
                case "6": await DeleteAsync();           break;
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
        var cancellations = await _service.GetAllAsync();
        Console.WriteLine("\n--- All Flight Cancellations ---");
        foreach (var c in cancellations) PrintCancellation(c);
    }

    private async Task GetByIdAsync()
    {
        Console.Write("Enter cancellation ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        var c = await _service.GetByIdAsync(id);
        if (c is null) Console.WriteLine($"Cancellation with ID {id} not found.");
        else           PrintCancellation(c);
    }

    private async Task CheckByFlightAsync()
    {
        Console.Write("Enter Scheduled Flight ID: ");
        if (!int.TryParse(Console.ReadLine(), out int flightId))
        { Console.WriteLine("Invalid ID."); return; }

        var c = await _service.GetByFlightAsync(flightId);
        if (c is null)
            Console.WriteLine($"Flight {flightId} has NOT been cancelled.");
        else
        {
            Console.WriteLine($"Flight {flightId} IS CANCELLED:");
            PrintCancellation(c);
        }
    }

    private async Task RegisterAsync()
    {
        Console.Write("Scheduled Flight ID: ");
        if (!int.TryParse(Console.ReadLine(), out int flightId))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("Cancellation Reason ID: ");
        if (!int.TryParse(Console.ReadLine(), out int reasonId))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("Notes (optional — press Enter to skip): ");
        var notesInput = Console.ReadLine()?.Trim();
        string? notes  = string.IsNullOrWhiteSpace(notesInput) ? null : notesInput;

        try
        {
            var created = await _service.CreateAsync(flightId, reasonId, notes);
            Console.WriteLine(
                $"Cancellation registered: [{created.Id}] Flight {created.ScheduledFlightId} | " +
                $"Reason {created.CancellationReasonId} | " +
                $"Cancelled at: {created.CancelledAt:yyyy-MM-dd HH:mm}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Validation error: {ex.Message}");
        }
    }

    private async Task UpdateNotesAsync()
    {
        Console.Write("Cancellation ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("New notes (press Enter to clear): ");
        var notesInput = Console.ReadLine()?.Trim();
        string? notes  = string.IsNullOrWhiteSpace(notesInput) ? null : notesInput;

        try
        {
            await _service.UpdateNotesAsync(id, notes);
            Console.WriteLine("Notes updated successfully.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Validation error: {ex.Message}");
        }
    }

    private async Task DeleteAsync()
    {
        Console.Write("Cancellation ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        await _service.DeleteAsync(id);
        Console.WriteLine("Cancellation record deleted successfully.");
    }

    // ── Helpers de presentación ───────────────────────────────────────────────

    private static void PrintCancellation(FlightCancellationDto c)
        => Console.WriteLine(
            $"  [{c.Id}] Flight: {c.ScheduledFlightId} | " +
            $"Reason: {c.CancellationReasonId} | " +
            $"Cancelled: {c.CancelledAt:yyyy-MM-dd HH:mm}" +
            (c.Notes is not null ? $" | Notes: {c.Notes}" : string.Empty));
}
