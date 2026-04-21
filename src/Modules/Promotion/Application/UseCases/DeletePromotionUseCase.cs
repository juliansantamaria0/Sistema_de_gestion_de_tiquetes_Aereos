namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.Promotion.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DeletePromotionUseCase
{
    private readonly IPromotionRepository _repository;
    private readonly IUnitOfWork          _unitOfWork;

    public DeletePromotionUseCase(IPromotionRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(new PromotionId(id), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
