namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DocumentType.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.UI;

public sealed class DocumentTypeConsoleUI : IModuleUI
{
    private readonly IDocumentTypeService _service;

    public string Key   => "document_type";
    public string Title => "Document Type Management";

    public DocumentTypeConsoleUI(IDocumentTypeService service) => _service = service;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        bool running = true;
        while (running && !cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("\n========== DOCUMENT TYPE MANAGEMENT ==========");
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
        if (!list.Any()) { Console.WriteLine("No document types found."); return; }
        Console.WriteLine($"\n{"ID",-6} {"Code",-10} {"Name",-30}");
        Console.WriteLine(new string('-', 48));
        foreach (var d in list)
            Console.WriteLine($"{d.DocumentTypeId,-6} {d.Code,-10} {d.Name,-30}");
    }

    private async Task GetByIdAsync(CancellationToken ct)
    {
        Console.Write("Document Type ID: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Invalid ID."); return; }
        var d = await _service.GetByIdAsync(id, ct);
        if (d is null) { Console.WriteLine("Not found."); return; }
        Console.WriteLine($"ID: {d.DocumentTypeId} | Code: {d.Code} | Name: {d.Name}");
    }

    private async Task CreateAsync(CancellationToken ct)
    {
        Console.Write("Name: ");
        var name = Console.ReadLine()?.Trim() ?? string.Empty;
        Console.Write("Code (e.g. CC, PA): ");
        var code = Console.ReadLine()?.Trim() ?? string.Empty;
        var created = await _service.CreateAsync(new CreateDocumentTypeRequest(name, code), ct);
        Console.WriteLine($"Created with ID: {created.DocumentTypeId}");
    }

    private async Task UpdateAsync(CancellationToken ct)
    {
        Console.Write("ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Invalid ID."); return; }
        Console.Write("New Name: ");
        var name = Console.ReadLine()?.Trim() ?? string.Empty;
        Console.Write("New Code: ");
        var code = Console.ReadLine()?.Trim() ?? string.Empty;
        var updated = await _service.UpdateAsync(id, new UpdateDocumentTypeRequest(name, code), ct);
        Console.WriteLine($"Updated: {updated.Code} - {updated.Name}");
    }

    private async Task DeleteAsync(CancellationToken ct)
    {
        Console.Write("ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Invalid ID."); return; }
        Console.Write($"Confirm delete {id}? (y/n): ");
        if (Console.ReadLine()?.Trim().ToLower() != "y") { Console.WriteLine("Cancelled."); return; }
        await _service.DeleteAsync(id, ct);
        Console.WriteLine("Deleted.");
    }
}
