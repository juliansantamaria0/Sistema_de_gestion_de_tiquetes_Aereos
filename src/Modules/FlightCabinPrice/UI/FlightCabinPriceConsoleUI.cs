namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCabinPrice.Application.Interfaces;

public sealed class FlightCabinPriceConsoleUI
{
    private readonly IFlightCabinPriceService _service;

    public FlightCabinPriceConsoleUI(IFlightCabinPriceService service) => _service = service;

    public async Task RunAsync()
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine("\n========== FLIGHT CABIN PRICE MODULE ==========");
            Console.WriteLine("1. List all  2. Get by ID  3. List by flight  4. Set price  5. Update price  6. Delete  0. Exit");
            Console.Write("Select: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1":
                    Console.WriteLine("\n--- All Flight Cabin Prices ---");
                    foreach (var p in await _service.GetAllAsync()) PrintPrice(p);
                    break;

                case "2":
                    Console.Write("ID: ");
                    if (int.TryParse(Console.ReadLine(), out int id))
                    { var p = await _service.GetByIdAsync(id); if (p is not null) PrintPrice(p); else Console.WriteLine("Not found."); }
                    break;

                case "3":
                    Console.Write("Scheduled Flight ID: ");
                    if (int.TryParse(Console.ReadLine(), out int fid))
                    { Console.WriteLine($"\n--- Prices for Flight {fid} ---"); foreach (var p in await _service.GetByFlightAsync(fid)) PrintPrice(p); }
                    break;

                case "4":
                    Console.Write("Scheduled Flight ID: "); if (!int.TryParse(Console.ReadLine(), out int sfid)) break;
                    Console.Write("Cabin Class ID: "); if (!int.TryParse(Console.ReadLine(), out int ccid)) break;
                    Console.Write("Fare Type ID: "); if (!int.TryParse(Console.ReadLine(), out int ftid)) break;
                    Console.Write("Price (>= 0): ");
                    if (!decimal.TryParse(Console.ReadLine()?.Replace(',', '.'),
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out decimal price)) break;
                    try
                    { var p = await _service.CreateAsync(sfid, ccid, ftid, price); Console.WriteLine($"Price set: [{p.Id}] {p.Price:F2}"); }
                    catch (ArgumentException ex) { Console.WriteLine($"Error: {ex.Message}"); }
                    break;

                case "5":
                    Console.Write("Price record ID: "); if (!int.TryParse(Console.ReadLine(), out int uid)) break;
                    Console.Write("New price (>= 0): ");
                    if (!decimal.TryParse(Console.ReadLine()?.Replace(',', '.'),
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out decimal np)) break;
                    try { await _service.UpdatePriceAsync(uid, np); Console.WriteLine("Price updated."); }
                    catch (ArgumentException ex) { Console.WriteLine($"Error: {ex.Message}"); }
                    break;

                case "6":
                    Console.Write("ID: "); if (!int.TryParse(Console.ReadLine(), out int did)) break;
                    await _service.DeleteAsync(did); Console.WriteLine("Deleted.");
                    break;

                case "0": running = false; break;
                default: Console.WriteLine("Invalid option."); break;
            }
        }
    }

    private static void PrintPrice(FlightCabinPriceDto p)
        => Console.WriteLine(
            $"  [{p.Id}] Flight:{p.ScheduledFlightId} | Cabin:{p.CabinClassId} | " +
            $"Fare:{p.FareTypeId} | Price:{p.Price:F2}");
}
