namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FlightCrew.Application.Interfaces;

public sealed class FlightCrewConsoleUI
{
    private readonly IFlightCrewService _service;

    public FlightCrewConsoleUI(IFlightCrewService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n========== FLIGHT CREW MODULE ==========");
            Console.WriteLine("1. List all crew assignments");
            Console.WriteLine("2. Get assignment by ID");
            Console.WriteLine("3. List crew by flight");
            Console.WriteLine("4. Assign crew member to flight");
            Console.WriteLine("5. Reassign crew role");
            Console.WriteLine("6. Remove crew assignment");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            var option = Console.ReadLine()?.Trim();

            switch (option)
            {
                case "1": await ListAllAsync();        break;
                case "2": await GetByIdAsync();        break;
                case "3": await ListByFlightAsync();   break;
                case "4": await AssignAsync();         break;
                case "5": await ReassignRoleAsync();   break;
                case "6": await RemoveAsync();         break;
                case "0": running = false;             break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    // ── Handlers ──────────────────────────────────────────────────────────────

    private async Task ListAllAsync()
    {
        var crew = await _service.GetAllAsync();
        Console.WriteLine("\n--- All Crew Assignments ---");

        foreach (var c in crew)
            PrintAssignment(c);
    }

    private async Task GetByIdAsync()
    {
        Console.Write("Enter crew assignment ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        var assignment = await _service.GetByIdAsync(id);

        if (assignment is null)
            Console.WriteLine($"Crew assignment with ID {id} not found.");
        else
            PrintAssignment(assignment);
    }

    private async Task ListByFlightAsync()
    {
        Console.Write("Enter Scheduled Flight ID: ");
        if (!int.TryParse(Console.ReadLine(), out int flightId))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        var crew = await _service.GetByFlightAsync(flightId);
        Console.WriteLine($"\n--- Crew for Scheduled Flight {flightId} ---");

        foreach (var c in crew)
            PrintAssignment(c);
    }

    private async Task AssignAsync()
    {
        Console.Write("Scheduled Flight ID: ");
        if (!int.TryParse(Console.ReadLine(), out int flightId))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("Employee ID: ");
        if (!int.TryParse(Console.ReadLine(), out int employeeId))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("Crew Role ID: ");
        if (!int.TryParse(Console.ReadLine(), out int roleId))
        { Console.WriteLine("Invalid ID."); return; }

        var created = await _service.CreateAsync(flightId, employeeId, roleId);
        Console.WriteLine(
            $"Crew assigned: [{created.Id}] " +
            $"Employee {created.EmployeeId} → Role {created.CrewRoleId} " +
            $"on Flight {created.ScheduledFlightId}");
    }

    private async Task ReassignRoleAsync()
    {
        Console.Write("Crew assignment ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("New Crew Role ID: ");
        if (!int.TryParse(Console.ReadLine(), out int roleId))
        { Console.WriteLine("Invalid ID."); return; }

        await _service.UpdateAsync(id, roleId);
        Console.WriteLine("Crew role reassigned successfully.");
    }

    private async Task RemoveAsync()
    {
        Console.Write("Enter crew assignment ID to remove: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        await _service.DeleteAsync(id);
        Console.WriteLine("Crew assignment removed successfully.");
    }

    // ── Helpers de presentación ───────────────────────────────────────────────

    private static void PrintAssignment(FlightCrewDto c)
        => Console.WriteLine(
            $"  [{c.Id}] Flight: {c.ScheduledFlightId} | " +
            $"Employee: {c.EmployeeId} | Role: {c.CrewRoleId} | " +
            $"Assigned: {c.CreatedAt:yyyy-MM-dd HH:mm}");
}
