namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.PassengerContact.Application.Interfaces;

public sealed class PassengerContactConsoleUI
{
    private readonly IPassengerContactService _service;

    public PassengerContactConsoleUI(IPassengerContactService service) => _service = service;

    public async Task RunAsync()
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine("\n========== PASSENGER CONTACT MODULE ==========");
            Console.WriteLine("1. List all  2. Get by ID  3. List by passenger  4. Add contact  5. Update  6. Delete  0. Exit");
            Console.Write("Select: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1":
                    foreach (var c in await _service.GetAllAsync()) PrintContact(c);
                    break;

                case "2":
                    Console.Write("ID: ");
                    if (int.TryParse(Console.ReadLine(), out int id))
                    { var c = await _service.GetByIdAsync(id); if (c is not null) PrintContact(c); else Console.WriteLine("Not found."); }
                    break;

                case "3":
                    Console.Write("Passenger ID: ");
                    if (int.TryParse(Console.ReadLine(), out int pid))
                        foreach (var c in await _service.GetByPassengerAsync(pid)) PrintContact(c);
                    break;

                case "4":
                    Console.Write("Passenger ID: "); if (!int.TryParse(Console.ReadLine(), out int p)) break;
                    Console.Write("Contact Type ID: "); if (!int.TryParse(Console.ReadLine(), out int t)) break;
                    Console.Write("Full Name: "); var fn = Console.ReadLine()?.Trim(); if (string.IsNullOrWhiteSpace(fn)) break;
                    Console.Write("Phone: "); var ph = Console.ReadLine()?.Trim(); if (string.IsNullOrWhiteSpace(ph)) break;
                    Console.Write("Relationship (optional): "); var rel = Console.ReadLine()?.Trim();
                    try
                    {
                        var c = await _service.CreateAsync(p, t, fn, ph,
                            string.IsNullOrWhiteSpace(rel) ? null : rel);
                        Console.WriteLine($"Contact added: [{c.Id}] {c.FullName}");
                    }
                    catch (ArgumentException ex) { Console.WriteLine($"Error: {ex.Message}"); }
                    break;

                case "5":
                    Console.Write("ID: "); if (!int.TryParse(Console.ReadLine(), out int uid)) break;
                    Console.Write("Full Name: "); var ufn = Console.ReadLine()?.Trim(); if (string.IsNullOrWhiteSpace(ufn)) break;
                    Console.Write("Phone: "); var uph = Console.ReadLine()?.Trim(); if (string.IsNullOrWhiteSpace(uph)) break;
                    Console.Write("Relationship (optional): "); var urel = Console.ReadLine()?.Trim();
                    await _service.UpdateAsync(uid, ufn, uph, string.IsNullOrWhiteSpace(urel) ? null : urel);
                    Console.WriteLine("Updated.");
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

    private static void PrintContact(PassengerContactDto c)
        => Console.WriteLine(
            $"  [{c.Id}] Passenger:{c.PassengerId} | Type:{c.ContactTypeId} | " +
            $"{c.FullName} | {c.Phone}" +
            (c.Relationship is not null ? $" | {c.Relationship}" : string.Empty));
}
