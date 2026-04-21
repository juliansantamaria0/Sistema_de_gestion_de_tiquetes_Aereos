namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Refund.Application.Interfaces;

public sealed class RefundConsoleUI
{
    private readonly IRefundService _service;

    public RefundConsoleUI(IRefundService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n========== REFUND MODULE ==========");
            Console.WriteLine("1. List all refunds");
            Console.WriteLine("2. Get refund by ID");
            Console.WriteLine("3. List refunds by payment");
            Console.WriteLine("4. Request refund");
            Console.WriteLine("5. Update refund status");
            Console.WriteLine("6. Delete refund");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            var option = Console.ReadLine()?.Trim();

            switch (option)
            {
                case "1": await ListAllAsync();          break;
                case "2": await GetByIdAsync();          break;
                case "3": await ListByPaymentAsync();    break;
                case "4": await RequestRefundAsync();    break;
                case "5": await UpdateStatusAsync();     break;
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
        var refunds = await _service.GetAllAsync();
        Console.WriteLine("\n--- All Refunds ---");
        foreach (var r in refunds) PrintRefund(r);
    }

    private async Task GetByIdAsync()
    {
        Console.Write("Enter refund ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        var r = await _service.GetByIdAsync(id);
        if (r is null) Console.WriteLine($"Refund with ID {id} not found.");
        else           PrintRefund(r);
    }

    private async Task ListByPaymentAsync()
    {
        Console.Write("Enter Payment ID: ");
        if (!int.TryParse(Console.ReadLine(), out int paymentId))
        { Console.WriteLine("Invalid ID."); return; }

        var refunds = await _service.GetByPaymentAsync(paymentId);
        Console.WriteLine($"\n--- Refunds for Payment {paymentId} ---");
        foreach (var r in refunds) PrintRefund(r);
    }

    private async Task RequestRefundAsync()
    {
        Console.Write("Payment ID: ");
        if (!int.TryParse(Console.ReadLine(), out int paymentId))
        { Console.WriteLine("Invalid."); return; }

        Console.Write("Initial Refund Status ID: ");
        if (!int.TryParse(Console.ReadLine(), out int statusId))
        { Console.WriteLine("Invalid."); return; }

        Console.Write("Amount to refund (>= 0): ");
        if (!decimal.TryParse(Console.ReadLine()?.Replace(',', '.'),
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out decimal amount))
        { Console.WriteLine("Invalid amount."); return; }

        Console.Write("Reason (optional): ");
        var reasonInput = Console.ReadLine()?.Trim();
        string? reason = string.IsNullOrWhiteSpace(reasonInput) ? null : reasonInput;

        try
        {
            var created = await _service.CreateAsync(paymentId, statusId, amount, reason);
            Console.WriteLine(
                $"Refund requested: [{created.Id}] Payment:{created.PaymentId} | " +
                $"Amount:{created.Amount:F2} | Status:{created.RefundStatusId} | " +
                $"Requested:{created.RequestedAt:yyyy-MM-dd HH:mm}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Validation error: {ex.Message}");
        }
    }

    private async Task UpdateStatusAsync()
    {
        Console.Write("Refund ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid."); return; }

        Console.Write("New Refund Status ID: ");
        if (!int.TryParse(Console.ReadLine(), out int statusId))
        { Console.WriteLine("Invalid."); return; }

        Console.Write("Processed at (yyyy-MM-dd HH:mm, optional): ");
        var procInput = Console.ReadLine()?.Trim();
        DateTime? processedAt = DateTime.TryParse(procInput, out var parsedDt) ? parsedDt : null;

        Console.Write("Reason (optional, Enter to keep current): ");
        var reasonInput = Console.ReadLine()?.Trim();
        string? reason = string.IsNullOrWhiteSpace(reasonInput) ? null : reasonInput;

        try
        {
            await _service.UpdateStatusAsync(id, statusId, processedAt, reason);
            Console.WriteLine("Refund status updated successfully.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Validation error: {ex.Message}");
        }
    }

    private async Task DeleteAsync()
    {
        Console.Write("Refund ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid."); return; }

        await _service.DeleteAsync(id);
        Console.WriteLine("Refund deleted successfully.");
    }

    // ── Helpers de presentación ───────────────────────────────────────────────

    private static void PrintRefund(RefundDto r)
        => Console.WriteLine(
            $"  [{r.Id}] Payment:{r.PaymentId} | Status:{r.RefundStatusId} | " +
            $"Amount:{r.Amount:F2} | Requested:{r.RequestedAt:yyyy-MM-dd HH:mm}" +
            (r.ProcessedAt.HasValue ? $" | Processed:{r.ProcessedAt:yyyy-MM-dd HH:mm}" : string.Empty) +
            (r.Reason is not null ? $" | Reason:{r.Reason}" : string.Empty));
}
