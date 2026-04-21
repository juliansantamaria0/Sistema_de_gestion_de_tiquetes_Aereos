namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Route.Application.Interfaces;

public sealed class RouteConsoleUI
{
    private readonly IRouteService _service;

    public RouteConsoleUI(IRouteService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n========== ROUTE MODULE ==========");
            Console.WriteLine("1. List all routes");
            Console.WriteLine("2. Get route by ID");
            Console.WriteLine("3. Find route by airports");
            Console.WriteLine("4. List routes by origin airport");
            Console.WriteLine("5. Create route");
            Console.WriteLine("6. Delete route");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            var option = Console.ReadLine()?.Trim();

            switch (option)
            {
                case "1": await ListAllAsync();          break;
                case "2": await GetByIdAsync();          break;
                case "3": await FindByAirportsAsync();   break;
                case "4": await ListByOriginAsync();     break;
                case "5": await CreateAsync();           break;
                case "6": await DeleteAsync();           break;
                case "0": running = false;               break;
                default: Console.WriteLine("Invalid option."); break;
            }
        }
    }

    private async Task ListAllAsync()
    {
        var routes = await _service.GetAllAsync();
        Console.WriteLine("\n--- All Routes ---");
        foreach (var r in routes) PrintRoute(r);
    }

    private async Task GetByIdAsync()
    {
        Console.Write("Enter route ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        var r = await _service.GetByIdAsync(id);
        if (r is null) Console.WriteLine($"Route with ID {id} not found.");
        else           PrintRoute(r);
    }

    private async Task FindByAirportsAsync()
    {
        Console.Write("Origin Airport ID: ");
        if (!int.TryParse(Console.ReadLine(), out int originId))
        { Console.WriteLine("Invalid."); return; }

        Console.Write("Destination Airport ID: ");
        if (!int.TryParse(Console.ReadLine(), out int destId))
        { Console.WriteLine("Invalid."); return; }

        var r = await _service.GetByAirportsAsync(originId, destId);
        if (r is null) Console.WriteLine($"No route found from {originId} to {destId}.");
        else           PrintRoute(r);
    }

    private async Task ListByOriginAsync()
    {
        Console.Write("Origin Airport ID: ");
        if (!int.TryParse(Console.ReadLine(), out int originId))
        { Console.WriteLine("Invalid."); return; }

        var routes = await _service.GetByOriginAsync(originId);
        Console.WriteLine($"\n--- Routes from Airport {originId} ---");
        foreach (var r in routes) PrintRoute(r);
    }

    private async Task CreateAsync()
    {
        Console.Write("Origin Airport ID: ");
        if (!int.TryParse(Console.ReadLine(), out int originId))
        { Console.WriteLine("Invalid."); return; }

        Console.Write("Destination Airport ID: ");
        if (!int.TryParse(Console.ReadLine(), out int destId))
        { Console.WriteLine("Invalid."); return; }

        try
        {
            var created = await _service.CreateAsync(originId, destId);
            Console.WriteLine($"Route created: [{created.Id}] {created.OriginAirportId} → {created.DestinationAirportId}");
        }
        catch (ArgumentException ex) { Console.WriteLine($"Validation error: {ex.Message}"); }
    }

    private async Task DeleteAsync()
    {
        Console.Write("Route ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid."); return; }

        await _service.DeleteAsync(id);
        Console.WriteLine("Route deleted successfully.");
    }

    private static void PrintRoute(RouteDto r)
        => Console.WriteLine(
            $"  [{r.Id}] Airport:{r.OriginAirportId} → Airport:{r.DestinationAirportId} | " +
            $"Created:{r.CreatedAt:yyyy-MM-dd}");
}
