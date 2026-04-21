namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightSeat.Application.Interfaces;

public sealed class FlightSeatConsoleUI
{
    private readonly IFlightSeatService _service;

    public FlightSeatConsoleUI(IFlightSeatService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n========== FLIGHT SEAT MODULE ==========");
            Console.WriteLine("1. List all flight seats");
            Console.WriteLine("2. Get flight seat by ID");
            Console.WriteLine("3. List seats by flight");
            Console.WriteLine("4. List available seats by flight");
            Console.WriteLine("5. Create flight seat");
            Console.WriteLine("6. Change seat status");
            Console.WriteLine("7. Delete flight seat");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            var option = Console.ReadLine()?.Trim();

            switch (option)
            {
                case "1": await ListAllAsync();               break;
                case "2": await GetByIdAsync();               break;
                case "3": await ListByFlightAsync();          break;
                case "4": await ListAvailableByFlightAsync(); break;
                case "5": await CreateAsync();                break;
                case "6": await ChangeStatusAsync();          break;
                case "7": await DeleteAsync();                break;
                case "0": running = false;                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    // ── Handlers ──────────────────────────────────────────────────────────────

    private async Task ListAllAsync()
    {
        var seats = await _service.GetAllAsync();
        Console.WriteLine("\n--- All Flight Seats ---");

        foreach (var s in seats)
            PrintSeat(s);
    }

    private async Task GetByIdAsync()
    {
        Console.Write("Enter flight seat ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        var seat = await _service.GetByIdAsync(id);

        if (seat is null)
            Console.WriteLine($"Flight seat with ID {id} not found.");
        else
            PrintSeat(seat);
    }

    private async Task ListByFlightAsync()
    {
        Console.Write("Enter Scheduled Flight ID: ");
        if (!int.TryParse(Console.ReadLine(), out int flightId))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        var seats = await _service.GetByFlightAsync(flightId);
        Console.WriteLine($"\n--- All Seats for Flight {flightId} ---");

        foreach (var s in seats)
            PrintSeat(s);
    }

    private async Task ListAvailableByFlightAsync()
    {
        Console.Write("Enter Scheduled Flight ID: ");
        if (!int.TryParse(Console.ReadLine(), out int flightId))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        var seats = await _service.GetAvailableByFlightAsync(flightId);
        var list  = seats.ToList();
        Console.WriteLine($"\n--- Available Seats for Flight {flightId} ({list.Count} available) ---");

        foreach (var s in list)
            PrintSeat(s);
    }

    private async Task CreateAsync()
    {
        Console.Write("Scheduled Flight ID: ");
        if (!int.TryParse(Console.ReadLine(), out int flightId))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("Seat Map ID: ");
        if (!int.TryParse(Console.ReadLine(), out int seatMapId))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("Seat Status ID: ");
        if (!int.TryParse(Console.ReadLine(), out int statusId))
        { Console.WriteLine("Invalid ID."); return; }

        var created = await _service.CreateAsync(flightId, seatMapId, statusId);
        Console.WriteLine(
            $"Flight seat created: [{created.Id}] " +
            $"Flight: {created.ScheduledFlightId} | SeatMap: {created.SeatMapId} | Status: {created.SeatStatusId}");
    }

    private async Task ChangeStatusAsync()
    {
        Console.Write("Flight seat ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("New Seat Status ID: ");
        if (!int.TryParse(Console.ReadLine(), out int statusId))
        { Console.WriteLine("Invalid ID."); return; }

        await _service.ChangeStatusAsync(id, statusId);
        Console.WriteLine("Flight seat status changed successfully.");
    }

    private async Task DeleteAsync()
    {
        Console.Write("Enter flight seat ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        await _service.DeleteAsync(id);
        Console.WriteLine("Flight seat deleted successfully.");
    }

    // ── Helpers de presentación ───────────────────────────────────────────────

    private static void PrintSeat(FlightSeatDto s)
        => Console.WriteLine(
            $"  [{s.Id}] Flight: {s.ScheduledFlightId} | SeatMap: {s.SeatMapId} | " +
            $"Status: {s.SeatStatusId} | Created: {s.CreatedAt:yyyy-MM-dd HH:mm}" +
            (s.UpdatedAt.HasValue ? $" | Updated: {s.UpdatedAt:yyyy-MM-dd HH:mm}" : string.Empty));
}
