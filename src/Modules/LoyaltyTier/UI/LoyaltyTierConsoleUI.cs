namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTier.Application.Interfaces;

public sealed class LoyaltyTierConsoleUI
{
    private readonly ILoyaltyTierService _service;

    public LoyaltyTierConsoleUI(ILoyaltyTierService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n========== LOYALTY TIER MODULE ==========");
            Console.WriteLine("1. List all tiers");
            Console.WriteLine("2. Get tier by ID");
            Console.WriteLine("3. List tiers by program");
            Console.WriteLine("4. Create tier");
            Console.WriteLine("5. Update tier");
            Console.WriteLine("6. Delete tier");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            var option = Console.ReadLine()?.Trim();

            switch (option)
            {
                case "1": await ListAllAsync();        break;
                case "2": await GetByIdAsync();        break;
                case "3": await ListByProgramAsync();  break;
                case "4": await CreateAsync();         break;
                case "5": await UpdateAsync();         break;
                case "6": await DeleteAsync();         break;
                case "0": running = false;             break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    private async Task ListAllAsync()
    {
        var tiers = await _service.GetAllAsync();
        Console.WriteLine("\n--- All Loyalty Tiers ---");
        foreach (var t in tiers) PrintTier(t);
    }

    private async Task GetByIdAsync()
    {
        Console.Write("Enter tier ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        var t = await _service.GetByIdAsync(id);
        if (t is null) Console.WriteLine($"Tier with ID {id} not found.");
        else           PrintTier(t);
    }

    private async Task ListByProgramAsync()
    {
        Console.Write("Enter Loyalty Program ID: ");
        if (!int.TryParse(Console.ReadLine(), out int programId))
        { Console.WriteLine("Invalid ID."); return; }

        var tiers = await _service.GetByProgramAsync(programId);
        Console.WriteLine($"\n--- Tiers for Program {programId} ---");
        foreach (var t in tiers) PrintTier(t);
    }

    private async Task CreateAsync()
    {
        Console.Write("Loyalty Program ID: ");
        if (!int.TryParse(Console.ReadLine(), out int programId))
        { Console.WriteLine("Invalid."); return; }

        Console.Write("Tier name (e.g. Classic, Silver, Gold, Diamond): ");
        var name = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(name)) { Console.WriteLine("Name cannot be empty."); return; }

        Console.Write("Minimum miles (>= 0, default 0): ");
        if (!int.TryParse(Console.ReadLine(), out int minMiles) || minMiles < 0)
            minMiles = 0;

        Console.Write("Benefits description (optional): ");
        var benefitsInput = Console.ReadLine()?.Trim();
        string? benefits = string.IsNullOrWhiteSpace(benefitsInput) ? null : benefitsInput;

        try
        {
            var created = await _service.CreateAsync(programId, name, minMiles, benefits);
            Console.WriteLine($"Tier created: [{created.Id}]");
            PrintTier(created);
        }
        catch (ArgumentException ex) { Console.WriteLine($"Validation error: {ex.Message}"); }
    }

    private async Task UpdateAsync()
    {
        Console.Write("Tier ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("Invalid."); return; }

        Console.Write("New name: ");
        var name = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(name)) { Console.WriteLine("Name cannot be empty."); return; }

        Console.Write("New minimum miles (>= 0): ");
        if (!int.TryParse(Console.ReadLine(), out int minMiles) || minMiles < 0)
        { Console.WriteLine("Invalid."); return; }

        Console.Write("New benefits (optional, Enter to clear): ");
        var benefitsInput = Console.ReadLine()?.Trim();
        string? benefits = string.IsNullOrWhiteSpace(benefitsInput) ? null : benefitsInput;

        try
        {
            await _service.UpdateAsync(id, name, minMiles, benefits);
            Console.WriteLine("Tier updated successfully.");
        }
        catch (ArgumentException ex) { Console.WriteLine($"Validation error: {ex.Message}"); }
    }

    private async Task DeleteAsync()
    {
        Console.Write("Tier ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("Invalid."); return; }

        await _service.DeleteAsync(id);
        Console.WriteLine("Tier deleted successfully.");
    }

    private static void PrintTier(LoyaltyTierDto t)
        => Console.WriteLine(
            $"  [{t.Id}] [{t.LoyaltyProgramId}] {t.Name} | Min: {t.MinMiles:N0} miles" +
            (t.Benefits is not null ? $" | {t.Benefits[..Math.Min(50, t.Benefits.Length)]}..." : string.Empty));
}
