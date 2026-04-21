namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class AddMilesUseCase
{
    private readonly ILoyaltyAccountRepository _repository;
    private readonly IUnitOfWork               _unitOfWork;

    public AddMilesUseCase(ILoyaltyAccountRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, int miles, CancellationToken cancellationToken = default)
    {
        var account = await _repository.GetByIdAsync(new LoyaltyAccountId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"LoyaltyAccount with id {id} was not found.");

        account.AddMiles(miles);
        await _repository.UpdateAsync(account, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
