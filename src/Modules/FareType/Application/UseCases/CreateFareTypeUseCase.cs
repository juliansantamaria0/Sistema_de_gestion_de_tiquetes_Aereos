namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Domain.Aggregate;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class CreateFareTypeUseCase
{
    private readonly IFareTypeRepository _repository;
    private readonly IUnitOfWork         _unitOfWork;

    public CreateFareTypeUseCase(IFareTypeRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<FareTypeAggregate> ExecuteAsync(
        string            name,
        bool              isRefundable,
        bool              isChangeable,
        int               advancePurchaseDays,
        bool              baggageIncluded,
        CancellationToken cancellationToken = default)
    {
        var fareType = new FareTypeAggregate(
            new FareTypeId(await GetNextIdAsync(cancellationToken)), name, isRefundable, isChangeable,
            advancePurchaseDays, baggageIncluded);

        await _repository.AddAsync(fareType, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return fareType;
    }

    private async Task<int> GetNextIdAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items.Select(x => x.Id.Value).DefaultIfEmpty(0).Max() + 1;
    }
}
