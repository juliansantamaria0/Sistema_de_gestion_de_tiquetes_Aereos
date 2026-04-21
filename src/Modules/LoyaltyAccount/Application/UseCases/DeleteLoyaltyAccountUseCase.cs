namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyAccount.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DeleteLoyaltyAccountUseCase
{
    private readonly ILoyaltyAccountRepository _repository;
    private readonly IUnitOfWork               _unitOfWork;

    public DeleteLoyaltyAccountUseCase(ILoyaltyAccountRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(new LoyaltyAccountId(id), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
