namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Application.Interfaces;

public sealed class DiscountTypeConsoleUI
{
    private readonly IDiscountTypeService _service;

    public DiscountTypeConsoleUI(IDiscountTypeService service) => _service = service;

    public async Task RunAsync()
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine("\n========== DISCOUNT TYPE MODULE ==========");
            Console.WriteLine("1. List all  2. Get by ID  3. Create  4. Update  5. Delete  0. Exit");
            Console.Write("Select: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1":
                    Console.WriteLine("\n--- All Discount Types ---");
                    foreach (var d in await _service.GetAllAsync()) PrintDiscount(d);
                    break;

                case "2":
                    Console.Write("ID: ");
                    if (int.TryParse(Console.ReadLine(), out int id))
                    { var d = await _service.GetByIdAsync(id); if (d is not null) PrintDiscount(d); else Console.WriteLine("Not found."); }
                    break;

                case "3":
                case "4":
                    bool isUpdate = Console.ReadLine() == null; // handled by switch — just input
                    int uid = 0;
                    if (Console.ReadLine()?.Trim() == "4")
                    { Console.Write("ID: "); if (!int.TryParse(Console.ReadLine(), out uid)) break; }

                    Console.Write("Name: "); var name = Console.ReadLine()?.Trim();
                    if (string.IsNullOrWhiteSpace(name)) break;

                    Console.Write("Percentage (0-100): ");
                    if (!decimal.TryParse(Console.ReadLine()?.Replace(',', '.'),
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out decimal pct))
                        break;

                    Console.Write("Age min (optional): ");
                    var aminStr = Console.ReadLine()?.Trim();
                    int? ageMin = int.TryParse(aminStr, out int am) ? am : null;

                    Console.Write("Age max (optional): ");
                    var amaxStr = Console.ReadLine()?.Trim();
                    int? ageMax = int.TryParse(amaxStr, out int ax) ? ax : null;

                    try
                    {
                        if (uid == 0)
                        { var c = await _service.CreateAsync(name, pct, ageMin, ageMax); Console.WriteLine($"Created: [{c.Id}] {c.Name}"); }
                        else
                        { await _service.UpdateAsync(uid, name, pct, ageMin, ageMax); Console.WriteLine("Updated."); }
                    }
                    catch (ArgumentException ex) { Console.WriteLine($"Error: {ex.Message}"); }
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

    private static void PrintDiscount(DiscountTypeDto d)
        => Console.WriteLine(
            $"  [{d.Id}] {d.Name} | {d.Percentage:F2}%" +
            (d.AgeMin.HasValue ? $" | AgeMin:{d.AgeMin}" : string.Empty) +
            (d.AgeMax.HasValue ? $" | AgeMax:{d.AgeMax}" : string.Empty));
}
