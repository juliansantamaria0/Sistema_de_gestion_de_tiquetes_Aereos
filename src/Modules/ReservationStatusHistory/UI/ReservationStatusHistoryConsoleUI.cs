namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ReservationStatusHistory.Application.Interfaces;

public sealed class ReservationStatusHistoryConsoleUI
{
    private readonly IReservationStatusHistoryService _service;

    public ReservationStatusHistoryConsoleUI(IReservationStatusHistoryService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n========== RESERVATION STATUS HISTORY MODULE ==========");
            Console.WriteLine("1. List all history entries");
            Console.WriteLine("2. Get entry by ID");
            Console.WriteLine("3. List history by reservation");
            Console.WriteLine("4. Record status change");
            Console.WriteLine("5. Delete entry");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            var option = Console.ReadLine()?.Trim();

            switch (option)
            {
                case "1": await ListAllAsync();           break;
                case "2": await GetByIdAsync();           break;
                case "3": await ListByReservationAsync(); break;
                case "4": await RecordAsync();            break;
                case "5": await DeleteAsync();            break;
                case "0": running = false;                break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    private async Task ListAllAsync()
    {
        var entries = await _service.GetAllAsync();
        Console.WriteLine("\n--- All Reservation Status History ---");
        foreach (var e in entries) PrintEntry(e);
    }

    private async Task GetByIdAsync()
    {
        Console.Write("Enter entry ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        var entry = await _service.GetByIdAsync(id);
        if (entry is null) Console.WriteLine($"Entry with ID {id} not found.");
        else               PrintEntry(entry);
    }

    private async Task ListByReservationAsync()
    {
        Console.Write("Enter Reservation ID: ");
        if (!int.TryParse(Console.ReadLine(), out int reservationId))
        { Console.WriteLine("Invalid ID."); return; }

        var entries = await _service.GetByReservationAsync(reservationId);
        Console.WriteLine($"\n--- Status History for Reservation {reservationId} ---");
        foreach (var e in entries) PrintEntry(e);
    }

    private async Task RecordAsync()
    {
        Console.Write("Reservation ID: ");
        if (!int.TryParse(Console.ReadLine(), out int reservationId))
        { Console.WriteLine("Invalid."); return; }

        Console.Write("Reservation Status ID: ");
        if (!int.TryParse(Console.ReadLine(), out int statusId))
        { Console.WriteLine("Invalid."); return; }

        Console.Write("Notes (optional): ");
        var notesInput = Console.ReadLine()?.Trim();
        string? notes = string.IsNullOrWhiteSpace(notesInput) ? null : notesInput;

        var entry = await _service.RecordAsync(reservationId, statusId, notes);
        Console.WriteLine($"Recorded: [{entry.Id}] Reservation:{entry.ReservationId} → Status:{entry.ReservationStatusId} | {entry.ChangedAt:yyyy-MM-dd HH:mm}");
    }

    private async Task DeleteAsync()
    {
        Console.Write("Entry ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid."); return; }

        await _service.DeleteAsync(id);
        Console.WriteLine("Entry deleted successfully.");
    }

    private static void PrintEntry(ReservationStatusHistoryDto e)
        => Console.WriteLine(
            $"  [{e.Id}] Reservation:{e.ReservationId} → Status:{e.ReservationStatusId} | " +
            $"{e.ChangedAt:yyyy-MM-dd HH:mm}" +
            (e.Notes is not null ? $" | {e.Notes}" : string.Empty));
}
