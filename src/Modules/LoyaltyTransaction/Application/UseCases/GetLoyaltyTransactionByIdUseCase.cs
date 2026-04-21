namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Domain.ValueObject;

public sealed class GetLoyaltyTransactionByIdUseCase
{
    private readonly ILoyaltyTransactionRepository _repository;

    public GetLoyaltyTransactionByIdUseCase(ILoyaltyTransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task<LoyaltyTransactionAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new LoyaltyTransactionId(id), cancellationToken);
}
