namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Ticket.Application.Interfaces;

public sealed class TicketConsoleUI
{
    private readonly ITicketService _service;

    public TicketConsoleUI(ITicketService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n========== TICKET MODULE ==========");
            Console.WriteLine("1. List all tickets");
            Console.WriteLine("2. Get ticket by ID");
            Console.WriteLine("3. Get ticket by reservation detail");
            Console.WriteLine("4. Issue ticket");
            Console.WriteLine("5. Change ticket status");
            Console.WriteLine("6. Delete ticket");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            var option = Console.ReadLine()?.Trim();

            switch (option)
            {
                case "1": await ListAllAsync();           break;
                case "2": await GetByIdAsync();           break;
                case "3": await GetByDetailAsync();       break;
                case "4": await IssueTicketAsync();       break;
                case "5": await ChangeStatusAsync();      break;
                case "6": await DeleteAsync();            break;
                case "0": running = false;                break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    // ── Handlers ──────────────────────────────────────────────────────────────

    private async Task ListAllAsync()
    {
        var tickets = await _service.GetAllAsync();
        Console.WriteLine("\n--- All Tickets ---");
        foreach (var t in tickets) PrintTicket(t);
    }

    private async Task GetByIdAsync()
    {
        Console.Write("Enter ticket ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        var t = await _service.GetByIdAsync(id);
        if (t is null) Console.WriteLine($"Ticket with ID {id} not found.");
        else           PrintTicket(t);
    }

    private async Task GetByDetailAsync()
    {
        Console.Write("Enter Reservation Detail ID: ");
        if (!int.TryParse(Console.ReadLine(), out int detailId))
        { Console.WriteLine("Invalid ID."); return; }

        var t = await _service.GetByReservationDetailAsync(detailId);
        if (t is null)
            Console.WriteLine($"No ticket issued for reservation detail {detailId}.");
        else
            PrintTicket(t);
    }

    private async Task IssueTicketAsync()
    {
        Console.Write("Ticket code (max 30 chars): ");
        var code = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(code))
        { Console.WriteLine("Code cannot be empty."); return; }

        Console.Write("Reservation Detail ID: ");
        if (!int.TryParse(Console.ReadLine(), out int detailId))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("Initial Ticket Status ID: ");
        if (!int.TryParse(Console.ReadLine(), out int statusId))
        { Console.WriteLine("Invalid ID."); return; }

        try
        {
            var created = await _service.CreateAsync(code, detailId, statusId);
            Console.WriteLine(
                $"Ticket issued: [{created.Id}] {created.TicketCode} | " +
                $"Detail:{created.ReservationDetailId} | Status:{created.TicketStatusId} | " +
                $"Issued:{created.IssueDate:yyyy-MM-dd HH:mm}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Validation error: {ex.Message}");
        }
    }

    private async Task ChangeStatusAsync()
    {
        Console.Write("Ticket ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("New Ticket Status ID: ");
        if (!int.TryParse(Console.ReadLine(), out int statusId))
        { Console.WriteLine("Invalid ID."); return; }

        await _service.ChangeStatusAsync(id, statusId);
        Console.WriteLine("Ticket status changed successfully.");
    }

    private async Task DeleteAsync()
    {
        Console.Write("Ticket ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        await _service.DeleteAsync(id);
        Console.WriteLine("Ticket deleted successfully.");
    }

    // ── Helpers de presentación ───────────────────────────────────────────────

    private static void PrintTicket(TicketDto t)
        => Console.WriteLine(
            $"  [{t.Id}] {t.TicketCode} | Detail:{t.ReservationDetailId} | " +
            $"Status:{t.TicketStatusId} | Issued:{t.IssueDate:yyyy-MM-dd HH:mm}" +
            (t.UpdatedAt.HasValue ? $" | Updated:{t.UpdatedAt:yyyy-MM-dd HH:mm}" : string.Empty));
}
