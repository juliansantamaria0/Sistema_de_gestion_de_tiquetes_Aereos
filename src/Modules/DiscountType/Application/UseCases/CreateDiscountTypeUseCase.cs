namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.DiscountType.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateDiscountTypeUseCase
{
    private readonly IDiscountTypeRepository _repository;
    private readonly IUnitOfWork             _unitOfWork;

    public CreateDiscountTypeUseCase(IDiscountTypeRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DiscountTypeAggregate> ExecuteAsync(
        string name, decimal percentage, int? ageMin, int? ageMax,
        CancellationToken cancellationToken = default)
    {
        var discountType = new DiscountTypeAggregate(
            new DiscountTypeId(await GetNextIdAsync(cancellationToken)), name, percentage, ageMin, ageMax);

        await _repository.AddAsync(discountType, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return discountType;
    }

    private async Task<int> GetNextIdAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(x => x.Id.Value).DefaultIfEmpty(0).Max() + 1;
    }
}
