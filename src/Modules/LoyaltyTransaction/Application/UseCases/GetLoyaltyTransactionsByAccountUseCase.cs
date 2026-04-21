namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyTransaction.Domain.Repositories;

/// <summary>
/// Historial de todas las transacciones (EARN y REDEEM) de una cuenta.
/// Caso de uso clave para mostrar el estado del saldo de millas.
/// </summary>
public sealed class GetLoyaltyTransactionsByAccountUseCase
{
    private readonly ILoyaltyTransactionRepository _repository;

    public GetLoyaltyTransactionsByAccountUseCase(ILoyaltyTransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<LoyaltyTransactionAggregate>> ExecuteAsync(
        int               loyaltyAccountId,
        CancellationToken cancellationToken = default)
        => await _repository.GetByAccountAsync(loyaltyAccountId, cancellationToken);
}
