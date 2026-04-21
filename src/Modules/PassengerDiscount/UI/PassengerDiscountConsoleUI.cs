namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerDiscount.Application.Interfaces;

public sealed class PassengerDiscountConsoleUI
{
    private readonly IPassengerDiscountService _service;

    public PassengerDiscountConsoleUI(IPassengerDiscountService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n========== PASSENGER DISCOUNT MODULE ==========");
            Console.WriteLine("1. List all discounts");
            Console.WriteLine("2. Get discount by ID");
            Console.WriteLine("3. List discounts by reservation detail");
            Console.WriteLine("4. Apply discount");
            Console.WriteLine("5. Adjust amount");
            Console.WriteLine("6. Remove discount");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            var option = Console.ReadLine()?.Trim();

            switch (option)
            {
                case "1": await ListAllAsync();          break;
                case "2": await GetByIdAsync();          break;
                case "3": await ListByDetailAsync();     break;
                case "4": await ApplyDiscountAsync();    break;
                case "5": await AdjustAmountAsync();     break;
                case "6": await RemoveDiscountAsync();   break;
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
        var discounts = await _service.GetAllAsync();
        Console.WriteLine("\n--- All Passenger Discounts ---");
        foreach (var d in discounts) PrintDiscount(d);
    }

    private async Task GetByIdAsync()
    {
        Console.Write("Enter discount ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        var discount = await _service.GetByIdAsync(id);
        if (discount is null) Console.WriteLine($"Passenger discount with ID {id} not found.");
        else                  PrintDiscount(discount);
    }

    private async Task ListByDetailAsync()
    {
        Console.Write("Enter Reservation Detail ID: ");
        if (!int.TryParse(Console.ReadLine(), out int detailId))
        { Console.WriteLine("Invalid ID."); return; }

        var discounts = await _service.GetByReservationDetailAsync(detailId);
        Console.WriteLine($"\n--- Discounts for Reservation Detail {detailId} ---");

        decimal total = 0;
        foreach (var d in discounts)
        {
            PrintDiscount(d);
            total += d.AmountApplied;
        }
        Console.WriteLine($"  Total discounts applied: {total:F2}");
    }

    private async Task ApplyDiscountAsync()
    {
        Console.Write("Reservation Detail ID: ");
        if (!int.TryParse(Console.ReadLine(), out int detailId))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("Discount Type ID: ");
        if (!int.TryParse(Console.ReadLine(), out int discountTypeId))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("Amount applied (>= 0): ");
        if (!decimal.TryParse(Console.ReadLine()?.Replace(',', '.'),
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out decimal amount))
        { Console.WriteLine("Invalid amount."); return; }

        try
        {
            var created = await _service.CreateAsync(detailId, discountTypeId, amount);
            Console.WriteLine(
                $"Discount applied: [{created.Id}] Detail {created.ReservationDetailId} | " +
                $"Type {created.DiscountTypeId} | Amount: {created.AmountApplied:F2}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Validation error: {ex.Message}");
        }
    }

    private async Task AdjustAmountAsync()
    {
        Console.Write("Discount ID to adjust: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("New amount (>= 0): ");
        if (!decimal.TryParse(Console.ReadLine()?.Replace(',', '.'),
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out decimal newAmount))
        { Console.WriteLine("Invalid amount."); return; }

        try
        {
            await _service.AdjustAmountAsync(id, newAmount);
            Console.WriteLine($"Amount adjusted to {newAmount:F2}.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Validation error: {ex.Message}");
        }
    }

    private async Task RemoveDiscountAsync()
    {
        Console.Write("Discount ID to remove: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        await _service.DeleteAsync(id);
        Console.WriteLine("Discount removed successfully.");
    }

    // ── Helpers de presentación ───────────────────────────────────────────────

    private static void PrintDiscount(PassengerDiscountDto d)
        => Console.WriteLine(
            $"  [{d.Id}] Detail: {d.ReservationDetailId} | " +
            $"Type: {d.DiscountTypeId} | Amount: {d.AmountApplied:F2}");
}
