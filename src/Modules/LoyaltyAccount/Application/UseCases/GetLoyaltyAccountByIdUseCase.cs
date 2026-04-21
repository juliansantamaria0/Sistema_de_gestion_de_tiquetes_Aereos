namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Domain.ValueObject;

public sealed class GetLoyaltyAccountByIdUseCase
{
    private readonly ILoyaltyAccountRepository _repository;

    public GetLoyaltyAccountByIdUseCase(ILoyaltyAccountRepository repository)
    {
        _repository = repository;
    }

    public async Task<LoyaltyAccountAggregate?> ExecuteAsync(
        int               id,
        CancellationToken cancellationToken = default)
        => await _repository.GetByIdAsync(new LoyaltyAccountId(id), cancellationToken);
}
