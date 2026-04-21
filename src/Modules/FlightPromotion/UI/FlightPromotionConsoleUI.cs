namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightPromotion.Application.Interfaces;

public sealed class FlightPromotionConsoleUI
{
    private readonly IFlightPromotionService _service;

    public FlightPromotionConsoleUI(IFlightPromotionService service) => _service = service;

    public async Task RunAsync()
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine("\n========== FLIGHT PROMOTION MODULE ==========");
            Console.WriteLine("1. List all  2. Get by ID  3. List by flight  4. List by promotion");
            Console.WriteLine("5. Assign promotion to flight  6. Remove assignment  0. Exit");
            Console.Write("Select: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1":
                    Console.WriteLine("\n--- All Flight-Promotion Assignments ---");
                    foreach (var fp in await _service.GetAllAsync()) PrintFP(fp);
                    break;

                case "2":
                    Console.Write("ID: ");
                    if (int.TryParse(Console.ReadLine(), out int id))
                    { var fp = await _service.GetByIdAsync(id); if (fp is not null) PrintFP(fp); else Console.WriteLine("Not found."); }
                    break;

                case "3":
                    Console.Write("Scheduled Flight ID: ");
                    if (int.TryParse(Console.ReadLine(), out int sfid))
                    {
                        var list = await _service.GetByFlightAsync(sfid);
                        Console.WriteLine($"\n--- Promotions for Flight {sfid} ---");
                        foreach (var fp in list) PrintFP(fp);
                    }
                    break;

                case "4":
                    Console.Write("Promotion ID: ");
                    if (int.TryParse(Console.ReadLine(), out int pid))
                    {
                        var list = await _service.GetByPromotionAsync(pid);
                        Console.WriteLine($"\n--- Flights for Promotion {pid} ---");
                        foreach (var fp in list) PrintFP(fp);
                    }
                    break;

                case "5":
                    Console.Write("Scheduled Flight ID: "); if (!int.TryParse(Console.ReadLine(), out int asf)) break;
                    Console.Write("Promotion ID: ");        if (!int.TryParse(Console.ReadLine(), out int ap))  break;
                    try
                    {
                        var created = await _service.AssignAsync(asf, ap);
                        Console.WriteLine($"Assigned: [{created.Id}] Flight:{created.ScheduledFlightId} ↔ Promotion:{created.PromotionId}");
                    }
                    catch (ArgumentException ex) { Console.WriteLine($"Error: {ex.Message}"); }
                    break;

                case "6":
                    Console.Write("Assignment ID to remove: ");
                    if (!int.TryParse(Console.ReadLine(), out int rid)) break;
                    await _service.RemoveAsync(rid);
                    Console.WriteLine("Assignment removed.");
                    break;

                case "0": running = false; break;
                default: Console.WriteLine("Invalid option."); break;
            }
        }
    }

    private static void PrintFP(FlightPromotionDto fp)
        => Console.WriteLine(
            $"  [{fp.Id}] Flight:{fp.ScheduledFlightId} ↔ Promotion:{fp.PromotionId}");
}
