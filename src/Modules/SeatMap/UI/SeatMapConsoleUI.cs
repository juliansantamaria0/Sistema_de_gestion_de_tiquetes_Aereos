namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.SeatMap.Application.Interfaces;

public sealed class SeatMapConsoleUI
{
    private readonly ISeatMapService _service;

    public SeatMapConsoleUI(ISeatMapService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n========== SEAT MAP MODULE ==========");
            Console.WriteLine("1. List all seat map entries");
            Console.WriteLine("2. Get seat map entry by ID");
            Console.WriteLine("3. List seats by aircraft type");
            Console.WriteLine("4. Create seat map entry");
            Console.WriteLine("5. Update seat map entry");
            Console.WriteLine("6. Delete seat map entry");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            var option = Console.ReadLine()?.Trim();

            switch (option)
            {
                case "1": await ListAllAsync();            break;
                case "2": await GetByIdAsync();            break;
                case "3": await ListByAircraftTypeAsync(); break;
                case "4": await CreateAsync();             break;
                case "5": await UpdateAsync();             break;
                case "6": await DeleteAsync();             break;
                case "0": running = false;                 break;
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
        Console.WriteLine("\n--- All Seat Map Entries ---");

        foreach (var s in seats)
            PrintSeat(s);
    }

    private async Task GetByIdAsync()
    {
        Console.Write("Enter seat map ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        var seat = await _service.GetByIdAsync(id);

        if (seat is null)
            Console.WriteLine($"Seat map entry with ID {id} not found.");
        else
            PrintSeat(seat);
    }

    private async Task ListByAircraftTypeAsync()
    {
        Console.Write("Enter Aircraft Type ID: ");
        if (!int.TryParse(Console.ReadLine(), out int aircraftTypeId))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        var seats = await _service.GetByAircraftTypeAsync(aircraftTypeId);
        Console.WriteLine($"\n--- Seat Map for Aircraft Type {aircraftTypeId} ---");
        Console.WriteLine($"  Total seats: {seats.Count()}");

        foreach (var s in seats)
            PrintSeat(s);
    }

    private async Task CreateAsync()
    {
        Console.Write("Aircraft Type ID: ");
        if (!int.TryParse(Console.ReadLine(), out int aircraftTypeId))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("Seat number (e.g. 12A): ");
        var seatNumber = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(seatNumber))
        { Console.WriteLine("Seat number cannot be empty."); return; }

        Console.Write("Cabin Class ID: ");
        if (!int.TryParse(Console.ReadLine(), out int cabinClassId))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("Seat features (optional, e.g. WINDOW — press Enter to skip): ");
        var features = Console.ReadLine()?.Trim();
        string? seatFeatures = string.IsNullOrWhiteSpace(features) ? null : features;

        var created = await _service.CreateAsync(
            aircraftTypeId, seatNumber, cabinClassId, seatFeatures);

        Console.WriteLine($"Seat map entry created: [{created.Id}] {created.SeatNumber}");
    }

    private async Task UpdateAsync()
    {
        Console.Write("Seat map entry ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("New Cabin Class ID: ");
        if (!int.TryParse(Console.ReadLine(), out int cabinClassId))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("New seat features (optional — press Enter to clear): ");
        var features = Console.ReadLine()?.Trim();
        string? seatFeatures = string.IsNullOrWhiteSpace(features) ? null : features;

        await _service.UpdateAsync(id, cabinClassId, seatFeatures);
        Console.WriteLine("Seat map entry updated successfully.");
    }

    private async Task DeleteAsync()
    {
        Console.Write("Enter seat map entry ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        await _service.DeleteAsync(id);
        Console.WriteLine("Seat map entry deleted successfully.");
    }

    // ── Helpers de presentación ───────────────────────────────────────────────

    private static void PrintSeat(SeatMapDto s)
        => Console.WriteLine(
            $"  [{s.Id}] Type: {s.AircraftTypeId} | Seat: {s.SeatNumber} | " +
            $"Class: {s.CabinClassId} | Features: {s.SeatFeatures ?? "—"}");
}
