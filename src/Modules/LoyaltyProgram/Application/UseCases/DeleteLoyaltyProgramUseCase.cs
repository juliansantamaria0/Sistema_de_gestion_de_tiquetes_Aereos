namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.LoyaltyProgram.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DeleteLoyaltyProgramUseCase
{
    private readonly ILoyaltyProgramRepository _repository;
    private readonly IUnitOfWork               _unitOfWork;

    public DeleteLoyaltyProgramUseCase(ILoyaltyProgramRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(new LoyaltyProgramId(id), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
