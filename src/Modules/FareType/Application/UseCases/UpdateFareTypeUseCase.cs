namespace Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Application.UseCases;

using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Domain.Repositories;
using Sistema_de_gestion_de_tiquetes_Aereos.Modules.FareType.Domain.ValueObject;
using Sistema_de_gestion_de_tiquetes_Aereos.Shared.Contracts;

public sealed class UpdateFareTypeUseCase
{
    private readonly IFareTypeRepository _repository;
    private readonly IUnitOfWork         _unitOfWork;

    public UpdateFareTypeUseCase(IFareTypeRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        int               id,
        string            name,
        bool              isRefundable,
        bool              isChangeable,
        int               advancePurchaseDays,
        bool              baggageIncluded,
        CancellationToken cancellationToken = default)
    {
        var fareType = await _repository.GetByIdAsync(new FareTypeId(id), cancellationToken)
            ?? throw new KeyNotFoundException($"FareType with id {id} was not found.");

        fareType.Update(name, isRefundable, isChangeable, advancePurchaseDays, baggageIncluded);
        await _repository.UpdateAsync(fareType, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
