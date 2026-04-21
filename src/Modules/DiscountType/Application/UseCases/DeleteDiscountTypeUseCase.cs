namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class DeleteDiscountTypeUseCase
{
    private readonly IDiscountTypeRepository _repository;
    private readonly IUnitOfWork             _unitOfWork;

    public DeleteDiscountTypeUseCase(IDiscountTypeRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(new DiscountTypeId(id), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
