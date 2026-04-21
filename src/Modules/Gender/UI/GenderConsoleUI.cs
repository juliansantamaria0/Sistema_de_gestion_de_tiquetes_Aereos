namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Gender.Application.Interfaces;

public sealed class GenderConsoleUI
{
    private readonly IGenderService _service;

    public GenderConsoleUI(IGenderService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n========== GENDER MODULE ==========");
            Console.WriteLine("1. List all genders");
            Console.WriteLine("2. Get gender by ID");
            Console.WriteLine("3. Create gender");
            Console.WriteLine("4. Update gender");
            Console.WriteLine("5. Delete gender");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            var option = Console.ReadLine()?.Trim();

            switch (option)
            {
                case "1": await ListAllAsync();  break;
                case "2": await GetByIdAsync();  break;
                case "3": await CreateAsync();   break;
                case "4": await UpdateAsync();   break;
                case "5": await DeleteAsync();   break;
                case "0": running = false;       break;
                default: Console.WriteLine("Invalid option."); break;
            }
        }
    }

    private async Task ListAllAsync()
    {
        var genders = await _service.GetAllAsync();
        Console.WriteLine("\n--- All Genders ---");
        foreach (var g in genders)
            Console.WriteLine($"  [{g.Id}] {g.Name}");
    }

    private async Task GetByIdAsync()
    {
        Console.Write("Enter gender ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        var g = await _service.GetByIdAsync(id);
        if (g is null) Console.WriteLine($"Gender with ID {id} not found.");
        else           Console.WriteLine($"  [{g.Id}] {g.Name}");
    }

    private async Task CreateAsync()
    {
        Console.Write("Enter name (e.g. MALE, FEMALE, OTHER, PREFER_NOT_TO_SAY): ");
        var name = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(name)) { Console.WriteLine("Name cannot be empty."); return; }

        var created = await _service.CreateAsync(name);
        Console.WriteLine($"Gender created: [{created.Id}] {created.Name}");
    }

    private async Task UpdateAsync()
    {
        Console.Write("Enter gender ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        Console.Write("Enter new name: ");
        var newName = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(newName)) { Console.WriteLine("Name cannot be empty."); return; }

        await _service.UpdateAsync(id, newName);
        Console.WriteLine("Gender updated successfully.");
    }

    private async Task DeleteAsync()
    {
        Console.Write("Enter gender ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        await _service.DeleteAsync(id);
        Console.WriteLine("Gender deleted successfully.");
    }
}
