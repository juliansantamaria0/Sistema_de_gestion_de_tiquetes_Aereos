namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Application.Interfaces;

public sealed class PromotionConsoleUI
{
    private readonly IPromotionService _service;

    public PromotionConsoleUI(IPromotionService service) => _service = service;

    public async Task RunAsync()
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine("\n========== PROMOTION MODULE ==========");
            Console.WriteLine("1. List all  2. Get by ID  3. List by airline");
            Console.WriteLine("4. List active today  5. Create  6. Update  7. Delete  0. Exit");
            Console.Write("Select: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1":
                    Console.WriteLine("\n--- All Promotions ---");
                    foreach (var p in await _service.GetAllAsync()) PrintPromotion(p);
                    break;

                case "2":
                    Console.Write("ID: ");
                    if (int.TryParse(Console.ReadLine(), out int id))
                    { var p = await _service.GetByIdAsync(id); if (p is not null) PrintPromotion(p); else Console.WriteLine("Not found."); }
                    break;

                case "3":
                    Console.Write("Airline ID: ");
                    if (int.TryParse(Console.ReadLine(), out int aid))
                    { Console.WriteLine($"\n--- Promotions for Airline {aid} ---"); foreach (var p in await _service.GetByAirlineAsync(aid)) PrintPromotion(p); }
                    break;

                case "4":
                    var today = DateOnly.FromDateTime(DateTime.UtcNow);
                    Console.WriteLine($"\n--- Active Promotions ({today}) ---");
                    foreach (var p in await _service.GetActiveAsync(today)) PrintPromotion(p);
                    break;

                case "5":
                case "6":
                    bool isUpdate = Console.ReadLine()?.Trim() == "6";
                    int uid = 0;
                    if (isUpdate) { Console.Write("ID: "); if (!int.TryParse(Console.ReadLine(), out uid)) break; }

                    Console.Write("Airline ID: "); if (!int.TryParse(Console.ReadLine(), out int airlineId)) break;
                    Console.Write("Name: "); var name = Console.ReadLine()?.Trim(); if (string.IsNullOrWhiteSpace(name)) break;

                    Console.Write("Discount % (0-100): ");
                    if (!decimal.TryParse(Console.ReadLine()?.Replace(',', '.'),
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out decimal pct)) break;

                    Console.Write("Valid from (yyyy-MM-dd): ");
                    if (!DateOnly.TryParse(Console.ReadLine(), out DateOnly from)) break;

                    Console.Write("Valid until (yyyy-MM-dd): ");
                    if (!DateOnly.TryParse(Console.ReadLine(), out DateOnly until)) break;

                    try
                    {
                        if (!isUpdate)
                        { var c = await _service.CreateAsync(airlineId, name, pct, from, until); Console.WriteLine($"Created: [{c.Id}] {c.Name}"); }
                        else
                        { await _service.UpdateAsync(uid, name, pct, from, until); Console.WriteLine("Updated."); }
                    }
                    catch (ArgumentException ex) { Console.WriteLine($"Error: {ex.Message}"); }
                    break;

                case "7":
                    Console.Write("ID: "); if (!int.TryParse(Console.ReadLine(), out int did)) break;
                    await _service.DeleteAsync(did); Console.WriteLine("Deleted.");
                    break;

                case "0": running = false; break;
                default: Console.WriteLine("Invalid option."); break;
            }
        }
    }

    private static void PrintPromotion(PromotionDto p)
        => Console.WriteLine(
            $"  [{p.Id}] {p.Name} | Airline:{p.AirlineId} | {p.DiscountPct:F2}% | " +
            $"{p.ValidFrom:yyyy-MM-dd} → {p.ValidUntil:yyyy-MM-dd}");
}
