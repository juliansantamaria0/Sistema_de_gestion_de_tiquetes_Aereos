namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.UI;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Application.Interfaces;

public sealed class LoyaltyTransactionConsoleUI
{
    private readonly ILoyaltyTransactionService _service;

    public LoyaltyTransactionConsoleUI(ILoyaltyTransactionService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n========== LOYALTY TRANSACTION MODULE ==========");
            Console.WriteLine("1. List all transactions");
            Console.WriteLine("2. Get transaction by ID");
            Console.WriteLine("3. List transactions by account");
            Console.WriteLine("4. Record EARN (miles accrual)");
            Console.WriteLine("5. Record REDEEM (miles redemption)");
            Console.WriteLine("6. Delete transaction record");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            var option = Console.ReadLine()?.Trim();

            switch (option)
            {
                case "1": await ListAllAsync();        break;
                case "2": await GetByIdAsync();        break;
                case "3": await ListByAccountAsync();  break;
                case "4": await RecordEarnAsync();     break;
                case "5": await RecordRedeemAsync();   break;
                case "6": await DeleteAsync();         break;
                case "0": running = false;             break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    private async Task ListAllAsync()
    {
        var txs = await _service.GetAllAsync();
        Console.WriteLine("\n--- All Loyalty Transactions ---");
        foreach (var t in txs) PrintTx(t);
    }

    private async Task GetByIdAsync()
    {
        Console.Write("Enter transaction ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid ID."); return; }

        var t = await _service.GetByIdAsync(id);
        if (t is null) Console.WriteLine($"Transaction with ID {id} not found.");
        else           PrintTx(t);
    }

    private async Task ListByAccountAsync()
    {
        Console.Write("Enter Loyalty Account ID: ");
        if (!int.TryParse(Console.ReadLine(), out int accountId))
        { Console.WriteLine("Invalid ID."); return; }

        var txs = await _service.GetByAccountAsync(accountId);
        var list = txs.ToList();

        Console.WriteLine($"\n--- Transactions for Account {accountId} ---");

        int totalEarned  = list.Where(t => t.TransactionType == "EARN").Sum(t => t.Miles);
        int totalRedeemed = list.Where(t => t.TransactionType == "REDEEM").Sum(t => t.Miles);

        foreach (var t in list) PrintTx(t);
        Console.WriteLine($"  Earned: {totalEarned:N0} | Redeemed: {totalRedeemed:N0}");
    }

    private async Task RecordEarnAsync()
    {
        Console.Write("Loyalty Account ID: ");
        if (!int.TryParse(Console.ReadLine(), out int accountId))
        { Console.WriteLine("Invalid."); return; }

        Console.Write("Ticket ID (optional): ");
        var ticketInput = Console.ReadLine()?.Trim();
        int? ticketId = int.TryParse(ticketInput, out int tParsed) ? tParsed : null;

        Console.Write("Miles to earn (> 0): ");
        if (!int.TryParse(Console.ReadLine(), out int miles) || miles <= 0)
        { Console.WriteLine("Invalid miles."); return; }

        try
        {
            var created = await _service.EarnAsync(accountId, ticketId, miles);
            Console.WriteLine($"EARN recorded: [{created.Id}] {created.Miles:N0} miles | " +
                              $"Account:{created.LoyaltyAccountId} | {created.TransactionDate:yyyy-MM-dd HH:mm}");
        }
        catch (ArgumentException ex) { Console.WriteLine($"Validation error: {ex.Message}"); }
    }

    private async Task RecordRedeemAsync()
    {
        Console.Write("Loyalty Account ID: ");
        if (!int.TryParse(Console.ReadLine(), out int accountId))
        { Console.WriteLine("Invalid."); return; }

        Console.Write("Ticket ID (optional): ");
        var ticketInput = Console.ReadLine()?.Trim();
        int? ticketId = int.TryParse(ticketInput, out int tParsed) ? tParsed : null;

        Console.Write("Miles to redeem (> 0): ");
        if (!int.TryParse(Console.ReadLine(), out int miles) || miles <= 0)
        { Console.WriteLine("Invalid miles."); return; }

        try
        {
            var created = await _service.RedeemAsync(accountId, ticketId, miles);
            Console.WriteLine($"REDEEM recorded: [{created.Id}] {created.Miles:N0} miles | " +
                              $"Account:{created.LoyaltyAccountId} | {created.TransactionDate:yyyy-MM-dd HH:mm}");
        }
        catch (ArgumentException ex) { Console.WriteLine($"Validation error: {ex.Message}"); }
    }

    private async Task DeleteAsync()
    {
        Console.Write("Transaction ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        { Console.WriteLine("Invalid."); return; }

        await _service.DeleteAsync(id);
        Console.WriteLine("Transaction record deleted successfully.");
    }

    private static void PrintTx(LoyaltyTransactionDto t)
        => Console.WriteLine(
            $"  [{t.Id}] {t.TransactionType,6} | {t.Miles,6:N0} miles | " +
            $"Account:{t.LoyaltyAccountId} | " +
            (t.TicketId.HasValue ? $"Ticket:{t.TicketId} | " : string.Empty) +
            $"{t.TransactionDate:yyyy-MM-dd HH:mm}");
}
