namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Application.Interfaces;

public interface ILoyaltyTransactionService
{
    Task<LoyaltyTransactionDto?>             GetByIdAsync(int id,                                                                   CancellationToken cancellationToken = default);
    Task<IEnumerable<LoyaltyTransactionDto>> GetAllAsync(                                                                           CancellationToken cancellationToken = default);
    Task<IEnumerable<LoyaltyTransactionDto>> GetByAccountAsync(int loyaltyAccountId,                                               CancellationToken cancellationToken = default);
    Task<LoyaltyTransactionDto>              EarnAsync(int loyaltyAccountId, int? ticketId, int miles,                             CancellationToken cancellationToken = default);
    Task<LoyaltyTransactionDto>              RedeemAsync(int loyaltyAccountId, int? ticketId, int miles,                           CancellationToken cancellationToken = default);
    Task                                     DeleteAsync(int id,                                                                   CancellationToken cancellationToken = default);
}

public sealed record LoyaltyTransactionDto(
    int      Id,
    int      LoyaltyAccountId,
    int?     TicketId,
    string   TransactionType,
    int      Miles,
    DateTime TransactionDate);
