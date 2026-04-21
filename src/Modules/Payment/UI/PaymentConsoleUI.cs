namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Payment.Application.Interfaces;

public sealed class PaymentConsoleUI
{
    private readonly IPaymentService _service;

    public PaymentConsoleUI(IPaymentService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n========== PAYMENT MODULE ==========");
            Console.WriteLine("1. List all payments");
            Console.WriteLine("2. Get payment by ID");
            Console.WriteLine("3. List by reservation");
            Console.WriteLine("4. List by ticket");
            Console.WriteLine("5. Register payment");
            Console.WriteLine("6. Update payment status");
            Console.WriteLine("7. Delete payment");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            var option = Console.ReadLine()?.Trim();

            switch (option)
            {
                case "1": await ListAllAsync();             break;
                case "2": await GetByIdAsync();             break;
                case "3": await ListByReservationAsync();   break;
                case "4": await ListByTicketAsync();        break;
                case "5": await RegisterPaymentAsync();     break;
                case "6": await UpdateStatusAsync();        break;
                case "7": await DeleteAsync();              break;
                case "0": running = false;                  break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    // ── Handlers ──────────────────────────────────────────────────────────────

    private async Task ListAllAsync()
    {
        var payments = await _service.GetAllAsync();
        Console.WriteLine("\n--- All Payments ---");
        foreach (var p in payments) PrintPayment(p);
    }

    private async Task GetByIdAsync()
    {
        Console.Write("Enter payment ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        var p = await _service.GetByIdAsync(id);
        if (p is null) Console.WriteLine($"Payment with ID {id} not found.");
        else           PrintPayment(p);
    }

    private async Task ListByReservationAsync()
    {
        Console.Write("Enter Reservation ID: ");
        if (!int.TryParse(Console.ReadLine(), out int resId))
        { Console.WriteLine("Invalid ID."); return; }

        var payments = await _service.GetByReservationAsync(resId);
        Console.WriteLine($"\n--- Payments for Reservation {resId} ---");
        foreach (var p in payments) PrintPayment(p);
    }

    private async Task ListByTicketAsync()
    {
        Console.Write("Enter Ticket ID: ");
        if (!int.TryParse(Console.ReadLine(), out int tickId))
        { Console.WriteLine("Invalid ID."); return; }

        var payments = await _service.GetByTicketAsync(tickId);
        Console.WriteLine($"\n--- Payments for Ticket {tickId} ---");
        foreach (var p in payments) PrintPayment(p);
    }

    private async Task RegisterPaymentAsync()
    {
        Console.WriteLine("Link to: 1) Reservation  2) Ticket");
        Console.Write("Select: ");
        var linkType = Console.ReadLine()?.Trim();

        int? reservationId = null;
        int? ticketId      = null;

        if (linkType == "1")
        {
            Console.Write("Reservation ID: ");
            if (!int.TryParse(Console.ReadLine(), out int rId)) { Console.WriteLine("Invalid."); return; }
            reservationId = rId;
        }
        else if (linkType == "2")
        {
            Console.Write("Ticket ID: ");
            if (!int.TryParse(Console.ReadLine(), out int tId)) { Console.WriteLine("Invalid."); return; }
            ticketId = tId;
        }
        else { Console.WriteLine("Invalid selection."); return; }

        Console.Write("Currency ID: ");
        if (!int.TryParse(Console.ReadLine(), out int currencyId)) { Console.WriteLine("Invalid."); return; }

        Console.Write("Amount (>= 0): ");
        if (!decimal.TryParse(Console.ReadLine()?.Replace(',', '.'),
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out decimal amount)) { Console.WriteLine("Invalid amount."); return; }

        Console.Write("Payment Status ID: ");
        if (!int.TryParse(Console.ReadLine(), out int statusId)) { Console.WriteLine("Invalid."); return; }

        Console.Write("Payment Method ID: ");
        if (!int.TryParse(Console.ReadLine(), out int methodId)) { Console.WriteLine("Invalid."); return; }

        Console.Write("Transaction reference (optional): ");
        var refInput = Console.ReadLine()?.Trim();
        string? txRef = string.IsNullOrWhiteSpace(refInput) ? null : refInput;

        try
        {
            var request = new CreatePaymentRequest(
                reservationId, ticketId, currencyId, amount, statusId, methodId, txRef);
            var created = await _service.CreateAsync(request);
            Console.WriteLine($"Payment registered: [{created.Id}] Amount: {created.Amount:F2} | Status: {created.PaymentStatusId}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Validation error: {ex.Message}");
        }
    }

    private async Task UpdateStatusAsync()
    {
        Console.Write("Payment ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("Invalid."); return; }

        Console.Write("New Payment Status ID: ");
        if (!int.TryParse(Console.ReadLine(), out int statusId)) { Console.WriteLine("Invalid."); return; }

        Console.Write("Transaction reference (optional): ");
        var refInput = Console.ReadLine()?.Trim();
        string? txRef = string.IsNullOrWhiteSpace(refInput) ? null : refInput;

        Console.Write("Rejection reason (optional): ");
        var rejInput = Console.ReadLine()?.Trim();
        string? rejection = string.IsNullOrWhiteSpace(rejInput) ? null : rejInput;

        try
        {
            await _service.UpdateStatusAsync(id, statusId, txRef, rejection);
            Console.WriteLine("Payment status updated successfully.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Validation error: {ex.Message}");
        }
    }

    private async Task DeleteAsync()
    {
        Console.Write("Payment ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("Invalid."); return; }

        await _service.DeleteAsync(id);
        Console.WriteLine("Payment deleted successfully.");
    }

    // ── Helpers de presentación ───────────────────────────────────────────────

    private static void PrintPayment(PaymentDto p)
    {
        var target = p.ReservationId.HasValue
            ? $"Reservation:{p.ReservationId}"
            : $"Ticket:{p.TicketId}";

        Console.WriteLine(
            $"  [{p.Id}] {target} | Currency:{p.CurrencyId} | " +
            $"Amount:{p.Amount:F2} | Status:{p.PaymentStatusId} | Method:{p.PaymentMethodId} | " +
            $"Date:{p.PaymentDate:yyyy-MM-dd HH:mm}" +
            (p.TransactionReference is not null ? $" | Ref:{p.TransactionReference}" : string.Empty));
    }
}
