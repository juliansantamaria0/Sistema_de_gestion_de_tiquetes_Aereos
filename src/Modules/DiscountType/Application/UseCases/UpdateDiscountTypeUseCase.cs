namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class UpdateDiscountTypeUseCase
{
    private readonly IDiscountTypeRepository _repository;
    private readonly IUnitOfWork             _unitOfWork;

    public UpdateDiscountTypeUseCase(IDiscountTypeRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int id, string name, decimal percentage, int? ageMin, int? ageMax,
        CancellationToken cancellationToken = default)
    {
        var discountType = await _repository.GetByIdAsync(new DiscountTypeId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"DiscountType with id {id} was not found.");

        discountType.Update(name, percentage, ageMin, ageMax);
        await _repository.UpdateAsync(discountType, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
