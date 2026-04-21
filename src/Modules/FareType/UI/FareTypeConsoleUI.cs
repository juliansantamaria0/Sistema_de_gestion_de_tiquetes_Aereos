namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Application.Interfaces;

public sealed class FareTypeConsoleUI
{
    private readonly IFareTypeService _service;

    public FareTypeConsoleUI(IFareTypeService service) => _service = service;

    public async Task RunAsync()
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine("\n========== FARE TYPE MODULE ==========");
            Console.WriteLine("1. List all  2. Get by ID  3. Create  4. Update  5. Delete  0. Exit");
            Console.Write("Select: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1":
                    Console.WriteLine("\n--- All Fare Types ---");
                    foreach (var f in await _service.GetAllAsync()) PrintFareType(f);
                    break;

                case "2":
                    Console.Write("ID: ");
                    if (int.TryParse(Console.ReadLine(), out int id))
                    { var f = await _service.GetByIdAsync(id); if (f is not null) PrintFareType(f); else Console.WriteLine("Not found."); }
                    break;

                case "3":
                    var ft = await ReadFareTypeInputAsync();
                    if (ft is not null)
                    {
                        var created = await _service.CreateAsync(ft.Name, ft.IsRefundable, ft.IsChangeable, ft.AdvancePurchaseDays, ft.BaggageIncluded);
                        Console.WriteLine($"Created: [{created.Id}] {created.Name}");
                    }
                    break;

                case "4":
                    Console.Write("ID: "); if (!int.TryParse(Console.ReadLine(), out int uid)) break;
                    var uft = await ReadFareTypeInputAsync();
                    if (uft is not null)
                    { await _service.UpdateAsync(uid, uft.Name, uft.IsRefundable, uft.IsChangeable, uft.AdvancePurchaseDays, uft.BaggageIncluded); Console.WriteLine("Updated."); }
                    break;

                case "5":
                    Console.Write("ID: "); if (!int.TryParse(Console.ReadLine(), out int did)) break;
                    await _service.DeleteAsync(did); Console.WriteLine("Deleted.");
                    break;

                case "0": running = false; break;
                default: Console.WriteLine("Invalid option."); break;
            }
        }
    }

    private static async Task<FareTypeInputModel?> ReadFareTypeInputAsync()
    {
        Console.Write("Name: "); var name = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(name)) { Console.WriteLine("Name cannot be empty."); return null; }

        Console.Write("Is refundable? (y/n): ");
        bool isRefundable = Console.ReadLine()?.Trim().ToLower() == "y";

        Console.Write("Is changeable? (y/n): ");
        bool isChangeable = Console.ReadLine()?.Trim().ToLower() == "y";

        Console.Write("Advance purchase days (>= 0, default 0): ");
        int.TryParse(Console.ReadLine(), out int days);
        if (days < 0) days = 0;

        Console.Write("Baggage included? (y/n): ");
        bool baggageIncluded = Console.ReadLine()?.Trim().ToLower() == "y";

        return await Task.FromResult(new FareTypeInputModel(
            name, isRefundable, isChangeable, days, baggageIncluded));
    }

    private static void PrintFareType(FareTypeDto f)
        => Console.WriteLine(
            $"  [{f.Id}] {f.Name} | Refundable:{f.IsRefundable} | Changeable:{f.IsChangeable} | " +
            $"AdvDays:{f.AdvancePurchaseDays} | Baggage:{f.BaggageIncluded}");

    private sealed record FareTypeInputModel(
        string Name, bool IsRefundable, bool IsChangeable,
        int AdvancePurchaseDays, bool BaggageIncluded);
}
