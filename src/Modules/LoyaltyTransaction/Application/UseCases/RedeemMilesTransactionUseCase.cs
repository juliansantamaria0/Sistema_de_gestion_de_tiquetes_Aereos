namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

/// <summary>
/// Registra una transacción de tipo REDEEM (redención de millas).
/// El saldo de la cuenta debe actualizarse por separado vía LoyaltyAccount.RedeemMiles().
/// </summary>
public sealed class RedeemMilesTransactionUseCase
{
    private readonly ILoyaltyTransactionRepository _repository;
    private readonly IUnitOfWork                   _unitOfWork;

    public RedeemMilesTransactionUseCase(ILoyaltyTransactionRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<LoyaltyTransactionAggregate> ExecuteAsync(
        int               loyaltyAccountId,
        int?              ticketId,
        int               miles,
        CancellationToken cancellationToken = default)
    {
        var transaction = new LoyaltyTransactionAggregate(
            new LoyaltyTransactionId(await GetNextIdAsync(cancellationToken)),
            loyaltyAccountId,
            ticketId,
            LoyaltyTransactionAggregate.TypeRedeem,
            miles,
            DateTime.UtcNow);

        await _repository.AddAsync(transaction, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return transaction;
    }

    private async Task<int> GetNextIdAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(x => x.Id.Value).DefaultIfEmpty(0).Max() + 1;
    }
}
