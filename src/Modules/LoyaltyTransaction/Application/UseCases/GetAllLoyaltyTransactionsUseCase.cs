namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Domain.Repositories;

public sealed class GetAllLoyaltyTransactionsUseCase
{
    private readonly ILoyaltyTransactionRepository _repository;

    public GetAllLoyaltyTransactionsUseCase(ILoyaltyTransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<LoyaltyTransactionAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
