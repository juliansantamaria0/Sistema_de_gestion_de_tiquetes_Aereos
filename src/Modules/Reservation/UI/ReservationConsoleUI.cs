namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Reservation.Application.Interfaces;

public sealed class ReservationConsoleUI
{
    private readonly IReservationService _service;

    public ReservationConsoleUI(IReservationService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n========== RESERVATION MODULE ==========");
            Console.WriteLine("1.  List all reservations");
            Console.WriteLine("2.  Get reservation by ID");
            Console.WriteLine("3.  List by customer");
            Console.WriteLine("4.  List by flight");
            Console.WriteLine("5.  Create reservation");
            Console.WriteLine("6.  Confirm reservation");
            Console.WriteLine("7.  Cancel reservation");
            Console.WriteLine("8.  Change status");
            Console.WriteLine("9.  Delete reservation");
            Console.WriteLine("0.  Exit");
            Console.Write("Select an option: ");

            var option = Console.ReadLine()?.Trim();

            switch (option)
            {
                case "1": await ListAllAsync();          break;
                case "2": await GetByIdAsync();          break;
                case "3": await ListByCustomerAsync();   break;
                case "4": await ListByFlightAsync();     break;
                case "5": await CreateAsync();           break;
                case "6": await ConfirmAsync();          break;
                case "7": await CancelAsync();           break;
                case "8": await ChangeStatusAsync();     break;
                case "9": await DeleteAsync();           break;
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
        var list = await _service.GetAllAsync();
        Console.WriteLine("\n--- All Reservations ---");
        foreach (var r in list) PrintReservation(r);
    }

    private async Task GetByIdAsync()
    {
        Console.Write("Enter reservation ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        var r = await _service.GetByIdAsync(id);
        if (r is null) Console.WriteLine($"Reservation with ID {id} not found.");
        else           PrintReservation(r);
    }

    private async Task ListByCustomerAsync()
    {
        Console.Write("Enter Customer ID: ");
        if (!int.TryParse(Console.ReadLine(), out int customerId))
        { Console.WriteLine("Invalid ID."); return; }

        var list = await _service.GetByCustomerAsync(customerId);
        Console.WriteLine($"\n--- Reservations for Customer {customerId} ---");
        foreach (var r in list) PrintReservation(r);
    }

    private async Task ListByFlightAsync()
    {
        Console.Write("Enter Scheduled Flight ID: ");
        if (!int.TryParse(Console.ReadLine(), out int flightId))
        { Console.WriteLine("Invalid ID."); return; }

        var list = await _service.GetByFlightAsync(flightId);
        Console.WriteLine($"\n--- Reservations for Flight {flightId} ---");
        foreach (var r in list) PrintReservation(r);
    }

    private async Task CreateAsync()
    {
        Console.Write("Reservation code (max 20 chars): ");
        var code = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(code))
        { Console.WriteLine("Code cannot be empty."); return; }

        Console.Write("Customer ID: ");
        if (!int.TryParse(Console.ReadLine(), out int customerId))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("Scheduled Flight ID: ");
        if (!int.TryParse(Console.ReadLine(), out int flightId))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("Initial Reservation Status ID: ");
        if (!int.TryParse(Console.ReadLine(), out int statusId))
        { Console.WriteLine("Invalid ID."); return; }

        try
        {
            var created = await _service.CreateAsync(code, customerId, flightId, statusId);
            Console.WriteLine($"Reservation created: [{created.Id}] {created.ReservationCode}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Validation error: {ex.Message}");
        }
    }

    private async Task ConfirmAsync()
    {
        Console.Write("Reservation ID to confirm: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("Confirmed Status ID (CONFIRMED): ");
        if (!int.TryParse(Console.ReadLine(), out int statusId))
        { Console.WriteLine("Invalid ID."); return; }

        try
        {
            await _service.ConfirmAsync(id, statusId);
            Console.WriteLine("Reservation confirmed successfully.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Business rule error: {ex.Message}");
        }
    }

    private async Task CancelAsync()
    {
        Console.Write("Reservation ID to cancel: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("Cancelled Status ID (CANCELLED): ");
        if (!int.TryParse(Console.ReadLine(), out int statusId))
        { Console.WriteLine("Invalid ID."); return; }

        try
        {
            await _service.CancelAsync(id, statusId);
            Console.WriteLine("Reservation cancelled successfully.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Business rule error: {ex.Message}");
        }
    }

    private async Task ChangeStatusAsync()
    {
        Console.Write("Reservation ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("New Status ID: ");
        if (!int.TryParse(Console.ReadLine(), out int statusId))
        { Console.WriteLine("Invalid ID."); return; }

        await _service.ChangeStatusAsync(id, statusId);
        Console.WriteLine("Status changed successfully.");
    }

    private async Task DeleteAsync()
    {
        Console.Write("Enter reservation ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        await _service.DeleteAsync(id);
        Console.WriteLine("Reservation deleted successfully.");
    }

    // ── Helpers de presentación ───────────────────────────────────────────────

    private static void PrintReservation(ReservationDto r)
    {
        var state = r.ConfirmedAt.HasValue ? $"CONFIRMED at {r.ConfirmedAt:yyyy-MM-dd HH:mm}"
                  : r.CancelledAt.HasValue ? $"CANCELLED at {r.CancelledAt:yyyy-MM-dd HH:mm}"
                  : "PENDING";

        Console.WriteLine(
            $"  [{r.Id}] {r.ReservationCode} | " +
            $"Customer: {r.CustomerId} | Flight: {r.ScheduledFlightId} | " +
            $"Status: {r.ReservationStatusId} | {state} | " +
            $"Created: {r.CreatedAt:yyyy-MM-dd HH:mm}");
    }
}
