namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PaymentMethod.Application.Interfaces;

public sealed class PaymentMethodConsoleUI
{
    private readonly IPaymentMethodService _service;

    public PaymentMethodConsoleUI(IPaymentMethodService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n========== PAYMENT METHOD MODULE ==========");
            Console.WriteLine("1. List all payment methods");
            Console.WriteLine("2. Get payment method by ID");
            Console.WriteLine("3. Create payment method");
            Console.WriteLine("4. Update payment method");
            Console.WriteLine("5. Delete payment method");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            var option = Console.ReadLine()?.Trim();

            switch (option)
            {
                case "1": await ListAllAsync();  break;
                case "2": await GetByIdAsync();  break;
                case "3": await CreateAsync();   break;
                case "4": await UpdateAsync();   break;
                case "5": await DeleteAsync();   break;
                case "0": running = false;       break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    // ── Handlers ──────────────────────────────────────────────────────────────

    private async Task ListAllAsync()
    {
        var methods = await _service.GetAllAsync();
        Console.WriteLine("\n--- All Payment Methods ---");

        foreach (var m in methods)
            Console.WriteLine($"  [{m.Id}] {m.Name}");
    }

    private async Task GetByIdAsync()
    {
        Console.Write("Enter payment method ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        var method = await _service.GetByIdAsync(id);

        if (method is null)
            Console.WriteLine($"Payment method with ID {id} not found.");
        else
            Console.WriteLine($"  [{method.Id}] {method.Name}");
    }

    private async Task CreateAsync()
    {
        Console.Write("Enter payment method name (e.g. CREDIT_CARD, DEBIT_CARD, CASH, TRANSFER): ");
        var name = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Name cannot be empty.");
            return;
        }

        var created = await _service.CreateAsync(name);
        Console.WriteLine($"Payment method created: [{created.Id}] {created.Name}");
    }

    private async Task UpdateAsync()
    {
        Console.Write("Enter payment method ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        Console.Write("Enter new name: ");
        var newName = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(newName))
        {
            Console.WriteLine("Name cannot be empty.");
            return;
        }

        await _service.UpdateAsync(id, newName);
        Console.WriteLine("Payment method updated successfully.");
    }

    private async Task DeleteAsync()
    {
        Console.Write("Enter payment method ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        await _service.DeleteAsync(id);
        Console.WriteLine("Payment method deleted successfully.");
    }
}
