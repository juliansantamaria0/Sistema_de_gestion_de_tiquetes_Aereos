namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.RouteSchedule.Application.Interfaces;

public sealed class RouteScheduleConsoleUI
{
    private readonly IRouteScheduleService _service;

    public RouteScheduleConsoleUI(IRouteScheduleService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n========== ROUTE SCHEDULE MODULE ==========");
            Console.WriteLine("1. List all schedules");
            Console.WriteLine("2. Get schedule by ID");
            Console.WriteLine("3. List schedules by base flight");
            Console.WriteLine("4. Create schedule");
            Console.WriteLine("5. Update schedule");
            Console.WriteLine("6. Delete schedule");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            var option = Console.ReadLine()?.Trim();

            switch (option)
            {
                case "1": await ListAllAsync();          break;
                case "2": await GetByIdAsync();          break;
                case "3": await ListByBaseFlightAsync(); break;
                case "4": await CreateAsync();           break;
                case "5": await UpdateAsync();           break;
                case "6": await DeleteAsync();           break;
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
        var schedules = await _service.GetAllAsync();
        Console.WriteLine("\n--- All Route Schedules ---");

        foreach (var s in schedules)
            Console.WriteLine(
                $"  [{s.Id}] FlightId: {s.BaseFlightId} | " +
                $"{s.DayOfWeekName} ({s.DayOfWeek}) | " +
                $"Departs: {s.DepartureTime:HH:mm}");
    }

    private async Task GetByIdAsync()
    {
        Console.Write("Enter schedule ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        var schedule = await _service.GetByIdAsync(id);

        if (schedule is null)
            Console.WriteLine($"Route schedule with ID {id} not found.");
        else
            Console.WriteLine(
                $"  [{schedule.Id}] FlightId: {schedule.BaseFlightId} | " +
                $"{schedule.DayOfWeekName} ({schedule.DayOfWeek}) | " +
                $"Departs: {schedule.DepartureTime:HH:mm}");
    }

    private async Task ListByBaseFlightAsync()
    {
        Console.Write("Enter Base Flight ID: ");
        if (!int.TryParse(Console.ReadLine(), out int baseFlightId))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        var schedules = await _service.GetByBaseFlightAsync(baseFlightId);
        Console.WriteLine($"\n--- Schedules for Base Flight {baseFlightId} ---");

        foreach (var s in schedules)
            Console.WriteLine(
                $"  [{s.Id}] {s.DayOfWeekName} ({s.DayOfWeek}) | Departs: {s.DepartureTime:HH:mm}");
    }

    private async Task CreateAsync()
    {
        Console.Write("Enter Base Flight ID: ");
        if (!int.TryParse(Console.ReadLine(), out int baseFlightId))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        Console.Write("Enter day of week (1=Mon, 2=Tue, 3=Wed, 4=Thu, 5=Fri, 6=Sat, 7=Sun): ");
        if (!byte.TryParse(Console.ReadLine(), out byte dayOfWeek) || dayOfWeek < 1 || dayOfWeek > 7)
        {
            Console.WriteLine("Invalid day. Must be between 1 and 7.");
            return;
        }

        Console.Write("Enter departure time (HH:mm, e.g. 14:30): ");
        if (!TimeOnly.TryParse(Console.ReadLine()?.Trim(), out TimeOnly departureTime))
        {
            Console.WriteLine("Invalid time format. Use HH:mm.");
            return;
        }

        var created = await _service.CreateAsync(baseFlightId, dayOfWeek, departureTime);
        Console.WriteLine(
            $"Route schedule created: [{created.Id}] " +
            $"{created.DayOfWeekName} at {created.DepartureTime:HH:mm}");
    }

    private async Task UpdateAsync()
    {
        Console.Write("Enter schedule ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        Console.Write("Enter new day of week (1=Mon … 7=Sun): ");
        if (!byte.TryParse(Console.ReadLine(), out byte dayOfWeek) || dayOfWeek < 1 || dayOfWeek > 7)
        {
            Console.WriteLine("Invalid day. Must be between 1 and 7.");
            return;
        }

        Console.Write("Enter new departure time (HH:mm): ");
        if (!TimeOnly.TryParse(Console.ReadLine()?.Trim(), out TimeOnly departureTime))
        {
            Console.WriteLine("Invalid time format. Use HH:mm.");
            return;
        }

        await _service.UpdateAsync(id, dayOfWeek, departureTime);
        Console.WriteLine("Route schedule updated successfully.");
    }

    private async Task DeleteAsync()
    {
        Console.Write("Enter schedule ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        await _service.DeleteAsync(id);
        Console.WriteLine("Route schedule deleted successfully.");
    }
}
