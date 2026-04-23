namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateLoyaltyAccountUseCase
{
    private readonly ILoyaltyAccountRepository _repository;
    private readonly IUnitOfWork               _unitOfWork;

    public CreateLoyaltyAccountUseCase(ILoyaltyAccountRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<LoyaltyAccountAggregate> ExecuteAsync(
        int               passengerId,
        int               loyaltyProgramId,
        int               loyaltyTierId,
        CancellationToken cancellationToken = default)
    {
        // LoyaltyAccountId(1) es placeholder; EF Core asigna el Id real al insertar.
        var account = new LoyaltyAccountAggregate(
            new LoyaltyAccountId(await GetNextIdAsync(cancellationToken)),
            passengerId,
            loyaltyProgramId,
            loyaltyTierId,
            totalMiles:     0,
            availableMiles: 0,
            DateTime.UtcNow);

        await _repository.AddAsync(account, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return account;
    }

    private async Task<int> GetNextIdAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(x => x.Id.Value).DefaultIfEmpty(0).Max() + 1;
    }
}
