namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketStatusHistory.Application.Interfaces;

public sealed class TicketStatusHistoryConsoleUI
{
    private readonly ITicketStatusHistoryService _service;

    public TicketStatusHistoryConsoleUI(ITicketStatusHistoryService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n========== TICKET STATUS HISTORY MODULE ==========");
            Console.WriteLine("1. List all history entries");
            Console.WriteLine("2. Get entry by ID");
            Console.WriteLine("3. List history by ticket");
            Console.WriteLine("4. Record status change");
            Console.WriteLine("5. Delete entry");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            var option = Console.ReadLine()?.Trim();

            switch (option)
            {
                case "1": await ListAllAsync();      break;
                case "2": await GetByIdAsync();      break;
                case "3": await ListByTicketAsync(); break;
                case "4": await RecordAsync();       break;
                case "5": await DeleteAsync();       break;
                case "0": running = false;           break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    private async Task ListAllAsync()
    {
        var entries = await _service.GetAllAsync();
        Console.WriteLine("\n--- All Ticket Status History ---");
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

    private async Task ListByTicketAsync()
    {
        Console.Write("Enter Ticket ID: ");
        if (!int.TryParse(Console.ReadLine(), out int ticketId))
        { Console.WriteLine("Invalid ID."); return; }

        var entries = await _service.GetByTicketAsync(ticketId);
        Console.WriteLine($"\n--- Status History for Ticket {ticketId} ---");
        foreach (var e in entries) PrintEntry(e);
    }

    private async Task RecordAsync()
    {
        Console.Write("Ticket ID: ");
        if (!int.TryParse(Console.ReadLine(), out int ticketId))
        { Console.WriteLine("Invalid."); return; }

        Console.Write("Ticket Status ID: ");
        if (!int.TryParse(Console.ReadLine(), out int statusId))
        { Console.WriteLine("Invalid."); return; }

        Console.Write("Notes (optional): ");
        var notesInput = Console.ReadLine()?.Trim();
        string? notes = string.IsNullOrWhiteSpace(notesInput) ? null : notesInput;

        var entry = await _service.RecordAsync(ticketId, statusId, notes);
        Console.WriteLine($"Recorded: [{entry.Id}] Ticket:{entry.TicketId} → Status:{entry.TicketStatusId} | {entry.ChangedAt:yyyy-MM-dd HH:mm}");
    }

    private async Task DeleteAsync()
    {
        Console.Write("Entry ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid."); return; }

        await _service.DeleteAsync(id);
        Console.WriteLine("Entry deleted successfully.");
    }

    private static void PrintEntry(TicketStatusHistoryDto e)
        => Console.WriteLine(
            $"  [{e.Id}] Ticket:{e.TicketId} → Status:{e.TicketStatusId} | " +
            $"{e.ChangedAt:yyyy-MM-dd HH:mm}" +
            (e.Notes is not null ? $" | {e.Notes}" : string.Empty));
}
