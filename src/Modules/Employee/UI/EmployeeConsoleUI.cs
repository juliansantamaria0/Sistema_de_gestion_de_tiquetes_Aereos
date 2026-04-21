namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Employee.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class EmployeeConsoleUI : IModuleUI
{
    private readonly IEmployeeService _service;

    public string Key   => "employee";
    public string Title => "Employee Management";

    public EmployeeConsoleUI(IEmployeeService service) => _service = service;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        bool running = true;
        while (running && !cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("\n========== EMPLOYEE MANAGEMENT ==========");
            Console.WriteLine("1. List all");
            Console.WriteLine("2. Get by ID");
            Console.WriteLine("3. Create");
            Console.WriteLine("4. Update");
            Console.WriteLine("5. Delete");
            Console.WriteLine("0. Back");
            Console.Write("Select: ");
            var opt = Console.ReadLine()?.Trim();
            try
            {
                switch (opt)
                {
                    case "1": await ListAllAsync(cancellationToken);  break;
                    case "2": await GetByIdAsync(cancellationToken);  break;
                    case "3": await CreateAsync(cancellationToken);   break;
                    case "4": await UpdateAsync(cancellationToken);   break;
                    case "5": await DeleteAsync(cancellationToken);   break;
                    case "0": running = false;                        break;
                    default:  Console.WriteLine("Invalid option.");   break;
                }
            }
            catch (Exception ex) { Console.WriteLine($"[ERROR] {ex.Message}"); }
        }
    }

    private async Task ListAllAsync(CancellationToken ct)
    {
        var list = await _service.GetAllAsync(ct);
        if (!list.Any()) { Console.WriteLine("No employees found."); return; }
        Console.WriteLine($"\n{"ID",-6} {"PersonId",-10} {"AirlineId",-10} {"JobPosId",-10} {"HireDate",-12} {"Active"}");
        Console.WriteLine(new string('-', 56));
        foreach (var e in list)
            Console.WriteLine($"{e.EmployeeId,-6} {e.PersonId,-10} {e.AirlineId,-10} {e.JobPositionId?.ToString() ?? "-",-10} {e.HireDate,-12} {e.IsActive}");
    }

    private async Task GetByIdAsync(CancellationToken ct)
    {
        Console.Write("Employee ID: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Invalid ID."); return; }
        var e = await _service.GetByIdAsync(id, ct);
        if (e is null) { Console.WriteLine("Not found."); return; }
        Console.WriteLine($"ID:{e.EmployeeId} Person:{e.PersonId} Airline:{e.AirlineId} JobPos:{e.JobPositionId?.ToString() ?? "-"} Hire:{e.HireDate} Active:{e.IsActive}");
    }

    private async Task CreateAsync(CancellationToken ct)
    {
        Console.Write("Person ID: ");
        if (!int.TryParse(Console.ReadLine(), out var pId)) { Console.WriteLine("Invalid."); return; }
        Console.Write("Airline ID: ");
        if (!int.TryParse(Console.ReadLine(), out var aId)) { Console.WriteLine("Invalid."); return; }
        Console.Write("Hire Date (yyyy-MM-dd): ");
        if (!DateOnly.TryParse(Console.ReadLine(), out var hd)) { Console.WriteLine("Invalid date."); return; }
        Console.Write("Job Position ID (optional, Enter to skip): ");
        var jpStr = Console.ReadLine()?.Trim();
        int? jpId = int.TryParse(jpStr, out var jpi) ? jpi : null;
        Console.Write("Is active? (y/n): ");
        var active = Console.ReadLine()?.Trim().ToLower() == "y";
        var created = await _service.CreateAsync(new CreateEmployeeRequest(pId, aId, hd, jpId, active), ct);
        Console.WriteLine($"Created with ID: {created.EmployeeId}");
    }

    private async Task UpdateAsync(CancellationToken ct)
    {
        Console.Write("Employee ID: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Invalid."); return; }
        Console.Write("New Airline ID: ");
        if (!int.TryParse(Console.ReadLine(), out var aId)) { Console.WriteLine("Invalid."); return; }
        Console.Write("New Job Position ID (optional): ");
        var jpStr = Console.ReadLine()?.Trim();
        int? jpId = int.TryParse(jpStr, out var jpi) ? jpi : null;
        Console.Write("New Hire Date (yyyy-MM-dd): ");
        if (!DateOnly.TryParse(Console.ReadLine(), out var hd)) { Console.WriteLine("Invalid date."); return; }
        Console.Write("Is active? (y/n): ");
        var active = Console.ReadLine()?.Trim().ToLower() == "y";
        var updated = await _service.UpdateAsync(id, new UpdateEmployeeRequest(aId, jpId, hd, active), ct);
        Console.WriteLine($"Updated employee ID: {updated.EmployeeId}");
    }

    private async Task DeleteAsync(CancellationToken ct)
    {
        Console.Write("Employee ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Invalid."); return; }
        Console.Write($"Confirm delete employee {id}? (y/n): ");
        if (Console.ReadLine()?.Trim().ToLower() != "y") { Console.WriteLine("Cancelled."); return; }
        await _service.DeleteAsync(id, ct);
        Console.WriteLine("Deleted.");
    }
}
