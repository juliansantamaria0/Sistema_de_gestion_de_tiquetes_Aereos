namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.TicketBaggage.Application.Interfaces;

public sealed class TicketBaggageConsoleUI
{
    private readonly ITicketBaggageService _service;

    public TicketBaggageConsoleUI(ITicketBaggageService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n========== TICKET BAGGAGE MODULE ==========");
            Console.WriteLine("1. List all ticket baggage records");
            Console.WriteLine("2. Get record by ID");
            Console.WriteLine("3. List baggage by ticket");
            Console.WriteLine("4. Add baggage to ticket");
            Console.WriteLine("5. Update quantity and fee");
            Console.WriteLine("6. Remove baggage record");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            var option = Console.ReadLine()?.Trim();

            switch (option)
            {
                case "1": await ListAllAsync();        break;
                case "2": await GetByIdAsync();        break;
                case "3": await ListByTicketAsync();   break;
                case "4": await AddBaggageAsync();     break;
                case "5": await UpdateAsync();         break;
                case "6": await RemoveAsync();         break;
                case "0": running = false;             break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    // ── Handlers ──────────────────────────────────────────────────────────────

    private async Task ListAllAsync()
    {
        var records = await _service.GetAllAsync();
        Console.WriteLine("\n--- All Ticket Baggage Records ---");
        foreach (var r in records) PrintRecord(r);
    }

    private async Task GetByIdAsync()
    {
        Console.Write("Enter record ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        var r = await _service.GetByIdAsync(id);
        if (r is null) Console.WriteLine($"Record with ID {id} not found.");
        else           PrintRecord(r);
    }

    private async Task ListByTicketAsync()
    {
        Console.Write("Enter Ticket ID: ");
        if (!int.TryParse(Console.ReadLine(), out int ticketId))
        { Console.WriteLine("Invalid ID."); return; }

        var records = await _service.GetByTicketAsync(ticketId);
        var list    = records.ToList();
        Console.WriteLine($"\n--- Baggage for Ticket {ticketId} ---");

        decimal totalFee = list.Sum(r => r.FeeCharged);
        foreach (var r in list) PrintRecord(r);
        Console.WriteLine($"  Total baggage fee: {totalFee:F2}");
    }

    private async Task AddBaggageAsync()
    {
        Console.Write("Ticket ID: ");
        if (!int.TryParse(Console.ReadLine(), out int ticketId))
        { Console.WriteLine("Invalid."); return; }

        Console.Write("Baggage Type ID: ");
        if (!int.TryParse(Console.ReadLine(), out int baggageTypeId))
        { Console.WriteLine("Invalid."); return; }

        Console.Write("Quantity (> 0, default 1): ");
        if (!int.TryParse(Console.ReadLine(), out int qty) || qty <= 0)
            qty = 1;

        Console.Write("Fee charged (>= 0, default 0): ");
        if (!decimal.TryParse(Console.ReadLine()?.Replace(',', '.'),
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out decimal fee))
            fee = 0m;

        try
        {
            var created = await _service.CreateAsync(ticketId, baggageTypeId, qty, fee);
            Console.WriteLine(
                $"Baggage added: [{created.Id}] Ticket:{created.TicketId} | " +
                $"Type:{created.BaggageTypeId} | Qty:{created.Quantity} | Fee:{created.FeeCharged:F2}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Validation error: {ex.Message}");
        }
    }

    private async Task UpdateAsync()
    {
        Console.Write("Record ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid."); return; }

        Console.Write("New quantity (> 0): ");
        if (!int.TryParse(Console.ReadLine(), out int qty) || qty <= 0)
        { Console.WriteLine("Invalid quantity."); return; }

        Console.Write("New fee charged (>= 0): ");
        if (!decimal.TryParse(Console.ReadLine()?.Replace(',', '.'),
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out decimal fee))
        { Console.WriteLine("Invalid fee."); return; }

        try
        {
            await _service.UpdateQuantityAndFeeAsync(id, qty, fee);
            Console.WriteLine("Baggage record updated successfully.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Validation error: {ex.Message}");
        }
    }

    private async Task RemoveAsync()
    {
        Console.Write("Record ID to remove: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid."); return; }

        await _service.DeleteAsync(id);
        Console.WriteLine("Baggage record removed successfully.");
    }

    // ── Helpers de presentación ───────────────────────────────────────────────

    private static void PrintRecord(TicketBaggageDto r)
        => Console.WriteLine(
            $"  [{r.Id}] Ticket:{r.TicketId} | Type:{r.BaggageTypeId} | " +
            $"Qty:{r.Quantity} | Fee:{r.FeeCharged:F2}");
}
