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
        
        var account = new LoyaltyAccountAggregate(
            new LoyaltyAccountId(0),
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
}
