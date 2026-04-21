namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Domain.Repositories;

public sealed class GetAllLoyaltyAccountsUseCase
{
    private readonly ILoyaltyAccountRepository _repository;

    public GetAllLoyaltyAccountsUseCase(ILoyaltyAccountRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<LoyaltyAccountAggregate>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        => await _repository.GetAllAsync(cancellationToken);
}
