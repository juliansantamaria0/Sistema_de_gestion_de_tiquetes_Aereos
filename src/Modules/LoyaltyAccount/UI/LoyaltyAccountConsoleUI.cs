namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Application.Interfaces;

public sealed class LoyaltyAccountConsoleUI
{
    private readonly ILoyaltyAccountService _service;

    public LoyaltyAccountConsoleUI(ILoyaltyAccountService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n========== LOYALTY ACCOUNT MODULE ==========");
            Console.WriteLine("1. List all accounts");
            Console.WriteLine("2. Get account by ID");
            Console.WriteLine("3. Get accounts by passenger");
            Console.WriteLine("4. Enroll passenger");
            Console.WriteLine("5. Add miles");
            Console.WriteLine("6. Redeem miles");
            Console.WriteLine("7. Upgrade tier");
            Console.WriteLine("8. Delete account");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            var option = Console.ReadLine()?.Trim();

            switch (option)
            {
                case "1": await ListAllAsync();          break;
                case "2": await GetByIdAsync();          break;
                case "3": await ListByPassengerAsync();  break;
                case "4": await EnrollAsync();           break;
                case "5": await AddMilesAsync();         break;
                case "6": await RedeemMilesAsync();      break;
                case "7": await UpgradeTierAsync();      break;
                case "8": await DeleteAsync();           break;
                case "0": running = false;               break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    private async Task ListAllAsync()
    {
        var accounts = await _service.GetAllAsync();
        Console.WriteLine("\n--- All Loyalty Accounts ---");
        foreach (var a in accounts) PrintAccount(a);
    }

    private async Task GetByIdAsync()
    {
        Console.Write("Enter account ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        var a = await _service.GetByIdAsync(id);
        if (a is null) Console.WriteLine($"Account with ID {id} not found.");
        else           PrintAccount(a);
    }

    private async Task ListByPassengerAsync()
    {
        Console.Write("Enter Passenger ID: ");
        if (!int.TryParse(Console.ReadLine(), out int passengerId))
        { Console.WriteLine("Invalid ID."); return; }

        var accounts = await _service.GetByPassengerAsync(passengerId);
        Console.WriteLine($"\n--- Loyalty Accounts for Passenger {passengerId} ---");
        foreach (var a in accounts) PrintAccount(a);
    }

    private async Task EnrollAsync()
    {
        Console.Write("Passenger ID: ");
        if (!int.TryParse(Console.ReadLine(), out int passengerId)) { Console.WriteLine("Invalid."); return; }

        Console.Write("Loyalty Program ID: ");
        if (!int.TryParse(Console.ReadLine(), out int programId)) { Console.WriteLine("Invalid."); return; }

        Console.Write("Initial Tier ID: ");
        if (!int.TryParse(Console.ReadLine(), out int tierId)) { Console.WriteLine("Invalid."); return; }

        try
        {
            var created = await _service.CreateAsync(passengerId, programId, tierId);
            Console.WriteLine($"Enrolled: [{created.Id}]");
            PrintAccount(created);
        }
        catch (ArgumentException ex) { Console.WriteLine($"Validation error: {ex.Message}"); }
    }

    private async Task AddMilesAsync()
    {
        Console.Write("Account ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("Invalid."); return; }

        Console.Write("Miles to add (> 0): ");
        if (!int.TryParse(Console.ReadLine(), out int miles) || miles <= 0)
        { Console.WriteLine("Invalid miles."); return; }

        try
        {
            await _service.AddMilesAsync(id, miles);
            Console.WriteLine($"{miles:N0} miles added successfully.");
        }
        catch (ArgumentException ex) { Console.WriteLine($"Validation error: {ex.Message}"); }
    }

    private async Task RedeemMilesAsync()
    {
        Console.Write("Account ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("Invalid."); return; }

        Console.Write("Miles to redeem (> 0): ");
        if (!int.TryParse(Console.ReadLine(), out int miles) || miles <= 0)
        { Console.WriteLine("Invalid miles."); return; }

        try
        {
            await _service.RedeemMilesAsync(id, miles);
            Console.WriteLine($"{miles:N0} miles redeemed successfully.");
        }
        catch (InvalidOperationException ex) { Console.WriteLine($"Business rule error: {ex.Message}"); }
        catch (ArgumentException ex)         { Console.WriteLine($"Validation error: {ex.Message}"); }
    }

    private async Task UpgradeTierAsync()
    {
        Console.Write("Account ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("Invalid."); return; }

        Console.Write("New Tier ID: ");
        if (!int.TryParse(Console.ReadLine(), out int tierId)) { Console.WriteLine("Invalid."); return; }

        await _service.UpgradeTierAsync(id, tierId);
        Console.WriteLine("Tier upgraded successfully.");
    }

    private async Task DeleteAsync()
    {
        Console.Write("Account ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("Invalid."); return; }

        await _service.DeleteAsync(id);
        Console.WriteLine("Account deleted successfully.");
    }

    private static void PrintAccount(LoyaltyAccountDto a)
        => Console.WriteLine(
            $"  [{a.Id}] Passenger:{a.PassengerId} | Program:{a.LoyaltyProgramId} | " +
            $"Tier:{a.LoyaltyTierId} | Total:{a.TotalMiles:N0} | " +
            $"Available:{a.AvailableMiles:N0} | Joined:{a.JoinedAt:yyyy-MM-dd}");
}
