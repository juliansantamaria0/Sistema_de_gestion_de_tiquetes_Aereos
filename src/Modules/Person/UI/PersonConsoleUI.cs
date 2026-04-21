namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Person.Application.Interfaces;

public sealed class PersonConsoleUI
{
    private readonly IPersonService _service;

    public PersonConsoleUI(IPersonService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n========== PERSON MODULE ==========");
            Console.WriteLine("1. List all persons");
            Console.WriteLine("2. Get person by ID");
            Console.WriteLine("3. Find by document");
            Console.WriteLine("4. Register person");
            Console.WriteLine("5. Update person");
            Console.WriteLine("6. Delete person");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            var option = Console.ReadLine()?.Trim();

            switch (option)
            {
                case "1": await ListAllAsync();        break;
                case "2": await GetByIdAsync();        break;
                case "3": await FindByDocumentAsync(); break;
                case "4": await RegisterAsync();       break;
                case "5": await UpdateAsync();         break;
                case "6": await DeleteAsync();         break;
                case "0": running = false;             break;
                default: Console.WriteLine("Invalid option."); break;
            }
        }
    }

    private async Task ListAllAsync()
    {
        var persons = await _service.GetAllAsync();
        Console.WriteLine("\n--- All Persons ---");
        foreach (var p in persons) PrintPerson(p);
    }

    private async Task GetByIdAsync()
    {
        Console.Write("Enter person ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        var p = await _service.GetByIdAsync(id);
        if (p is null) Console.WriteLine($"Person with ID {id} not found.");
        else           PrintPerson(p);
    }

    private async Task FindByDocumentAsync()
    {
        Console.Write("Document Type ID: ");
        if (!int.TryParse(Console.ReadLine(), out int typeId))
        { Console.WriteLine("Invalid."); return; }

        Console.Write("Document Number: ");
        var docNum = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(docNum))
        { Console.WriteLine("Document number cannot be empty."); return; }

        var p = await _service.GetByDocumentAsync(typeId, docNum);
        if (p is null) Console.WriteLine("Person not found.");
        else           PrintPerson(p);
    }

    private async Task RegisterAsync()
    {
        Console.Write("Document Type ID: ");
        if (!int.TryParse(Console.ReadLine(), out int typeId)) { Console.WriteLine("Invalid."); return; }

        Console.Write("Document Number: ");
        var docNum = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(docNum)) { Console.WriteLine("Empty."); return; }

        Console.Write("First Name: ");
        var firstName = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(firstName)) { Console.WriteLine("Empty."); return; }

        Console.Write("Last Name: ");
        var lastName = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(lastName)) { Console.WriteLine("Empty."); return; }

        Console.Write("Birth Date (yyyy-MM-dd, optional): ");
        var bdInput = Console.ReadLine()?.Trim();
        DateOnly? birthDate = DateOnly.TryParse(bdInput, out var bd) ? bd : null;

        Console.Write("Gender ID (optional): ");
        var gInput = Console.ReadLine()?.Trim();
        int? genderId = int.TryParse(gInput, out int g) && g > 0 ? g : null;

        try
        {
            var created = await _service.CreateAsync(
                typeId, docNum, firstName, lastName, birthDate, genderId);
            Console.WriteLine($"Person registered: [{created.Id}] {created.FirstName} {created.LastName}");
        }
        catch (ArgumentException ex) { Console.WriteLine($"Validation error: {ex.Message}"); }
    }

    private async Task UpdateAsync()
    {
        Console.Write("Person ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("Invalid."); return; }

        Console.Write("First Name: ");
        var firstName = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(firstName)) { Console.WriteLine("Empty."); return; }

        Console.Write("Last Name: ");
        var lastName = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(lastName)) { Console.WriteLine("Empty."); return; }

        Console.Write("Birth Date (yyyy-MM-dd, optional): ");
        var bdInput = Console.ReadLine()?.Trim();
        DateOnly? birthDate = DateOnly.TryParse(bdInput, out var bd) ? bd : null;

        Console.Write("Gender ID (optional): ");
        var gInput = Console.ReadLine()?.Trim();
        int? genderId = int.TryParse(gInput, out int g) && g > 0 ? g : null;

        try
        {
            await _service.UpdateAsync(id, firstName, lastName, birthDate, genderId);
            Console.WriteLine("Person updated successfully.");
        }
        catch (ArgumentException ex) { Console.WriteLine($"Validation error: {ex.Message}"); }
    }

    private async Task DeleteAsync()
    {
        Console.Write("Person ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid."); return; }

        await _service.DeleteAsync(id);
        Console.WriteLine("Person deleted successfully.");
    }

    private static void PrintPerson(PersonDto p)
        => Console.WriteLine(
            $"  [{p.Id}] {p.FirstName} {p.LastName} | Doc:{p.DocumentTypeId}/{p.DocumentNumber}" +
            (p.BirthDate.HasValue ? $" | DOB:{p.BirthDate}" : string.Empty) +
            (p.GenderId.HasValue  ? $" | Gender:{p.GenderId}" : string.Empty));
}
