namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Application.Services;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Application.Interfaces;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Application.UseCases;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Domain.Aggregate;

public sealed class LoyaltyTransactionService : ILoyaltyTransactionService
{
    private readonly EarnMilesTransactionUseCase             _earn;
    private readonly RedeemMilesTransactionUseCase           _redeem;
    private readonly DeleteLoyaltyTransactionUseCase         _delete;
    private readonly GetAllLoyaltyTransactionsUseCase        _getAll;
    private readonly GetLoyaltyTransactionByIdUseCase        _getById;
    private readonly GetLoyaltyTransactionsByAccountUseCase  _getByAccount;

    public LoyaltyTransactionService(
        EarnMilesTransactionUseCase            earn,
        RedeemMilesTransactionUseCase          redeem,
        DeleteLoyaltyTransactionUseCase        delete,
        GetAllLoyaltyTransactionsUseCase       getAll,
        GetLoyaltyTransactionByIdUseCase       getById,
        GetLoyaltyTransactionsByAccountUseCase getByAccount)
    {
        _earn         = earn;
        _redeem       = redeem;
        _delete       = delete;
        _getAll       = getAll;
        _getById      = getById;
        _getByAccount = getByAccount;
    }

    public async Task<LoyaltyTransactionDto> EarnAsync(
        int               loyaltyAccountId,
        int?              ticketId,
        int               miles,
        CancellationToken cancellationToken = default)
    {
        var agg = await _earn.ExecuteAsync(loyaltyAccountId, ticketId, miles, cancellationToken);
        return ToDto(agg);
    }

    public async Task<LoyaltyTransactionDto> RedeemAsync(
        int               loyaltyAccountId,
        int?              ticketId,
        int               miles,
        CancellationToken cancellationToken = default)
    {
        var agg = await _redeem.ExecuteAsync(loyaltyAccountId, ticketId, miles, cancellationToken);
        return ToDto(agg);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await _delete.ExecuteAsync(id, cancellationToken);

    public async Task<IEnumerable<LoyaltyTransactionDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _getAll.ExecuteAsync(cancellationToken);
        return list.Select(ToDto);
    }

    public async Task<LoyaltyTransactionDto?> GetByIdAsync(
        int               id,
        CancellationToken cancellationToken = default)
    {
        var agg = await _getById.ExecuteAsync(id, cancellationToken);
        return agg is null ? null : ToDto(agg);
    }

    public async Task<IEnumerable<LoyaltyTransactionDto>> GetByAccountAsync(
        int               loyaltyAccountId,
        CancellationToken cancellationToken = default)
    {
        var list = await _getByAccount.ExecuteAsync(loyaltyAccountId, cancellationToken);
        return list.Select(ToDto);
    }

    private static LoyaltyTransactionDto ToDto(LoyaltyTransactionAggregate agg)
        => new(
            agg.Id.Value,
            agg.LoyaltyAccountId,
            agg.TicketId,
            agg.TransactionType,
            agg.Miles,
            agg.TransactionDate);
}
