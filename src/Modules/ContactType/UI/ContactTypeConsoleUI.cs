namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.ContactType.Application.Interfaces;

public sealed class ContactTypeConsoleUI
{
    private readonly IContactTypeService _service;

    public ContactTypeConsoleUI(IContactTypeService service) => _service = service;

    public async Task RunAsync()
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine("\n========== CONTACT TYPE MODULE ==========");
            Console.WriteLine("1. List all  2. Get by ID  3. Create  4. Update  5. Delete  0. Exit");
            Console.Write("Select: ");
            switch (Console.ReadLine()?.Trim())
            {
                case "1": foreach (var c in await _service.GetAllAsync()) Console.WriteLine($"  [{c.Id}] {c.Name}"); break;
                case "2":
                    Console.Write("ID: ");
                    if (int.TryParse(Console.ReadLine(), out int id))
                    { var c = await _service.GetByIdAsync(id); Console.WriteLine(c is null ? "Not found." : $"  [{c.Id}] {c.Name}"); }
                    break;
                case "3":
                    Console.Write("Name (EMERGENCY, SECONDARY): ");
                    var name = Console.ReadLine()?.Trim();
                    if (!string.IsNullOrWhiteSpace(name))
                    { var c = await _service.CreateAsync(name); Console.WriteLine($"Created: [{c.Id}] {c.Name}"); }
                    break;
                case "4":
                    Console.Write("ID: "); if (!int.TryParse(Console.ReadLine(), out int uid)) break;
                    Console.Write("New name: "); var nn = Console.ReadLine()?.Trim();
                    if (!string.IsNullOrWhiteSpace(nn)) { await _service.UpdateAsync(uid, nn); Console.WriteLine("Updated."); }
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
}
